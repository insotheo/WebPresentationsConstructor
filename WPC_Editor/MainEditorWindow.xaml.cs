using System.Windows;
using System.IO;
using System;
using MessageBoxesWindows;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainEditorWindow.xaml
    /// </summary>
    public partial class MainEditorWindow : Window
    {
        private string projectFolder;
        private string assetsFolder;
        private string cacheFolder;

        private ConfigWorker config;
        private FileViewerWindow fileViewer;
        private ConfigEditorWindow configEditorWindow; 

        public MainEditorWindow()
        {
            InitializeComponent();
            try
            {
                projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "projects", File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt")));
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"));

                assetsFolder = Path.Combine(projectFolder, "Assets");
                cacheFolder = Path.Combine(projectFolder, "cache");

                config = new ConfigWorker(Path.Combine(projectFolder, "settings.config"));
                if (!config.isTheSamePC())
                {
                    var ans = MessBox.showQuestionWithTwoOptions("Информация о создателе не схожа с информацией о Вас. Желаете ли Вы открыть данный проект?");
                    if(ans == MessageBoxResult.No)
                    {
                        Environment.Exit(0);
                    }
                }

                this.Title = this.Title.Replace("pname", $"{Path.GetFileName(projectFolder)}");
                
                foreach(string file in config.usingStyles)
                {
                    if(!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", file.Trim())) &&
                        !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                    {
                        throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                    }
                }
                foreach (string file in config.usingScripts)
                {
                    if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", file.Trim())) &&
                        !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                    {
                        throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                    }
                }
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
                Environment.Exit(-1);
            }
        }

        private void showProjectFilesBtn_Click(object sender, RoutedEventArgs e)
        {
            fileViewer = new FileViewerWindow(assetsFolder, ref config);
            fileViewer.ShowDialog();
            config = fileViewer.configWorker;
        }

        private void editConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            configEditorWindow = new ConfigEditorWindow(ref config, assetsFolder);
            configEditorWindow.ShowDialog();
            if (configEditorWindow.isApplied)
            {
                config = configEditorWindow.newConfig;
            }
        }
    }
}
