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
        public bool isSaved = false;

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
                else { return; }
            }
            saver.save(tree[0].widget as WidgetBody, tree[0].widgetsOfScene, Path.Combine(assetsFolder, currentPage));
            isSaved = true;
        }

        public List<WidgetsTreeItem> load(string pageName)
        {
            string path = Path.Combine(assetsFolder, pageName);
            if(!File.Exists(path))
            {
                throw new Exception("Файл не существует!");
            }
            WidgetBodySaveData wpsd = loader.load(path);
            if (wpsd != null)
            {
                List<WidgetsTreeItem> loadedTree = new List<WidgetsTreeItem>();
                loadedTree.Add(new WidgetsTreeItem(wpsd.body));
                List<WidgetsTreeItem> loadedWidgets = new List<WidgetsTreeItem>();
                foreach (WidgetSaveData data in wpsd.widgets)
                {
                    var wid = data.getByName();
                    if (wid is WidgetGroup)
                    {
                        var groupWid = wid as WidgetGroup;
                        groupWid.kids = data.getKids();
                        wid = groupWid;
                    }
                    else if (wid is WidgetList)
                    {
                        var listWid = wid as WidgetList;
                        listWid.content = data.getKids();
                        wid = listWid;
                    }
                    else if(wid is WidgetMarquee)
                    {
                        var marqWid = wid as WidgetMarquee;
                        marqWid.elements = data.getKids();
                        wid = marqWid;
                    }
                    loadedWidgets.Add(new WidgetsTreeItem(wid));
                }
                loadedTree[0].widgetsOfScene = loadedWidgets;
                return loadedTree;
            }
            else
            {
                List<WidgetsTreeItem> newTree = new List<WidgetsTreeItem>
                {
                    new WidgetsTreeItem(new WidgetBody())
                };
                return newTree;
            }
        }

        public static List<WidgetsTreeItem> staticLoad(string assetsFold, string pageName)
        {
            DataLoader _loader = new DataLoader();
            string path = Path.Combine(assetsFold, pageName);
            if (!File.Exists(path))
            {
                throw new Exception("Файл не существует!");
            }
            WidgetBodySaveData wpsd = _loader.load(path);
            if (wpsd != null)
            {
                List<WidgetsTreeItem> loadedTree = new List<WidgetsTreeItem>();
                loadedTree.Add(new WidgetsTreeItem(wpsd.body));
                List<WidgetsTreeItem> loadedWidgets = new List<WidgetsTreeItem>();
                foreach (WidgetSaveData data in wpsd.widgets)
                {
                    var wid = data.getByName();
                    if (wid is WidgetGroup)
                    {
                        var groupWid = wid as WidgetGroup;
                        groupWid.kids = data.getKids();
                        wid = groupWid;
                    }
                    else if (wid is WidgetList)
                    {
                        var listWid = wid as WidgetList;
                        listWid.content = data.getKids();
                        wid = listWid;
                    }
                    else if (wid is WidgetMarquee)
                    {
                        var marqWid = wid as WidgetMarquee;
                        marqWid.elements = data.getKids();
                        wid = marqWid;
                    }
                    loadedWidgets.Add(new WidgetsTreeItem(wid));
                }
                loadedTree[0].widgetsOfScene = loadedWidgets;
                return loadedTree;
            }
            else
            {
                List<WidgetsTreeItem> newTree = new List<WidgetsTreeItem>
                {
                    new WidgetsTreeItem(new WidgetBody())
                };
                return newTree;
            }
        }
    }
}
