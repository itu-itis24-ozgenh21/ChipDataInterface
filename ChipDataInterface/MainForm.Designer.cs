namespace ChipDataInterface
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainLayoutPanel = new TableLayoutPanel();
            txLabel1 = new Label();
            rxLabel1 = new Label();
            txBitsPanel = new FlowLayoutPanel();
            rxBitsPanel = new FlowLayoutPanel();
            sendButton = new Button();
            mainLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(txLabel1, 0, 0);
            mainLayoutPanel.Controls.Add(rxLabel1, 0, 1);
            mainLayoutPanel.Controls.Add(txBitsPanel, 1, 0);
            mainLayoutPanel.Controls.Add(rxBitsPanel, 1, 1);
            mainLayoutPanel.Controls.Add(sendButton, 1, 2);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.Padding = new Padding(30);
            mainLayoutPanel.RowCount = 3;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainLayoutPanel.Size = new Size(742, 393);
            mainLayoutPanel.TabIndex = 0;
            // 
            // txLabel1
            // 
            txLabel1.Dock = DockStyle.Fill;
            txLabel1.Font = new Font("Segoe UI", 16F);
            txLabel1.Location = new Point(30, 30);
            txLabel1.Margin = new Padding(0);
            txLabel1.Name = "txLabel1";
            txLabel1.Size = new Size(80, 133);
            txLabel1.TabIndex = 0;
            txLabel1.Text = "Tx";
            txLabel1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // rxLabel1
            // 
            rxLabel1.AutoSize = true;
            rxLabel1.Dock = DockStyle.Fill;
            rxLabel1.Font = new Font("Segoe UI", 16F);
            rxLabel1.Location = new Point(30, 163);
            rxLabel1.Margin = new Padding(0);
            rxLabel1.Name = "rxLabel1";
            rxLabel1.Size = new Size(80, 133);
            rxLabel1.TabIndex = 1;
            rxLabel1.Text = "Rx";
            rxLabel1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txBitsPanel
            // 
            txBitsPanel.AutoScroll = true;
            txBitsPanel.Dock = DockStyle.Fill;
            txBitsPanel.Location = new Point(110, 30);
            txBitsPanel.Margin = new Padding(0);
            txBitsPanel.Name = "txBitsPanel";
            txBitsPanel.Padding = new Padding(10, 35, 10, 10);
            txBitsPanel.Size = new Size(602, 133);
            txBitsPanel.TabIndex = 2;
            // 
            // rxBitsPanel
            // 
            rxBitsPanel.AutoScroll = true;
            rxBitsPanel.Dock = DockStyle.Fill;
            rxBitsPanel.Location = new Point(110, 163);
            rxBitsPanel.Margin = new Padding(0);
            rxBitsPanel.Name = "rxBitsPanel";
            rxBitsPanel.Padding = new Padding(10, 35, 10, 10);
            rxBitsPanel.Size = new Size(602, 133);
            rxBitsPanel.TabIndex = 3;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.None;
            sendButton.BackColor = Color.RoyalBlue;
            sendButton.Cursor = Cursors.Hand;
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.ForeColor = Color.White;
            sendButton.Location = new Point(341, 305);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(140, 48);
            sendButton.TabIndex = 0;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;
            sendButton.Click += sendButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(742, 393);
            Controls.Add(mainLayoutPanel);
            MinimumSize = new Size(640, 360);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Chip Data Interface";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private Label txLabel1;
        private Label rxLabel1;
        private FlowLayoutPanel txBitsPanel;
        private FlowLayoutPanel rxBitsPanel;
        private Button sendButton;
    }
}
