using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows;
using System.IO;

public partial class ParametresWindow : Window
{
    private ParametersManager pm;

    public ParametresWindow()
    {
        InitializeComponent();
        pm = new ParametersManager();
        ChargerParametresDansUI();
    }

    private void ChargerParametresDansUI()
    {
        // Format du log
        foreach (ComboBoxItem item in CB_TypeLog.Items)
        {
            if (item.Content.ToString() == pm.Parametres.FormatLog)
            {
                item.IsSelected = true;
                break;
            }
        }

        // Extensions
        LstExtensions.Items.Clear();
        foreach (var ext in pm.Parametres.ExtensionsChiffrees)
            LstExtensions.Items.Add(ext);

        // Logiciels
        LstLogiciels.Items.Clear();
        foreach (var logic in pm.Parametres.CheminsLogiciels)
            LstLogiciels.Items.Add(logic);

        // Langue
        foreach (ComboBoxItem item in CB_Langue.Items)
        {
            if (item.Content.ToString() == pm.Parametres.Langue)
            {
                item.IsSelected = true;
                break;
            }
        }
    }

    private void AjouterExtension_Click(object sender, RoutedEventArgs e)
    {
        string ext = TxtNouvelleExtension.Text.Trim();

        if (!ext.StartsWith("."))
        {
            MessageBox.Show("L'extension doit commencer par un point (ex: .pdf)", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!LstExtensions.Items.Contains(ext))
            LstExtensions.Items.Add(ext);

        TxtNouvelleExtension.Clear();
    }

    private void SupprimerExtension_Click(object sender, RoutedEventArgs e)
    {
        if (LstExtensions.SelectedItem != null)
            LstExtensions.Items.Remove(LstExtensions.SelectedItem);
    }

    private void AjouterLogiciel_Click(object sender, RoutedEventArgs e)
    {
        string path = TxtNouveauLogiciel.Text.Trim();

        if (!string.IsNullOrEmpty(path) && File.Exists(path) && path.EndsWith(".exe"))
        {
            if (!LstLogiciels.Items.Contains(path))
                LstLogiciels.Items.Add(path);
        }
        else
        {
            MessageBox.Show("Chemin invalide ou non .exe.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        TxtNouveauLogiciel.Clear();
    }

    private void SupprimerLogiciel_Click(object sender, RoutedEventArgs e)
    {
        if (LstLogiciels.SelectedItem != null)
            LstLogiciels.Items.Remove(LstLogiciels.SelectedItem);
    }

    private void ParcourirLogiciel_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Fichiers exécutables (*.exe)|*.exe";

        if (dialog.ShowDialog() == true)
        {
            TxtNouveauLogiciel.Text = dialog.FileName;
        }
    }

    private void Valider_Click(object sender, RoutedEventArgs e)
    {
        // Format Log
        pm.Parametres.FormatLog = ((ComboBoxItem)CB_TypeLog.SelectedItem).Content.ToString();

        // Extensions
        pm.Parametres.ExtensionsChiffrees = LstExtensions.Items.Cast<string>().ToList();

        // Logiciels
        pm.Parametres.CheminsLogiciels = LstLogiciels.Items.Cast<string>().ToList();

        // Langue
        pm.Parametres.Langue = ((ComboBoxItem)CB_Langue.SelectedItem).Content.ToString();

        // Sauvegarde
        pm.Save();

        MessageBox.Show("Paramètres sauvegardés !");
        Close(); // Fermer la fenêtre si tu veux
    }

    private void Annuler_Click(object sender, RoutedEventArgs e)
    {
        Close(); // Ferme simplement la fenêtre
    }
}