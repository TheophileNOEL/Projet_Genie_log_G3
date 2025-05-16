using System.Windows;

namespace EasySave_G3_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Stub vide pour rendre valide le Click="Modifier_Click" du XAML
        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            // rien ici
        }

        // Stub vide pour rendre valide le Click="Supprimer_Click" du XAML
        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            // rien ici
        }
    }

    // Classe de données utilisée dans <DataGrid.Items> de votre XAML
    public class Sauvegarde
    {
        public string Name { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string BackupType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Etat { get; set; } = string.Empty;
        public bool Select { get; set; }
    }
}
