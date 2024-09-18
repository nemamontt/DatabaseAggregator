using DatabaseAggregator.Core;
using DatabaseAggregator.Model;
using DatabaseAggregator.View;
using System.Windows;

namespace DatabaseAggregator.ViewModel
{
    class ViewModelAggregator : ObservableObject
    {
        private Model.Model model;

        private BaseView BV { get; set; }
        private SettingView SV { get; set; }
        private HelpView HV { get; set; }

        public RelayCommand HelpViewCommand { get; set; }
        public RelayCommand SettingViewCommand { get; set; }
        public RelayCommand BaseViewCommand { get; set; }
        public RelayCommand SettingViewButtonCommand { get; set; }

        private string urlNvd;
        public string UrlNvd
        {
            get { return urlNvd; }
            set { urlNvd = value; }
        }

        private string urlFstek;
        public string UrlFstek
        {
            get { return urlFstek; }
            set { urlFstek = value; }
        }

        private string urlJvn;
        public string UrlJvn
        {
            get { return urlJvn; }
            set { urlJvn = value; }
        }

        private string apiKeyNvd;
        public string ApiKeyNvd
        {
            get { return apiKeyNvd; }
            set { apiKeyNvd = value; }
        }

        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private bool beginWork;
        public bool BeginWork
        {
            get
            {
                return beginWork;
            }
            set
            {
                beginWork = value;
            }
        }

        private string progressText;
        public string ProgressText
        {
            get
            {
                return progressText;
            }
            set
            {
                progressText = value;
            }
        }

        public ViewModelAggregator()
        {
/*         BV = new();
            SV = new();
            HV = new();
            model = new();

            HelpViewCommand = new RelayCommand(o =>
              {
                  CurrentView = HV;
              });

            SettingViewCommand = new RelayCommand(o =>
            {
                CurrentView = SV;
            });
            BaseViewCommand = new RelayCommand(o =>
            {
                CurrentView = BV;
            });
            SettingViewButtonCommand = new RelayCommand(o =>
            {
                ProgramConfiguration progConfig = new(UrlFstek, UrlNvd, ApiKeyNvd, UrlJvn);
                model.SaveFile(progConfig);
            });*/
        }
    }
}