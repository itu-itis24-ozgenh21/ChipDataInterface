using System.Collections.Generic;
using System.Windows.Forms;

namespace ChipDataInterface
{
    public partial class MainForm : Form
    {
        private const int DefaultBitCount = 8;

        private readonly List<BitButton> _txBitButtons = new();
        private readonly List<BitButton> _rxBitButtons = new();

        public MainForm()
        {
            InitializeComponent();

            CreateTxBitButtons();
            CreateRxBitButtons();
        }

        private void CreateTxBitButtons()
        {
            txBitsPanel.Controls.Clear();
            _txBitButtons.Clear();

            for (int bitIndex = DefaultBitCount - 1; bitIndex >= 0; bitIndex--)
            {
                BitButton bitButton = new(bitIndex, isReadOnly: false)
                {
                    Name = $"txBit{bitIndex}Button"
                };

                _txBitButtons.Add(bitButton);
                txBitsPanel.Controls.Add(bitButton);
            }
        }
        private void CreateRxBitButtons()
        {
            rxBitsPanel.Controls.Clear();
            _rxBitButtons.Clear();

            for (int bitIndex = DefaultBitCount - 1; bitIndex >= 0; bitIndex--)
            {
                BitButton bitButton = new(bitIndex, isReadOnly: true)
                {
                    Name = $"rxBit{bitIndex}Button"
                };

                _rxBitButtons.Add(bitButton);
                rxBitsPanel.Controls.Add(bitButton);
            }

        }

        private void sendButton_Click(object sender, EventArgs e)
        {

        }
    }
}