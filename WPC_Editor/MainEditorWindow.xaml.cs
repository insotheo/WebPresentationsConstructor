using MessageBoxesWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPC_Editor.Widgets;

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
                    if (ans == MessageBoxResult.No)
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
            catch (Exception ex)
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
            if (webCanvas.Source != homePage)
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
                if (!File.Exists(Path.Combine(cacheFolder, "INDEX.html")))
                {
                    throw new Exception("Ошибка файла!");
                }
                Process.Start($"\"{Path.Combine(cacheFolder, "INDEX.html")}\"");
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private async void refreshWebCanvas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                holdOnWindow = new HoldOnWindow("building");
                this.IsEnabled = false;
                holdOnWindow.Show();
                await Task.Run(() => builder.fastBuild(tree[0].widgetsOfScene, ref config));
                holdOnWindow.Close();
                this.IsEnabled = true;
                goHomeOnWebcanvas();
                webCanvas.Reload();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
                holdOnWindow.Close();
            }
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
                    case "Перенос":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetNextLine()));
                        break;
                    case "Фото":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetImage()));
                        break;
                    case "Видео":
                        tree[0].widgetsOfScene.Add(new WidgetsTreeItem(new WidgetVideo()));
                        break;
                }

                refreshTreeview();
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
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
                    switch (el.widget.tag)
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

                        case "Кнопка":
                            var widgetButton = el.widget as WidgetButton;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.SelectedIndex = 3;
                            contentTabber.Visibility = Visibility.Visible;
                            ButtonOnclickEventsCB.Items.Clear();
                            List<string> events = JavaScriptWorker.getAllEvents(ref config, assetsFolder);
                            foreach (string eventName in events)
                            {
                                ButtonOnclickEventsCB.Items.Add(eventName);
                            }
                            buttonText.Text = widgetButton.content;
                            if (events.Count > 0 && Array.IndexOf(events.ToArray(), widgetButton.onclick) == -1)
                            {
                                widgetButton.onclick = null;
                                widgetButton.arguments = null;
                            }
                            ButtonOnclickEventsCB.Text = widgetButton.onclick;
                            argumentsForButton.Text = widgetButton.arguments;
                            if (!widgetButton.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 2;
                                buttonFontSize.Text = widgetButton.fontSize.ToString();
                                buttonFontFamily.Text = widgetButton.fontFamily;
                                buttonFontWeight.Text = widgetButton.fontWeight;
                                buttonFontColor.Text = widgetButton.fontColorHEX;
                                buttonBackgroundColor.Text = widgetButton.backgroundColorHEX;
                                buttonBorderColor.Text = widgetButton.borderColorHEX;
                                buttonBorderRadius.Text = widgetButton.borderRadius;
                                buttonCursor.Text = widgetButton.cursor;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Фото":
                            var widgetImg = el.widget as WidgetImage;
                            contentTabber.SelectedIndex = 4;
                            imageFilesCB.Items.Clear();
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Visible;
                            List<string> files = FilesWorker.getAllFilesByExt(assetsFolder, new string[] { ".bmp", ".png", ".jpeg", ".jpg" });
                            foreach (string file in files)
                            {
                                imageFilesCB.Items.Add(file);
                            }
                            if (widgetImg.contentType == 'f' && widgetImg.href != String.Empty && Array.IndexOf(files.ToArray(), widgetImg.href) != -1)
                            {
                                imageFilesCB.SelectedIndex = Array.IndexOf(files.ToArray(), widgetImg.href);
                            }
                            else if (widgetImg.contentType == 'l' && widgetImg.href != String.Empty)
                            {
                                imageLinkToThePhoto.Text = widgetImg.href;
                            }
                            if (!widgetImg.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 3;
                                imageWidth.Text = widgetImg.width;
                                imageHeight.Text = widgetImg.height;
                                imageRadius.Text = widgetImg.radius;
                                imageRotAngle.Text = widgetImg.rotationAngle;
                                ImageContentSelector.SelectedIndex = widgetImg.contentType == 'f' ? 0 : 1;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Видео":
                            var videoWidget = el.widget as WidgetVideo;
                            contentTabber.SelectedIndex = 5;
                            videoFileCB.Items.Clear();
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Visible;
                            List<string> mediaFiles = FilesWorker.getAllFilesByExt(assetsFolder, new string[] { ".mp4", ".mov", ".mp3", ".wav", ".ogg" });
                            foreach(string file in mediaFiles)
                            {
                                videoFileCB.Items.Add(file);
                            }
                            if(videoWidget.src != String.Empty && Array.IndexOf(mediaFiles.ToArray(), videoWidget.src) != -1)
                            {
                                videoFileCB.SelectedIndex = Array.IndexOf(mediaFiles.ToArray(), videoWidget.src);
                            }
                            videoLoop.IsChecked = videoWidget.isLoop;
                            videoShowControlsCB.IsChecked = videoWidget.showControls;
                            if (!videoWidget.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 4;
                                videoHeight.Text = videoWidget.height;
                                videoWidth.Text = videoWidget.width;
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

                        case "Перенос":
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
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
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
                            widget.content = textContent.Text;
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
                        }
                        textContent.Text = String.Empty;
                        textFontFamily.Text = String.Empty;
                        textFontWeight.Text = String.Empty;
                        textFontColor.Text = String.Empty;
                        textFontSize.Text = String.Empty;
                        textBackgroundColor.Text = String.Empty;
                        textBackgroundRadius.Text = String.Empty;
                        break;

                    case "Ссылка":
                        var widgetLink = el.widget as WidgetLink;
                        if (isElUseCSS.IsChecked == false)
                        {
                            widgetLink.useStyle = false;
                            widgetLink.content = linkText.Text;
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
                        }
                        textContent.Text = String.Empty;
                        textFontFamily.Text = String.Empty;
                        textFontWeight.Text = String.Empty;
                        textFontColor.Text = String.Empty;
                        textFontSize.Text = String.Empty;
                        textBackgroundColor.Text = String.Empty;
                        textBackgroundRadius.Text = String.Empty;
                        break;

                    case "Кнопка":
                        var widgetButton = el.widget as WidgetButton;
                        if (isElUseCSS.IsChecked == false)
                        {
                            widgetButton.useStyle = false;
                            widgetButton.content = buttonText.Text;
                            widgetButton.arguments = argumentsForButton.Text;
                            widgetButton.onclick = ButtonOnclickEventsCB.SelectedItem == null ? "" : ButtonOnclickEventsCB.SelectedItem.ToString();
                            widgetButton.fontSize = buttonFontSize.Text == String.Empty ? widgetButton.fontSize : int.Parse(buttonFontSize.Text);
                            widgetButton.fontFamily = buttonFontFamily.Text == String.Empty ? widgetButton.fontFamily : buttonFontFamily.Text;
                            widgetButton.fontWeight = buttonFontWeight.Text == String.Empty ? widgetButton.fontWeight : buttonFontWeight.Text;
                            widgetButton.fontColorHEX = buttonFontColor.Text == String.Empty ? widgetButton.fontColorHEX : buttonFontColor.Text;
                            widgetButton.backgroundColorHEX = buttonBackgroundColor.Text == String.Empty ? widgetButton.backgroundColorHEX : buttonBackgroundColor.Text;
                            widgetButton.borderColorHEX = buttonBorderColor.Text == String.Empty ? widgetButton.borderColorHEX : buttonBorderColor.Text;
                            widgetButton.borderRadius = buttonBorderRadius.Text == String.Empty ? widgetButton.borderRadius : buttonBorderRadius.Text;
                            widgetButton.cursor = buttonCursor.Text == String.Empty ? widgetButton.cursor : buttonCursor.Text;
                        }
                        else
                        {
                            widgetButton.useStyle = true;
                            widgetButton.content = buttonText.Text;
                            widgetButton.arguments = argumentsForButton.Text;
                            widgetButton.onclick = ButtonOnclickEventsCB.SelectedItem == null ? "" : ButtonOnclickEventsCB.SelectedItem.ToString();
                        }
                        buttonText.Text = String.Empty;
                        argumentsForButton.Text = String.Empty;
                        ButtonOnclickEventsCB.Items.Clear();
                        buttonFontSize.Text = String.Empty;
                        buttonFontFamily.Text = String.Empty;
                        buttonFontWeight.Text = String.Empty;
                        buttonFontColor.Text = String.Empty;
                        buttonBackgroundColor.Text = String.Empty;
                        buttonBorderColor.Text = String.Empty;
                        buttonBorderRadius.Text = String.Empty;
                        buttonCursor.Text = String.Empty;
                        break;

                    case "Фото":
                        var widgetImg = el.widget as WidgetImage;
                        if(isElUseCSS.IsChecked == false)
                        {
                            widgetImg.useStyle = false;
                            if (ImageContentSelector.SelectedIndex == 0)
                            {
                                widgetImg.contentType = 'f';
                                widgetImg.href = imageFilesCB.SelectedItem == null ? "" : imageFilesCB.SelectedItem.ToString();
                            }
                            else
                            {
                                widgetImg.contentType = 'l';
                                widgetImg.href = imageLinkToThePhoto.Text;
                            }
                            widgetImg.radius = imageRadius.Text == String.Empty ? widgetImg.radius : imageRadius.Text;
                            widgetImg.height = imageHeight.Text == String.Empty ? widgetImg.height : imageHeight.Text;
                            widgetImg.width = imageWidth.Text == String.Empty ? widgetImg.width : imageWidth.Text;
                            widgetImg.rotationAngle = imageRotAngle.Text == String.Empty ? widgetImg.rotationAngle : imageRotAngle.Text;
                        }
                        else
                        {
                            widgetImg.useStyle = true;
                            if(ImageContentSelector.SelectedIndex == 0)
                            {
                                widgetImg.contentType = 'f';
                                widgetImg.href = imageFilesCB.SelectedItem == null ? "" : imageFilesCB.SelectedItem.ToString();
                            }
                            else
                            {
                                widgetImg.contentType = 'l';
                                widgetImg.href = imageLinkToThePhoto.Text;
                            }
                        }
                        imageFilesCB.Items.Clear();
                        imageLinkToThePhoto.Text = String.Empty;
                        imageRadius.Text = String.Empty;
                        imageHeight.Text = String.Empty;
                        imageWidth.Text = String.Empty;
                        imageRotAngle.Text = String.Empty;
                        break;

                    case "Видео":
                        var videoWidget = el.widget as WidgetVideo;
                        videoWidget.src = videoFileCB.SelectedItem == null ? "" : videoFileCB.SelectedItem.ToString();
                        videoWidget.isLoop = (bool)videoLoop.IsChecked;
                        videoWidget.showControls = (bool)videoShowControlsCB.IsChecked;
                        if (isElUseCSS.IsChecked == false)
                        {
                            videoWidget.useStyle = false;
                            videoWidget.height = videoHeight.Text == String.Empty ? videoWidget.height : videoHeight.Text;
                            videoWidget.width = videoWidth.Text == String.Empty ? videoWidget.width : videoWidth.Text;
                        }
                        else
                        {
                            videoWidget.useStyle = true;
                            videoFileCB.Items.Clear();
                        }
                        videoHeight.Text = String.Empty;
                        videoLoop.IsChecked = false;
                        videoShowControlsCB.IsChecked = false;
                        videoWidth.Text = String.Empty;
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

        #region colors preview rectangles
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

        private void buttonFontColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                buttonFontColorPreview.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(buttonFontColor.Text)));
            }
            catch { }
        }

        private void buttonBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                buttonBackgroundColorPreview.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(buttonBackgroundColor.Text)));
            }
            catch { }
        }

        private void buttonBorderColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                buttonBorderColorPreview.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(buttonBorderColor.Text)));
            }
            catch { }
        }
        #endregion
    }
}
