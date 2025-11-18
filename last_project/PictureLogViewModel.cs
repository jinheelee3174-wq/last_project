using System;
using System.Collections.ObjectModel; // (중요!) List 대신 ObservableCollection 사용
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace last_project
{
    /// <summary>
    /// WpfPictureLog 컨트롤의 데이터(로그 목록)를 관리하는 ViewModel 클래스입니다.
    /// </summary>
    public class PictureLogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// (핵심) 사진 로그 항목들의 '목록'입니다.
        /// ObservableCollection을 사용해야 UI가 자동으로 업데이트됩니다.
        /// </summary>
        public ObservableCollection<PictureLogEntry> LogEntries { get; set; }

        public PictureLogViewModel()
        {
            LogEntries = new ObservableCollection<PictureLogEntry>();

            // (테스트용) 샘플 데이터가 필요하면 여기서 추가
            // AddLog("C:\\Path\\To\\SampleImage.jpg", "로그 뷰어 시작됨");
        }

        /// <summary>
        /// (외부에서 호출) 새 사진 로그를 목록의 '맨 위에' 추가합니다.
        /// </summary>
        /// <param name="imagePath">저장된 이미지 파일 경로</param>
        /// <param name="description">로그 설명</param>
        public void AddLog(string imagePath, string description)
        {
            var newEntry = new PictureLogEntry
            {
                ImagePath = imagePath,
                Description = description,
                Timestamp = DateTime.Now
            };

            // (중요) WPF 컨트롤은 UI 스레드에서 생성/수정되어야 합니다.
            // 혹시라도 다른 스레드(예: 카메라 캡처 스레드)에서 이 함수를 호출할 경우를 대비해
            // UI 스레드에서 실행되도록 보장하는 것이 안전합니다.
            // (지금은 WinForms 기반이라 Application.Current가 null일 수 있으니,
            //  WinForms에서 호출할 때 스레드 처리를 하거나, 우선 이대로 둡니다.)

            // LogEntries.Insert(0, newEntry); // 0번 인덱스(맨 위)에 추가

            // (수정) 더 안전한 스레드 처리 (WPF/WinForms 하이브리드 환경 고려)
            // 우선 간단하게 직접 추가로 진행합니다.
            LogEntries.Insert(0, newEntry);
        }


        // --- INotifyPropertyChanged 구현 ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}