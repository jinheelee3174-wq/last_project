using System.Globalization;

namespace last_project
{
    public partial class Form1 : Form
    {
        public Form1()
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
            // 한국식 요일 포함 포맷
            var ko = new CultureInfo("ko-KR");
            lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd (ddd) HH:mm:ss", ko);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
