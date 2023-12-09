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

            if (Directory.GetDirectories(Path.Combine(appDir, "projects")).Length > 0)
            {
                foreach (string project in Directory.GetDirectories(Path.Combine(appDir, "projects")))
                {
                    if (Directory.GetDirectories(project).Length + Directory.GetFiles(project).Length >= 3)
                    {
                        projectsListLB.Items.Add(project.Split('\\').Last());
                    }
                }
            }
            for(int  i = 0; i < 100; i++)
            {
                projectsListLB.Items.Add(i.ToString());
            }

            GC.Collect();
        }

        private void refreshListBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void createNewProjectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editSelectedProjectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
