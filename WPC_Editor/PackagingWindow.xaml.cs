using System.Windows;
using MessageBoxesWindows;
using WPC_BUILD_CLASS;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для PackagingWindow.xaml
    /// </summary>
    
    internal class filesTreeElementClass
    {
        public string title {  get; set; }
        public List<filesTreeElementClass> kids { get; set; }


        public filesTreeElementClass(string title, List<string> listFiles)
        {
            this.title = title;
            kids = new List<filesTreeElementClass>();
            if(listFiles != null)
            {
                if(listFiles.Count > 0)
                {
                    foreach(string el in listFiles)
                    {
                        kids.Add(new filesTreeElementClass(el));
                    }
                }
            }
        }

        public filesTreeElementClass(string title)
        {
            this.title = title;
            kids = new List<filesTreeElementClass>();
        }
    }


    public partial class PackagingWindow : Window
    {
        private List<filesTreeElementClass> filesTreeElementClasses;

        private BuilderClass builder;
        private ConfigWorker config;

        public PackagingWindow(ref BuilderClass builder, ref ConfigWorker config, string assetsFolder)
        {
            InitializeComponent();

            this.builder = builder;
            this.config = config;

            //tree view fill
            filesTreeElementClasses = new List<filesTreeElementClass>();
            List<string> pages = FilesWorker.getAllFilesByExt(assetsFolder, ".wpcsave");
            filesTreeElementClasses.Add(new filesTreeElementClass($"Страницы({pages.Count.ToString()})", pages));
            filesTreeElementClasses.Add(new filesTreeElementClass($"Скрипты({config.usingScripts.Count.ToString()})", config.usingScripts));
            filesTreeElementClasses.Add(new filesTreeElementClass($"Стили({config.usingStyles.Count.ToString()})", config.usingStyles));
            filesTreeElementClasses.Add(new filesTreeElementClass($"Прочие файлы({config.additionalFilesForBuilding.Count.ToString()})", config.additionalFilesForBuilding));
            filesTree.ItemsSource = filesTreeElementClasses;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json")))
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json"));
            }
            if(Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "projects", config.projectName, ".build")))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "projects", config.projectName, ".build"), true);
            }
            FileStream binFile = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"));
            binFile.Close();
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "bin.txt"), config.projectName);
            MainEditorWindow editor = new MainEditorWindow();
            editor.Show();
            this.Close();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(pathToSaveTB.Text.Trim() == String.Empty)
                {
                    throw new Exception("Путь не может быть пустым");
                }
                if (!Directory.Exists(pathToSaveTB.Text))
                {
                    throw new Exception("Путь не существует");
                }
                if(Directory.GetFiles(pathToSaveTB.Text).Length + Directory.GetDirectories(pathToSaveTB.Text).Length > 0){
                    throw new Exception("Выбранная папка должна быть пустой");
                }
                if(Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "projects", config.projectName, ".build")))
                {
                    Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "projects", config.projectName, ".build"), true);
                }
                if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json")))
                {
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json"));
                }
                BUILD_DATA_CLASS data = new BUILD_DATA_CLASS((bool)allowMobilesCB.IsChecked,
                    (bool)removeCommentsCB.IsChecked,
                     (bool)zipCB.IsChecked,
                     (bool)tabHtmlCB.IsChecked,
                     (bool)sortCB.IsChecked,
                     pathToSaveTB.Text);
                FileStream jsonFile = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json"));
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "projects", config.projectName, ".build"));
                string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                jsonFile.Close();
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json"), jsonData);
                HoldOnWindow holdOn = new HoldOnWindow("Preparing...");
                holdOn.Show();
                this.WindowState = WindowState.Minimized;
                this.IsEnabled = false;
                Task.Run(() => builder.build(data.sort, config, data.allowMobileDevices)).Wait();
                Task.Delay(10).Wait();
                holdOn.Close();
                this.WindowState = WindowState.Normal;
                this.IsEnabled = true;
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.ToString());
                this.IsEnabled = true;
            }
        }

        private void browsePathBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Выберите ПУСТУЮ папку, где будет находиться результат сборки"
            };
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    pathToSaveTB.Text = fbd.SelectedPath;
                    if (Directory.GetFiles(pathToSaveTB.Text).Length + Directory.GetDirectories(pathToSaveTB.Text).Length > 0)
                    {
                        throw new Exception("Выбранная папка должна быть пустой");
                    }
                }
                catch(Exception ex)
                {
                    MessBox.showError(ex.Message);
                    pathToSaveTB.Text = String.Empty;
                }
            }
        }
    }
}
