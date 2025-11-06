using System.Globalization;

namespace last_project
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void lblClock_Click(object sender, EventArgs e)
        {

        }


        private void UpdateClock()
        {
            var now = DateTime.Now;

            // 1줄: "yyyy -MM-dd"
            string line1 = now.ToString("yyyy -MM-dd", CultureInfo.InvariantCulture);

            // 2줄: 영어 요일
            string line2 = now.ToString("dddd", new CultureInfo("en-US"));

            // 3줄: 시간(24시간제). 12시간제로 원하면 "tt h:mm"으로 바꿔.
            string line3 = now.ToString("HH:mm", CultureInfo.InvariantCulture);

            lblClock.Text = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 라벨이 왼쪽 패널 전체 너비를 차지하도록
            lblClock.AutoSize = false;
            lblClock.Dock = DockStyle.Top;          // ← 패널의 상단에 가로로 꽉 차게
            lblClock.Height = 80;                   // 필요하면 70~100 사이로 조정
            lblClock.TextAlign = ContentAlignment.MiddleCenter; // ← 가로/세로 가운데 정렬
            lblClock.Padding = new Padding(0, 6, 0, 0);         // 살짝 내려오게(옵션)
            lblClock.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            timer1.Interval = 1000;
            timer1.Start();
            UpdateClock();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel1_DoubleClick(object sender, EventArgs e)
        {
            int step = 60; // 아주 조금 (10~16 픽셀 정도 추천)
            int min = splitContainer1.Panel1MinSize;
            int max = splitContainer1.Width - splitContainer1.Panel2MinSize;

            int next = splitContainer1.SplitterDistance + step; // ← ‘조금 늘리기’
            if (next > max) next = max;                         // 범위 보정

            splitContainer1.SplitterDistance = next;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            // 1. setting 폼의 새 인스턴스(객체)를 만듭니다.
            setting settingForm = new setting();

            // 2. 폼을 'Modal'로 띄웁니다.
            // (이 창이 닫히기 전까지 main 폼을 클릭할 수 없습니다.)
            settingForm.ShowDialog();
        }
    }
}
