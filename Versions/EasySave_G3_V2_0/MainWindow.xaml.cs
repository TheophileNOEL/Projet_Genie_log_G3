using EasySave.Core;
using EasySave_G3_V1;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace EasySave_G3_V2_0
{

    public partial class MainWindow : Window
    {
        ConsoleViewModel consoleViewModel = new ConsoleViewModel();
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public MainWindow()
        {
            InitializeComponent();


            consoleViewModel.GetLangages().SearchLangages();
            Langage language = new Langage("Frensh.Json", Path.Combine(exePath, @"..\\..\\..\\Langages\\French.json"));
            ScenarioList scenarioList = consoleViewModel.GetScenarioList();
            try
            {
                language.LoadLangage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading language : " + ex.Message);
            }
            try
            {
                scenarioList.Load(Path.Combine(exePath, @"..\\..\\..\\scenarios.json"));
                SaveDataGrid.ItemsSource = scenarioList.Get();
                foreach (var item in SaveDataGrid.Items)
                {
                    if (item is Scenario scenario)
                    {
                        scenario.SetState(BackupState.Pending);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading scenario : " + ex.Message);
            }
            UpdateType();
            ReadOnly();
        }

        private void ReadOnly()
        {
            SaveDataGrid.IsEnabled = true;
            TxtBoxName.IsEnabled = false;
            TxTBoxSource.IsEnabled = false;
            TxTBoxTarget.IsEnabled = false;
            TxTBoxDescription.IsEnabled = false;
            CbBox_Type.IsEnabled = false;
            Button_Validation.IsEnabled = false;
        }
        private void WriteOnly(int id = 0, string? Name = null, string? source = null, string? target = null, string? description=null, BackupType type=BackupType.Full)
        {
            SaveDataGrid.IsEnabled = false;
            TxtBoxName.IsEnabled = true;
            TxTBoxSource.IsEnabled = true;
            TxTBoxTarget.IsEnabled = true;
            TxTBoxDescription.IsEnabled = true;
            CbBox_Type.IsEnabled = true;
            Button_Validation.IsEnabled = true;
            TxtBoxName.Text = Name;
            TxTBoxSource.Text = source;
            TxTBoxTarget.Text = target;
            TxTBoxDescription.Text = description;
            CbBox_Type.SelectedItem = type;
            Grid_Modify.Name = Grid_Modify.Name+id.ToString();
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SaveDataGrid.Items)
            {
                if (item is Scenario scenario)
                {
                    if (scenario.IsSelected)
                    {
                        scenario.SetState(BackupState.Running);
                        SaveDataGrid.Items.Refresh();
                        List<string> Back = scenario.Execute();
                        if (Back[1].Contains("path") || Back[1].Contains("not found"))
                        { 
                            scenario.SetState(BackupState.Failed);
                            foreach (string message in Back)
                            {
                                MessageBox.Show(message);
                            }
                            SaveDataGrid.Items.Refresh();
                        }
                        else
                        {
                            scenario.SetState(BackupState.Completed);
                            SaveDataGrid.Items.Refresh();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error when loading scenario : " + item.ToString());
                }
            }
        }

        private void AddScenario_Click(object sender, RoutedEventArgs e)
        {
            string? name = null;
            string? source = null;
            string? target = null;
            string? description = null;
            BackupType type = BackupType.Full;
            WriteOnly();
        }

        private void Button_Validation_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(Grid_Modify.Name.Substring(Grid_Modify.Name.Length - 1));
            if (id !=0)
            {
                string name = TxtBoxName.Text;
                string source = TxTBoxSource.Text;
                string target = TxTBoxTarget.Text;
                string description = TxTBoxDescription.Text;
                BackupType type = GetBackupType(CbBox_Type.SelectedIndex);
                consoleViewModel.GetScenarioList().Modify(id,id,name,source,target,type,description);
            }
            else
            {
                string name = TxtBoxName.Text;
                string source = TxTBoxSource.Text;
                string target = TxTBoxTarget.Text;
                string description = TxTBoxDescription.Text;
                BackupType type = GetBackupType(CbBox_Type.SelectedIndex);
                consoleViewModel.GetScenarioList().CreateScenario(name,source,target,type,description);
            }
            ReadOnly();
            SaveDataGrid.ItemsSource = consoleViewModel.GetScenarioList().Get();
            SaveDataGrid.Items.Refresh();
            Grid_Modify.Name = "Grid_Modify";
        }
        private void UpdateType()
        {
            int countType = Enum.GetNames(typeof(BackupType)).Length;
            for (int i = 0; i < countType; i++)
            {
                CbBox_Type.Items.Add(Enum.GetName(typeof(BackupType), i));
            }
        }
        private void DeleteScenario_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            var element = button.DataContext as Scenario;
            MessageBoxResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer ce scénario ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
                );
            if (result == MessageBoxResult.Yes)
            {
                consoleViewModel.GetScenarioList().RemoveScenario(element.GetId());
            }
            else
            {
                MessageBox.Show("Suppression annulée.");
            }
            SaveDataGrid.Items.Refresh();
        }
        private BackupType GetBackupType(int id_cbbox)
        {
            BackupType type;
            switch (id_cbbox)
            {
                case 0:
                    type = BackupType.Full;
                    break;
                case 1:
                    type = BackupType.Differential;
                    break;
                default:
                    type = BackupType.Full;
                    break;
            }
            return type;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            ReadOnly();
            SaveDataGrid.Items.Refresh();
        }
        private void ModifyScenario_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            var element = button.DataContext as Scenario;
            WriteOnly(element.GetId(),element.GetName(),element.GetSource(),element.GetTarget(), element.GetDescription(),element.GetSceanrioType());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo cmd = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WorkingDirectory = "..\\..\\..\\..\\EasySave_G3_V1_1\\bin\\Debug\\net8.0"
            };
            Process.Start(cmd);
        }
        
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ParametresWindow parametresWindow = new ParametresWindow();
            parametresWindow.Owner = this;
            parametresWindow.Show();
        }
    }
}