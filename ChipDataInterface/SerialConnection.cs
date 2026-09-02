using System;
using System.IO;
using System.IO.Ports;

namespace ChipDataInterface
{
    /// <summary>
    /// Bir COM port üzerinden UART bağlantısının açılmasını,
    /// RAW byte gönderilip alınmasını ve bağlantının güvenli
    /// şekilde kapatılmasını yönetir.
    /// </summary>
    internal sealed class SerialConnection : IDisposable
    {
        // Buradaki 8, arayüzde gösterilen payload uzunluğu değildir.
        // UART'ın 8N1 iletişim biçimindeki veri biti sayısıdır.
        private const int UartDataBits = 8;

        private const int TimeoutMilliseconds = 1000;

        private SerialPort? _serialPort;

        public bool IsConnected =>
            _serialPort?.IsOpen == true;

        /// <summary>
        /// UART üzerinden yeni bir byte alındığında tetiklenir.
        /// Bu olay arka plan iş parçacığında çalışır.
        /// </summary>
        public event Action<byte>? ByteReceived;

        /// <summary>
        /// UART verisi okunurken hata oluştuğunda tetiklenir.
        /// </summary>
        public event Action<Exception>? ReceiveError;

        public void Connect(
            string portName,
            int baudRate)
        {
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

            Disconnect();

            string normalizedPortName =
                portName.Trim();

            SerialPort serialPort = new(
                normalizedPortName,
                baudRate,
                Parity.None,
                UartDataBits,
                StopBits.One)
            {
                Handshake = Handshake.None,
                ReadTimeout = TimeoutMilliseconds,
                WriteTimeout = TimeoutMilliseconds,
                ReceivedBytesThreshold = 1,
                DtrEnable = false,
                RtsEnable = false
            };

            // Port açılmadan önce veri alma olayı bağlanır.
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
        /// Verilen byte dizisini herhangi bir metin dönüşümü
        /// uygulamadan RAW veri olarak UART'a yazar.
        /// </summary>
        public void Send(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            if (data.Length == 0)
            {
                throw new ArgumentException(
                    "Gönderilecek veri boş olamaz.",
                    nameof(data));
            }

            if (_serialPort is null ||
                !_serialPort.IsOpen)
            {
                throw new InvalidOperationException(
                    "Veri göndermek için önce COM bağlantısı açılmalıdır.");
            }

            _serialPort.Write(
                data,
                offset: 0,
                count: data.Length);
        }

        /// <summary>
        /// SerialPort tarafından veri geldiği bildirildiğinde,
        /// alınabilecek bütün byte'ları sırayla okur.
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
                ReceiveError?.Invoke(exception);
            }
        }

        public void Disconnect()
        {
            if (_serialPort is null)
            {
                return;
            }

            // Alan önceden temizlenerek bağlantının artık açık
            // olmadığı diğer metotlara hemen bildirilir.
            SerialPort serialPort = _serialPort;
            _serialPort = null;

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

        public void Dispose()
        {
            Disconnect();
        }
    }
}