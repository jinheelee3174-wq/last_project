using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace last_project
{
    public partial class setting : Form
    {
        public setting()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private Size largeFormSize = new Size(1043,658);
        private Size smallFormSize = new Size(410, 575);
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1번 인덱스("제품 품목 설정")가 선택되었는지 확인
            if (tabControl1.SelectedIndex == 1)
            {
                // 폼의 크기를 '작은' 크기로 변경합니다.
                this.Size = smallFormSize;
            }
            // 그 외 다른 탭들 ("슬롯 상세 설정" - 0번, "수동 제어" - 2번 등)
            else
            {
                // 폼의 크기를 '큰' 크기로 다시 되돌립니다.
                this.Size = largeFormSize;
            }
        }
    }
}
