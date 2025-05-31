using EasySave_G3_V2_0.Services;
using System.Windows;

namespace EasySave_G3_V2_0
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Charger la langue depuis settings.json
            var pm = new ParametersManager();
            string langue = pm.Parametres.Langue; // ex: "French" ou "English"
            LocalizationManager.ChangeLanguage(langue);

            // Puis démarrer la MainWindow
            var main = new MainWindow();
            main.Show();
        }
    }
}
