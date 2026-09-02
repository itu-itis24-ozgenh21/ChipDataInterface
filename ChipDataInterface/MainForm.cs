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

        #endregion

        #region Types

        /// <summary>
        /// Günlük kayıtlarının türünü ve ekranda kullanılacak rengini belirler.
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

        // Read işlemi sırasında veri gelmezse bağlantıyı kapatır.
        private readonly System.Windows.Forms.Timer _readTimeoutTimer = new();

        // Yalnızca aktif bir Read sırasında gelen ilk byte'ın
        // RX alanında işlenmesini sağlar.
        private bool _isReadPending;

        #endregion

        #region Initialization

        public MainForm()
        {
            InitializeComponent();

            CreateTxBitButtons();
            CreateRxBitButtons();
            InitializeReadOperation();
            RegisterEventHandlers();

            WriteLog(
                LogEntryType.Information,
                "Application ready. Select COM port and baud rate.");

            InitializeConnectionOptions();
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

            _readTimeoutTimer.Tick +=
                ReadTimeoutTimer_Tick;

            _serialConnection.ByteReceived +=
                SerialConnection_ByteReceived;

            _serialConnection.ReceiveError +=
                SerialConnection_ReceiveError;
        }

        #endregion

        #region Bit Buttons

        /// <summary>
        /// Kullanıcı tarafından değiştirilebilen TX bitlerini oluşturur.
        /// Bitler ekranda soldan sağa 7, 6, ..., 0 şeklinde gösterilir.
        /// </summary>
        private void CreateTxBitButtons()
        {
            txBitsPanel.Controls.Clear();
            _txBitButtons.Clear();

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
        /// Alınan veriyi gösterecek, kullanıcı tarafından değiştirilemeyen
        /// RX bitlerini oluşturur.
        /// </summary>
        private void CreateRxBitButtons()
        {
            rxBitsPanel.Controls.Clear();
            _rxBitButtons.Clear();

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
        /// Bit butonlarını ekranda görülen sırayla metne dönüştürür.
        /// Örneğin bit 7-0 değerleri 10101100 biçiminde gösterilir.
        /// </summary>
        private static string BuildBitString(
            IReadOnlyList<BitButton> bitButtons)
        {
            char[] bitCharacters =
                new char[bitButtons.Count];

            for (int index = 0;
                 index < bitButtons.Count;
                 index++)
            {
                bitCharacters[index] =
                    bitButtons[index].BitValue ? '1' : '0';
            }

            return new string(bitCharacters);
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
        /// Haberleşme sırasında port, baud rate ve işlem butonlarının
        /// değiştirilmesini engeller.
        ///
        /// Günlük temizleme butonu iletişimden bağımsız olduğu için
        /// kullanılabilir durumda bırakılır.
        /// </summary>
        private void SetCommunicationControlsEnabled(bool enabled)
        {
            portComboBox.Enabled = enabled;
            baudRateComboBox.Enabled = enabled;
            refreshPortsButton.Enabled = enabled;
            sendButton.Enabled = enabled;
            readButton.Enabled = enabled;
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
        /// TX butonlarında seçilen bitleri RAW byte dizisine dönüştürür.
        ///
        /// Mevcut 8 bitlik arayüzde sonuç tek byte'tır.
        /// Örneğin 10101100 seçimi 0xAC byte'ını üretir.
        ///
        /// Bit sayısı ileride artırılırsa ilk byte bit 0-7'yi,
        /// ikinci byte bit 8-15'i içerir.
        /// </summary>
        private byte[] BuildTxData()
        {
            int byteCount =
                (_txBitButtons.Count + BitsPerByte - 1)
                / BitsPerByte;

            byte[] txData =
                new byte[byteCount];

            foreach (BitButton bitButton in _txBitButtons)
            {
                if (!bitButton.BitValue)
                {
                    continue;
                }

                int byteIndex =
                    bitButton.BitIndex / BitsPerByte;

                int bitPosition =
                    bitButton.BitIndex % BitsPerByte;

                txData[byteIndex] |=
                    (byte)(1 << bitPosition);
            }

            return txData;
        }

        /// <summary>
        /// Gönderilen TX verisini günlükte gösterilecek biçime dönüştürür.
        /// </summary>
        private string BuildTxSummary(byte[] txData)
        {
            string bitData =
                BuildBitString(_txBitButtons);

            string hexData =
                BitConverter.ToString(txData);

            return $"{BuildConnectionSummary()} | " +
                   $"Bits: {bitData} | " +
                   $"{txData.Length} byte ({DataFormatName}) | " +
                   $"HEX: {hexData}";
        }

        /// <summary>
        /// TX verisini seçilen UART bağlantısına gönderir.
        ///
        /// Başarılı işlem yalnızca işlem günlüğüne yazılır.
        /// Hata veya eksik seçim varsa hem günlüğe yazılır hem
        /// kullanıcıya açılır pencere gösterilir.
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
                    $"Transmission started | {txSummary}");

                _serialConnection.Send(txData);

                // Cihaz sürekli iletişimde bırakılmayacağı için gönderim
                // tamamlanır tamamlanmaz COM portu kapatılır.
                _serialConnection.Disconnect();

                WriteLog(
                    LogEntryType.Success,
                    $"TX completed | {txSummary} | " +
                    "COM connection closed.");
            }
            catch (InvalidOperationException exception)
            {
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
        /// Read butonuna basıldığında COM portunu geçici olarak açar
        /// ve hedef cihazın göndereceği ilk byte'ı bekler.
        ///
        /// Bu metot hedef cihaza okuma komutu göndermez.
        /// EEPROM verisini okuyup UART'a gönderme işlemi hedef cihazın
        /// kendi yazılımı tarafından gerçekleştirilmelidir.
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

            // Port açılır açılmaz byte gelebileceği için bekleme durumu
            // bağlantı kurulmadan önce etkinleştirilir.
            _isReadPending = true;

            try
            {
                EnsureSerialConnection();

                WriteLog(
                    LogEntryType.Rx,
                    $"RX listening started | " +
                    $"{BuildConnectionSummary()} | " +
                    $"Waiting up to {ReadTimeoutMilliseconds} ms.");

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
        /// Alınan byte'ın her bitini karşılık gelen RX butonunda gösterir.
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
        /// Alınan RX verisini günlükte gösterilecek biçime dönüştürür.
        /// </summary>
        private string BuildRxSummary(byte receivedByte)
        {
            string bitData =
                BuildBitString(_rxBitButtons);

            return $"{BuildConnectionSummary()} | " +
                   $"Bits: {bitData} | " +
                   $"1 byte ({DataFormatName}) | " +
                   $"HEX: {receivedByte:X2}";
        }

        /// <summary>
        /// SerialPort veri alma olayı arayüz thread'inden farklı bir
        /// thread üzerinde çalışabileceği için işlem arayüz thread'ine aktarılır.
        /// </summary>
        private void SerialConnection_ByteReceived(byte receivedByte)
        {
            PostToUiThread(
                () => HandleReceivedByte(receivedByte));
        }

        /// <summary>
        /// Read başladıktan sonra alınan ilk byte'ı RX alanında gösterir.
        /// Daha sonra gelen byte'lar yeni bir Read başlatılana kadar işlenmez.
        /// </summary>
        private void HandleReceivedByte(byte receivedByte)
        {
            if (!_isReadPending)
            {
                return;
            }

            DisplayRxByte(receivedByte);

            string rxSummary =
                BuildRxSummary(receivedByte);

            EndReadOperation();

            WriteLog(
                LogEntryType.Success,
                $"RX completed | {rxSummary} | " +
                "COM connection closed.");
        }

        /// <summary>
        /// Arka plandaki RX işlemi sırasında oluşan hatayı
        /// arayüz thread'ine aktarır.
        /// </summary>
        private void SerialConnection_ReceiveError(
            Exception exception)
        {
            PostToUiThread(
                () => HandleReceiveError(exception));
        }

        /// <summary>
        /// Aktif Read sırasında oluşan okuma hatasını işler.
        /// </summary>
        private void HandleReceiveError(Exception exception)
        {
            if (!_isReadPending)
            {
                return;
            }

            EndReadOperation();

            ReportError(
                "RX Error",
                $"UART read operation failed: " +
                exception.Message);
        }

        /// <summary>
        /// Belirlenen süre içerisinde byte alınmazsa Read işlemini
        /// sonlandırır ve kullanıcıya uyarı verir.
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

            EndReadOperation();

            ReportWarning(
                "RX Timeout",
                $"{ReadTimeoutMilliseconds} ms without receiving UART data. " +
                "Reception failed. COM connection closed.");
        }

        /// <summary>
        /// Read işlemini sonlandırır, zamanlayıcıyı durdurur,
        /// COM portunu kapatır ve arayüzü tekrar kullanılabilir yapar.
        /// </summary>
        private void EndReadOperation()
        {
            _readTimeoutTimer.Stop();
            _isReadPending = false;

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
        /// Kayıt türleri farklı renklerle gösterilir.
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

            activityLogTextBox.SelectionStart =
                activityLogTextBox.TextLength;

            activityLogTextBox.SelectionLength = 0;
            activityLogTextBox.SelectionColor =
                Color.DimGray;

            activityLogTextBox.AppendText(
                $"[{DateTime.Now:HH:mm:ss}] ");

            activityLogTextBox.SelectionColor = color;

            activityLogTextBox.AppendText(
                $"{label,-10}");

            activityLogTextBox.SelectionColor =
                Color.FromArgb(45, 45, 48);

            activityLogTextBox.AppendText(
                $"{message}{Environment.NewLine}");

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