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
                homePage = new Uri(Path.Combine(cacheFolder, "INDEX.html"));
                webCanvas.Source = homePage;

                tree.Add(new WidgetsTreeItem(new WidgetBody()));
                sceneTree.ItemsSource = tree;

                foreach (string type in WidgetInput.rus_types)
                {
                    inputTypeCB.Items.Add(type);
                }

                foreach (string option in WidgetGroup.justifying_rus)
                {
                    groupJustifyingCB.Items.Add(option);
                }

                foreach(string option in WidgetBody.imageSize_rus)
                {
                    bodyImageSizeCB.Items.Add(option);
                }

                foreach (string option in WidgetBody.imageRepeat_rus)
                {
                    bodyImageRepeatCB.Items.Add(option);
                }

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

        private void kidsToWidgetOfScene(ref WidgetsTreeItem parent, List<Widget> list)
        {
            if (parent != null)
            {
                parent.widgetsOfScene.Clear();
                WidgetsTreeItem kid = null;
                foreach (Widget widget in list)
                {
                    kid = new WidgetsTreeItem(widget);
                    parent.widgetsOfScene.Add(kid);
                    if (widget is WidgetGroup)
                    {
                        kidsToWidgetOfScene(ref kid, (widget as WidgetGroup).kids);
                    }
                    else if (widget is WidgetList)
                    {
                        kidsToWidgetOfScene(ref kid, (widget as WidgetList).content);
                    }
                }
            }
        }

        private WidgetsTreeItem FindParentItem(WidgetsTreeItem item, WidgetsTreeItem currentItem)
        {
            foreach (var childItem in currentItem.widgetsOfScene)
            {
                if (childItem == item)
                    return currentItem;
                var parentItem = FindParentItem(item, childItem);
                if (parentItem != null)
                    return parentItem;
            }
            return null;
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
                //webCanvas.Source = new Uri("about:blank");
                await Task.Run(() => builder.fastBuild(tree[0].widgetsOfScene, ref config, tree[0].widget as WidgetBody));
                holdOnWindow.Close();
                this.IsEnabled = true;
                webCanvas.Source = homePage;
                goHomeOnWebcanvas();
                Task.Delay(10).Wait();
                webCanvas.Reload();
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.ToString());
                holdOnWindow.Close();
                this.IsEnabled = true;
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

                var selectedItem = sceneTree.SelectedItem as WidgetsTreeItem;
                if (selectedItem == null)
                    selectedItem = tree[0];

                WidgetsTreeItem newWidgetItem = null;

                switch (btn.Content.ToString())
                {
                    case "Текст":
                        newWidgetItem = new WidgetsTreeItem(new WidgetText());
                        break;
                    case "Ссылка":
                        newWidgetItem = new WidgetsTreeItem(new WidgetLink());
                        break;
                    case "Кнопка":
                        newWidgetItem = new WidgetsTreeItem(new WidgetButton());
                        break;
                    case "Перенос":
                        newWidgetItem = new WidgetsTreeItem(new WidgetNextLine());
                        break;
                    case "Фото":
                        newWidgetItem = new WidgetsTreeItem(new WidgetImage());
                        break;
                    case "Видео":
                        newWidgetItem = new WidgetsTreeItem(new WidgetVideo());
                        break;
                    case "Ввод":
                        newWidgetItem = new WidgetsTreeItem(new WidgetInput());
                        break;
                    case "Группа":
                        newWidgetItem = new WidgetsTreeItem(new WidgetGroup());
                        break;
                    case "Список":
                        newWidgetItem = new WidgetsTreeItem(new WidgetList());
                        break;
                    case "HTML":
                        newWidgetItem = new WidgetsTreeItem(new WidgetHtmlSource());
                        break;
                }

                if (newWidgetItem != null)
                {
                    selectedItem = sceneTree.SelectedItem as WidgetsTreeItem;
                    if (selectedItem == null)
                        selectedItem = tree[0];

                    if (selectedItem.widget is WidgetGroup)
                    {
                        var group = selectedItem.widget as WidgetGroup;
                        group.addKid(newWidgetItem.widget);
                        kidsToWidgetOfScene(ref selectedItem, group.kids);
                        refreshTreeview();
                    }
                    else if (selectedItem.widget is WidgetList)
                    {
                        var list = selectedItem.widget as WidgetList;
                        if (newWidgetItem.widget is WidgetText || newWidgetItem.widget is WidgetImage || newWidgetItem.widget is WidgetVideo || newWidgetItem.widget is WidgetList || newWidgetItem.widget is WidgetButton)
                        {
                            list.addContent(newWidgetItem.widget);
                            kidsToWidgetOfScene(ref selectedItem, list.content);
                            refreshTreeview();
                        }
                        else
                        {
                            MessBox.showInfo($"Элемент \"{newWidgetItem.widget.tag}\" не может являться элементом списка!");
                        }
                    }
                    else
                    {
                        var parentItem = FindParentItem(selectedItem, tree[0]);
                        if (parentItem != null && parentItem.widget is WidgetGroup)
                        {
                            var group = parentItem.widget as WidgetGroup;
                            group.addKid(newWidgetItem.widget);
                            kidsToWidgetOfScene(ref parentItem, group.kids);
                            refreshTreeview();
                        }
                        else if (parentItem != null && parentItem.widget is WidgetList)
                        {
                            var list = parentItem.widget as WidgetList;
                            list.addContent(newWidgetItem.widget);
                            kidsToWidgetOfScene(ref parentItem, list.content);
                            refreshTreeview();
                        }
                        else
                        {
                            if (selectedItem.widget.tag == "body")
                            {
                                tree[0].widgetsOfScene.Add(newWidgetItem);
                                refreshTreeview();
                            }
                        }
                    }
                }
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
                                textBackgroundColor.Text = widget.backgroundColorHEX;
                                textBackgroundRadius.Text = widget.backgroundRad;
                                textMargin.Text = widget.margin;
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
                                textBackgroundColor.Text = widgetLink.backgroundColorHEX;
                                textBackgroundRadius.Text = widgetLink.backgroundRad;
                                textMargin.Text = widgetLink.margin;
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
                                buttonMargin.Text = widgetButton.margin;
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
                                imageMargin.Text = widgetImg.margin;
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
                            foreach (string file in mediaFiles)
                            {
                                videoFileCB.Items.Add(file);
                            }
                            if (videoWidget.src != String.Empty && Array.IndexOf(mediaFiles.ToArray(), videoWidget.src) != -1)
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
                                videoMargin.Text = videoWidget.margin;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Ввод":
                            var inputWidget = el.widget as WidgetInput;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.SelectedIndex = 6;
                            contentTabber.Visibility = Visibility.Visible;
                            inputTypeCB.SelectedIndex = Array.IndexOf(WidgetInput.rus_types, inputWidget.type);
                            inputPlaceholder.Text = inputWidget.placeholder;
                            inputValue.Text = inputWidget.content;
                            inputIsReadOnly.IsChecked = inputWidget.isReadonly;
                            if (!inputWidget.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 1;
                                textFontFamily.Text = inputWidget.fontFamily;
                                textFontSize.Text = inputWidget.fontSize.ToString();
                                textFontWeight.Text = inputWidget.fontWeight;
                                textFontColor.Text = inputWidget.fontColorHEX;
                                textBackgroundColor.Text = inputWidget.backgroundColorHEX;
                                textBackgroundRadius.Text = inputWidget.backgroundRad;
                                textMargin.Text = inputWidget.margin;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Группа":
                            var groupWidget = el.widget as WidgetGroup;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Collapsed;
                            contentTabber.SelectedIndex = 0;
                            if (!groupWidget.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 5;
                                groupJustifyingCB.SelectedIndex = Array.IndexOf(WidgetGroup.justifying_rus, groupWidget.justifyContent);
                                groupBackgroundColor.Text = groupWidget.backgroundColorHEX;
                                groupRadius.Text = groupWidget.radius;
                                groupMargin.Text = groupWidget.margin;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "Список":
                            var listWidget = el.widget as WidgetList;
                            contentTabber.Visibility = Visibility.Visible;
                            contentTabber.SelectedIndex = 7;
                            removeElementBtn.IsEnabled = true;
                            listNumCB.IsChecked = listWidget.isNumeric;
                            if (!listWidget.useStyle)
                            {
                                propertiesTabber.Visibility = Visibility.Visible;
                                propertiesTabber.SelectedIndex = 1;
                                textFontFamily.Text = listWidget.fontFamily;
                                textFontSize.Text = listWidget.fontSize.ToString();
                                textFontWeight.Text = listWidget.fontWeight;
                                textFontColor.Text = listWidget.fontColor;
                                textBackgroundColor.Text = listWidget.backgroundColor;
                                textBackgroundRadius.Text = listWidget.borderRadius;
                                textMargin.Text = listWidget.margin;
                            }
                            else
                            {
                                propertiesTabber.Visibility = Visibility.Collapsed;
                                propertiesTabber.SelectedIndex = 0;
                            }
                            break;

                        case "HTML Source":
                            var htmlSrc = el.widget as WidgetHtmlSource;
                            removeElementBtn.IsEnabled = true;
                            contentTabber.Visibility = Visibility.Visible;
                            contentTabber.SelectedIndex = 8;
                            propertiesTabber.Visibility = Visibility.Collapsed;
                            propertiesTabber.SelectedIndex = 0;
                            List<string> names = FilesWorker.getAllFilesByExt(assetsFolder, ".html");
                            htmlFileCB.Items.Clear();
                            foreach (string name in names)
                            {
                                htmlFileCB.Items.Add(name);
                            }
                            if (htmlSrc.type == WidgetHtmlSource.ContentTypeOfHtmlSource.text)
                            {
                                htmlSourceTextBox.Text = htmlSrc.content;
                                htmlContentTabber.SelectedIndex = 0;
                            }
                            else
                            {
                                htmlContentTabber.SelectedIndex = 1;
                                if(htmlSrc != null)
                                {
                                    int index = Array.IndexOf(names.ToArray(), htmlSrc.content);
                                    if (htmlSrc.content != null && index == -1)
                                    {
                                        htmlSrc.content = String.Empty;
                                    }
                                    else
                                    {
                                        htmlFileCB.SelectedIndex = index;
                                    }
                                }
                            }
                            break;

                        case "body":
                            var body = el.widget as WidgetBody;
                            removeElementBtn.IsEnabled = false;
                            contentTabber.SelectedIndex = 0;
                            contentTabber.Visibility = Visibility.Collapsed;
                            if (!body.useStyle)
                            {
                                backPhotoFilesCB.Items.Clear();
                                List<string> files2 = FilesWorker.getAllFilesByExt(assetsFolder, new string[] { ".bmp", ".png", ".jpeg", ".jpg" });
                                foreach (string file in files2)
                                {
                                    backPhotoFilesCB.Items.Add(file);
                                }
                                propertiesTabber.SelectedIndex = 6;
                                propertiesTabber.Visibility = Visibility.Visible;
                                if(body.type == WidgetBody.CommonType.color)
                                {
                                    bodyFillTypeTabber.SelectedIndex = 0;
                                    pageBackgroudColorTB.Text = body.color;
                                }
                                else
                                {
                                    bodyFillTypeTabber.SelectedIndex = 1;
                                    if(body.photoType == WidgetBody.PhotoType.link)
                                    {
                                        bodyImageTypeTabber.SelectedIndex = 0;
                                        backPhotoLinkTB.Text = body.imageHref;
                                    }
                                    else
                                    {
                                        bodyImageTypeTabber.SelectedIndex = 1;
                                        int index = Array.IndexOf(files2.ToArray(), body.imageHref);
                                        if (body.imageHref != null && index != -1)
                                        {
                                            backPhotoFilesCB.SelectedIndex = index;
                                        }
                                        else
                                        {
                                            body.imageHref = null;
                                        }
                                    }
                                    bodyBackImageBlurTB.Text = body.blurRadius;
                                    bodyImageSizeCB.SelectedIndex = body.imageSize != null ? Array.IndexOf(WidgetBody.imageSize_rus, body.imageSize) : 0;
                                    bodyImageRepeatCB.SelectedIndex = body.imageRepeat != null ? Array.IndexOf(WidgetBody.imageRepeat_rus, body.imageRepeat) : 0;
                                }
                            }
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
                            widget.fontFamily = textFontFamily.Text == String.Empty ? "Arial" : textFontFamily.Text;
                            widget.fontWeight = textFontWeight.Text == String.Empty ? "400" : textFontWeight.Text;
                            widget.fontColorHEX = textFontColor.Text == String.Empty ? "#000000" : textFontColor.Text;
                            widget.fontSize = textFontSize.Text == String.Empty ? 18 : int.Parse(textFontSize.Text);
                            widget.backgroundColorHEX = textBackgroundColor.Text == String.Empty ? "Transparent" : textBackgroundColor.Text;
                            widget.backgroundRad = textBackgroundRadius.Text == String.Empty ? "0" : textBackgroundRadius.Text;
                            widget.margin = textMargin.Text == String.Empty ? "0 0 0 0" : textMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
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
                        textMargin.Text = String.Empty;
                        break;

                    case "Ссылка":
                        var widgetLink = el.widget as WidgetLink;
                        if (isElUseCSS.IsChecked == false)
                        {
                            widgetLink.useStyle = false;
                            widgetLink.content = linkText.Text;
                            widgetLink.href = linkLinkAdress.Text;
                            widgetLink.fontFamily = textFontFamily.Text == String.Empty ? "Arial" : textFontFamily.Text;
                            widgetLink.fontWeight = textFontWeight.Text == String.Empty ? "400" : textFontWeight.Text;
                            widgetLink.fontColorHEX = textFontColor.Text == String.Empty ? "#000000" : textFontColor.Text;
                            widgetLink.fontSize = textFontSize.Text == String.Empty ? 18 : int.Parse(textFontSize.Text);
                            widgetLink.backgroundColorHEX = textBackgroundColor.Text == String.Empty ? "Transparent" : textBackgroundColor.Text;
                            widgetLink.backgroundRad = textBackgroundRadius.Text == String.Empty ? "0" : textBackgroundRadius.Text;
                            widgetLink.margin = textMargin.Text == String.Empty ? "0 0 0 0" : textMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
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
                        textMargin.Text = String.Empty;
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
                            widgetButton.fontSize = buttonFontSize.Text == String.Empty ? 18 : int.Parse(buttonFontSize.Text);
                            widgetButton.fontFamily = buttonFontFamily.Text == String.Empty ? "Arial" : buttonFontFamily.Text;
                            widgetButton.fontWeight = buttonFontWeight.Text == String.Empty ? "400" : buttonFontWeight.Text;
                            widgetButton.fontColorHEX = buttonFontColor.Text == String.Empty ? "#ffffff" : buttonFontColor.Text;
                            widgetButton.backgroundColorHEX = buttonBackgroundColor.Text == String.Empty ? "#373737" : buttonBackgroundColor.Text;
                            widgetButton.borderColorHEX = buttonBorderColor.Text == String.Empty ? "#000000" : buttonBorderColor.Text;
                            widgetButton.margin = buttonMargin.Text == String.Empty ? "0 0 0 0" : buttonMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
                            widgetButton.borderRadius = buttonBorderRadius.Text == String.Empty ? "0" : buttonBorderRadius.Text;
                            widgetButton.cursor = buttonCursor.Text == String.Empty ? "pointer" : buttonCursor.Text;
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
                        buttonMargin.Text = String.Empty;
                        buttonBorderColor.Text = String.Empty;
                        buttonBorderRadius.Text = String.Empty;
                        buttonCursor.Text = String.Empty;
                        break;

                    case "Фото":
                        var widgetImg = el.widget as WidgetImage;
                        if (isElUseCSS.IsChecked == false)
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
                            widgetImg.radius = imageRadius.Text == String.Empty ? "0" : imageRadius.Text;
                            widgetImg.height = imageHeight.Text == String.Empty ? "100" : imageHeight.Text;
                            widgetImg.width = imageWidth.Text == String.Empty ? "100" : imageWidth.Text;
                            widgetImg.rotationAngle = imageRotAngle.Text == String.Empty ? "0" : imageRotAngle.Text;
                            widgetImg.margin = imageMargin.Text == String.Empty ? "0 0 0 0" : imageMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
                        }
                        else
                        {
                            widgetImg.useStyle = true;
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
                        }
                        imageFilesCB.Items.Clear();
                        imageLinkToThePhoto.Text = String.Empty;
                        imageRadius.Text = String.Empty;
                        imageHeight.Text = String.Empty;
                        imageMargin.Text = String.Empty;
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
                            videoWidget.height = videoHeight.Text == String.Empty ? "100" : videoHeight.Text;
                            videoWidget.width = videoWidth.Text == String.Empty ? "100" : videoWidth.Text;
                            videoWidget.margin = videoMargin.Text == String.Empty ? "0 0 0 0" : videoMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
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
                        videoMargin.Text = String.Empty;
                        break;

                    case "Ввод":
                        var inputWidget = el.widget as WidgetInput;
                        inputWidget.type = WidgetInput.rus_types[inputTypeCB.SelectedIndex];
                        inputWidget.placeholder = inputPlaceholder.Text;
                        inputWidget.content = inputValue.Text;
                        inputWidget.isReadonly = (bool)inputIsReadOnly.IsChecked;
                        if (isElUseCSS.IsChecked == false)
                        {
                            inputWidget.useStyle = false;
                            inputWidget.fontFamily = textFontFamily.Text == String.Empty ? "Arial" : textFontFamily.Text;
                            inputWidget.fontWeight = textFontWeight.Text == String.Empty ? "400" : textFontWeight.Text;
                            inputWidget.fontColorHEX = textFontColor.Text == String.Empty ? "#000000" : textFontColor.Text;
                            inputWidget.fontSize = textFontSize.Text == String.Empty ? 18 : int.Parse(textFontSize.Text);
                            inputWidget.backgroundColorHEX = textBackgroundColor.Text == String.Empty ? "Transparent" : textBackgroundColor.Text;
                            inputWidget.backgroundRad = textBackgroundRadius.Text == String.Empty ? "0" : textBackgroundRadius.Text;
                            inputWidget.margin = textMargin.Text == String.Empty ? "0 0 0 0" : textMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
                        }
                        else
                        {
                            inputWidget.useStyle = true;
                        }
                        textContent.Text = String.Empty;
                        textFontFamily.Text = String.Empty;
                        textFontWeight.Text = String.Empty;
                        textFontColor.Text = String.Empty;
                        textFontSize.Text = String.Empty;
                        textMargin.Text = String.Empty;
                        textBackgroundColor.Text = String.Empty;
                        textBackgroundRadius.Text = String.Empty;
                        break;

                    case "Группа":
                        var groupWidget = el.widget as WidgetGroup;
                        if (isElUseCSS.IsChecked == false)
                        {
                            groupWidget.useStyle = false;
                            groupWidget.justifyContent = WidgetGroup.justifying_rus[groupJustifyingCB.SelectedIndex];
                            groupWidget.backgroundColorHEX = groupBackgroundColor.Text == String.Empty ? "Transparent" : groupBackgroundColor.Text;
                            groupWidget.radius = groupRadius.Text == String.Empty ? "0" : groupRadius.Text;
                            groupWidget.margin = groupMargin.Text == String.Empty ? "0 0 0 0" : groupMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
                        }
                        else
                        {
                            groupWidget.useStyle = true;
                        }
                        groupRadius.Text = String.Empty;
                        groupMargin.Text = String.Empty;
                        groupBackgroundColor.Text = String.Empty;
                        break;

                    case "HTML Source":
                        var htmlSrc = el.widget as WidgetHtmlSource;
                        if(htmlContentTabber.SelectedIndex == 0)
                        {
                            htmlSrc.content = htmlSourceTextBox.Text;
                            htmlSrc.type = WidgetHtmlSource.ContentTypeOfHtmlSource.text;
                        }
                        else
                        {
                            htmlSrc.type = WidgetHtmlSource.ContentTypeOfHtmlSource.file;
                            htmlSrc.content = htmlFileCB.SelectedItem == null ? "" : htmlFileCB.SelectedItem.ToString();
                        }
                        htmlSourceTextBox.Text = String.Empty;
                        htmlFileCB.Items.Clear();
                        break;

                    case "Список":
                        var listWidget = el.widget as WidgetList;
                        listWidget.isNumeric = (bool)listNumCB.IsChecked;
                        if (isElUseCSS.IsChecked == false)
                        {
                            listWidget.useStyle = false;
                            listWidget.fontFamily = textFontFamily.Text == String.Empty ? "Arial" : textFontFamily.Text;
                            listWidget.fontWeight = textFontWeight.Text == String.Empty ? "400" : textFontWeight.Text;
                            listWidget.fontColor = textFontColor.Text == String.Empty ? "#000000" : textFontColor.Text;
                            listWidget.fontSize = textFontSize.Text == String.Empty ? 18 : int.Parse(textFontSize.Text);
                            listWidget.backgroundColor = textBackgroundColor.Text == String.Empty ? "Transparent" : textBackgroundColor.Text;
                            listWidget.borderRadius = textBackgroundRadius.Text == String.Empty ? "0" : textBackgroundRadius.Text;
                            listWidget.margin = textMargin.Text == String.Empty ? "0 0 0 0" : textMargin.Text.Trim().Replace(",", null).Replace(";", null).Replace("/", null);
                        }
                        else
                        {
                            listWidget.useStyle = true;
                        }
                        textFontFamily.Text = String.Empty;
                        textFontWeight.Text = String.Empty;
                        textFontColor.Text = String.Empty;
                        textFontSize.Text = String.Empty;
                        textBackgroundColor.Text = String.Empty;
                        textBackgroundRadius.Text = String.Empty;
                        textMargin.Text = String.Empty;
                        break;

                    case "body":
                        var body = el.widget as WidgetBody;
                        if (isElUseCSS.IsChecked == false)
                        {
                            body.useStyle = false;
                            if (bodyFillTypeTabber.SelectedIndex == 0)
                            {
                                body.color = pageBackgroudColorTB.Text == String.Empty ? "#ffffff" : pageBackgroudColorTB.Text;
                                body.type = WidgetBody.CommonType.color;
                            }
                            else
                            {
                                body.type = WidgetBody.CommonType.photo;
                                if (bodyImageTypeTabber.SelectedIndex == 0)
                                {
                                    body.photoType = WidgetBody.PhotoType.link;
                                    body.imageHref = backPhotoLinkTB.Text;
                                }
                                else
                                {
                                    body.photoType = WidgetBody.PhotoType.image;
                                    body.imageHref = backPhotoFilesCB.SelectedItem == null ? "" : backPhotoFilesCB.SelectedItem.ToString();
                                }
                                body.blurRadius = bodyBackImageBlurTB.Text == String.Empty ? "0" : bodyBackImageBlurTB.Text;
                                body.imageSize = WidgetBody.imageSize_rus[bodyImageSizeCB.SelectedIndex < 0 ? 0 : bodyImageSizeCB.SelectedIndex];
                                body.imageRepeat = WidgetBody.imageRepeat_rus[bodyImageRepeatCB.SelectedIndex < 0 ? 0 : bodyImageRepeatCB.SelectedIndex];
                            }
                        }
                        else
                        {
                            body.useStyle = true;
                        }
                        pageBackgroudColorTB.Text = String.Empty;
                        backPhotoLinkTB.Text = String.Empty;
                        bodyBackImageBlurTB.Text = String.Empty;
                        break;

                    default: break;
                }

                refreshTreeview();
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private void removeElementBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = sceneTree.SelectedItem as WidgetsTreeItem;
                if (selectedItem != null)
                {
                    var parentItem = FindParentItem(selectedItem, tree[0]);
                    if (parentItem.widget is WidgetGroup && parentItem != null)
                    {
                        var group = parentItem.widget as WidgetGroup;
                        group.removeKid(selectedItem.widget);
                        parentItem.widgetsOfScene.Remove(selectedItem);
                    }
                    else if (parentItem.widget is WidgetList && parentItem != null)
                    {
                        var list = parentItem.widget as WidgetList;
                        list.removeContent(selectedItem.widget);
                        parentItem.widgetsOfScene.Remove(selectedItem);
                    }
                    else if (parentItem != null)
                    {
                        parentItem.widgetsOfScene.Remove(selectedItem);
                    }
                    refreshTreeview();
                }
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
            }
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

        private void groupBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                groupBackColorPreview.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(groupBackgroundColor.Text)));
            }
            catch { }
        }

        private void pageBackgroudColorTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                pageBackColor.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(pageBackgroudColorTB.Text)));
            }
            catch { }
        }

        #endregion


        #region under tree buttons
        private void treeElementUpBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = sceneTree.SelectedItem as WidgetsTreeItem;
            if (selectedItem != null)
            {
                var parentItem = FindParentItem(selectedItem, tree[0]);
                if (parentItem != null)
                {
                    var index = parentItem.widgetsOfScene.IndexOf(selectedItem);
                    if (index > 0)
                    {
                        if (parentItem == tree[0])
                        {
                            tree[0].widgetsOfScene.RemoveAt(index);
                            tree[0].widgetsOfScene.Insert(index - 1, selectedItem);
                        }
                        else
                        {
                            parentItem.widgetsOfScene.RemoveAt(index);
                            parentItem.widgetsOfScene.Insert(index - 1, selectedItem);
                            if (parentItem.widget is WidgetList)
                            {
                                var list = parentItem.widget as WidgetList;
                                list.content.RemoveAt(index);
                                list.content.Insert(index - 1, selectedItem.widget);
                            }
                            else if (parentItem.widget is WidgetGroup)
                            {
                                var group = parentItem.widget as WidgetGroup;
                                group.kids.RemoveAt(index);
                                group.kids.Insert(index - 1, selectedItem.widget);
                            }
                        }

                        refreshTreeview();
                    }
                    else
                    {
                        MessBox.showInfo("Невозможно переместить этот элемент.");
                    }
                }
            }
        }

        private void treeElementDownBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = sceneTree.SelectedItem as WidgetsTreeItem;
            if (selectedItem != null)
            {
                var parentItem = FindParentItem(selectedItem, tree[0]);
                if (parentItem != null)
                {
                    var index = parentItem.widgetsOfScene.IndexOf(selectedItem);
                    if (index < parentItem.widgetsOfScene.Count - 1)
                    {
                        if (parentItem == tree[0])
                        {
                            tree[0].widgetsOfScene.RemoveAt(index);
                            tree[0].widgetsOfScene.Insert(index + 1, selectedItem);
                        }
                        else
                        {
                            parentItem.widgetsOfScene.RemoveAt(index);
                            parentItem.widgetsOfScene.Insert(index + 1, selectedItem);
                            if (parentItem.widget is WidgetList)
                            {
                                var list = parentItem.widget as WidgetList;
                                list.content.RemoveAt(index);
                                list.content.Insert(index + 1, selectedItem.widget);
                            }
                            else if (parentItem.widget is WidgetGroup)
                            {
                                var group = parentItem.widget as WidgetGroup;
                                group.kids.RemoveAt(index);
                                group.kids.Insert(index + 1, selectedItem.widget);
                            }
                        }

                        refreshTreeview();

                    }
                    else
                    {
                        MessBox.showInfo("Невозможно переместить этот элемент.");
                    }
                }
            }
        }

        private void makeKidToElementBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedTreeItem = sceneTree.SelectedItem as WidgetsTreeItem;
            if (selectedTreeItem != null)
            {
                var parentTreeItem = FindParentItem(selectedTreeItem, tree[0]);
                if (parentTreeItem != null)
                {
                    int index = Array.IndexOf(parentTreeItem.widgetsOfScene.ToArray(), selectedTreeItem) + 1;
                    if (index != -1 && index < parentTreeItem.widgetsOfScene.Count)
                    {
                        var nextEl = parentTreeItem.widgetsOfScene[index];
                        if (nextEl != null)
                        {
                            bool operationDone = false;
                            if (nextEl.widget is WidgetGroup)
                            {
                                var group = nextEl.widget as WidgetGroup;
                                group.addKid(selectedTreeItem.widget);
                                operationDone = true;
                            }
                            else if (nextEl.widget is WidgetList)
                            {
                                var list = nextEl.widget as WidgetList;
                                list.addContent(selectedTreeItem.widget);
                                operationDone = list.doesContentExist(selectedTreeItem.widget);
                            }

                            if (operationDone)
                            {
                                parentTreeItem.widgetsOfScene.Remove(selectedTreeItem);
                                if (nextEl.widget is WidgetGroup)
                                {
                                    var group = nextEl.widget as WidgetGroup;
                                    kidsToWidgetOfScene(ref nextEl, group.kids);
                                }
                                else if (nextEl.widget is WidgetList)
                                {
                                    var list = nextEl.widget as WidgetList;
                                    kidsToWidgetOfScene(ref nextEl, list.content);
                                }
                                refreshTreeview();
                            }
                        }
                    }
                }
            }
        }

        private void leaveParentElementBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedTreeItem = sceneTree.SelectedItem as WidgetsTreeItem;
            var parentTreeItem = FindParentItem(selectedTreeItem, tree[0]);

            if (parentTreeItem != null)
            {

                var par = FindParentItem(parentTreeItem, tree[0]);
                if (par != null)
                {
                    if (parentTreeItem.widget.tag == "body")
                    {
                        return;
                    }

                    else
                    {
                        if (parentTreeItem.widget is WidgetGroup)
                        {
                            (parentTreeItem.widget as WidgetGroup).removeKid(selectedTreeItem.widget);
                            parentTreeItem.widgetsOfScene.Remove(selectedTreeItem);
                        }
                        else if (parentTreeItem.widget is WidgetList)
                        {
                            (parentTreeItem.widget as WidgetList).removeContent(selectedTreeItem.widget);
                            parentTreeItem.widgetsOfScene.Remove(selectedTreeItem);
                        }
                        par.widgetsOfScene.Add(selectedTreeItem);
                        if (par != null && par.widget is WidgetGroup)
                        {
                            var group = par.widget as WidgetGroup;
                            group.addKid(selectedTreeItem.widget);
                            kidsToWidgetOfScene(ref par, group.kids);
                            refreshTreeview();
                        }
                        else if (par != null && par.widget is WidgetList)
                        {
                            var list = par.widget as WidgetList;
                            list.addContent(selectedTreeItem.widget);
                            kidsToWidgetOfScene(ref par, list.content);
                            refreshTreeview();
                        }
                    }

                    refreshTreeview();
                }
            }
        }

        #endregion
    }
}
