using System;
using System.IO.Ports;

namespace ChipDataInterface
{
    /// <summary>
    /// COM portunun listelenmesi, bağlantı ayarlarının doğrulanması ve
    /// haberleşme kontrollerinin yönetilmesiyle ilgili MainForm bölümüdür.
    /// </summary>
    public partial class MainForm
    {
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
    }
}
