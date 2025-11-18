using SysColor = System.Drawing.Color;
namespace last_project
{
    partial class main
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
        //
        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            splitContainer1 = new SplitContainer();
            lblClock = new Label();
            elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            webViewCam1 = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            timer1 = new System.Windows.Forms.Timer(components);
            bindingSource1 = new BindingSource(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webViewCam1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SysColor.Black;
            splitContainer1.Panel1.Controls.Add(lblClock);
            splitContainer1.Panel1.Controls.Add(elementHost1);
            splitContainer1.Panel1.ForeColor = SystemColors.ActiveCaptionText;
            splitContainer1.Panel1.Paint += splitContainer1_Panel1_Paint;
            splitContainer1.Panel1.DoubleClick += splitContainer1_Panel1_DoubleClick;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = SysColor.Black;
            splitContainer1.Panel2.Controls.Add(dataGridView1);
            splitContainer1.Panel2.Controls.Add(chart2);
            splitContainer1.Panel2.Controls.Add(chart1);
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Panel2.Controls.Add(elementHost2);
            splitContainer1.Size = new Size(915, 649);
            splitContainer1.SplitterDistance = 162;
            splitContainer1.TabIndex = 0;
            // 
            // lblClock
            // 
            lblClock.AutoSize = true;
            lblClock.Dock = DockStyle.Top;
            lblClock.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblClock.ForeColor = SystemColors.Control;
            lblClock.Location = new Point(0, 0);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(42, 21);
            lblClock.TabIndex = 2;
            lblClock.Text = "시간";
            lblClock.TextAlign = ContentAlignment.MiddleCenter;
            lblClock.Click += lblClock_Click;
            // 
            // elementHost1
            // 
            elementHost1.Dock = DockStyle.Fill;
            elementHost1.Location = new Point(0, 0);
            elementHost1.Name = "elementHost1";
            elementHost1.Size = new Size(162, 649);
            elementHost1.TabIndex = 3;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.BackgroundColor = SysColor.Black;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6, Column7 });
            dataGridView1.Location = new Point(3, 393);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(746, 253);
            dataGridView1.TabIndex = 6;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            // 
            // Column1
            // 
            Column1.HeaderText = "재고상태";
            Column1.Name = "Column1";
            Column1.Width = 110;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "item_code";
            Column2.HeaderText = "품목번호";
            Column2.Name = "Column2";
            Column2.Width = 110;
            // 
            // Column3
            // 
            Column3.DataPropertyName = "brand";
            Column3.HeaderText = "브랜드";
            Column3.Name = "Column3";
            Column3.Width = 110;
            // 
            // Column4
            // 
            Column4.DataPropertyName = "color";
            Column4.HeaderText = "색상";
            Column4.Name = "Column4";
            Column4.Width = 110;
            // 
            // Column5
            // 
            Column5.DataPropertyName = "size";
            Column5.HeaderText = "사이즈";
            Column5.Name = "Column5";
            Column5.Width = 110;
            // 
            // Column6
            // 
            Column6.DataPropertyName = "category";
            Column6.HeaderText = "카테고리";
            Column6.Name = "Column6";
            Column6.Width = 110;
            // 
            // Column7
            // 
            Column7.DataPropertyName = "stock";
            Column7.HeaderText = "재고";
            Column7.Name = "Column7";
            Column7.Width = 110;
            // 
            // chart2
            // 
            chartArea1.Name = "ChartArea1";
            chart2.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart2.Legends.Add(legend1);
            chart2.Location = new Point(525, 188);
            chart2.Name = "chart2";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chart2.Series.Add(series1);
            chart2.Size = new Size(216, 165);
            chart2.TabIndex = 5;
            chart2.Text = "chart2";
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            chart1.Legends.Add(legend2);
            chart1.Location = new Point(525, 24);
            chart1.Name = "chart1";
            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            chart1.Series.Add(series2);
            chart1.Size = new Size(216, 162);
            chart1.TabIndex = 4;
            chart1.Text = "chart1";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(-9, 2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(533, 351);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(webViewCam1);
            tabPage1.ForeColor = SystemColors.ControlDarkDark;
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(525, 323);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "CAM1";
            tabPage1.UseVisualStyleBackColor = true;
            tabPage1.Click += tabPage1_Click_1;
            // 
            // webViewCam1
            // 
            webViewCam1.AllowExternalDrop = true;
            webViewCam1.CreationProperties = null;
            webViewCam1.DefaultBackgroundColor = SysColor.Black;
            webViewCam1.Dock = DockStyle.Fill;
            webViewCam1.Location = new Point(3, 3);
            webViewCam1.Name = "webViewCam1";
            webViewCam1.Size = new Size(519, 317);
            webViewCam1.TabIndex = 0;
            webViewCam1.ZoomFactor = 1D;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(525, 323);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "CAM2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(525, 323);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "CAM3";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(525, 323);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "PI CAM";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(525, 323);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "All CAM";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // elementHost2
            // 
            elementHost2.Location = new Point(-1, 353);
            elementHost2.Name = "elementHost2";
            elementHost2.Size = new Size(300, 40);
            elementHost2.TabIndex = 9;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(915, 649);
            Controls.Add(splitContainer1);
            Name = "main";
            Text = "        ";
            Load += main_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart2).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webViewCam1).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
      

        private Label lblClock;
        private System.Windows.Forms.Timer timer1;
        private BindingSource bindingSource1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private DataGridView dataGridView1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewCam1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
    }
}
