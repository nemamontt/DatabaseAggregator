using ViewModels;

namespace DatabaseAggregator.ViewModel
{
    public class ViewModelAggregator : ViewModelBase
    {
        public BaseViewModel BVM { get; }
        public SettingViewModel SVM { get; }
        public HelpViewModel HVM { get; }

        public object? CurrentViewModel { get => Get<object>(); private set => Set(value); }

        private bool BeginWork { get => Get<bool>(); set => Set(value); }

        public string ProgressText { get => Get<string>() ?? string.Empty; set => Set(value); }
        public ViewModelAggregator()
        {
            BVM = new();
            SVM = new();
            HVM = new();
        }

        public RelayCommand SetCurrentViewModel => GetCommand(vm => CurrentViewModel = vm);
    }
}
