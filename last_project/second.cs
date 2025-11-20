using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration; // ElementHost (WPF 호스팅용)
using System.Net.Http; // 서버 통신용
using Newtonsoft.Json; // JSON 처리용
using System.Text;
using System.Threading.Tasks;

namespace last_project
{
    public partial class second : Form
    {
        // WPF 로그인 컨트롤을 담을 변수
        private WpfLogin wpfLoginControl;

        public second()
        {
            InitializeComponent();

            // 프로그램이 시작되면 기존 WinForms 화면을 지우고 WPF 화면을 로드합니다.
            InitializeWpfLogin();
        }

        // ▼▼▼ [핵심] 기존 화면을 WPF 로그인 화면으로 교체하는 함수 ▼▼▼
        private void InitializeWpfLogin()
        {
            // 1. 폼 기본 설정 (다크 테마에 맞게 배경색 변경 및 크기 조정)
            this.Text = "Login System";
            this.Size = new Size(460, 640); // WPF 디자인에 맞춰 크기 조절
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // 창 크기 조절 고정
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 18); // 배경색 #121212
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. 기존 디자이너에 있던 컨트롤들(버튼, 텍스트박스 등) 모두 제거
            this.Controls.Clear();

            // 3. WPF를 담을 그릇(ElementHost) 생성
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill; // 화면에 꽉 채우기

            // 4. 우리가 만든 WPF 로그인 컨트롤 생성
            wpfLoginControl = new WpfLogin();

            // 5. [이벤트 연결] WPF 컨트롤에서 보낸 신호를 이 파일의 함수와 연결
            wpfLoginControl.LoginClicked += WpfLoginControl_LoginClicked;       // 로그인 버튼 누름
            wpfLoginControl.RegisterClicked += WpfLoginControl_RegisterClicked; // 회원가입 버튼 누름

            // 6. 화면에 추가
            host.Child = wpfLoginControl;
            this.Controls.Add(host);
        }

        // ---------------------------------------------------------------
        // [기능 1] 로그인 로직 (서버와 통신)
        // ---------------------------------------------------------------
        private async void WpfLoginControl_LoginClicked(object sender, EventArgs e)
        {
            // WPF 컨트롤에서 입력한 ID/PW 가져오기
            string inputId = wpfLoginControl.UserId;
            string inputPw = wpfLoginControl.UserPw;

            // 1. 서버로 보낼 데이터 JSON 포장
            var loginData = new { id = inputId, pw = inputPw };
            string jsonString = JsonConvert.SerializeObject(loginData);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // 2. 서버 URL (app.py에 정의된 로그인 주소)
            string url = "http://127.0.0.1:5000/api/login";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // POST 요청 전송
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // ★ 로그인 성공 ★
                        MessageBox.Show($"환영합니다, {inputId}님!", "로그인 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 현재 로그인 창 숨기기
                        this.Hide();

                        // 메인 프로그램(main.cs) 실행
                        var f = new main();
                        f.StartPosition = FormStartPosition.CenterScreen;

                        // 메인 창이 닫히면 프로그램 전체 종료
                        f.FormClosed += (s, args) => this.Close();

                        f.Show();
                    }
                    else
                    {
                        // ★ 로그인 실패 (비번 틀림, 아이디 없음 등) ★
                        string errorMsg = await response.Content.ReadAsStringAsync();

                        // (선택사항) JSON 에러 메시지 예쁘게 파싱
                        try
                        {
                            dynamic errObj = JsonConvert.DeserializeObject(errorMsg);
                            errorMsg = errObj.message;
                        }
                        catch { }

                        MessageBox.Show($"로그인 실패: {errorMsg}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 오류: {ex.Message}\n(app.py가 켜져 있는지 확인하세요)", "통신 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------
        // [기능 2] 회원가입 버튼 클릭 시 (회원가입 모달창 띄우기)
        // ---------------------------------------------------------------
        private void WpfLoginControl_RegisterClicked(object sender, EventArgs e)
        {
            OpenRegisterForm();
        }

        // 회원가입 창을 띄우는 함수 (기존 btnRegister_Click 로직을 여기로 이동)
        private void OpenRegisterForm()
        {
            // 1. 새 윈폼 창 생성 (껍데기)
            Form registerForm = new Form();
            registerForm.Text = "회원가입";
            registerForm.Size = new Size(440, 550);
            registerForm.StartPosition = FormStartPosition.CenterParent;
            registerForm.BackColor = Color.FromArgb(30, 30, 30);
            registerForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            registerForm.MaximizeBox = false;

            // 2. WPF 호스팅 준비
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // 3. WPF 회원가입 컨트롤 생성
            WpfRegistration wpfReg = new WpfRegistration();

            // --- [이벤트] 취소 버튼 ---
            wpfReg.CancelClicked += (s, args) => registerForm.Close();

            // --- [이벤트] 중복 확인 버튼 ---
            wpfReg.CheckDuplicateClicked += async (s, args) =>
            {
                string userId = wpfReg.UserId;
                string url = $"http://127.0.0.1:5000/api/check_id?id={userId}";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                            MessageBox.Show("사용 가능한 아이디입니다.", "확인 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("이미 사용 중인 아이디입니다.", "중복", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex) { MessageBox.Show($"서버 연결 실패: {ex.Message}", "오류"); }
            };

            // --- [이벤트] 회원가입 완료 버튼 ---
            wpfReg.RegisterClicked += async (s, args) =>
            {
                var joinData = new { id = wpfReg.UserId, pw = wpfReg.UserPw };
                string jsonString = JsonConvert.SerializeObject(joinData);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                string url = "http://127.0.0.1:5000/api/register";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("회원가입이 완료되었습니다!", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            registerForm.Close(); // 성공 시 창 닫기
                        }
                        else
                        {
                            string errorMsg = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"회원가입 실패: {errorMsg}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show($"오류 발생: {ex.Message}", "오류"); }
            };

            // 4. 화면에 추가 및 띄우기
            host.Child = wpfReg;
            registerForm.Controls.Add(host);
            registerForm.ShowDialog(this);
        }

        // ====================================================================
        // [레거시 코드] 디자이너 오류 방지를 위해 빈 껍데기로 남겨둡니다.
        // (이 함수들은 이제 실행되지 않습니다.)
        // ====================================================================
        private void btnLogin_Click(object sender, EventArgs e) { }
        private void btnRegister_Click(object sender, EventArgs e) { }
        private void txtId_TextChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}