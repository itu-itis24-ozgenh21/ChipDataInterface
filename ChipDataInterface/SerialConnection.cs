using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ChipDataInterface
{
    /// <summary>
    /// Bir sanal COM portu üzerinden FTDI/TTL-UART haberleşmesini yönetir.
    ///
    /// Bu sınıf yalnızca UART taşıma katmanından sorumludur. Veriyi metne
    /// dönüştürmez, byte veya bit sırasını değiştirmez ve EEPROM komutu gibi
    /// cihaza özel bir protokol uygulamaz.
    /// </summary>
    internal sealed class SerialConnection : IDisposable
    {
        // Projenin sabit UART çerçevesi 8N1'dir:
        // 1 başlangıç biti, 8 veri biti, eşlik biti yok ve 1 dur biti.
        private const int UartDataBits = 8;
        private const int UartFrameBitCount = 10;

        private const int IoTimeoutMilliseconds = 1000;
        private const int TransmitDrainPollMilliseconds = 1;

        private SerialPort? _serialPort;
        private bool _isDisposed;

        public bool IsConnected =>
            !_isDisposed && _serialPort?.IsOpen == true;

        /// <summary>
        /// UART'tan bir byte alındığında tetiklenir. SerialPort bu olayı
        /// arka plan thread'inde oluşturduğu için arayüz kodu Invoke veya
        /// BeginInvoke kullanarak UI thread'ine dönmelidir.
        /// </summary>
        public event Action<byte>? ByteReceived;

        /// <summary>
        /// Aktif alım sırasında beklenen bir seri port hatası oluştuğunda
        /// tetiklenir.
        /// </summary>
        public event Action<Exception>? ReceiveError;

        /// <summary>
        /// Seçilen COM portunu 8N1 ve donanımsal/yazılımsal akış kontrolü
        /// olmadan açar. Baud rate hedef cihazdaki değerle aynı olmalıdır.
        /// </summary>
        public void Connect(string portName, int baudRate)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentException(
                    "COM port adı boş olamaz.",
                    nameof(portName));
            }

            if (baudRate <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baudRate),
                    baudRate,
                    "Baud rate sıfırdan büyük olmalıdır.");
            }

            if (IsConnected)
            {
                throw new InvalidOperationException(
                    "Bir COM port bağlantısı zaten açık.");
            }

            // Önceki denemeden kalmış kapalı SerialPort nesnesini temizler.
            Disconnect();

            SerialPort serialPort = new(
                portName.Trim(),
                baudRate,
                Parity.None,
                UartDataBits,
                StopBits.One)
            {
                Handshake = Handshake.None,
                ReadTimeout = IoTimeoutMilliseconds,
                WriteTimeout = IoTimeoutMilliseconds,
                ReceivedBytesThreshold = 1,

                // Bu proje yalnızca TX, RX ve GND ile çalışır. DTR/RTS'nin
                // bazı hedef kartlarda reset veya kontrol sinyali üretmesini
                // önlemek için iki çıkış da devre dışıdır.
                DtrEnable = false,
                RtsEnable = false
            };

            // Port açıldığı anda veri gelebileceğinden olay önce bağlanır.
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
                _serialPort = serialPort;
            }
            catch
            {
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Verilen diziyi sırasını değiştirmeden RAW byte olarak gönderir.
        /// Örneğin { A0, 00, 0F } dizisi hatta aynı byte sırasıyla çıkar.
        /// UART içindeki başlangıç/dur bitlerini ve veri bitlerinin fiziksel
        /// seri sırasını FT232RL donanımı otomatik olarak üretir.
        /// </summary>
        public void Send(byte[] data)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(data);

            if (data.Length == 0)
            {
                throw new ArgumentException(
                    "Gönderilecek veri boş olamaz.",
                    nameof(data));
            }

            SerialPort serialPort =
                GetOpenSerialPort();

            // Byte[] overload'u kullanıldığı için kodlama, satır sonu veya
            // ASCII dönüşümü uygulanmaz. offset=0 ile dizinin ilk byte'ı
            // önce, ardından kalan byte'lar indeks sırasıyla yazılır.
            serialPort.Write(
                data,
                offset: 0,
                count: data.Length);

            WaitForTransmitCompletion(
                serialPort,
                data.Length);
        }

        /// <summary>
        /// Bilgisayar ve sürücü gönderme tamponunun boşalmasını bekler.
        /// Ardından FT232RL içindeki son verinin de UART hattından çıkabilmesi
        /// için 8N1 ve seçilen baud rate üzerinden koruyucu süre hesaplar.
        /// Böylece çağıran kod Send dönüşünde portu güvenle kapatabilir.
        /// </summary>
        private static void WaitForTransmitCompletion(
            SerialPort serialPort,
            int byteCount)
        {
            Stopwatch stopwatch =
                Stopwatch.StartNew();

            while (serialPort.BytesToWrite > 0)
            {
                if (stopwatch.ElapsedMilliseconds >=
                    IoTimeoutMilliseconds)
                {
                    throw new TimeoutException(
                        "UART gönderme tamponu zamanında boşalmadı.");
                }

                Thread.Sleep(
                    TransmitDrainPollMilliseconds);
            }

            // 8N1'de her byte hatta toplam 10 bit olarak çıkar. Tamponun
            // sıfırlanması sürücünün veriyi FTDI'ye teslim ettiğini gösterir;
            // bu kısa ek bekleme, cihaz FIFO'sundaki son çerçeveyi de kapsar.
            double transmissionMilliseconds =
                byteCount * UartFrameBitCount * 1000.0 /
                serialPort.BaudRate;

            int guardDelayMilliseconds =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        transmissionMilliseconds));

            Thread.Sleep(guardDelayMilliseconds);
        }

        /// <summary>
        /// DataReceived her byte için ayrı ayrı oluşmak zorunda değildir.
        /// Bu nedenle olay geldiğinde alım tamponunda bulunan bütün byte'lar
        /// FIFO/geliş sırasıyla okunur ve tek tek üst katmana iletilir.
        /// </summary>
        private void SerialPort_DataReceived(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            if (sender is not SerialPort serialPort)
            {
                return;
            }

            try
            {
                while (serialPort.IsOpen &&
                       serialPort.BytesToRead > 0)
                {
                    int receivedValue =
                        serialPort.ReadByte();

                    if (receivedValue < 0)
                    {
                        return;
                    }

                    ByteReceived?.Invoke(
                        (byte)receivedValue);
                }
            }
            catch (Exception exception) when (
                exception is InvalidOperationException or
                IOException or
                UnauthorizedAccessException or
                TimeoutException)
            {
                // Kullanıcının bilinçli Disconnect çağrısı sırasında oluşan
                // kapanma hatasını yeni bir RX hatası olarak bildirmez.
                if (ReferenceEquals(
                    _serialPort,
                    serialPort))
                {
                    ReceiveError?.Invoke(exception);
                }
            }
        }

        /// <summary>
        /// Açık COM portunu kapatır, olay aboneliğini kaldırır ve Windows
        /// kaynağını serbest bırakır. Birden fazla kez çağrılması güvenlidir.
        /// </summary>
        public void Disconnect()
        {
            SerialPort? serialPort =
                _serialPort;

            // Diğer metotlar bağlantıyı hemen kapalı görsün ve bilinçli port
            // kapanışı ReceiveError olarak raporlanmasın diye önce temizlenir.
            _serialPort = null;

            if (serialPort is null)
            {
                return;
            }

            serialPort.DataReceived -= SerialPort_DataReceived;

            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            finally
            {
                serialPort.Dispose();
            }
        }

        private SerialPort GetOpenSerialPort()
        {
            if (_serialPort is null ||
                !_serialPort.IsOpen)
            {
                throw new InvalidOperationException(
                    "İşlem için önce COM bağlantısı açılmalıdır.");
            }

            return _serialPort;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(
                    nameof(SerialConnection));
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                Disconnect();
            }
            finally
            {
                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
