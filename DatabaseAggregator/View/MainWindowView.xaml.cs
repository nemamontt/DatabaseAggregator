using System.Windows;
using WPFCustomMessageBox;
using DatabaseAggregator.ViewModel;

namespace DatabaseAggregator
{
    public partial class MainWindowView : Window
    {
        private ViewModelAggregator ViewModel { get; }

        public  MainWindowView()
        {
            InitializeComponent();
            ViewModel = new();
        }
    }
}