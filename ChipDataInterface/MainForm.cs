using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace ChipDataInterface
{
    /// <summary>
    /// Ana kullanıcı arayüzünü ve UART haberleşme akışını yönetir.
    ///
    /// Uygulama FTDI üzerinden UART verisi gönderir ve alır.
    /// EEPROM'a doğrudan erişmez. EEPROM okuma ve yazma işlemleri
    /// hedef cihazın yazılımının sorumluluğundadır.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constants

        private const int BitsPerByte = 8;

        // Arayüzde gösterilen TX ve RX bitlerinin sayısıdır.
        // Mevcut projede 8 bit, UART'a tek byte olarak aktarılır.
        private const int PayloadBitCount = 8;

        // Read işleminin hedef cihazdan veri bekleyeceği süredir.
        // Bu değer UART ayarı değil, uygulamaya ait zaman aşımıdır.
        private const int ReadTimeoutMilliseconds = 2000;

        // Bitler ASCII metni olarak değil, doğrudan byte olarak aktarılır.
        private const string DataFormatName = "RAW";

        // Kullanıcı hedef cihazın ayarına uygun değeri kendisi seçer.
        private static readonly int[] SupportedBaudRates =
        {
            9600,
            19200,
            38400,
            57600
        };

        // Uygulamanın desteklediği veri uzunluklarıdır.
        // Her seçenek ekranda gösterilecek metni ve gerçek byte sayısını taşır.
        private static readonly PayloadSizeOption[] SupportedPayloadSizes =
        {
            new PayloadSizeOption(
                1,
                "1 Byte (8 Bits)"),

            new PayloadSizeOption(
                8,
                "8 Bytes (64 Bits)")
        };

        #endregion

        #region Types

        /// <summary>
        /// Günlük kayıtlarının türünü belirler.
        /// </summary>
        private enum LogEntryType
        {
            Information,
            Tx,
            Rx,
            Success,
            Warning,
            Error
        }

        #endregion

        #region Fields

        private readonly SerialConnection _serialConnection = new();

        private readonly List<BitButton> _txBitButtons = new();
        private readonly List<BitButton> _rxBitButtons = new();

        // Çoklu-byte modunda kullanılan TX ve RX HEX kutularıdır.
        // Liste sırası aynı zamanda aktarım sırasıdır: listenin ilk elemanı
        // ekrandaki en soldaki kutudur ve UART'a ilk yazılan/alınan byte'tır.
        // Bu katmanda endian dönüşümü veya manuel bit tersleme yapılmaz.
        private readonly List<HexByteTextBox> _txByteTextBoxes = new();
        private readonly List<HexByteTextBox> _rxByteTextBoxes = new();

        // Read işlemi sırasında veri gelmezse bağlantıyı kapatır.
        private readonly System.Windows.Forms.Timer _readTimeoutTimer = new();

        // Yalnızca aktif bir Read işlemi sırasında gelen byte'ların
        // işlenmesini sağlar.
        private bool _isReadPending;

        // Read başladığında seçilen payload uzunluğuna göre oluşturulur.
        // Gelen byte'lar tamamlanana kadar burada sırasıyla biriktirilir.
        private byte[] _receiveBuffer =
            Array.Empty<byte>();

        // Aktif Read sırasında tamponda kaç byte bulunduğunu tutar.
        private int _receivedByteCount;

        #endregion

        #region Initialization

        public MainForm()
        {
            InitializeComponent();

            InitializeConnectionOptions();
            InitializePayloadSizeOptions();
            InitializeReadOperation();
            RegisterEventHandlers();

            // Başlangıçta Payload Size seçilmediği için
            // TX ve RX alanları boş bırakılır.
            RebuildPayloadControls();

            WriteLog(
                LogEntryType.Information,
                "Application ready. Select COM port, baud rate and payload size.");
        }

        /// <summary>
        /// Baud rate listesini doldurur ve mevcut COM portlarını tarar.
        /// Başlangıçta baud rate bilerek seçilmez.
        /// </summary>
        private void InitializeConnectionOptions()
        {
            baudRateComboBox.Items.Clear();

            foreach (int baudRate in SupportedBaudRates)
            {
                baudRateComboBox.Items.Add(baudRate);
            }

            baudRateComboBox.SelectedIndex = -1;

            RefreshAvailablePorts();
        }

        private void InitializePayloadSizeOptions()
        {
            payloadSizeComboBox.Items.Clear();

            // ComboBox'ta seçenek nesnesinin DisplayText özelliği gösterilir.
            // Program işlemleri ekrandaki yazıya göre değil,
            // seçeneğin ByteCount değerine göre yapacaktır.
            payloadSizeComboBox.DisplayMember =
                nameof(PayloadSizeOption.DisplayText);

            foreach (PayloadSizeOption payloadSize in SupportedPayloadSizes)
            {
                payloadSizeComboBox.Items.Add(payloadSize);
            }

            // Yanlış veri uzunluğuyla kazara işlem yapılmasını engellemek için
            // başlangıçta herhangi bir seçenek otomatik olarak seçilmez.
            payloadSizeComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Read işleminde kullanılacak zaman aşımı süresini hazırlar.
        /// </summary>
        private void InitializeReadOperation()
        {
            _readTimeoutTimer.Interval =
                ReadTimeoutMilliseconds;
        }

        /// <summary>
        /// Arayüz ve seri bağlantı olaylarını ilgili metotlara bağlar.
        /// Bütün olay bağlantıları burada tutularak takip edilmeleri kolaylaştırılır.
        /// </summary>
        private void RegisterEventHandlers()
        {
            refreshPortsButton.Click +=
                refreshPortsButton_Click;

            sendButton.Click +=
                sendButton_Click;

            readButton.Click +=
                readButton_Click;

            clearLogButton.Click +=
                clearLogButton_Click;

            portComboBox.SelectedIndexChanged +=
                ConnectionSettingChanged;

            baudRateComboBox.SelectedIndexChanged +=
                ConnectionSettingChanged;

            payloadSizeComboBox.SelectedIndexChanged +=
                PayloadSizeComboBox_SelectedIndexChanged;

            portComboBox.SelectionChangeCommitted +=
                SelectionComboBox_SelectionChangeCommitted;

            baudRateComboBox.SelectionChangeCommitted +=
                SelectionComboBox_SelectionChangeCommitted;

            payloadSizeComboBox.SelectionChangeCommitted +=
                SelectionComboBox_SelectionChangeCommitted;

            _readTimeoutTimer.Tick +=
                ReadTimeoutTimer_Tick;

            _serialConnection.ByteReceived +=
                SerialConnection_ByteReceived;

            _serialConnection.ReceiveError +=
                SerialConnection_ReceiveError;
        }

        /// <summary>
        /// Kullanıcı seçimini tamamladıktan sonra ComboBox odağını kaldırır.
        /// Seçilen değer korunur, yalnızca mavi seçim vurgusu kaybolur.
        /// </summary>
        private void SelectionComboBox_SelectionChangeCommitted(
            object? sender,
            EventArgs e)
        {
            PostToUiThread(
                () => ActiveControl = null);
        }

        #endregion

        #region Payload Controls

        /// <summary>
        /// Kullanıcının seçtiği veri uzunluğunu döndürür.
        /// Herhangi bir seçim yapılmamışsa null döner.
        /// </summary>
        private PayloadSizeOption? SelectedPayloadSize =>
            payloadSizeComboBox.SelectedItem as PayloadSizeOption;

        /// <summary>
        /// TX ve RX panellerindeki eski kontrolleri kaldırır ve
        /// kullandıkları Windows kaynaklarını serbest bırakır.
        /// </summary>
        private void ClearPayloadControls()
        {
            DisposeChildControls(txBitsPanel);
            DisposeChildControls(rxBitsPanel);

            _txBitButtons.Clear();
            _rxBitButtons.Clear();

            _txByteTextBoxes.Clear();
            _rxByteTextBoxes.Clear();
        }

        private static void DisposeChildControls(
            Control container)
        {
            // Controls.Clear() kontrolleri yalnızca panelden kaldırabilir.
            // Dispose çağrısı, mod tekrar tekrar değiştirildiğinde
            // kullanılmayan kontrollerin bellekte kalmasını engeller.
            for (int index = container.Controls.Count - 1;
                 index >= 0;
                 index--)
            {
                Control childControl =
                    container.Controls[index];

                container.Controls.RemoveAt(index);
                childControl.Dispose();
            }
        }

        /// <summary>
        /// Seçilen veri uzunluğuna uygun TX ve RX kontrollerini oluşturur.
        /// </summary>
        private void RebuildPayloadControls()
        {
            ClearPayloadControls();

            PayloadSizeOption? payloadSize =
                SelectedPayloadSize;

            if (payloadSize is null)
            {
                SetCommunicationControlsEnabled(true);
                return;
            }

            if (payloadSize.ByteCount == 1)
            {
                CreateTxBitButtons();
                CreateRxBitButtons();
            }
            else
            {
                CreateTxByteTextBoxes(payloadSize.ByteCount);
                CreateRxByteTextBoxes(payloadSize.ByteCount);
            }

            SetCommunicationControlsEnabled(true);
        }

        /// <summary>
        /// Payload Size değiştiğinde eski bağlantıyı kapatır ve
        /// TX/RX alanlarını yeni veri uzunluğuna göre oluşturur.
        /// </summary>
        private void PayloadSizeComboBox_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (_isReadPending)
            {
                EndReadOperation();
            }
            else
            {
                _serialConnection.Disconnect();
            }

            RebuildPayloadControls();

            if (SelectedPayloadSize is PayloadSizeOption payloadSize)
            {
                WriteLog(
                    LogEntryType.Information,
                    $"Payload size selected: " +
                    $"{payloadSize.ByteCount} byte " +
                    $"({payloadSize.BitCount} bits).");
            }
        }

        /// <summary>
        /// 1-byte modunda kullanıcı tarafından değiştirilebilen
        /// sekiz TX bit butonunu oluşturur.
        /// </summary>
        private void CreateTxBitButtons()
        {
            for (int bitIndex = PayloadBitCount - 1;
                 bitIndex >= 0;
                 bitIndex--)
            {
                BitButton bitButton = new(
                    bitIndex,
                    isReadOnly: false)
                {
                    Name = $"txBit{bitIndex}Button"
                };

                _txBitButtons.Add(bitButton);
                txBitsPanel.Controls.Add(bitButton);
            }
        }

        /// <summary>
        /// 1-byte modunda gelen byte'ın bitlerini gösterecek
        /// sekiz salt okunur RX bit butonunu oluşturur.
        /// </summary>
        private void CreateRxBitButtons()
        {
            for (int bitIndex = PayloadBitCount - 1;
                 bitIndex >= 0;
                 bitIndex--)
            {
                BitButton bitButton = new(
                    bitIndex,
                    isReadOnly: true)
                {
                    Name = $"rxBit{bitIndex}Button"
                };

                _rxBitButtons.Add(bitButton);
                rxBitsPanel.Controls.Add(bitButton);
            }
        }

        /// <summary>
        /// Çoklu-byte modunda UART'a gönderilecek HEX kutularını oluşturur.
        /// İlk oluşturulan kutu ekranın solunda yer alır ve Byte 0'ı,
        /// yani ilk gönderilecek byte'ı temsil eder.
        /// </summary>
        private void CreateTxByteTextBoxes(int byteCount)
        {
            for (int byteIndex = 0;
                 byteIndex < byteCount;
                 byteIndex++)
            {
                HexByteTextBox byteTextBox = new(
                    byteIndex,
                    isReadOnly: false)
                {
                    Name = $"txByte{byteIndex}TextBox"
                };

                _txByteTextBoxes.Add(byteTextBox);
                txBitsPanel.Controls.Add(byteTextBox);
            }
        }

        /// <summary>
        /// Çoklu-byte modunda alınan verileri gösterecek salt okunur HEX
        /// kutularını oluşturur. İlk alınan byte en soldaki kutuya yazılır.
        /// </summary>
        private void CreateRxByteTextBoxes(int byteCount)
        {
            for (int byteIndex = 0;
                 byteIndex < byteCount;
                 byteIndex++)
            {
                HexByteTextBox byteTextBox = new(
                    byteIndex,
                    isReadOnly: true)
                {
                    Name = $"rxByte{byteIndex}TextBox"
                };

                _rxByteTextBoxes.Add(byteTextBox);
                rxBitsPanel.Controls.Add(byteTextBox);
            }
        }

        #endregion

        #region COM Port Settings

        /// <summary>
        /// Bilgisayarda bulunan COM portlarını yeniden listeler.
        ///
        /// Önceden seçilen port hâlâ mevcutsa seçim korunur.
        /// Tek port varsa otomatik seçilir. Birden fazla port varsa
        /// yanlış cihaza gönderim yapılmaması için kullanıcı seçim yapar.
        /// </summary>
        private void RefreshAvailablePorts()
        {
            string? previouslySelectedPort =
                portComboBox.SelectedItem as string;

            string[] portNames =
                SerialPort.GetPortNames();

            Array.Sort(
                portNames,
                StringComparer.OrdinalIgnoreCase);

            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(portNames);

            if (previouslySelectedPort is not null &&
                portComboBox.Items.Contains(previouslySelectedPort))
            {
                portComboBox.SelectedItem =
                    previouslySelectedPort;
            }
            else if (portNames.Length == 1)
            {
                portComboBox.SelectedIndex = 0;
            }
            else
            {
                portComboBox.SelectedIndex = -1;
            }

            if (portNames.Length == 0)
            {
                WriteLog(
                    LogEntryType.Warning,
                    "No available COM port was found.");
            }
            else
            {
                WriteLog(
                    LogEntryType.Information,
                    $"COM ports refreshed: " +
                    string.Join(", ", portNames));
            }
        }

        private void refreshPortsButton_Click(
            object? sender,
            EventArgs e)
        {
            RefreshAvailablePorts();
        }

        /// <summary>
        /// COM portu veya baud rate değiştirildiğinde eski ayarlara ait
        /// olabilecek bağlantıyı kapatır.
        /// </summary>
        private void ConnectionSettingChanged(
            object? sender,
            EventArgs e)
        {
            _serialConnection.Disconnect();

            if (sender == portComboBox &&
                portComboBox.SelectedItem is string portName)
            {
                WriteLog(
                    LogEntryType.Information,
                    $"COM port selected: {portName}.");
            }

            if (sender == baudRateComboBox &&
                baudRateComboBox.SelectedItem is int baudRate)
            {
                WriteLog(
                    LogEntryType.Information,
                    $"Baud rate selected: {baudRate}.");
            }
        }

        /// <summary>
        /// Kullanıcının seçtiği COM portu ve baud rate ile bağlantıyı açar.
        /// Seçimlerden biri yapılmamışsa kullanıcıya bildirilecek bir
        /// InvalidOperationException oluşturur.
        /// </summary>
        private void EnsureSerialConnection()
        {
            if (_serialConnection.IsConnected)
            {
                return;
            }

            if (portComboBox.SelectedItem is not string portName)
            {
                throw new InvalidOperationException(
                    "Please select a COM port to use.");
            }

            if (baudRateComboBox.SelectedItem is not int baudRate)
            {
                throw new InvalidOperationException(
                    "Please select a baud rate to use.");
            }

            _serialConnection.Connect(
                portName,
                baudRate);
        }

        /// <summary>
        /// Haberleşme sırasında bağlantı, baud rate ve payload ayarlarının
        /// değiştirilmesini engeller.
        ///
        /// Send ve Read yalnızca geçerli bir payload uzunluğu seçilmişse
        /// kullanılabilir.
        /// </summary>
        private void SetCommunicationControlsEnabled(bool enabled)
        {
            portComboBox.Enabled = enabled;
            baudRateComboBox.Enabled = enabled;
            payloadSizeComboBox.Enabled = enabled;
            refreshPortsButton.Enabled = enabled;

            bool payloadSizeIsSelected =
                SelectedPayloadSize is not null;

            sendButton.Enabled =
                enabled && payloadSizeIsSelected;

            readButton.Enabled =
                enabled && payloadSizeIsSelected;
        }

        /// <summary>
        /// Günlük kayıtlarında kullanılacak port ve baud bilgisini üretir.
        /// </summary>
        private string BuildConnectionSummary()
        {
            string portName =
                portComboBox.SelectedItem as string ?? "Not selected";

            string baudRate =
                baudRateComboBox.SelectedItem?.ToString()
                ?? "Not selected";

            return $"{portName} @ {baudRate} baud";
        }

        #endregion

        #region TX Operation

        /// <summary>
        /// Seçilen payload moduna göre gönderilecek RAW byte
        /// dizisini oluşturur.
        ///
        /// 1-byte modunda bit butonları tek byte'a dönüştürülür.
        /// Çoklu-byte modunda HEX kutuları ekrandaki soldan sağa
        /// sıraları korunarak RAW byte dizisine aktarılır.
        /// </summary>
        private byte[] BuildTxData()
        {
            PayloadSizeOption payloadSize =
                SelectedPayloadSize
                ?? throw new InvalidOperationException(
                    "Please select a payload size.");

            if (payloadSize.ByteCount == 1)
            {
                return BuildTxDataFromBitButtons();
            }

            return BuildTxDataFromHexFields(
                payloadSize.ByteCount);
        }

        /// <summary>
        /// Sekiz TX bit butonunu tek RAW byte değerine dönüştürür.
        ///
        /// Örnek:
        /// 10101100 = 0xAC
        /// </summary>
        private byte[] BuildTxDataFromBitButtons()
        {
            if (_txBitButtons.Count != PayloadBitCount)
            {
                throw new InvalidOperationException(
                    "The 1-byte TX controls are not ready.");
            }

            byte txByte = 0;

            foreach (BitButton bitButton in _txBitButtons)
            {
                if (!bitButton.BitValue)
                {
                    continue;
                }

                txByte |=
                    (byte)(1 << bitButton.BitIndex);
            }

            return new[] { txByte };
        }

        /// <summary>
        /// TX HEX kutularındaki değerleri ekranda görülen soldan sağa
        /// sırayı değiştirmeden RAW byte dizisine dönüştürür.
        ///
        /// Örnek ekran:
        /// A0 00 00 00 00 00 00 0F
        ///
        /// SerialPort.Write'a verilen ve hedef UART'ın alacağı byte sırası:
        /// A0 00 00 00 00 00 00 0F
        /// </summary>
        private byte[] BuildTxDataFromHexFields(
            int expectedByteCount)
        {
            if (_txByteTextBoxes.Count != expectedByteCount)
            {
                throw new InvalidOperationException(
                    "The multi-byte TX controls are not ready.");
            }

            byte[] txData =
                new byte[expectedByteCount];

            for (int byteIndex = 0;
                 byteIndex < expectedByteCount;
                 byteIndex++)
            {
                // GetValue, ekrandaki 00-FF değerini gerçek bir byte'a
                // dönüştürür. String veya ASCII dönüşümü uygulanmaz.
                txData[byteIndex] =
                    _txByteTextBoxes[byteIndex].GetValue();
            }

            return txData;
        }

        /// <summary>
        /// Byte dizisini sekiz bitlik gruplardan oluşan binary
        /// gösterime dönüştürür.
        ///
        /// Örnek:
        /// AC-01 = 10101100 00000001
        /// </summary>
        private static string BuildBinaryData(
            IReadOnlyList<byte> data)
        {
            string[] binaryBytes =
                new string[data.Count];

            for (int index = 0;
                 index < data.Count;
                 index++)
            {
                binaryBytes[index] =
                    Convert.ToString(data[index], 2)
                        .PadLeft(BitsPerByte, '0');
            }

            return string.Join(
                " ",
                binaryBytes);
        }

        /// <summary>
        /// UART'a gönderilecek TX verisini, ekranda girildiği ve hatta
        /// yazıldığı ortak sırayla günlük formatına dönüştürür.
        /// </summary>
        private string BuildTxSummary(byte[] txData)
        {
            PayloadSizeOption payloadSize =
                SelectedPayloadSize
                ?? throw new InvalidOperationException(
                    "Please select a payload size.");

            string binaryData =
                BuildBinaryData(txData);

            string hexData =
                BitConverter.ToString(txData);

            string byteUnit =
                txData.Length == 1
                    ? "byte"
                    : "bytes";

            return $"{BuildConnectionSummary()} | " +
                   $"{payloadSize.BitCount} bits | " +
                   $"{txData.Length} {byteUnit} ({DataFormatName}) | " +
                   $"BIN: {binaryData} | " +
                   $"HEX: {hexData}";
        }

        /// <summary>
        /// Seçilen TX verisini UART bağlantısına RAW byte dizisi
        /// olarak gönderir.
        ///
        /// SerialConnection, verinin bilgisayardaki gönderme tamponundan
        /// çıkmasını bekler. Ardından cihaz sürekli bağlı bırakılmaması
        /// için COM portu kapatılır.
        /// </summary>
        private void sendButton_Click(
            object? sender,
            EventArgs e)
        {
            SetCommunicationControlsEnabled(false);

            try
            {
                byte[] txData =
                    BuildTxData();

                EnsureSerialConnection();

                string txSummary =
                    BuildTxSummary(txData);

                WriteLog(
                    LogEntryType.Tx,
                    "Transmission started" +
                    Environment.NewLine +
                    txSummary);

                _serialConnection.Send(txData);

                _serialConnection.Disconnect();

                WriteLog(
                    LogEntryType.Success,
                    $"TX completed | Sent " +
                    $"{txData.Length}/{txData.Length} bytes" +
                    Environment.NewLine +
                    txSummary +
                    Environment.NewLine +
                    "COM connection closed.");
            }
            catch (InvalidOperationException exception)
            {
                _serialConnection.Disconnect();

                ReportWarning(
                    "TX Warning",
                    exception.Message);
            }
            catch (FormatException exception)
            {
                // Kullanıcının HEX kutusuna geçersiz bir değer
                // girmesi sistem hatası değil, giriş uyarısıdır.
                _serialConnection.Disconnect();

                ReportWarning(
                    "TX Warning",
                    exception.Message);
            }
            catch (Exception exception)
            {
                _serialConnection.Disconnect();

                ReportError(
                    "TX Error",
                    $"UART transmission failed: " +
                    exception.Message);
            }
            finally
            {
                SetCommunicationControlsEnabled(true);
            }
        }

        #endregion

        #region RX Operation

        /// <summary>
        /// Read düğmesine basıldığında COM portunu geçici olarak açar
        /// ve seçilen payload uzunluğundaki veriyi bekler.
        ///
        /// Bu metot hedef cihaza herhangi bir okuma komutu göndermez.
        /// Hedef cihazın veriyi kendi UART TX hattından göndermesi gerekir.
        /// </summary>
        private void readButton_Click(
            object? sender,
            EventArgs e)
        {
            if (_isReadPending)
            {
                return;
            }

            SetCommunicationControlsEnabled(false);

            try
            {
                PayloadSizeOption payloadSize =
                    SelectedPayloadSize
                    ?? throw new InvalidOperationException(
                        "Please select a payload size.");

                // Port açılır açılmaz veri gelebileceği için tampon,
                // bağlantı kurulmadan önce hazırlanır.
                _receiveBuffer =
                    new byte[payloadSize.ByteCount];

                _receivedByteCount = 0;
                _isReadPending = true;

                EnsureSerialConnection();

                string byteUnit =
                    payloadSize.ByteCount == 1
                        ? "byte"
                        : "bytes";

                WriteLog(
                    LogEntryType.Rx,
                    $"RX listening started | " +
                    $"{BuildConnectionSummary()} | " +
                    $"Waiting for {payloadSize.ByteCount} {byteUnit} | " +
                    $"Timeout: {ReadTimeoutMilliseconds} ms.");

                _readTimeoutTimer.Start();
            }
            catch (InvalidOperationException exception)
            {
                EndReadOperation();

                ReportWarning(
                    "RX Warning",
                    exception.Message);
            }
            catch (Exception exception)
            {
                EndReadOperation();

                ReportError(
                    "RX Error",
                    $"UART read operation failed: " +
                    exception.Message);
            }
        }

        /// <summary>
        /// Tek byte'lık RX verisinin her bitini karşılık gelen
        /// salt okunur BitButton üzerinde gösterir.
        /// </summary>
        private void DisplayRxByte(byte receivedByte)
        {
            foreach (BitButton bitButton in _rxBitButtons)
            {
                bool bitValue =
                    (receivedByte &
                     (1 << bitButton.BitIndex)) != 0;

                bitButton.SetValue(bitValue);
            }
        }

        /// <summary>
        /// Tamamlanan RX verisini seçilen payload moduna uygun
        /// arayüz kontrollerinde gösterir.
        ///
        /// UART'tan ilk gelen byte ilk (en soldaki) HEX kutusunda,
        /// sonraki byte'lar da geliş sıraları korunarak gösterilir.
        /// </summary>
        private void DisplayRxData(byte[] receivedData)
        {
            PayloadSizeOption payloadSize =
                SelectedPayloadSize
                ?? throw new InvalidOperationException(
                    "Please select a payload size.");

            if (receivedData.Length != payloadSize.ByteCount)
            {
                throw new InvalidOperationException(
                    "Received data length does not match " +
                    "the selected payload size.");
            }

            if (payloadSize.ByteCount == 1)
            {
                // Tek byte'ta byte sırası olmadığı için mevcut
                // bit gösterim yöntemi doğrudan kullanılabilir.
                DisplayRxByte(receivedData[0]);
                return;
            }

            if (_rxByteTextBoxes.Count != receivedData.Length)
            {
                throw new InvalidOperationException(
                    "The multi-byte RX controls are not ready.");
            }

            for (int byteIndex = 0;
                 byteIndex < receivedData.Length;
                 byteIndex++)
            {
                _rxByteTextBoxes[byteIndex].SetValue(
                    receivedData[byteIndex]);
            }
        }

        /// <summary>
        /// UART'tan alınan RX verisini geliş sırasını değiştirmeden
        /// günlük formatına dönüştürür.
        /// </summary>
        private string BuildRxSummary(byte[] receivedData)
        {
            PayloadSizeOption payloadSize =
                SelectedPayloadSize
                ?? throw new InvalidOperationException(
                    "Please select a payload size.");

            string binaryData =
                BuildBinaryData(receivedData);

            string hexData =
                BitConverter.ToString(receivedData);

            string byteUnit =
                receivedData.Length == 1
                    ? "byte"
                    : "bytes";

            return $"{BuildConnectionSummary()} | " +
                   $"{payloadSize.BitCount} bits | " +
                   $"{receivedData.Length} {byteUnit} ({DataFormatName}) | " +
                   $"BIN: {binaryData} | " +
                   $"HEX: {hexData}";
        }

        /// <summary>
        /// SerialPort veri alma olayı arayüz thread'inden farklı bir
        /// thread üzerinde çalıştığı için alınan byte'ın işlenmesini
        /// Windows Forms arayüz thread'ine aktarır.
        /// </summary>
        private void SerialConnection_ByteReceived(
            byte receivedByte)
        {
            PostToUiThread(
                () => HandleReceivedByte(receivedByte));
        }

        /// <summary>
        /// UART'tan gelen byte'ları seçilen payload uzunluğuna ulaşıncaya
        /// kadar sırayla RX tamponunda biriktirir.
        ///
        /// Beklenen bütün veri alındığında RX alanı güncellenir,
        /// işlem günlüğüne sonuç yazılır ve COM bağlantısı kapatılır.
        /// </summary>
        private void HandleReceivedByte(byte receivedByte)
        {
            if (!_isReadPending ||
                _receiveBuffer.Length == 0)
            {
                return;
            }

            if (_receivedByteCount >=
                _receiveBuffer.Length)
            {
                return;
            }

            _receiveBuffer[_receivedByteCount] =
                receivedByte;

            _receivedByteCount++;

            // Beklenen bütün byte'lar henüz alınmadıysa
            // bağlantı açık tutularak sonraki byte beklenir.
            if (_receivedByteCount <
                _receiveBuffer.Length)
            {
                return;
            }

            byte[] completedData =
                (byte[])_receiveBuffer.Clone();

            DisplayRxData(completedData);

            string rxSummary =
                BuildRxSummary(completedData);

            EndReadOperation();

            WriteLog(
                LogEntryType.Success,
                $"RX completed | Received " +
                $"{completedData.Length}/{completedData.Length} bytes" +
                Environment.NewLine +
                rxSummary +
                Environment.NewLine +
                "COM connection closed.");
        }

        /// <summary>
        /// Arka plandaki RX işlemi sırasında oluşan hatayı
        /// Windows Forms arayüz thread'ine aktarır.
        /// </summary>
        private void SerialConnection_ReceiveError(
            Exception exception)
        {
            PostToUiThread(
                () => HandleReceiveError(exception));
        }

        /// <summary>
        /// Aktif Read sırasında meydana gelen seri port hatasını
        /// işler ve alınmış kısmi veri miktarını bildirir.
        /// </summary>
        private void HandleReceiveError(
            Exception exception)
        {
            if (!_isReadPending)
            {
                return;
            }

            int expectedByteCount =
                _receiveBuffer.Length;

            int receivedByteCount =
                _receivedByteCount;

            string partialHex =
                BuildPendingRxHex();

            EndReadOperation();

            ReportError(
                "RX Error",
                $"UART read operation failed: " +
                $"{exception.Message} " +
                $"Received {receivedByteCount}/" +
                $"{expectedByteCount} bytes. " +
                $"Partial HEX: {partialHex}. " +
                "COM connection closed.");
        }

        /// <summary>
        /// Tamponda bulunan fakat henüz tamamlanmamış RX verisini
        /// HEX biçiminde döndürür.
        /// </summary>
        private string BuildPendingRxHex()
        {
            if (_receivedByteCount == 0)
            {
                return "None";
            }

            return BitConverter.ToString(
                _receiveBuffer,
                startIndex: 0,
                length: _receivedByteCount);
        }

        /// <summary>
        /// Belirlenen süre içerisinde seçilen payload uzunluğu
        /// tamamlanmazsa Read işlemini sonlandırır.
        /// </summary>
        private void ReadTimeoutTimer_Tick(
            object? sender,
            EventArgs e)
        {
            if (!_isReadPending)
            {
                _readTimeoutTimer.Stop();
                return;
            }

            int expectedByteCount =
                _receiveBuffer.Length;

            int receivedByteCount =
                _receivedByteCount;

            string partialHex =
                BuildPendingRxHex();

            EndReadOperation();

            ReportWarning(
                "RX Timeout",
                $"UART reception timed out after " +
                $"{ReadTimeoutMilliseconds} ms. " +
                $"Received {receivedByteCount}/" +
                $"{expectedByteCount} bytes. " +
                $"Partial HEX: {partialHex}. " +
                "COM connection closed.");
        }

        /// <summary>
        /// Aktif Read işlemini sonlandırır, zamanlayıcıyı durdurur,
        /// RX tamponunu temizler, COM portunu kapatır ve arayüzü
        /// tekrar kullanılabilir hâle getirir.
        /// </summary>
        private void EndReadOperation()
        {
            _readTimeoutTimer.Stop();
            _isReadPending = false;

            _receiveBuffer =
                Array.Empty<byte>();

            _receivedByteCount = 0;

            try
            {
                _serialConnection.Disconnect();
            }
            finally
            {
                SetCommunicationControlsEnabled(true);
            }
        }

        #endregion

        #region Activity Log

        /// <summary>
        /// İşlem günlüğüne saat, işlem türü ve açıklama ekler.
        /// Birden fazla satır içeren kayıtların devam satırlarını
        /// ilk mesaj satırıyla aynı hizada gösterir.
        /// </summary>
        private void WriteLog(
            LogEntryType entryType,
            string message)
        {
            if (activityLogTextBox.IsDisposed)
            {
                return;
            }

            (string label, Color color) =
                entryType switch
                {
                    LogEntryType.Information =>
                        ("INFO", Color.SlateGray),

                    LogEntryType.Tx =>
                        ("TX", Color.RoyalBlue),

                    LogEntryType.Rx =>
                        ("RX", Color.DarkCyan),

                    LogEntryType.Success =>
                        ("SUCCESS", Color.SeaGreen),

                    LogEntryType.Warning =>
                        ("WARNING", Color.DarkOrange),

                    LogEntryType.Error =>
                        ("ERROR", Color.Firebrick),

                    _ =>
                        ("INFO", Color.SlateGray)
                };

            string timestampText =
                $"[{DateTime.Now:HH:mm:ss}] ";

            string labelText =
                $"{label,-10}";

            // Devam satırlarını saat ve kayıt türünün altında değil,
            // mesajın başladığı sütunda göstermek için boşluk ekler.
            string continuationIndent =
                new string(
                    ' ',
                    timestampText.Length + labelText.Length);

            string formattedMessage =
                message.Replace(
                    Environment.NewLine,
                    Environment.NewLine + continuationIndent);

            activityLogTextBox.SelectionStart =
                activityLogTextBox.TextLength;

            activityLogTextBox.SelectionLength = 0;
            activityLogTextBox.SelectionColor =
                Color.DimGray;

            activityLogTextBox.AppendText(timestampText);

            activityLogTextBox.SelectionColor = color;
            activityLogTextBox.AppendText(labelText);

            activityLogTextBox.SelectionColor =
                Color.FromArgb(45, 45, 48);

            activityLogTextBox.AppendText(
                formattedMessage + Environment.NewLine);

            activityLogTextBox.SelectionColor =
                activityLogTextBox.ForeColor;

            activityLogTextBox.SelectionStart =
                activityLogTextBox.TextLength;

            activityLogTextBox.ScrollToCaret();
        }

        private void clearLogButton_Click(
            object? sender,
            EventArgs e)
        {
            activityLogTextBox.Clear();

            WriteLog(
                LogEntryType.Information,
                "Activity log cleared.");
        }

        /// <summary>
        /// Normal çalışmayı durduran bir uyarıyı hem günlüğe yazar
        /// hem de kullanıcıya uyarı penceresi gösterir.
        /// </summary>
        private void ReportWarning(
            string title,
            string message)
        {
            WriteLog(
                LogEntryType.Warning,
                message);

            MessageBox.Show(
                this,
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Haberleşme veya sistem hatasını hem günlüğe yazar
        /// hem de kullanıcıya hata penceresi gösterir.
        /// </summary>
        private void ReportError(
            string title,
            string message)
        {
            WriteLog(
                LogEntryType.Error,
                message);

            MessageBox.Show(
                this,
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        #endregion

        #region Thread Management

        /// <summary>
        /// Seri portun arka plan thread'inden gelen işlemi Windows Forms
        /// arayüz thread'ine güvenli biçimde aktarır.
        /// </summary>
        private void PostToUiThread(Action action)
        {
            if (IsDisposed ||
                Disposing ||
                !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
                // Form durum kontrolünden hemen sonra kapanmış olabilir.
                // Kapanan arayüz için alınan byte'ın gösterilmesi gerekmez.
            }
        }

        #endregion

        #region Form Lifecycle

        /// <summary>
        /// Uygulama kapanırken zamanlayıcıyı, seri port olaylarını
        /// ve COM bağlantısını temizler.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _readTimeoutTimer.Stop();

            _readTimeoutTimer.Tick -=
                ReadTimeoutTimer_Tick;

            _serialConnection.ByteReceived -=
                SerialConnection_ByteReceived;

            _serialConnection.ReceiveError -=
                SerialConnection_ReceiveError;

            _serialConnection.Dispose();
            _readTimeoutTimer.Dispose();

            base.OnFormClosed(e);
        }

        #endregion
    }
}