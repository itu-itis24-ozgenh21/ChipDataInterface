using System;
using System.Windows.Forms;

namespace ChipDataInterface
{
    /// <summary>
    /// Seçilen payload uzunluğuna göre TX ve RX giriş/gösterim
    /// kontrollerini oluşturan MainForm bölümüdür.
    /// </summary>
    public partial class MainForm
    {
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
    }
}
