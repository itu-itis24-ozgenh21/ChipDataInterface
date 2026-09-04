using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChipDataInterface
{
    /// <summary>
    /// Activity Log kayıtlarını ve kullanıcıya gösterilen uyarı/hata
    /// bildirimlerini yöneten MainForm bölümüdür.
    /// </summary>
    public partial class MainForm
    {
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
    }
}
