using DatabaseAggregator.Core;
using DatabaseAggregator.Model;
using DatabaseAggregator.View;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Shapes;

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
        public RelayCommand CrateViewCommand { get; set; }
        public RelayCommand SettingViewButtonCommand { get; set; }
        public RelayCommand SaveViewCommand { get; set; }

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

        private string helpText;
        public string HelpText
        {
            get { return helpText; }
            set { helpText = value; }
        }

        private ProgramConfiguration progConfig;
        public ProgramConfiguration ProgConfig
        {
            get { return progConfig; }
            set { progConfig = value; }
        }

        private ObservableCollection<Database> database;
        public ObservableCollection<Database> Database
        {
            get { return database; }
            set { database = value; }
        }

        private readonly string pathToResources = System.IO.Path.Combine(Environment.CurrentDirectory, "Resource");

        public ViewModelAggregator()
        {
            BV = new();
            SV = new();
            HV = new();
            model = new();

            if (!Directory.Exists(pathToResources))
                Directory.CreateDirectory(pathToResources);

            ImportFile(false);

            SaveViewCommand = new RelayCommand(o =>
            {     
                SaveFile(Database);
            });

            CrateViewCommand = new RelayCommand(o =>
            {
                CurrentView = BV;
                CreatFile();
            });

            /*         

                        HelpViewCommand = new RelayCommand(o =>
                          {
                              CurrentView = HV;
                          });

                        SettingViewCommand = new RelayCommand(o =>
                        {
                            CurrentView = SV;
                        });
                        SettingViewButtonCommand = new RelayCommand(o =>
                        {
                            ProgramConfiguration progConfig = new(UrlFstek, UrlNvd, ApiKeyNvd, UrlJvn);
                            model.SaveFile(progConfig);
                        });*/
        }

        public void ImportFile(bool selectionWindow) 
        {
            if(selectionWindow)
            {
                OpenFileDialog openFileDialog = new() { Filter = "Json files (*.json)|*.json" };

                if (openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    string jsonString = File.ReadAllText(filePath);
                    Database = JsonSerializer.Deserialize<ObservableCollection<Database>>(jsonString);
                }
            }
            else
            {
                var filePath = System.IO.Path.Combine(pathToResources, "programConfiguration.json");
                if (!System.IO.Path.Exists(filePath))
                {
                    MessageBox.Show("Файл конфигурации отсутствует, заполните его вручную");
                }
                else
                {
                    string jsonString = File.ReadAllText(filePath);
                    ProgConfig = JsonSerializer.Deserialize<ProgramConfiguration>(jsonString);
                }               
            }        
        }

        public void CreatFile()
        {
            if(ProgConfig is null)
            {
                MessageBox.Show("Отсутсвует файл конфигурации, создайте его вручную", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Database = model.CreatDatabase();
            }           
        }
        public void SaveFile(object objectSerializations)
        {
            SaveFileDialog saveFileDialog = new() { Filter = "Json files (*.json)|*.json|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() is true)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(objectSerializations, options);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
    }
}