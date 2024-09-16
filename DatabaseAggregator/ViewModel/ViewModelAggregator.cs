using DatabaseAggregator.View;
using DatabaseAggregator.Core;

namespace DatabaseAggregator.ViewModel
{
    class ViewModelAggregator : ObservableObject
    {
        private BaseView BV { get; set; }
        private SettingView SV { get; set; }
        private HelpView HV { get; set; }
        private object _currentView;
       
        public object CurrentView 
        {
            get => _currentView;
            set 
            { 
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        public bool beginWork;
        private bool BeginWork
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
        public string progressText;
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

        }
    }
}
