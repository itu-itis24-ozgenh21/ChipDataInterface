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
            connectionPanel = new FlowLayoutPanel();
            portLabel = new Label();
            portComboBox = new ComboBox();
            refreshPortsButton = new Button();
            baudRateLabel = new Label();
            baudRateComboBox = new ComboBox();
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
            connectionPanel.SuspendLayout();
            actionButtonsPanel.SuspendLayout();
            logContainerPanel.SuspendLayout();
            SuspendLayout();

            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.BackColor = Color.White;
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 90F));
            mainLayoutPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            mainLayoutPanel.Controls.Add(connectionPanel, 0, 0);
            mainLayoutPanel.SetColumnSpan(connectionPanel, 2);

            mainLayoutPanel.Controls.Add(txLabel, 0, 1);
            mainLayoutPanel.Controls.Add(txBitsPanel, 1, 1);

            mainLayoutPanel.Controls.Add(rxLabel, 0, 2);
            mainLayoutPanel.Controls.Add(rxBitsPanel, 1, 2);

            mainLayoutPanel.Controls.Add(actionButtonsPanel, 1, 3);

            mainLayoutPanel.Controls.Add(logContainerPanel, 0, 4);
            mainLayoutPanel.SetColumnSpan(logContainerPanel, 2);

            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.GrowStyle =
                TableLayoutPanelGrowStyle.FixedSize;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.Padding = new Padding(24);
            mainLayoutPanel.RowCount = 5;

            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 68F));

            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 104F));

            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 104F));

            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 64F));

            mainLayoutPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            mainLayoutPanel.Size = new Size(820, 680);
            mainLayoutPanel.TabIndex = 0;

            // 
            // connectionPanel
            // 
            connectionPanel.AutoScroll = true;
            connectionPanel.BackColor =
                Color.FromArgb(244, 248, 255);
            connectionPanel.BorderStyle = BorderStyle.FixedSingle;
            connectionPanel.Controls.Add(portLabel);
            connectionPanel.Controls.Add(portComboBox);
            connectionPanel.Controls.Add(refreshPortsButton);
            connectionPanel.Controls.Add(baudRateLabel);
            connectionPanel.Controls.Add(baudRateComboBox);
            connectionPanel.Dock = DockStyle.Fill;
            connectionPanel.Location = new Point(24, 24);
            connectionPanel.Margin = new Padding(0);
            connectionPanel.Name = "connectionPanel";
            connectionPanel.Padding = new Padding(12, 10, 12, 10);
            connectionPanel.Size = new Size(772, 68);
            connectionPanel.TabIndex = 0;
            connectionPanel.WrapContents = false;

            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Font = new Font("Segoe UI", 10F);
            portLabel.ForeColor = Color.FromArgb(31, 41, 55);
            portLabel.Margin = new Padding(0, 10, 8, 0);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(83, 23);
            portLabel.TabIndex = 0;
            portLabel.Text = "COM Port";

            // 
            // portComboBox
            // 
            portComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;
            portComboBox.FormattingEnabled = true;
            portComboBox.Margin = new Padding(0, 6, 12, 0);
            portComboBox.Name = "portComboBox";
            portComboBox.Size = new Size(110, 28);
            portComboBox.TabIndex = 0;

            // 
            // refreshPortsButton
            // 
            refreshPortsButton.BackColor = Color.White;
            refreshPortsButton.Cursor = Cursors.Hand;
            refreshPortsButton.FlatAppearance.BorderColor =
                Color.RoyalBlue;
            refreshPortsButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(235, 240, 255);
            refreshPortsButton.FlatStyle = FlatStyle.Flat;
            refreshPortsButton.ForeColor = Color.RoyalBlue;
            refreshPortsButton.Margin = new Padding(0, 3, 24, 0);
            refreshPortsButton.Name = "refreshPortsButton";
            refreshPortsButton.Size = new Size(86, 34);
            refreshPortsButton.TabIndex = 1;
            refreshPortsButton.Text = "Refresh";
            refreshPortsButton.UseVisualStyleBackColor = false;

            // 
            // baudRateLabel
            // 
            baudRateLabel.AutoSize = true;
            baudRateLabel.Font = new Font("Segoe UI", 10F);
            baudRateLabel.ForeColor = Color.FromArgb(31, 41, 55);
            baudRateLabel.Margin = new Padding(0, 10, 8, 0);
            baudRateLabel.Name = "baudRateLabel";
            baudRateLabel.Size = new Size(87, 23);
            baudRateLabel.TabIndex = 0;
            baudRateLabel.Text = "Baud Rate";

            // 
            // baudRateComboBox
            // 
            baudRateComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;
            baudRateComboBox.FormattingEnabled = true;
            baudRateComboBox.Margin = new Padding(0, 6, 0, 0);
            baudRateComboBox.Name = "baudRateComboBox";
            baudRateComboBox.Size = new Size(120, 28);
            baudRateComboBox.TabIndex = 2;

            // 
            // txLabel
            // 
            txLabel.Dock = DockStyle.Fill;
            txLabel.Font = new Font("Segoe UI", 16F);
            txLabel.ForeColor = Color.FromArgb(31, 41, 55);
            txLabel.Location = new Point(24, 92);
            txLabel.Margin = new Padding(0);
            txLabel.Name = "txLabel";
            txLabel.Size = new Size(90, 104);
            txLabel.TabIndex = 0;
            txLabel.Text = "Tx";
            txLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // txBitsPanel
            // 
            txBitsPanel.AutoScroll = true;
            txBitsPanel.BackColor =
                Color.FromArgb(250, 251, 253);
            txBitsPanel.BorderStyle = BorderStyle.FixedSingle;
            txBitsPanel.Dock = DockStyle.Fill;
            txBitsPanel.Location = new Point(114, 92);
            txBitsPanel.Margin = new Padding(0);
            txBitsPanel.Name = "txBitsPanel";
            txBitsPanel.Padding = new Padding(10, 20, 10, 10);
            txBitsPanel.Size = new Size(682, 104);
            txBitsPanel.TabIndex = 3;
            txBitsPanel.WrapContents = false;

            // 
            // rxLabel
            // 
            rxLabel.Dock = DockStyle.Fill;
            rxLabel.Font = new Font("Segoe UI", 16F);
            rxLabel.ForeColor = Color.FromArgb(31, 41, 55);
            rxLabel.Location = new Point(24, 196);
            rxLabel.Margin = new Padding(0);
            rxLabel.Name = "rxLabel";
            rxLabel.Size = new Size(90, 104);
            rxLabel.TabIndex = 0;
            rxLabel.Text = "Rx";
            rxLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // rxBitsPanel
            // 
            rxBitsPanel.AutoScroll = true;
            rxBitsPanel.BackColor =
                Color.FromArgb(250, 251, 253);
            rxBitsPanel.BorderStyle = BorderStyle.FixedSingle;
            rxBitsPanel.Dock = DockStyle.Fill;
            rxBitsPanel.Location = new Point(114, 196);
            rxBitsPanel.Margin = new Padding(0);
            rxBitsPanel.Name = "rxBitsPanel";
            rxBitsPanel.Padding = new Padding(10, 20, 10, 10);
            rxBitsPanel.Size = new Size(682, 104);
            rxBitsPanel.TabIndex = 4;
            rxBitsPanel.WrapContents = false;

            // 
            // actionButtonsPanel
            // 
            actionButtonsPanel.Anchor = AnchorStyles.None;
            actionButtonsPanel.AutoSize = true;
            actionButtonsPanel.AutoSizeMode =
                AutoSizeMode.GrowAndShrink;
            actionButtonsPanel.Controls.Add(sendButton);
            actionButtonsPanel.Controls.Add(readButton);
            actionButtonsPanel.FlowDirection =
                FlowDirection.LeftToRight;
            actionButtonsPanel.Location = new Point(303, 303);
            actionButtonsPanel.Margin = new Padding(0);
            actionButtonsPanel.Name = "actionButtonsPanel";
            actionButtonsPanel.Size = new Size(304, 58);
            actionButtonsPanel.TabIndex = 5;
            actionButtonsPanel.WrapContents = false;

            // 
            // sendButton
            // 
            sendButton.BackColor = Color.RoyalBlue;
            sendButton.Cursor = Cursors.Hand;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(48, 82, 180);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Segoe UI", 10F);
            sendButton.ForeColor = Color.White;
            sendButton.Margin = new Padding(6);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(140, 46);
            sendButton.TabIndex = 5;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = false;

            // 
            // readButton
            // 
            readButton.BackColor = Color.White;
            readButton.Cursor = Cursors.Hand;
            readButton.FlatAppearance.BorderColor =
                Color.RoyalBlue;
            readButton.FlatAppearance.BorderSize = 1;
            readButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(235, 240, 255);
            readButton.FlatStyle = FlatStyle.Flat;
            readButton.Font = new Font("Segoe UI", 10F);
            readButton.ForeColor = Color.RoyalBlue;
            readButton.Margin = new Padding(6);
            readButton.Name = "readButton";
            readButton.Size = new Size(140, 46);
            readButton.TabIndex = 6;
            readButton.Text = "Read";
            readButton.UseVisualStyleBackColor = false;

            // 
            // logContainerPanel
            // 
            logContainerPanel.BackColor =
                Color.FromArgb(244, 248, 255);
            logContainerPanel.ColumnCount = 2;
            logContainerPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            logContainerPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 90F));
            logContainerPanel.Controls.Add(logTitleLabel, 0, 0);
            logContainerPanel.Controls.Add(clearLogButton, 1, 0);
            logContainerPanel.Controls.Add(activityLogTextBox, 0, 1);
            logContainerPanel.SetColumnSpan(activityLogTextBox, 2);
            logContainerPanel.Dock = DockStyle.Fill;
            logContainerPanel.Location = new Point(24, 372);
            logContainerPanel.Margin = new Padding(0, 8, 0, 0);
            logContainerPanel.Name = "logContainerPanel";
            logContainerPanel.Padding = new Padding(10);
            logContainerPanel.RowCount = 2;
            logContainerPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 36F));
            logContainerPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            logContainerPanel.Size = new Size(772, 284);
            logContainerPanel.TabIndex = 7;

            // 
            // logTitleLabel
            // 
            logTitleLabel.Dock = DockStyle.Fill;
            logTitleLabel.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold);
            logTitleLabel.ForeColor =
                Color.FromArgb(31, 41, 55);
            logTitleLabel.Location = new Point(10, 10);
            logTitleLabel.Margin = new Padding(0);
            logTitleLabel.Name = "logTitleLabel";
            logTitleLabel.Size = new Size(662, 36);
            logTitleLabel.TabIndex = 0;
            logTitleLabel.Text = "Activity Log";
            logTitleLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            // 
            // clearLogButton
            // 
            clearLogButton.Anchor =
                AnchorStyles.Top | AnchorStyles.Right;
            clearLogButton.BackColor = Color.White;
            clearLogButton.Cursor = Cursors.Hand;
            clearLogButton.FlatAppearance.BorderColor =
                Color.Silver;
            clearLogButton.FlatStyle = FlatStyle.Flat;
            clearLogButton.ForeColor =
                Color.FromArgb(55, 65, 81);
            clearLogButton.Location = new Point(680, 14);
            clearLogButton.Margin = new Padding(8, 4, 0, 4);
            clearLogButton.Name = "clearLogButton";
            clearLogButton.Size = new Size(82, 28);
            clearLogButton.TabIndex = 8;
            clearLogButton.Text = "Clear";
            clearLogButton.UseVisualStyleBackColor = false;

            // 
            // activityLogTextBox
            // 
            activityLogTextBox.BackColor = Color.White;
            activityLogTextBox.BorderStyle =
                BorderStyle.FixedSingle;
            activityLogTextBox.DetectUrls = false;
            activityLogTextBox.Dock = DockStyle.Fill;
            activityLogTextBox.Font =
                new Font("Consolas", 9F);
            activityLogTextBox.ForeColor =
                Color.FromArgb(45, 45, 48);
            activityLogTextBox.Location =
                new Point(10, 52);
            activityLogTextBox.Margin =
                new Padding(0, 6, 0, 0);
            activityLogTextBox.Name =
                "activityLogTextBox";
            activityLogTextBox.ReadOnly = true;
            activityLogTextBox.ScrollBars =
                RichTextBoxScrollBars.Vertical;
            activityLogTextBox.Size =
                new Size(752, 222);
            activityLogTextBox.TabIndex = 9;
            activityLogTextBox.TabStop = false;
            activityLogTextBox.Text = "";
            activityLogTextBox.WordWrap = true;

            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(820, 680);
            Controls.Add(mainLayoutPanel);
            MinimumSize = new Size(760, 640);
            Name = "MainForm";
            StartPosition =
                FormStartPosition.CenterScreen;
            Text = "Chip Data Interface";

            logContainerPanel.ResumeLayout(false);
            actionButtonsPanel.ResumeLayout(false);
            connectionPanel.ResumeLayout(false);
            connectionPanel.PerformLayout();
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private FlowLayoutPanel connectionPanel;
        private Label portLabel;
        private ComboBox portComboBox;
        private Button refreshPortsButton;
        private Label baudRateLabel;
        private ComboBox baudRateComboBox;
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