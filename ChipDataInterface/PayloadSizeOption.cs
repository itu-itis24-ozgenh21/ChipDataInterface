using System;

namespace ChipDataInterface
{
    /// <summary>
    /// Kullanıcının seçebileceği veri uzunluğunu temsil eder.
    /// ComboBox'ta görünen metni ve UART üzerinden aktarılacak
    /// gerçek byte sayısını birlikte tutar.
    /// </summary>
    internal sealed class PayloadSizeOption
    {
        /// <summary>
        /// UART üzerinden gönderilecek veya okunacak byte sayısıdır.
        /// </summary>
        public int ByteCount { get; }

        /// <summary>
        /// Byte sayısının bit karşılığıdır.
        /// Örnek: 8 byte = 64 bit.
        /// </summary>
        public int BitCount => checked(ByteCount * 8);

        /// <summary>
        /// ComboBox içerisinde kullanıcıya gösterilecek açıklamadır.
        /// </summary>
        public string DisplayText { get; }

        public PayloadSizeOption(
            int byteCount,
            string displayText)
        {
            if (byteCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(byteCount),
                    byteCount,
                    "Byte sayısı sıfırdan büyük olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(displayText))
            {
                throw new ArgumentException(
                    "Gösterilecek seçenek metni boş olamaz.",
                    nameof(displayText));
            }

            ByteCount = byteCount;
            DisplayText = displayText;
        }

        public override string ToString()
        {
            // ComboBox bu metodu kullanarak seçeneğin adını gösterir.
            return DisplayText;
        }
    }
}