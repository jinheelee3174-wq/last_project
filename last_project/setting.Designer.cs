namespace last_project
{
    partial class setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            splitContainer1 = new SplitContainer();
            dataGridView1 = new DataGridView();
            tabPage2 = new TabPage();
            dataGridView2 = new DataGridView();
            btnRefresh = new Button();
            btnDelete = new Button();
            button3 = new Button();
            btnRegister = new Button();
            groupBox2 = new GroupBox();
            tabPage3 = new TabPage();
            Column13 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            Column9 = new DataGridViewTextBoxColumn();
            Column10 = new DataGridViewTextBoxColumn();
            Column11 = new DataGridViewTextBoxColumn();
            Column12 = new DataGridViewTextBoxColumn();
            txtStock = new TextBox();
            txtCategory = new TextBox();
            txtSize = new TextBox();
            txtColor = new TextBox();
            txtBrand = new TextBox();
            txtItemCode = new TextBox();
            label7 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label6 = new Label();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(0, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1043, 658);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(splitContainer1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1035, 630);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "슬롯 상세 설정";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dataGridView1);
            splitContainer1.Size = new Size(1029, 624);
            splitContainer1.SplitterDistance = 627;
            splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6 });
            dataGridView1.Location = new Point(0, 214);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(387, 339);
            dataGridView1.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dataGridView2);
            tabPage2.Controls.Add(btnRefresh);
            tabPage2.Controls.Add(btnDelete);
            tabPage2.Controls.Add(button3);
            tabPage2.Controls.Add(btnRegister);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1035, 630);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "제품 품목 설정";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            dataGridView2.Location = new Point(0, 0);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(240, 150);
            dataGridView2.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(0, 0);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 23);
            btnRefresh.TabIndex = 1;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(0, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 2;
            // 
            // button3
            // 
            button3.Location = new Point(0, 0);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 3;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(0, 0);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(75, 23);
            btnRegister.TabIndex = 4;
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(200, 100);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1035, 630);
            tabPage3.TabIndex = 2;
            // 
            // Column13
            // 
            Column13.Name = "Column13";
            // 
            // Column8
            // 
            Column8.Name = "Column8";
            // 
            // Column9
            // 
            Column9.Name = "Column9";
            // 
            // Column10
            // 
            Column10.Name = "Column10";
            // 
            // Column11
            // 
            Column11.Name = "Column11";
            // 
            // Column12
            // 
            Column12.Name = "Column12";
            // 
            // txtStock
            // 
            txtStock.Location = new Point(0, 0);
            txtStock.Name = "txtStock";
            txtStock.Size = new Size(100, 23);
            txtStock.TabIndex = 0;
            // 
            // txtCategory
            // 
            txtCategory.Location = new Point(0, 0);
            txtCategory.Name = "txtCategory";
            txtCategory.Size = new Size(100, 23);
            txtCategory.TabIndex = 0;
            // 
            // txtSize
            // 
            txtSize.Location = new Point(0, 0);
            txtSize.Name = "txtSize";
            txtSize.Size = new Size(100, 23);
            txtSize.TabIndex = 0;
            // 
            // txtColor
            // 
            txtColor.Location = new Point(0, 0);
            txtColor.Name = "txtColor";
            txtColor.Size = new Size(100, 23);
            txtColor.TabIndex = 0;
            // 
            // txtBrand
            // 
            txtBrand.Location = new Point(0, 0);
            txtBrand.Name = "txtBrand";
            txtBrand.Size = new Size(100, 23);
            txtBrand.TabIndex = 0;
            // 
            // txtItemCode
            // 
            txtItemCode.Location = new Point(0, 0);
            txtItemCode.Name = "txtItemCode";
            txtItemCode.Size = new Size(100, 23);
            txtItemCode.TabIndex = 0;
            // 
            // label7
            // 
            label7.Location = new Point(0, 0);
            label7.Name = "label7";
            label7.Size = new Size(100, 23);
            label7.TabIndex = 0;
            // 
            // label11
            // 
            label11.Location = new Point(0, 0);
            label11.Name = "label11";
            label11.Size = new Size(100, 23);
            label11.TabIndex = 0;
            // 
            // label10
            // 
            label10.Location = new Point(0, 0);
            label10.Name = "label10";
            label10.Size = new Size(100, 23);
            label10.TabIndex = 0;
            // 
            // label9
            // 
            label9.Location = new Point(0, 0);
            label9.Name = "label9";
            label9.Size = new Size(100, 23);
            label9.TabIndex = 0;
            // 
            // label8
            // 
            label8.Location = new Point(0, 0);
            label8.Name = "label8";
            label8.Size = new Size(100, 23);
            label8.TabIndex = 0;
            // 
            // label6
            // 
            label6.Location = new Point(0, 0);
            label6.Name = "label6";
            label6.Size = new Size(100, 23);
            label6.TabIndex = 0;
            // 
            // Column1
            // 
            Column1.HeaderText = "ID";
            Column1.Name = "Column1";
            Column1.Width = 64;
            // 
            // Column2
            // 
            Column2.HeaderText = "X";
            Column2.Name = "Column2";
            Column2.Width = 64;
            // 
            // Column3
            // 
            Column3.HeaderText = "Y";
            Column3.Name = "Column3";
            Column3.Width = 64;
            // 
            // Column4
            // 
            Column4.HeaderText = "W";
            Column4.Name = "Column4";
            Column4.Width = 64;
            // 
            // Column5
            // 
            Column5.HeaderText = "H";
            Column5.Name = "Column5";
            Column5.Width = 64;
            // 
            // Column6
            // 
            Column6.HeaderText = "활성화";
            Column6.Name = "Column6";
            Column6.Width = 64;
            // 
            // setting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1023, 582);
            Controls.Add(tabControl1);
            Name = "setting";
            Text = "setting";
            Load += setting_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // --- ▼▼▼ [복구] 맨 아래 변수 선언 코드 ▼▼▼ ---
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private GroupBox groupBox2;
        private TextBox txtCategory;
        private TextBox txtSize;
        private TextBox txtColor;
        private TextBox txtBrand;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label6;
        private DataGridView dataGridView2;
        private Button btnRefresh;
        private Button btnDelete;
        private Button button3;
        private Button btnRegister;
        private DataGridViewTextBoxColumn Column13;
        private DataGridViewTextBoxColumn Column8;
        private DataGridViewTextBoxColumn Column9;
        private DataGridViewTextBoxColumn Column10;
        private DataGridViewTextBoxColumn Column11;
        private DataGridViewTextBoxColumn Column12;
        private TextBox txtItemCode;
        private TextBox txtStock;
        private Label label7;

        // --- (★★★★★) tabPage1의 컨트롤 변수들 "복구" (★★★★★) ---
        private SplitContainer splitContainer1;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        // --- ▲▲▲ [복구] 여기까지 ▲▲▲ ---
    }
}