using System;
using System.Windows; // WPF용
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading; // Timer용

namespace last_project
{
    public partial class WpfMyProfile : System.Windows.Controls.UserControl
    {
        private DispatcherTimer timer;

        public WpfMyProfile()
        {
            InitializeComponent();
            LoadSessionData();
            StartTimer();
        }

        private void LoadSessionData()
        {
            // 1. Session의 데이터로 UI 채우기
            // (주의: XAML에 해당 컨트롤들이 존재해야 에러가 안 납니다. 위 1번 코드를 꼭 먼저 붙여넣으세요)

            if (TxtBigName != null) TxtBigName.Text = Session.UserName;
            if (TxtBigRole != null) TxtBigRole.Text = $"ROLE: {Session.Role}";

            // 로그인 시간 표시
            if (TxtLastLogin != null) TxtLastLogin.Text = Session.LoginTime.ToString("yyyy-MM-dd HH:mm");

            // 입력창에 데이터 바인딩
            if (InId != null) InId.Text = Session.UserId;
            if (InName != null) InName.Text = Session.UserName;
            if (InNickname != null) InNickname.Text = Session.Nickname;
            if (InBirth != null) InBirth.Text = Session.Birthdate;
            if (InEmail != null) InEmail.Text = Session.Email;
            if (InPhone != null) InPhone.Text = Session.Phone;

            // 2. 프로필 사진 로드
            if (!string.IsNullOrEmpty(Session.ProfileImagePath))
            {
                try
                {
                    ProfileImgBrush.ImageSource = new BitmapImage(new Uri(Session.ProfileImagePath));
                    TxtNoImg.Visibility = Visibility.Collapsed;
                }
                catch { }
            }
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                if (TxtNowTime != null) TxtNowTime.Text = DateTime.Now.ToString("HH:mm:ss");

                TimeSpan elapsed = DateTime.Now - Session.LoginTime;
                if (TxtElapsedTime != null)
                {
                    TxtElapsedTime.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        (int)elapsed.TotalHours, elapsed.Minutes, elapsed.Seconds);
                }
            };
            timer.Start();
        }

        // [중요] 사진 업로드 버튼 클릭 이벤트
        private void BtnUploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(dlg.FileName));
                    ProfileImgBrush.ImageSource = bitmap;
                    TxtNoImg.Visibility = Visibility.Collapsed;

                    // 세션에 경로 저장
                    Session.ProfileImagePath = dlg.FileName;
                }
                catch
                {
                    System.Windows.MessageBox.Show("이미지 로드 실패");
                }
            }
        }

        // [중요] 저장 버튼 클릭 이벤트
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. 비밀번호 변경 로직 확인
            string currentPw = PwCurrent.Password;
            string newPw = PwNew.Password;
            string confirmPw = PwNewConfirm.Password;

            bool isPwChange = !string.IsNullOrEmpty(newPw);

            if (isPwChange)
            {
                if (string.IsNullOrEmpty(currentPw))
                {
                    System.Windows.MessageBox.Show("비밀번호를 변경하려면 현재 비밀번호를 입력하세요."); return;
                }
                if (newPw != confirmPw)
                {
                    System.Windows.MessageBox.Show("새 비밀번호가 일치하지 않습니다."); return;
                }
                // TODO: 여기서 서버로 비밀번호 변경 API 호출
            }

            // 2. 정보 업데이트 (입력한 값을 Session에 반영)
            Session.UserName = InName.Text;
            Session.Nickname = InNickname.Text;
            Session.Email = InEmail.Text;
            Session.Phone = InPhone.Text;
            Session.Birthdate = InBirth.Text;

            // UI 갱신
            TxtBigName.Text = Session.UserName;

            string msg = "회원 정보가 수정되었습니다.";
            if (isPwChange) msg += "\n(비밀번호도 변경되었습니다.)";

            System.Windows.MessageBox.Show(msg, "알림", MessageBoxButton.OK, MessageBoxImage.Information);

            // 입력창 비우기
            PwCurrent.Password = ""; PwNew.Password = ""; PwNewConfirm.Password = "";
        }
    }
}