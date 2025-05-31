using System.Windows;

namespace EasySave.ConsoleRemote
{
    public partial class MainWindow : Window
    {
        private SocketClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new SocketClient();

            bool connected = _client.Connect();
            if (!connected)
            {
                MessageBox.Show("Connexion au serveur impossible.");
            }
        }

        
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            string scenario = ScenarioTextBox.Text;
            _client.SendCommand($"pause:{scenario}");
            ResponseTextBlock.Text = "Réponse : " + _client.ReadResponse();
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            string scenario = ScenarioTextBox.Text;
            _client.SendCommand($"resume:{scenario}");
            ResponseTextBlock.Text = "Réponse : " + _client.ReadResponse();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            string scenario = ScenarioTextBox.Text;
            _client.SendCommand($"stop:{scenario}");
            ResponseTextBlock.Text = "Réponse : " + _client.ReadResponse();
        }

    }
}
