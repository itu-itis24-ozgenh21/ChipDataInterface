using System;

namespace ChipDataInterface
{
    /// <summary>
    /// UART alım oturumunu, RX tamponunu, zaman aşımını ve alınan
    /// verinin arayüzde gösterilmesini yöneten MainForm bölümüdür.
    /// </summary>
    public partial class MainForm
    {
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
    }
}