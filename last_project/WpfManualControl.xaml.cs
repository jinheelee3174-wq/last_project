using System;
using System.IO.Ports;     // (필수) 아두이노 통신(SerialPort)을 위해
using System.Windows;      // (필수) RoutedEventArgs 등을 사용하기 위해
using System.Windows.Controls; // (필수) UserControl, Slider, TextBlock 등을 사용하기 위해
using System.Windows.Input;  // (필수) MouseButtonEventArgs 등을 사용하기 위해
using System.ComponentModel; // <-- ▼▼▼ 1. '디자이너 모드' 확인을 위해 1줄 추가! ▼▼▼
using System.IO.Ports;
// (중요) x:Class="last_project.WpfManualControl"와 일치해야 합니다.
namespace last_project
{
    /// <summary>
    /// WpfManualControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfManualControl : System.Windows.Controls.UserControl
    {
        // --- ▼▼▼ [수정!] 'SerialPort'를 여기서 '선언'만 합니다. (초기화 X) ▼▼▼ ---
        SerialPort arduinoPort;
        // --- ▲▲▲ 'new SerialPort()'를 지워야 디자이너 오류가 안 납니다! ▲▲▲ ---

        public WpfManualControl()
        {
            InitializeComponent();

            // --- ▼▼▼ [추가!] '디자이너 모드'가 "아닐" 때만(실제 실행될 때만) SerialPort를 초기화합니다. ▼▼▼ ---
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (!OperatingSystem.IsWindows())
                {
                    NotifySerialPortUnavailable("현재 환경에서는 직렬 포트를 사용할 수 없습니다.");
                    return;
                }

                try
                {
                // (이제 '실행'될 때만 이 코드가 작동하여, 디자이너가 멈추지 않습니다)
                arduinoPort = new SerialPort();
                }
                catch (PlatformNotSupportedException ex)
                {
                    NotifySerialPortUnavailable($"직렬 포트를 사용할 수 없는 환경입니다: {ex.Message}");
                }
                catch (Exception ex)
                {
                    NotifySerialPortUnavailable($"직렬 포트 초기화 실패: {ex.Message}");
                }

                // (옵션) 여기에 'COM' 포트 연결하는 초기화 코드를 넣을 수 있습니다.
                // (예: TryConnectToArduino("COM3");)
            }
        }

        // ========== XAML에서 연결한 이벤트 핸들러들 ==========

        // --- 차량 이동 ---
        private void ForwardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SendSerialCommand("FORWARD");
            AddLog("전진 명령 전송");
        }

        private void ForwardButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SendSerialCommand("STOP");
            AddLog("정지 (전진 버튼 뗌)");
        }

        private void BackwardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SendSerialCommand("BACKWARD");
            AddLog("후진 명령 전송");
        }

        private void BackwardButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SendSerialCommand("STOP");
            AddLog("정지 (후진 버튼 뗌)");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SendSerialCommand("STOP");
            AddLog("정지 명령 전송 (수동)");
        }

        // --- 속도 조절 ---
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // (중요) SpeedValueText가 null이 아닐 때만 실행 (프로그램 시작 시 오류 방지)
            if (SpeedValueText != null)
            {
                int newSpeed = (int)e.NewValue;
                SpeedValueText.Text = newSpeed.ToString();
                SendSerialCommand($"SPEED,{newSpeed}");
                AddLog($"속도 {newSpeed}로 변경");
            }
        }

        // --- 보조 기능 ---
        private void HonkButton_Click(object sender, RoutedEventArgs e)
        {
            SendSerialCommand("HONK");
            AddLog("경적 울리기 명령 전송");
        }


        // ========== 로그 및 통신을 위한 헬퍼(Helper) 함수 ==========

        private void NotifySerialPortUnavailable(string message)
        {
            arduinoPort = null;
            AddLog(message);

            // ★ 여기만 수정!
            System.Windows.MessageBox.Show(
                message,
                "수동 제어",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }


        private void AddLog(string message)
        {
            // (방어 코드) 디자이너 모드이거나 LogTextBox가 아직 로드 안됐으면 실행 안 함
            if (LogTextBox == null) return;

            string logEntry = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}\n";
            LogTextBox.AppendText(logEntry);
            LogTextBox.ScrollToEnd();
        }

        private void SendSerialCommand(string command)
        {
            // (수정) arduinoPort가 null이 아닐 때만(디자이너 모드가 아닐 때만) 실행
            if (arduinoPort != null && arduinoPort.IsOpen)
            {
                try
                {
                    arduinoPort.WriteLine(command);
                }
                catch (Exception ex)
                {
                    AddLog($"전송 오류: {ex.Message}");
                }
            }
            else
            {
                // (디자이너 모드일 때는 이 로그를 찍지 않음)
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    AddLog("오류: 아두이노가 연결되지 않았습니다.");
                }
            }
            System.Diagnostics.Debug.WriteLine($"[Serial 전송 시도] {command}");
        }
    }
}
