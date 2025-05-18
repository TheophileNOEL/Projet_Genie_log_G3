using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Reflection;
using EasySave.Core;
using EasySave_G3_V1;
using EasySave_G3_V2_0;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

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
            ScenarioList scenarioList = new ScenarioList();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading scenario : " + ex.Message);
            }
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
        private void WriteOnly()
        {
            SaveDataGrid.IsEnabled = false;
            TxtBoxName.IsEnabled = true;
            TxTBoxSource.IsEnabled = true;
            TxTBoxTarget.IsEnabled = true;
            TxTBoxDescription.IsEnabled = true;
            CbBox_Type.IsEnabled = true;
            Button_Validation.IsEnabled = true;
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
                        Thread.Sleep(2000);
                        if (Back[0].StartsWith("Error"))
                        {
                            MessageBox.Show(Back[0]);
                            scenario.SetState(BackupState.Failed);
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
            WriteOnly();
            TxtBoxName.Text = "";
            TxTBoxSource.Text = "";
            TxTBoxTarget.Text = "";
            TxTBoxDescription.Text = "";
            CbBox_Type.SelectedIndex = -1;
        }
    }
}