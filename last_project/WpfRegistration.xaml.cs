using System;
using System.Windows;
using System.Windows.Controls;

namespace last_project
{
    public partial class WpfRegistration : System.Windows.Controls.UserControl
    {
        // 부모 폼(WinForms)으로 보낼 신호들
        public event EventHandler RegisterClicked;
        public event EventHandler CancelClicked;
        public event EventHandler CheckDuplicateClicked;

        // 입력값을 외부에서 가져갈 수 있게 속성 정의
        public string UserId => TxtId.Text;
        public string UserPw => TxtPw.Password;
        public string UserPwConfirm => TxtPwConfirm.Password;

        public WpfRegistration()
        {
            InitializeComponent();
        }

        // 중복 확인 버튼
        private void BtnCheckDup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                // [수정] 명시적으로 System.Windows.MessageBox 사용
                System.Windows.MessageBox.Show("아이디를 입력해주세요.");
                return;
            }
            CheckDuplicateClicked?.Invoke(this, EventArgs.Empty);
        }

        // 회원가입 버튼
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 간단한 유효성 검사
            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(UserPw))
            {
                // [수정] 명시적으로 System.Windows.MessageBox 사용
                System.Windows.MessageBox.Show("아이디와 비밀번호를 모두 입력해주세요.");
                return;
            }

            if (UserPw != UserPwConfirm)
            {
                // [수정] WPF 스타일의 옵션 사용 (MessageBoxButton.OK, MessageBoxImage.Warning)
                System.Windows.MessageBox.Show("비밀번호가 일치하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RegisterClicked?.Invoke(this, EventArgs.Empty);
        }

        // 취소 버튼
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}