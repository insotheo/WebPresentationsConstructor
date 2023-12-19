using System.Windows;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using MessageBoxesWindows;
using Microsoft.Win32;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для FileViewerWindow.xaml
    /// </summary>
    public partial class FileViewerWindow : Window
    {
        private List<FileListboxItemClass> files;
        private string folder;

        private SaveFileDialog createFileDialog;
        private OpenFileDialog importFileDialog;

        public ConfigWorker configWorker;

        public FileViewerWindow(string assetsPath, ref ConfigWorker configWorker)
        {
            InitializeComponent();
            folder = assetsPath;
            files = new List<FileListboxItemClass>();
            refreshListBox();
            this.configWorker = configWorker;
        }

        private void refreshListBox()
        {
            filesLB.Items.Clear();
            files.Clear();
            if (Directory.GetFiles(folder).Length > 0)
            {
                foreach (string asset in Directory.GetFiles(folder))
                {
                    files.Add(new FileListboxItemClass(asset));
                }
                foreach (var file in files)
                {
                    filesLB.Items.Add(file);
                }
            }
        }

        #region left buttons

        private void createFileBtn_Click(object sender, RoutedEventArgs e)
        {
            createFileDialog = new SaveFileDialog()
            {
                InitialDirectory = folder,
                Filter = "Script(*.js)| *.js|Style(*.css)| *.css|Text(*.txt)| *.txt",
                AddExtension = true
            };
            createFileDialog.ShowDialog();
            if(createFileDialog.FileName != String.Empty)
            {
                FileStream creation = File.Create(createFileDialog.FileName);
                creation.Close();
                refreshListBox();
            }
        }

        private void removeFileBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (filesLB.Items.Count > 0 && filesLB.SelectedItem != null)
                {
                    if (MessBox.showQuestionWithTwoOptions($"Вы уверены, что хотите удалить файл \"{(filesLB.SelectedItem as FileListboxItemClass).fileName}\"?") == MessageBoxResult.Yes)
                    {
                        string p = (filesLB.SelectedItem as FileListboxItemClass).path.Trim();
                        if(Path.GetExtension(p) == ".js" || Path.GetExtension(p) == ".css")
                        {
                            switch (Path.GetExtension(p))
                            {
                                case ".js":
                                    if(configWorker.usingScripts.IndexOf(Path.GetFileName(p)) != -1)
                                    {
                                        configWorker.usingScripts.RemoveAt(configWorker.usingScripts.IndexOf(Path.GetFileName(p).Trim()));
                                    }
                                    break;
                                case ".css":
                                    if (configWorker.usingStyles.IndexOf(Path.GetFileName(p)) != -1)
                                    {
                                        configWorker.usingStyles.RemoveAt(configWorker.usingStyles.IndexOf(Path.GetFileName(p).Trim()));
                                    }
                                    break;
                            }
                        }
                        File.Delete(p);
                        configWorker.overwriteFile();
                        refreshListBox();
                    }
                }
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private void importFileBtn_Click(object sender, RoutedEventArgs e)
        {
            importFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Multiselect = false,
                Filter = "Script(*.js)| *.js|Style(*.css)| *.css| Image(*.bmp, *.png, *.jpeg, *.jpg)| *.bmp; *.png; *.jpg; *.jpeg|Text(*.txt)| *.txt| All(*.*)| *.*",
                InitialDirectory = Path.Combine("C:\\Users", Environment.UserName, "Documents")
            };
            importFileDialog.ShowDialog();
            if(importFileDialog.FileName != String.Empty)
            {
                File.Copy(importFileDialog.FileName,
                    Path.Combine(folder, Path.GetFileName(importFileDialog.FileName)));
                refreshListBox();
            }
        }

        private void vsCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "code",
                    Arguments = $"\"{folder}\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                this.Close();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        private void showInExplorerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{folder}\""
                });
                this.Close();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }

        #endregion

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (searchTB.Text != String.Empty)
            {
                files.Clear();
                foreach (string asset in Directory.GetFiles(folder))
                {
                    files.Add(new FileListboxItemClass(asset));
                }
                List<FileListboxItemClass> res = new List<FileListboxItemClass>();
                foreach(var file in files)
                {
                    if (file.fileName.ToLower().Trim().Contains(searchTB.Text.ToLower().Trim()))
                    {
                        res.Add(file);
                    }
                }
                filesLB.Items.Clear();
                foreach(var file in res)
                {
                    filesLB.Items.Add(file);
                }
            }
            else
                refreshListBox();
        }
    }
}
