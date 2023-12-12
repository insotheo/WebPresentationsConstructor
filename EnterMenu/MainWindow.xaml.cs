using MessageBoxesWindows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Diagnostics;

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

            this.Title = this.Title.Replace("ver", Assembly.GetExecutingAssembly().GetName().Version.ToString(4));

            refreshListOfProjects();

            GC.Collect();
        }

        private void refreshListOfProjects()
        {
            projectsListLB.Items.Clear();
            if (Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "projects")).Length > 0)
            {
                foreach (string folderOfProject in Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "projects")))
                {
                   if(Directory.Exists(Path.Combine(folderOfProject, "Assets")) &&
                        Directory.Exists(Path.Combine(folderOfProject, "cache")) &&
                        File.Exists(Path.Combine(folderOfProject, "settings.config")))
                    {
                        projectsListLB.Items.Add(folderOfProject.Split('\\').Last().Trim());
                    }
                }

                GC.Collect();
            }
        }

        #region bottom buttons
        private void refreshListBtn_Click(object sender, RoutedEventArgs e)
        {
            refreshListOfProjects();
        }

        private void createNewProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            NameOfNewProjectDialog creationDialog = new NameOfNewProjectDialog();
            creationDialog.ShowDialog();
            if(creationDialog.isCreated == true)
            {
                refreshListOfProjects();
            }
        }

        private void editSelectedProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            if(projectsListLB.Items.Count > 0 && projectsListLB.SelectedItem != null)
            {
                try
                {
                    if(!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "WPC_Editor.exe")))
                    {
                        throw new Exception("Отсутствует файл редактора!");
                    }
                    FileStream idFile = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"));
                    idFile.Close();
                    File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"), projectsListLB.SelectedItem.ToString());
                    Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "WPC_Editor.exe"));
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessBox.showError(ex.Message);
                    if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt")))
                    {
                        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"));
                    }
                }
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (projectsListLB.Items.Count > 0 && projectsListLB.SelectedItem != null)
            {
                var ans = MessBox.showQuestionWithTwoOptions("Желаете ли Вы удалить проект? Учтите, что потом Вы не сможете редактировать его или восстановить!");
                try
                {
                    if (ans == MessageBoxResult.Yes)
                    {
                        Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(),
                            "projects", projectsListLB.SelectedItem.ToString()), true);
                        projectsListLB.Items.Remove(projectsListLB.SelectedItem);
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessBox.showError(ex.Message);
                }
            }
        }

        private void showInExplorerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (projectsListLB.Items.Count > 0 && projectsListLB.SelectedItem != null)
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{Path.Combine(Directory.GetCurrentDirectory(), "projects", projectsListLB.SelectedItem.ToString())}\""
                });
                GC.Collect();
            }
            projectsListLB.SelectedItem = null;
        }
        #endregion

        //Searching button
        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (searchQueryTB.Text == String.Empty)
            {
                refreshListOfProjects();
                return;
            }
            else
            {
                List<string> res = new List<string>();
                foreach (var el in Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "projects")))
                {
                    if (el.Split('\\').Last().ToString().ToLower().Trim().Contains(searchQueryTB.Text.ToLower().Trim()))
                    {
                        res.Add(el.Split('\\').Last().ToString());
                    }
                }
                projectsListLB.Items.Clear();
                foreach (var el in res)
                {
                    projectsListLB.Items.Add(el.ToString());
                }
            }
        }

    }
}
