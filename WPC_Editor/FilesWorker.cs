﻿using System.IO;
using System.Collections.Generic;

namespace WPC_Editor
{
    static class FilesWorker
    {
        public static readonly string[] imgExtensions = new string[] { ".bmp", ".png", ".jpeg", ".jpg" };

        public static List<string> getAllScripts(string assetsFolderPath)
        {
            List<string> scriptsNames = new List<string>();
            foreach(string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "scripts")))
            {
                if(Path.GetExtension(file) == ".js")
                {
                    scriptsNames.Add(Path.GetFileName(file));
                }
            }
            foreach (string file in Directory.GetFiles(assetsFolderPath))
            {
                if (Path.GetExtension(file) == ".js")
                {
                    scriptsNames.Add(Path.GetFileName(file));
                }
            }
            return scriptsNames;
        }

        public static List<string> getAllStyles(string assetsFolderPath)
        {
            List<string> stylesNames = new List<string>();
            foreach (string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "styles")))
            {
                if (Path.GetExtension(file) == ".css")
                {
                    stylesNames.Add(Path.GetFileName(file));
                }
            }
            foreach (string file in Directory.GetFiles(assetsFolderPath))
            {
                if (Path.GetExtension(file) == ".css")
                {
                    stylesNames.Add(Path.GetFileName(file));
                }
            }
            return stylesNames;
        }

        public static List<string> getAllFilesByExt(string assetsFolderPath, string ext)
        {
            List<string> files = new List<string>();
            foreach(string file in Directory.GetFiles(assetsFolderPath))
            {
                if(Path.GetExtension(file) == ext.Trim())
                {
                    files.Add(Path.GetFileName(file));
                }
            }
            return files;
        }

        public static List<string> getAllFilesByExt(string assetsFolderPath, string[] exts)
        {
            List<string> files = new List<string>();
            foreach (string ext in exts)
            {
                foreach (string file in Directory.GetFiles(assetsFolderPath))
                {
                    if (Path.GetExtension(file) == ext.Trim())
                    {
                        files.Add(Path.GetFileName(file));
                    }
                }
            }
            return files;
        }
    }
}
