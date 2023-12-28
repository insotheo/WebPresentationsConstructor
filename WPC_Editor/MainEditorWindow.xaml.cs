using System.Windows;
using System.IO;
using System;
using MessageBoxesWindows;
using WPC_Editor.Widgets;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;
using System.Threading.Tasks;

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
        private BuilderClass builder;
        private HoldOnWindow holdOnWindow;
        private Uri homePage;

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
                builder = new BuilderClass(cacheFolder, assetsFolder);
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

                builder.Init(ref config);
                webCanvas.Source = homePage = new Uri(Path.Combine(cacheFolder, "INDEX.html"));

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

        private void goHomeOnWebcanvas()
        {
            if(webCanvas.Source != homePage)
            {
                webCanvas.Source = homePage;
            }
        }

        #region top buttons

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

        private void showInWebBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!File.Exists(Path.Combine(cacheFolder, "INDEX.html")))
                {
                    throw new Exception("Ошибка файла!");
                }
                Process.Start($"\"{Path.Combine(cacheFolder, "INDEX.html")}\"");
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private async void refreshWebCanvas_Click(object sender, RoutedEventArgs e)
        {
            holdOnWindow = new HoldOnWindow("building");
            this.IsEnabled = false;
            holdOnWindow.Show();
            await Task.Run(() => builder.fastBuild(tree[0].widgetsOfScene, ref config));
            holdOnWindow.Close();
            this.IsEnabled = true;
            goHomeOnWebcanvas();
            webCanvas.Reload();
            GC.Collect();
        }

        private void openHomePageForWebCanvasBtn_Click(object sender, RoutedEventArgs e)
        {
            goHomeOnWebcanvas();
        }

        #endregion

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
                    case "Ссылка":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetLink()));
                        break;
                    case "Кнопка":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetButton()));
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
                    isElUseCSS.IsChecked = el.widget.useStyle;
                    propertiesGrid.Visibility = Visibility.Visible;
                    switch(el.widget.tag)
                    {
                        case "Текст":
                            var widget = el.widget as WidgetText;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Visible;
                            contentTabber.SelectedIndex = 1;
                            textContent.Text = widget.content;
                            if (!widget.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 1;
                                textFontFamily.Text = widget.fontFamily;
                                textFontSize.Text = widget.fontSize.ToString();
                                textFontWeight.Text = widget.fontWeight;
                                textFontColor.Text = widget.fontColorHEX;
                                textPreviewColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(widget.fontColorHEX)));
                                textBackgroundColor.Text = widget.backgroundColorHEX;
                                textPreviewBackgroundColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(widget.backgroundColorHEX)));
                                textBackgroundRadius.Text = widget.backgroundRad;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Ссылка":
                            var widgetLink = el.widget as WidgetLink;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Visible;
                            contentTabber.SelectedIndex = 2;
                            linkText.Text = widgetLink.content;
                            linkLinkAdress.Text = widgetLink.href;
                            if (!widgetLink.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 1;
                                textFontFamily.Text = widgetLink.fontFamily;
                                textFontSize.Text = widgetLink.fontSize.ToString();
                                textFontWeight.Text = widgetLink.fontWeight;
                                textFontColor.Text = widgetLink.fontColorHEX;
                                textPreviewColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(widgetLink.fontColorHEX)));
                                textBackgroundColor.Text = widgetLink.backgroundColorHEX;
                                textPreviewBackgroundColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(widgetLink.backgroundColorHEX)));
                                textBackgroundRadius.Text = widgetLink.backgroundRad;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "body":
                            removeElementBtn.IsEnabled = false;
                            propertiesTabber.SelectedIndex = 0;
                            contentTabber.SelectedIndex = 0;
                            propertiesTabber.Visibility = Visibility.Collapsed;
                            contentTabber.Visibility = Visibility.Collapsed;
                            break;

                        default:
                            removeElementBtn.IsEnabled = true;
                            propertiesTabber.SelectedIndex = 0;
                            contentTabber.SelectedIndex = 0;
                            propertiesTabber.Visibility = Visibility.Collapsed;
                            contentTabber.Visibility = Visibility.Collapsed;
                            break;
                    }
                }
                else
                {
                    propertiesTabber.SelectedIndex = 0;
                    contentTabber.SelectedIndex = 0;
                    propertiesTabber.Visibility = Visibility.Collapsed;
                    contentTabber.Visibility = Visibility.Collapsed;
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
            try
            {
                var el = sceneTree.SelectedItem as WidgetsTreeItem;
                el.widget.name = ElNameTb.Text;
                switch (el.widget.tag)
                {
                    case "Текст":
                        var widget = el.widget as WidgetText;
                        if (isElUseCSS.IsChecked == false)
                        {
                            widget.useStyle = false;
                            widget.content = textContent.Text == String.Empty ? widget.content : textContent.Text;
                            widget.fontFamily = textFontFamily.Text == String.Empty ? widget.fontFamily : textFontFamily.Text;
                            widget.fontWeight = textFontWeight.Text == String.Empty ? widget.fontWeight : textFontWeight.Text;
                            widget.fontColorHEX = textFontColor.Text == String.Empty ? widget.fontColorHEX : textFontColor.Text;
                            widget.fontSize = textFontSize.Text == String.Empty ? widget.fontSize : int.Parse(textFontSize.Text);
                            widget.backgroundColorHEX = textBackgroundColor.Text == String.Empty ? widget.backgroundColorHEX : textBackgroundColor.Text;
                            widget.backgroundRad = textBackgroundRadius.Text == String.Empty ? widget.backgroundRad : textBackgroundRadius.Text;
                        }
                        else
                        {
                            widget.useStyle = true;
                            widget.content = textContent.Text;
                            textContent.Text = String.Empty;
                            textFontFamily.Text = String.Empty;
                            textFontWeight.Text = String.Empty;
                            textFontColor.Text = String.Empty;
                            textFontSize.Text = String.Empty;
                            textBackgroundColor.Text = String.Empty;
                            textBackgroundRadius.Text = String.Empty;
                        }
                        break;

                    case "Ссылка":
                        var widgetLink = el.widget as WidgetLink;
                        if (isElUseCSS.IsChecked == false)
                        {
                            widgetLink.useStyle = false;
                            widgetLink.content = linkText.Text == String.Empty ? widgetLink.content : linkText.Text;
                            widgetLink.href = linkLinkAdress.Text == String.Empty ? widgetLink.href : linkLinkAdress.Text;
                            widgetLink.fontFamily = textFontFamily.Text == String.Empty ? widgetLink.fontFamily : textFontFamily.Text;
                            widgetLink.fontWeight = textFontWeight.Text == String.Empty ? widgetLink.fontWeight : textFontWeight.Text;
                            widgetLink.fontColorHEX = textFontColor.Text == String.Empty ? widgetLink.fontColorHEX : textFontColor.Text;
                            widgetLink.fontSize = textFontSize.Text == String.Empty ? widgetLink.fontSize : int.Parse(textFontSize.Text);
                            widgetLink.backgroundColorHEX = textBackgroundColor.Text == String.Empty ? widgetLink.backgroundColorHEX : textBackgroundColor.Text;
                            widgetLink.backgroundRad = textBackgroundRadius.Text == String.Empty ? widgetLink.backgroundRad : textBackgroundRadius.Text;
                        }
                        else
                        {
                            widgetLink.useStyle = true;
                            widgetLink.content = linkText.Text;
                            widgetLink.href = linkLinkAdress.Text;
                            textContent.Text = String.Empty;
                            textFontFamily.Text = String.Empty;
                            textFontWeight.Text = String.Empty;
                            textFontColor.Text = String.Empty;
                            textFontSize.Text = String.Empty;
                            textBackgroundColor.Text = String.Empty;
                            textBackgroundRadius.Text = String.Empty;
                        }
                        break;

                    default: break;
                }

                refreshTreeview();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private void removeElementBtn_Click(object sender, RoutedEventArgs e)
        {
            var el = sceneTree.SelectedItem as WidgetsTreeItem;
            tree[0].widgetsOfScene.Remove(el);
            refreshTreeview();
        }
        #endregion

        private void textFontColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                textPreviewColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(textFontColor.Text)));
            }
            catch { }
        }

        private void textBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                textPreviewBackgroundColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(textBackgroundColor.Text)));
            }
            catch { }
        }
    }
}
