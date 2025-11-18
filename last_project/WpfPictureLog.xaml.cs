using System.Windows.Controls;
using System.Windows; // FrameworkElement를 위해 추가
using System.Windows.Input; // MouseButtonEventArgs를 위해 추가

namespace last_project
{
    /// <summary>
    /// WpfPictureLog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfPictureLog : System.Windows.Controls.UserControl
    {
        public WpfPictureLog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// (★★★★★ 추가!) XAML의 작은 이미지를 클릭했을 때 실행되는 함수
        /// </summary>
        private void SmallImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 1. 클릭된 컨트롤(Image)을 가져옵니다.
            var imageControl = sender as FrameworkElement;
            if (imageControl == null) return;

            // 2. 그 컨트롤에 바인딩된 데이터(PictureLogEntry)를 가져옵니다.
            var logEntry = imageControl.DataContext as PictureLogEntry;
            if (logEntry == null) return;

            // 3. 데이터에서 이미지 경로(string)를 확인합니다.
            if (string.IsNullOrEmpty(logEntry.ImagePath))
            {
                // (혹시 모를 오류 방지)
                System.Windows.MessageBox.Show("이미지 경로가 없습니다.");
                return;
            }

            // 4. (핵심) PictureViewerWindow를 '새로' 만듭니다.
            //    생성자에 클릭된 이미지의 경로(logEntry.ImagePath)를 전달합니다.
            PictureViewerWindow viewer = new PictureViewerWindow(logEntry.ImagePath);

            // 5. 새 창을 띄웁니다.
            viewer.ShowDialog();
        }
    }
}