using System.Windows;
using System;
using MessageBoxesWindows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для ConfigEditorWindow.xaml
    /// </summary>
    public partial class ConfigEditorWindow : Window
    {
        public ConfigWorker newConfig;
        public bool isApplied = false;

        public ConfigEditorWindow(ref ConfigWorker config, string assetsFolder)
        {
            InitializeComponent();
            if (config != null)
            {
                newConfig = config;
                this.Title = this.Title.Replace("ptitle", config.title);
                titleTB.Text = config.title;
                charsetTB.Text = config.charset;
                languageTB.Text = config.language;
                foreach(string style in config.usingStyles)
                {
                    if (style != String.Empty)
                    {
                        stylesLB.Items.Add(style.Trim());
                    }
                }
                foreach(string script in config.usingScripts)
                {
                    if (script != String.Empty)
                    {
                        scriptsLB.Items.Add(script.Trim());
                    }
                }
                foreach(string style in FilesWorker.getAllStyles(assetsFolder))
                {
                    if (style != String.Empty)
                    {
                        stylesCB.Items.Add(style.Trim());
                    }
                }
                foreach(string script in FilesWorker.getAllScripts(assetsFolder))
                {
                    if (script != String.Empty)
                    {
                        scriptsCB.Items.Add(script.Trim());
                    }
                }
            }
        }

        private void updtaeInfoAboutTheCreatorBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                newConfig.updateInformationAboutTheCreator();
            }
            catch (Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        #region Styles buttons
        private void addToStylesBtn_Click(object sender, RoutedEventArgs e)
        {
            if(stylesCB.SelectedItem != null)
            {
                stylesLB.Items.Add(stylesCB.SelectedItem);
                stylesLB.SelectedItem = null;
            }
        }

        private void removeFromStylesBtn_Click(object sender, RoutedEventArgs e)
        {
            if(stylesLB.SelectedItem != null && stylesLB.Items.Count > 0)
            {
                stylesLB.Items.Remove(stylesLB.SelectedItem);
                stylesLB.SelectedItem = null;
            }
        }

        private void upPositionOfElInStylesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (stylesLB.SelectedItem != null && stylesLB.Items.Count > 0 
                && stylesLB.SelectedIndex - 1 > -1)
            {
                var next = stylesLB.Items[stylesLB.SelectedIndex - 1];
                var now = stylesLB.Items[stylesLB.SelectedIndex];
                stylesLB.Items[stylesLB.SelectedIndex - 1] = now;
                stylesLB.Items[stylesLB.SelectedIndex] = next;
            }
        }

        private void downPositionOfElInStylesBtn_Click(object sender, RoutedEventArgs e)
        {
            if(stylesLB.SelectedItem != null && stylesLB.Items.Count > 0 
                && stylesLB.SelectedIndex + 1 != stylesLB.Items.Count)
            {
                var prev = stylesLB.Items[stylesLB.SelectedIndex + 1];
                var now = stylesLB.Items[stylesLB.SelectedIndex];
                stylesLB.Items[stylesLB.SelectedIndex + 1] = now;
                stylesLB.Items[stylesLB.SelectedIndex] = prev;
            }
        }
        #endregion

        #region Scripts buttons
        private void addScriptToScriptsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (scriptsCB.SelectedItem != null)
            {
                scriptsLB.Items.Add(scriptsCB.SelectedItem);
                scriptsLB.SelectedItem = null;
            }
        }

        private void removeScriptFromScrtipsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (scriptsLB.SelectedItem != null && scriptsLB.Items.Count > 0)
            {
                scriptsLB.Items.Remove(scriptsLB.SelectedItem);
                scriptsLB.SelectedItem = null;
            }
        }

        private void upPositionOfElInScriptsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (scriptsLB.SelectedItem != null && scriptsLB.Items.Count > 0
                && scriptsLB.SelectedIndex - 1 > -1)
            {
                var next = scriptsLB.Items[scriptsLB.SelectedIndex - 1];
                var now = scriptsLB.Items[scriptsLB.SelectedIndex];
                scriptsLB.Items[scriptsLB.SelectedIndex - 1] = now;
                scriptsLB.Items[scriptsLB.SelectedIndex] = next;
            }
        }

        private void downPositionOfElInScriptsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (scriptsLB.SelectedItem != null && scriptsLB.Items.Count > 0
                && scriptsLB.SelectedIndex + 1 != scriptsLB.Items.Count)
            {
                var prev = scriptsLB.Items[scriptsLB.SelectedIndex + 1];
                var now = scriptsLB.Items[scriptsLB.SelectedIndex];
                scriptsLB.Items[scriptsLB.SelectedIndex + 1] = now;
                scriptsLB.Items[scriptsLB.SelectedIndex] = prev;
            }

        }
        #endregion

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            isApplied = false;
            this.Close();
        }

        private async void saveAndApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                newConfig.title = titleTB.Text.Trim().Replace(":", null);
                newConfig.language = languageTB.Text.Trim().Replace(":", null);
                newConfig.charset = charsetTB.Text.Trim().Replace(":", null);

                newConfig.usingScripts.Clear();
                newConfig.usingStyles.Clear();

                List<string> list = new List<string>();
                foreach(var script in scriptsLB.Items)
                {
                    list.Add(script.ToString());
                }
                newConfig.usingScripts = list.GroupBy(x => x).Where(x => x.Count() == 1).Select(x => x.Key).ToList();
                list.Clear();
                foreach (var style in stylesLB.Items)
                {
                    list.Add(style.ToString());
                }
                newConfig.usingStyles = list.GroupBy(x => x).Where(x => x.Count() == 1).Select(x => x.Key).ToList();
                list.Clear();

                isApplied = true;
                this.IsEnabled = false;
                await Task.Run(() => newConfig.overwriteFile());
                MessBox.showInfo("Успешно!");
                this.Close();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }
    }
}
