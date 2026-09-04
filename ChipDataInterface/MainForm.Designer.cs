namespace ChipDataInterface
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            mainLayoutPanel = new TableLayoutPanel();
            headerPanel = new TableLayoutPanel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            connectionPanel = new FlowLayoutPanel();
            portLabel = new Label();
            portComboBoxBorderPanel = new Panel();
            portComboBox = new ComboBox();
            refreshPortsButton = new Button();
            baudRateLabel = new Label();
            baudRateComboBoxBorderPanel = new Panel();
            baudRateComboBox = new ComboBox();
            payloadSizeLabel = new Label();
            payloadSizeComboBoxBorderPanel = new Panel();
            payloadSizeComboBox = new ComboBox();
            txLabel = new Label();
            txBitsPanel = new FlowLayoutPanel();
            rxLabel = new Label();
            rxBitsPanel = new FlowLayoutPanel();
            actionButtonsPanel = new FlowLayoutPanel();
            sendButton = new Button();
            readButton = new Button();
            logContainerPanel = new TableLayoutPanel();
            logTitleLabel = new Label();
            clearLogButton = new Button();
            activityLogTextBox = new RichTextBox();

            mainLayoutPanel.SuspendLayout();
            headerPanel.SuspendLayout();
            connectionPanel.SuspendLayout();
            portComboBoxBorderPanel.SuspendLayout();
            baudRateComboBoxBorderPanel.SuspendLayout();
            payloadSizeComboBoxBorderPanel.SuspendLayout();
            actionButtonsPanel.SuspendLayout();
            logContainerPanel.SuspendLayout();
            SuspendLayout();

            //
            // mainLayoutPanel
            //
            mainLayoutPanel.BackColor =
                Color.FromArgb(243, 246, 251);
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 104F));
            mainLayoutPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(headerPanel, 0, 0);
            mainLayoutPanel.SetColumnSpan(headerPanel, 2);
            mainLayoutPanel.Controls.Add(connectionPanel, 0, 1);
            mainLayoutPanel.SetColumnSpan(connectionPanel, 2);
            mainLayoutPanel.Controls.Add(txLabel, 0, 2);
            mainLayoutPanel.Controls.Add(txBitsPanel, 1, 2);
            mainLayoutPanel.Controls.Add(rxLabel, 0, 3);
            mainLayoutPanel.Controls.Add(rxBitsPanel, 1, 3);
            mainLayoutPanel.Controls.Add(actionButtonsPanel, 0, 4);
            mainLayoutPanel.SetColumnSpan(actionButtonsPanel, 2);
            mainLayoutPanel.Controls.Add(logContainerPanel, 0, 5);
            mainLayoutPanel.SetColumnSpan(logContainerPanel, 2);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.GrowStyle =
                TableLayoutPanelGrowStyle.FixedSize;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.Padding = new Padding(20);
            mainLayoutPanel.RowCount = 6;
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 86F));
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 82F));
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 116F));
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 116F));
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 72F));
            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Size = new Size(940, 740);
            mainLayoutPanel.TabIndex = 0;

            //
            // headerPanel
            //
            headerPanel.BackColor =
                Color.FromArgb(30, 41, 59);
            headerPanel.ColumnCount = 1;
            headerPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            headerPanel.Controls.Add(titleLabel, 0, 0);
            headerPanel.Controls.Add(subtitleLabel, 0, 1);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(20, 20);
            headerPanel.Margin = new Padding(0, 0, 0, 12);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(20, 10, 20, 8);
            headerPanel.RowCount = 2;
            headerPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 36F));
            headerPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            headerPanel.Size = new Size(900, 74);
            headerPanel.TabIndex = 0;

            //
            // titleLabel
            //
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font(
                "Segoe UI",
                16F,
                FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 10);
            titleLabel.Margin = new Padding(0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(860, 36);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "UART Data Interface";
            titleLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            //
            // subtitleLabel
            //
            subtitleLabel.Dock = DockStyle.Fill;
            subtitleLabel.Font = new Font("Segoe UI", 9F);
            subtitleLabel.ForeColor =
                Color.FromArgb(203, 213, 225);
            subtitleLabel.Location = new Point(20, 46);
            subtitleLabel.Margin = new Padding(0);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(860, 20);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text =
                "FTDI USB-UART  •  RAW byte transmission  •  8N1";
            subtitleLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            //
            // connectionPanel
            //
            connectionPanel.AutoScroll = true;
            connectionPanel.BackColor = Color.White;
            connectionPanel.BorderStyle = BorderStyle.FixedSingle;
            connectionPanel.Controls.Add(portLabel);
            connectionPanel.Controls.Add(portComboBoxBorderPanel);
            connectionPanel.Controls.Add(refreshPortsButton);
            connectionPanel.Controls.Add(baudRateLabel);
            connectionPanel.Controls.Add(baudRateComboBoxBorderPanel);
            connectionPanel.Controls.Add(payloadSizeLabel);
            connectionPanel.Controls.Add(payloadSizeComboBoxBorderPanel);
            connectionPanel.Dock = DockStyle.Fill;
            connectionPanel.FlowDirection =
                FlowDirection.LeftToRight;
            connectionPanel.Location = new Point(20, 106);
            connectionPanel.Margin = new Padding(0, 0, 0, 12);
            connectionPanel.Name = "connectionPanel";
            connectionPanel.Padding = new Padding(16, 13, 16, 12);
            connectionPanel.Size = new Size(900, 70);
            connectionPanel.TabIndex = 1;
            connectionPanel.WrapContents = false;

            //
            // portLabel
            //
            portLabel.AutoSize = true;
            portLabel.Font = new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold);
            portLabel.ForeColor =
                Color.FromArgb(51, 65, 85);
            portLabel.Margin = new Padding(0, 9, 8, 0);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(75, 21);
            portLabel.TabIndex = 0;
            portLabel.Text = "COM Port";

            //
            // portComboBoxBorderPanel
            //
            portComboBoxBorderPanel.BackColor =
                Color.FromArgb(100, 116, 139);
            portComboBoxBorderPanel.Controls.Add(portComboBox);
            portComboBoxBorderPanel.Location = new Point(99, 15);
            portComboBoxBorderPanel.Margin =
                new Padding(0, 2, 10, 0);
            portComboBoxBorderPanel.Name =
                "portComboBoxBorderPanel";
            portComboBoxBorderPanel.Padding = new Padding(1);
            portComboBoxBorderPanel.Size = new Size(100, 34);
            portComboBoxBorderPanel.TabIndex = 0;

            //
            // portComboBox
            //
            portComboBox.BackColor =
                Color.FromArgb(248, 250, 252);
            portComboBox.Dock = DockStyle.Fill;
            portComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;
            portComboBox.FlatStyle = FlatStyle.Flat;
            portComboBox.Font = new Font("Segoe UI", 9.5F);
            portComboBox.ForeColor =
                Color.FromArgb(15, 23, 42);
            portComboBox.FormattingEnabled = true;
            portComboBox.Location = new Point(1, 1);
            portComboBox.Margin = new Padding(0);
            portComboBox.Name = "portComboBox";
            portComboBox.Size = new Size(98, 29);
            portComboBox.TabIndex = 0;

            //
            // refreshPortsButton
            //
            refreshPortsButton.BackColor =
                Color.FromArgb(239, 246, 255);
            refreshPortsButton.Cursor = Cursors.Hand;
            refreshPortsButton.FlatAppearance.BorderColor =
                Color.FromArgb(147, 197, 253);
            refreshPortsButton.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(219, 234, 254);
            refreshPortsButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(219, 234, 254);
            refreshPortsButton.FlatStyle = FlatStyle.Flat;
            refreshPortsButton.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold);
            refreshPortsButton.ForeColor =
                Color.FromArgb(37, 99, 235);
            refreshPortsButton.Margin = new Padding(0, 1, 18, 0);
            refreshPortsButton.Name = "refreshPortsButton";
            refreshPortsButton.Size = new Size(86, 36);
            refreshPortsButton.TabIndex = 1;
            refreshPortsButton.Text = "Refresh";
            refreshPortsButton.UseVisualStyleBackColor = false;

            //
            // baudRateLabel
            //
            baudRateLabel.AutoSize = true;
            baudRateLabel.Font = new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold);
            baudRateLabel.ForeColor =
                Color.FromArgb(51, 65, 85);
            baudRateLabel.Margin = new Padding(0, 9, 8, 0);
            baudRateLabel.Name = "baudRateLabel";
            baudRateLabel.Size = new Size(83, 21);
            baudRateLabel.TabIndex = 0;
            baudRateLabel.Text = "Baud Rate";

            //
            // baudRateComboBoxBorderPanel
            //
            baudRateComboBoxBorderPanel.BackColor =
                Color.FromArgb(100, 116, 139);
            baudRateComboBoxBorderPanel.Controls.Add(baudRateComboBox);
            baudRateComboBoxBorderPanel.Location = new Point(396, 15);
            baudRateComboBoxBorderPanel.Margin =
                new Padding(0, 2, 18, 0);
            baudRateComboBoxBorderPanel.Name =
                "baudRateComboBoxBorderPanel";
            baudRateComboBoxBorderPanel.Padding = new Padding(1);
            baudRateComboBoxBorderPanel.Size = new Size(112, 34);
            baudRateComboBoxBorderPanel.TabIndex = 2;

            //
            // baudRateComboBox
            //
            baudRateComboBox.BackColor =
                Color.FromArgb(248, 250, 252);
            baudRateComboBox.Dock = DockStyle.Fill;
            baudRateComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;
            baudRateComboBox.FlatStyle = FlatStyle.Flat;
            baudRateComboBox.Font = new Font("Segoe UI", 9.5F);
            baudRateComboBox.ForeColor =
                Color.FromArgb(15, 23, 42);
            baudRateComboBox.FormattingEnabled = true;
            baudRateComboBox.Location = new Point(1, 1);
            baudRateComboBox.Margin = new Padding(0);
            baudRateComboBox.Name = "baudRateComboBox";
            baudRateComboBox.Size = new Size(110, 29);
            baudRateComboBox.TabIndex = 2;

            //
            // payloadSizeLabel
            //
            payloadSizeLabel.AutoSize = true;
            payloadSizeLabel.Font = new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold);
            payloadSizeLabel.ForeColor =
                Color.FromArgb(51, 65, 85);
            payloadSizeLabel.Margin = new Padding(0, 9, 8, 0);
            payloadSizeLabel.Name = "payloadSizeLabel";
            payloadSizeLabel.Size = new Size(100, 21);
            payloadSizeLabel.TabIndex = 0;
            payloadSizeLabel.Text = "Payload Size";

            //
            // payloadSizeComboBoxBorderPanel
            //
            payloadSizeComboBoxBorderPanel.BackColor =
                Color.FromArgb(100, 116, 139);
            payloadSizeComboBoxBorderPanel.Controls.Add(payloadSizeComboBox);
            payloadSizeComboBoxBorderPanel.Location = new Point(634, 15);
            payloadSizeComboBoxBorderPanel.Margin =
                new Padding(0, 2, 0, 0);
            payloadSizeComboBoxBorderPanel.Name =
                "payloadSizeComboBoxBorderPanel";
            payloadSizeComboBoxBorderPanel.Padding = new Padding(1);
            payloadSizeComboBoxBorderPanel.Size = new Size(170, 34);
            payloadSizeComboBoxBorderPanel.TabIndex = 3;

            //
            // payloadSizeComboBox
            //
            payloadSizeComboBox.BackColor =
                Color.FromArgb(248, 250, 252);
            payloadSizeComboBox.Dock = DockStyle.Fill;
            payloadSizeComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;
            payloadSizeComboBox.FlatStyle = FlatStyle.Flat;
            payloadSizeComboBox.Font = new Font("Segoe UI", 9.5F);
            payloadSizeComboBox.ForeColor =
                Color.FromArgb(15, 23, 42);
            payloadSizeComboBox.FormattingEnabled = true;
            payloadSizeComboBox.Location = new Point(1, 1);
            payloadSizeComboBox.Margin = new Padding(0);
            payloadSizeComboBox.Name = "payloadSizeComboBox";
            payloadSizeComboBox.Size = new Size(168, 29);
            payloadSizeComboBox.TabIndex = 3;

            //
            // txLabel
            //
            txLabel.BackColor =
                Color.FromArgb(239, 246, 255);
            txLabel.BorderStyle = BorderStyle.FixedSingle;
            txLabel.Dock = DockStyle.Fill;
            txLabel.Font = new Font(
                "Segoe UI",
                15F,
                FontStyle.Bold);
            txLabel.ForeColor =
                Color.FromArgb(37, 99, 235);
            txLabel.Location = new Point(20, 188);
            txLabel.Margin = new Padding(0, 0, 0, 10);
            txLabel.Name = "txLabel";
            txLabel.Size = new Size(104, 106);
            txLabel.TabIndex = 0;
            txLabel.Text = "TX";
            txLabel.TextAlign = ContentAlignment.MiddleCenter;

            //
            // txBitsPanel
            //
            txBitsPanel.AutoScroll = true;
            txBitsPanel.BackColor = Color.White;
            txBitsPanel.BorderStyle = BorderStyle.FixedSingle;
            txBitsPanel.Dock = DockStyle.Fill;
            txBitsPanel.FlowDirection =
                FlowDirection.LeftToRight;
            txBitsPanel.Location = new Point(124, 188);
            txBitsPanel.Margin = new Padding(0, 0, 0, 10);
            txBitsPanel.Name = "txBitsPanel";
            txBitsPanel.Padding = new Padding(18, 24, 18, 12);
            txBitsPanel.Size = new Size(796, 106);
            txBitsPanel.TabIndex = 4;
            txBitsPanel.WrapContents = false;

            //
            // rxLabel
            //
            rxLabel.BackColor =
                Color.FromArgb(240, 253, 250);
            rxLabel.BorderStyle = BorderStyle.FixedSingle;
            rxLabel.Dock = DockStyle.Fill;
            rxLabel.Font = new Font(
                "Segoe UI",
                15F,
                FontStyle.Bold);
            rxLabel.ForeColor =
                Color.FromArgb(13, 148, 136);
            rxLabel.Location = new Point(20, 304);
            rxLabel.Margin = new Padding(0, 0, 0, 10);
            rxLabel.Name = "rxLabel";
            rxLabel.Size = new Size(104, 106);
            rxLabel.TabIndex = 0;
            rxLabel.Text = "RX";
            rxLabel.TextAlign = ContentAlignment.MiddleCenter;

            //
            // rxBitsPanel
            //
            rxBitsPanel.AutoScroll = true;
            rxBitsPanel.BackColor = Color.White;
            rxBitsPanel.BorderStyle = BorderStyle.FixedSingle;
            rxBitsPanel.Dock = DockStyle.Fill;
            rxBitsPanel.FlowDirection =
                FlowDirection.LeftToRight;
            rxBitsPanel.Location = new Point(124, 304);
            rxBitsPanel.Margin = new Padding(0, 0, 0, 10);
            rxBitsPanel.Name = "rxBitsPanel";
            rxBitsPanel.Padding = new Padding(18, 24, 18, 12);
            rxBitsPanel.Size = new Size(796, 106);
            rxBitsPanel.TabIndex = 5;
            rxBitsPanel.WrapContents = false;

            //
            // actionButtonsPanel
            //
            actionButtonsPanel.Anchor = AnchorStyles.None;
            actionButtonsPanel.AutoSize = true;
            actionButtonsPanel.AutoSizeMode =
                AutoSizeMode.GrowAndShrink;
            actionButtonsPanel.BackColor = Color.Transparent;
            actionButtonsPanel.Controls.Add(sendButton);
            actionButtonsPanel.Controls.Add(readButton);
            actionButtonsPanel.FlowDirection =
                FlowDirection.LeftToRight;
            actionButtonsPanel.Location = new Point(306, 416);
            actionButtonsPanel.Margin = new Padding(0);
            actionButtonsPanel.Name = "actionButtonsPanel";
            actionButtonsPanel.Size = new Size(328, 60);
            actionButtonsPanel.TabIndex = 6;
            actionButtonsPanel.WrapContents = false;

            //
            // sendButton
            //
            sendButton.BackColor =
                Color.FromArgb(37, 99, 235);
            sendButton.Cursor = Cursors.Hand;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(30, 64, 175);
            sendButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(29, 78, 216);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold);
            sendButton.ForeColor = Color.White;
            sendButton.Margin = new Padding(7, 6, 7, 6);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(150, 48);
            sendButton.TabIndex = 5;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;

            //
            // readButton
            //
            readButton.BackColor =
                Color.FromArgb(13, 148, 136);
            readButton.Cursor = Cursors.Hand;
            readButton.FlatAppearance.BorderSize = 0;
            readButton.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(17, 94, 89);
            readButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(15, 118, 110);
            readButton.FlatStyle = FlatStyle.Flat;
            readButton.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold);
            readButton.ForeColor = Color.White;
            readButton.Margin = new Padding(7, 6, 7, 6);
            readButton.Name = "readButton";
            readButton.Size = new Size(150, 48);
            readButton.TabIndex = 6;
            readButton.Text = "Read";
            readButton.UseVisualStyleBackColor = false;

            //
            // logContainerPanel
            //
            logContainerPanel.BackColor = Color.White;
            logContainerPanel.BorderStyle =
                BorderStyle.FixedSingle;
            logContainerPanel.ColumnCount = 2;
            logContainerPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            logContainerPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 100F));
            logContainerPanel.Controls.Add(logTitleLabel, 0, 0);
            logContainerPanel.Controls.Add(clearLogButton, 1, 0);
            logContainerPanel.Controls.Add(activityLogTextBox, 0, 1);
            logContainerPanel.SetColumnSpan(activityLogTextBox, 2);
            logContainerPanel.Dock = DockStyle.Fill;
            logContainerPanel.Location = new Point(20, 486);
            logContainerPanel.Margin = new Padding(0, 6, 0, 0);
            logContainerPanel.Name = "logContainerPanel";
            logContainerPanel.Padding = new Padding(14, 10, 14, 14);
            logContainerPanel.RowCount = 2;
            logContainerPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40F));
            logContainerPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            logContainerPanel.Size = new Size(900, 234);
            logContainerPanel.TabIndex = 7;

            //
            // logTitleLabel
            //
            logTitleLabel.Dock = DockStyle.Fill;
            logTitleLabel.Font = new Font(
                "Segoe UI",
                10.5F,
                FontStyle.Bold);
            logTitleLabel.ForeColor =
                Color.FromArgb(30, 41, 59);
            logTitleLabel.Location = new Point(14, 10);
            logTitleLabel.Margin = new Padding(0);
            logTitleLabel.Name = "logTitleLabel";
            logTitleLabel.Size = new Size(770, 40);
            logTitleLabel.TabIndex = 0;
            logTitleLabel.Text = "Activity Log";
            logTitleLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            //
            // clearLogButton
            //
            clearLogButton.Anchor = AnchorStyles.Right;
            clearLogButton.BackColor =
                Color.FromArgb(248, 250, 252);
            clearLogButton.Cursor = Cursors.Hand;
            clearLogButton.FlatAppearance.BorderColor =
                Color.FromArgb(203, 213, 225);
            clearLogButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(241, 245, 249);
            clearLogButton.FlatStyle = FlatStyle.Flat;
            clearLogButton.Font = new Font("Segoe UI", 9F);
            clearLogButton.ForeColor =
                Color.FromArgb(71, 85, 105);
            clearLogButton.Location = new Point(791, 15);
            clearLogButton.Margin = new Padding(6, 5, 0, 5);
            clearLogButton.Name = "clearLogButton";
            clearLogButton.Size = new Size(80, 30);
            clearLogButton.TabIndex = 7;
            clearLogButton.Text = "Clear";
            clearLogButton.UseVisualStyleBackColor = false;

            //
            // activityLogTextBox
            //
            activityLogTextBox.BackColor =
                Color.FromArgb(248, 250, 252);
            activityLogTextBox.BorderStyle =
                BorderStyle.FixedSingle;
            activityLogTextBox.DetectUrls = false;
            activityLogTextBox.Dock = DockStyle.Fill;
            activityLogTextBox.Font =
                new Font("Consolas", 9F);
            activityLogTextBox.ForeColor =
                Color.FromArgb(45, 45, 48);
            activityLogTextBox.Location =
                new Point(14, 56);
            activityLogTextBox.Margin =
                new Padding(0, 6, 0, 0);
            activityLogTextBox.Name =
                "activityLogTextBox";
            activityLogTextBox.ReadOnly = true;
            activityLogTextBox.ScrollBars =
                RichTextBoxScrollBars.Vertical;
            activityLogTextBox.Size =
                new Size(856, 163);
            activityLogTextBox.TabIndex = 8;
            activityLogTextBox.TabStop = false;
            activityLogTextBox.Text = "";
            activityLogTextBox.WordWrap = true;

            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(243, 246, 251);
            ClientSize = new Size(940, 740);
            Controls.Add(mainLayoutPanel);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(860, 680);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "UART Data Interface";

            logContainerPanel.ResumeLayout(false);
            actionButtonsPanel.ResumeLayout(false);
            payloadSizeComboBoxBorderPanel.ResumeLayout(false);
            baudRateComboBoxBorderPanel.ResumeLayout(false);
            portComboBoxBorderPanel.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            connectionPanel.ResumeLayout(false);
            connectionPanel.PerformLayout();
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private TableLayoutPanel headerPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private FlowLayoutPanel connectionPanel;
        private Label portLabel;
        private Panel portComboBoxBorderPanel;
        private ComboBox portComboBox;
        private Button refreshPortsButton;
        private Label baudRateLabel;
        private Panel baudRateComboBoxBorderPanel;
        private ComboBox baudRateComboBox;
        private Label payloadSizeLabel;
        private Panel payloadSizeComboBoxBorderPanel;
        private ComboBox payloadSizeComboBox;
        private Label txLabel;
        private FlowLayoutPanel txBitsPanel;
        private Label rxLabel;
        private FlowLayoutPanel rxBitsPanel;
        private FlowLayoutPanel actionButtonsPanel;
        private Button sendButton;
        private Button readButton;
        private TableLayoutPanel logContainerPanel;
        private Label logTitleLabel;
        private Button clearLogButton;
        private RichTextBox activityLogTextBox;
    }
}
