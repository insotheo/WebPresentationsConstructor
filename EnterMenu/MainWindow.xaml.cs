using System.IO;
using System.Windows;
using System;
using MessageBoxesWindows;

namespace ProjectsManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                string appDir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(Path.Combine(appDir, "projects")))
                    Directory.CreateDirectory(Path.Combine(appDir, "projects"));

                if (!Directory.Exists(Path.Combine(appDir, "styles")) || !Directory.Exists(Path.Combine(appDir, "scripts")))
                    throw new Exception("Нарушена целостность приложения!");

            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
                Environment.Exit(-1);
            }

            InitializeComponent();
            GC.Collect();
        }
    }
}
