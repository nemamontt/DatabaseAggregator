using System.Windows;
using DatabaseAggregator.ViewModel;

namespace DatabaseAggregator
{
    public partial class MainWindowView : Window
    {
        private ViewModelAggregator ViewModel { get; }

        public  MainWindowView()
        {
            InitializeComponent();

            CreatButton.Click += (s, e) =>
            {
               
            };

            SaveButton.Click += (s, e) =>
            {
                
            };
        }
    }
}