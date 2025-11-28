using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace last_project
{
    public partial class WpfManualControl : System.Windows.Controls.UserControl
    {
        private const string ESP01_IP = "192.168.0.7";
        private const string ESP01_PORT = "80";
        private const string CAM_SERVER_IP = "192.168.0.72";
        private const string STREAM_NAME = "mystream";
        private const string WEBRTC_URL = $"http://{CAM_SERVER_IP}:8889/{STREAM_NAME}";

        private readonly HttpClient client;

        public WpfManualControl()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                InitializeCameraAsync();
            }
        }

        private async void InitializeCameraAsync()
        {
            try
            {
                await CameraWebView.EnsureCoreWebView2Async(null);
                CameraWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                CameraWebView.CoreWebView2.Navigate(WEBRTC_URL);
                AddLog($"[Camera] 연결 시도: {WEBRTC_URL}");
            }
            catch (Exception ex)
            {
                AddLog($"[Error] 카메라 초기화 실패: {ex.Message}");
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                string css = @"
                    video { object-fit: cover !important; width: 100% !important; height: 100% !important; position: absolute; top: 0; left: 0; }
                    body { margin: 0 !important; padding: 0 !important; overflow: hidden !important; background-color: black !important; }";
                string script = $"var style = document.createElement('style'); style.type = 'text/css'; style.innerHTML = `{css}`; document.body.appendChild(style);";
                await CameraWebView.CoreWebView2.ExecuteScriptAsync(script);
                AddLog("[Camera] 화면 최적화 완료.");
            }
        }

        // =========================================================
        //  [2] 차량 제어 버튼 이벤트 (Preview 사용)
        // =========================================================

        // ▲ 전진 (PreviewMouseDown 사용)
        private async void ForwardButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddLog("▲ 전진 버튼 눌림"); // 버튼 반응 확인용 로그
            await SendCommandToCar("F");
        }

        // ▲ 전진 뗌 (PreviewMouseUp 사용)
        private async void ForwardButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("S");
            AddLog("■ 정지 (버튼 뗌)");
        }

        // ▼ 후진 (PreviewMouseDown 사용)
        private async void BackwardButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddLog("▼ 후진 버튼 눌림");
            await SendCommandToCar("B");
        }

        // ▼ 후진 뗌 (PreviewMouseUp 사용)
        private async void BackwardButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            await SendCommandToCar("S");
            AddLog("■ 정지 (버튼 뗌)");
        }

        // ■ 정지 (클릭)
        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("S");
            AddLog("■ 강제 정지 명령 전송");
        }

        // 📢 경적 (클릭)
        private async void HonkButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandToCar("H");
            AddLog("📢 빵빵!");
        }

        // =========================================================
        //  [3] 로그 및 기타 기능
        // =========================================================

        private void BtnExpandLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogForm logForm = new LogForm();
                logForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                logForm.Show();
                AddLog("[System] 로그 상세 창 열기");
            }
            catch (Exception ex) { AddLog($"[Error] 로그 창 열기 실패: {ex.Message}"); }
        }

        // 로그 출력 헬퍼 함수 (수정됨)
        private void AddLog(string message)
        {
            // 1. [핵심] 전역 로그 매니저에 저장 (이 한 줄 덕분에 새 창에서도 보입니다!)
            last_project.LogManager.Add(message);

            // 2. 현재 화면(WpfManualControl)의 작은 로그창에 표시
            if (LogTextBox == null) return;

            Dispatcher.Invoke(() =>
            {
                // 화면에는 시간까지 찍어서 보여줌
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}\r\n";
                LogTextBox.AppendText(logEntry);
                LogTextBox.ScrollToEnd(); // 항상 맨 아래로 스크롤
            });
        }

        private async Task SendCommandToCar(string command)
        {
            if (client == null) return;

            string url = $"http://{ESP01_IP}:{ESP01_PORT}/?cmd={command}&speed=200";

            // ★ 통신 시도 로그 추가 (예전처럼)
            AddLog($"[통신] '{command}' 명령 전송 중... ({url})");

            try
            {
                using (var cts = new System.Threading.CancellationTokenSource(500))
                {
                    await client.GetAsync(url, cts.Token);
                }
                // ★ 성공 로그는 원하시면 주석 해제하세요 (너무 많아질까봐 기본은 끔)
                // AddLog($"[통신] '{command}' 전송 완료.");
            }
            catch (Exception ex)
            {
                AddLog($"[오류] 통신 실패: {ex.Message}");
            }
        }
    }
}