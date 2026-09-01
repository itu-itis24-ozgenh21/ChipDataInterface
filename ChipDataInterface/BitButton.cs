using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChipDataInterface
{
    internal class BitButton : Button
    {
        public int BitIndex { get; }

        public bool BitValue { get; private set; }

        public bool IsReadOnly { get; }

        public BitButton(int bitIndex, bool isReadOnly)
        {
            BitIndex = bitIndex;
            IsReadOnly = isReadOnly;

            Text = "0";
            Size = new Size(56, 56);
            Margin = new Padding(5);
            TabStop = !isReadOnly;
        }

        public void SetValue(bool value)
        {
            BitValue = value;
            Text = value ? "1" : "0";
        }

        protected override void OnClick(EventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            SetValue(!BitValue);
            base.OnClick(e);
        }
    }
}