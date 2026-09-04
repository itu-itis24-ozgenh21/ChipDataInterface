using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ChipDataInterface
{
    /// <summary>
    /// Bir byte değerini 00 ile FF arasında hexadecimal biçimde
    /// girmek veya görüntülemek için kullanılan metin kutusudur.
    /// </summary>
    internal sealed class HexByteTextBox : TextBox
    {
        private const int FieldWidth = 64;
        private const int FieldHeight = 38;

        /// <summary>
        /// Kutunun 8 byte'lık veri içerisindeki sırasıdır.
        /// İlk gönderilecek kutunun indeksi sıfırdır.
        /// </summary>
        public int ByteIndex { get; }

        public HexByteTextBox(int byteIndex, bool isReadOnly)
        {
            if (byteIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(byteIndex),
                    byteIndex,
                    "Byte indeksi negatif olamaz.");
            }

            ByteIndex = byteIndex;

            Name = $"byte{byteIndex}TextBox";
            Size = new Size(FieldWidth, FieldHeight);
            Margin = new Padding(5);
            MaxLength = 2;
            CharacterCasing = CharacterCasing.Upper;
            TextAlign = HorizontalAlignment.Center;

            ReadOnly = isReadOnly;
            TabStop = !isReadOnly;

            SetValue(0);
        }

        /// <summary>
        /// Kutudaki hexadecimal yazıyı gerçek byte değerine dönüştürür.
        /// </summary>
        public byte GetValue()
        {
            if (TryGetValue(out byte value))
            {
                return value;
            }

            throw new FormatException(
                $"Byte {ByteIndex} için 00 ile FF arasında " +
                "geçerli bir hexadecimal değer girilmelidir.");
        }

        public bool TryGetValue(out byte value)
        {
            return byte.TryParse(
                Text.Trim(),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out value);
        }

        /// <summary>
        /// Verilen byte değerini iki karakterli HEX olarak gösterir.
        /// Örnek: 10 değeri 0A şeklinde gösterilir.
        /// </summary>
        public void SetValue(byte value)
        {
            Text = value.ToString(
                "X2",
                CultureInfo.InvariantCulture);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                base.OnKeyPress(e);
                return;
            }

            if (!IsHexDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.KeyChar = char.ToUpperInvariant(e.KeyChar);
            base.OnKeyPress(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            // Kullanıcı A yazdıysa kutudan çıkarken 0A biçimine getirir.
            if (TryGetValue(out byte value))
            {
                SetValue(value);
            }

            base.OnLeave(e);
        }

        private static bool IsHexDigit(char character)
        {
            return character is >= '0' and <= '9'
                or >= 'A' and <= 'F'
                or >= 'a' and <= 'f';
        }
    }
}