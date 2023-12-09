using System.IO;
using System.Windows;
using System;
using MessageBoxesWindows;
using System.Linq;

namespace ProjectsManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string appDir = Directory.GetCurrentDirectory();

            try
            {

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

            foreach (string project in Directory.GetDirectories(Path.Combine(appDir, "projects")))
            {
                if (Directory.GetDirectories(project).Length + Directory.GetFiles(project).Length >= 3)
                {
                    projectsListLB.Items.Add(project.Split('\\').Last());
                }
            }

            GC.Collect();
        }
    }
}
