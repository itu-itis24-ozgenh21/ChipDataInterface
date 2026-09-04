using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChipDataInterface
{
    /// <summary>
    /// Arayüzde tek bir biti temsil eden düğmedir.
    /// Düzenlenebilir olduğunda kullanıcı tarafından 0 ve 1 arasında değiştirilir.
    /// Salt okunur olduğunda yalnızca program tarafından güncellenebilir.
    /// </summary>
    internal sealed class BitButton : Button
    {
        private const int ButtonSize = 56;
        private const int ButtonMargin = 5;

        /// <summary>
        /// Bitin veri içerisindeki konumudur.
        /// Örneğin 8 bitlik bir veride 0 ile 7 arasındadır.
        /// </summary>
        public int BitIndex { get; }

        /// <summary>
        /// Bitin mevcut değeridir: false = 0, true = 1.
        /// </summary>
        public bool BitValue { get; private set; }

        /// <summary>
        /// Kullanıcının bu bitin değerini değiştirip değiştiremeyeceğini belirtir.
        /// Salt okunur bitler program tarafından SetValue ile güncellenebilir.
        /// </summary>
        public bool IsReadOnly { get; }

        public BitButton(int bitIndex, bool isReadOnly)
        {
            if (bitIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bitIndex),
                    bitIndex,
                    "Bit numarası negatif olamaz.");
            }

            BitIndex = bitIndex;
            IsReadOnly = isReadOnly;

            Size = new Size(ButtonSize, ButtonSize);
            Margin = new Padding(ButtonMargin);
            TabStop = !isReadOnly;
            Cursor = isReadOnly
                ? Cursors.Default
                : Cursors.Hand;

            SetValue(false);
        }

        /// <summary>
        /// Bitin mantıksal değerini ve ekranda gösterilen metni günceller.
        /// RX bitleri de gelen veriye göre bu metotla güncellenecektir.
        /// </summary>
        public void SetValue(bool value)
        {
            BitValue = value;
            Text = value ? "1" : "0";  
        }

        protected override void OnClick(EventArgs e)
        {
            // RX gibi salt okunur bitlerin kullanıcı tarafından
            // değiştirilmesine izin verilmez.
            if (IsReadOnly)
            {
                return;
            }

            SetValue(!BitValue);

            // Düğmeye bağlanmış başka Click olaylarının çalışmasını sağlar.
            base.OnClick(e);
        }
    }
}