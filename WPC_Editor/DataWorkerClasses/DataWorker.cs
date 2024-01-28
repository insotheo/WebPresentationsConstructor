using System;
using System.IO;
using WPC_Editor.Widgets;
using System.Collections.Generic;
using MessageBoxesWindows;
using Microsoft.Win32;

namespace WPC_Editor.DataWorkerClasses
{
    public class DataWorker
    {
        //page extension - ".wpcsave"

        private DataSaver saver;
        private DataLoader loader;

        public string currentPage;

        private readonly string assetsFolder;

        public DataWorker(string assetsFolder)
        {
            saver = new DataSaver();
            loader = new DataLoader();
            currentPage = String.Empty;
            this.assetsFolder = assetsFolder;
        }

        public void setPage(string newPage)
        {
            if(newPage != currentPage && newPage != null)
            {
                if(!File.Exists(Path.Combine(assetsFolder, newPage)))
                {
                    throw new Exception("Попытка установить несуществующий файл страницы.");
                }
                currentPage = newPage;
            }
        }

        public void save(List<WidgetsTreeItem> tree)
        {
            if(tree == null)
            {
                throw new Exception("Пустое дерево");
            }
            if(currentPage == null || currentPage == String.Empty)
            {
                if(MessBox.showQuestionWithTwoOptions("У вас нет файла для сохранения. Желаете его создать?") == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        SaveFileDialog sfd = new SaveFileDialog()
                        {
                            Filter = "Page(*.wpcsave)| *.wpcsave",
                            AddExtension = true,
                            InitialDirectory = assetsFolder
                        };
                        sfd.ShowDialog();
                        if (sfd.FileName != null)
                        {
                            FileStream fs = File.Create(sfd.FileName);
                            fs.Close();
                            currentPage = sfd.FileName;
                        }
                    }
                    catch { }
                }
            }
            saver.save(tree[0].widget as WidgetBody, tree[0].widgetsOfScene, Path.Combine(assetsFolder, currentPage));
        }
    }
}
