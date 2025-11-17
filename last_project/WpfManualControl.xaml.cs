using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
// --- ▼▼▼ [1. 추가!] 네트워크 통신(HttpClient)을 위해 2줄 추가 ▼▼▼ ---
using System.Net.Http;
using System.Threading.Tasks;

// (네임스페이스는 last_project가 맞습니다)
namespace last_project
{
    /// <summary>
    /// WpfManualControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfManualControl : System.Windows.Controls.UserControl
    {
        // --- ▼▼▼ [2. 수정!] 'testcar' 샘플의 설정값으로 변경 ▼▼▼ ---
        // -----------------------------------------------------------------
        // [설정] (testcar 샘플의 IP와 포트)
        private const string ESP01_IP = "192.168.0.7";
        private const string ESP01_PORT = "80";
        // -----------------------------------------------------------------

        // --- ▼▼▼ [3. 수정!] 'static readonly'를 제거하고 'instance' 변수로 변경 ▼▼▼ ---
        private readonly HttpClient client;
        // --- ▲▲▲ ---

        public WpfManualControl()
        {
            InitializeComponent();

            // --- ▼▼▼ [4. 수정!] 디자이너 모드가 아닐 때 'instance'로 초기화 ▼▼▼ ---
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // (이 컨트롤 전용 HttpClient 생성)
                client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3); // (testcar 샘플과 동일하게)
            }
            // --- ▲▲▲ ---
        }

        // ========== XAML에서 연결한 이벤트 핸들러들 ==========

        // --- 차량 이동 (전진/후진: 누르고 있는 동안) ---
        private async void ForwardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("F"); // "FORWARD" -> "F"
            AddLog("전진 명령 전송");
        }

        private async void ForwardButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("S"); // "STOP" -> "S" (마우스 떼면 정지)
            AddLog("정지 (전진 버튼 뗌)");
        }

        private async void BackwardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("B"); // "BACKWARD" -> "B"
            AddLog("후진 명령 전송");
        }

        private async void BackwardButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("S"); // "STOP" -> "S" (마우스 떼면 정지)
            AddLog("정지 (후진 버튼 뗌)");
        }

        // --- 차량 이동 (정지) ---
        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("S"); // "STOP" -> "S"
            AddLog("정지 명령 전송 (수동)");
        }

        // --- 속도 조절 ---
        private async void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SpeedValueText != null)
            {
                int newSpeed = (int)e.NewValue;
                SpeedValueText.Text = newSpeed.ToString();

                await SendCommandToCar("V");
            }
        }

        // --- 보조 기능 ---
        private async void HonkButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("H");
            AddLog("경적 울리기(H) 명령 전송");
        }


        // ========== 로그 및 통신을 위한 헬퍼(Helper) 함수 ==========

        private void AddLog(string message)
        {
            if (LogTextBox == null) return;

            // 비동기(async) 메서드에서 UI 컨트롤을 안전하게 업데이트
            Dispatcher.Invoke(() =>
            {
                string logEntry = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}\n";
                LogTextBox.AppendText(logEntry);
                LogTextBox.ScrollToEnd();
            });
        }

        /// <summary>
        /// 'testcar' 샘플과 동일한 로직으로 ESP-01에 HTTP 명령 전송
        /// </summary>
        private async Task SendCommandToCar(string command)
        {
            // --- ▼▼▼ [5. 수정!] client가 null일 경우(디자이너 모드) 실행 방지 ▼▼▼ ---
            if (client == null)
            {
                AddLog("[디자인 모드] 전송 스킵됨.");
                return;
            }
            // --- ▲▲▲ ---

            AddLog($"'{command}' 명령 전송 시도...");

            string url = $"http://{ESP01_IP}:{ESP01_PORT}/?cmd={command}";

            if ((command == "F" || command == "B" || command == "V") && this.SpeedSlider != null)
            {
                int speed = (int)this.SpeedSlider.Value;
                url += $"&speed={speed}";
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                if (command != "V")
                {
                    AddLog($"성공: '{command}' (응답: {responseBody})");
                }
                System.Diagnostics.Debug.WriteLine($"[C#] Sent {command}. URL: {url}");
            }
            catch (HttpRequestException ex)
            {
                AddLog($"[Error] 연결 실패. IP({ESP01_IP}) 확인.");
            }
            catch (TaskCanceledException ex)
            {
                AddLog($"[Error] 타임아웃. ESP-01 응답 없음.");
            }
            catch (Exception ex)
            {
                AddLog($"[Error] 알 수 없는 오류: {ex.Message}");
            }
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("F"); // "F" (방향+속도) 명령 전송
        }

        private async void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("B"); // "B" (방향+속도) 명령 전송
        }
    }
}