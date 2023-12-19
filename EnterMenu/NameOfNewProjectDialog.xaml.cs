using System.Windows;
using MessageBoxesWindows;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ProjectsManager
{
    /// <summary>
    /// Логика взаимодействия для NameOfNewProjectDialog.xaml
    /// </summary>
    public partial class NameOfNewProjectDialog : Window
    {
        public bool isCreated = false;

        public NameOfNewProjectDialog()
        {
            InitializeComponent();
        }

        private void cancelCreationBtn_Click(object sender, RoutedEventArgs e)
        {
            isCreated = false;
            this.Close();
        }

        private void confirmCreationBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(nameTB.Text.Trim() == String.Empty)
                {
                    throw new Exception("Пустое имя проекта невозможно!");
                }
                foreach(char badChar in Path.GetInvalidFileNameChars())
                {
                    if (nameTB.Text.ToLower().Trim().Contains(badChar.ToString()))
                    {
                        throw new Exception($"Название проекта не может содержать символы, запрещенные в системе! В данном случае: '{badChar}'");
                    }
                }
                if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim())))
                {
                    throw new Exception("Проект с таким названием уже существует!");
                }
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim()));
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim(), "Assets"));
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim(), "cache"));
                FileStream configFile = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim(), "settings.config"));
                configFile.Close();
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "projects", nameTB.Text.Trim(), "settings.config"),
                    $"title: {nameTB.Text.Trim()}\nlanguage: Rus\ncreatorsUsername: {Environment.UserName}\ncreatorsMachineName: {Environment.MachineName}\ncreatorsOS: {Environment.OSVersion.VersionString}\nis64Bit: {Environment.Is64BitOperatingSystem.ToString()}\nusingStyles:\nusingScripts:\nappEditorVersion: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}\ncharset: UTF-8");
                isCreated = true;
                this.Close();
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
        }
    }
}
