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
    public partial class second : Form
    {
        public second()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // 현재 로그인 폼은 숨기고 메인 띄우기
            this.Hide();

            var f = new main(/* 필요하면 txtId.Text 전달 */);
            f.StartPosition = FormStartPosition.CenterScreen;

            // 메인 폼이 닫히면 로그인 폼도 같이 종료
            f.FormClosed += (s, args) => this.Close();

            f.Show();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var reg = new new_registration())
            {
                reg.StartPosition = FormStartPosition.CenterParent;
                reg.ShowDialog(this); // 닫히면 다시 second로 포커스 복귀
            }
        }
    }
}
