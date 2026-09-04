using System;
using System.Collections.Generic;

namespace ChipDataInterface
{
    /// <summary>
    /// Arayüzdeki TX değerlerini RAW byte dizisine dönüştüren ve
    /// UART üzerinden gönderen MainForm bölümüdür.
    /// </summary>
    public partial class MainForm
    {
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
    }
}
