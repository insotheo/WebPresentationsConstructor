using System.Windows;
using System.IO;
using System;
using MessageBoxesWindows;
using WPC_Editor.Widgets;
using System.Collections.Generic;
using System.Windows.Controls;

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

        private List<WidgetsTreeItem> tree;

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

                tree = new List<WidgetsTreeItem>();
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

                if(!File.Exists(Path.Combine(cacheFolder, "INDEX.html")))
                {
                    FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                    indexHTML.Close();
                }
                else
                {
                    Directory.Delete(cacheFolder, true);
                    Directory.CreateDirectory(cacheFolder);
                    FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                    indexHTML.Close();
                }
                webCanvas.Source = new Uri(Path.Combine(cacheFolder, "INDEX.html"));

                tree.Add(new WidgetsTreeItem(new Widget() { name = "CanvasBody", tag = "body", HTML_TAG = "body" }));
                sceneTree.ItemsSource = tree;


                GC.Collect();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
                Environment.Exit(-1);
            }
        }

        private void refreshTreeview()
        {
            sceneTree.ItemsSource = null;
            sceneTree.ItemsSource = tree;
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

        private void createNewElementOnPageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;

                switch (btn.Content.ToString())
                {
                    case "Текст":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetText()));
                        break;
                }

                refreshTreeview();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.ToString());
            }
        }

        #region Properties
        private void sceneTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (sceneTree.SelectedItem != null && sceneTree.Items.Count > 0)
                {
                    var el = sceneTree.SelectedItem as WidgetsTreeItem;
                    ElTagTb.Text = el.widget.tag;
                    ElNameTb.Text = el.widget.name;
                    propertiesGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    propertiesGrid.Visibility = Visibility.Collapsed;
                }
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.ToString());
            }
        }

        private void applyChangesForElBtn_Click(object sender, RoutedEventArgs e)
        {
            propertiesGrid.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
