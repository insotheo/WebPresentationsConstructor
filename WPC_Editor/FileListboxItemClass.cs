﻿using System.IO;
using System.Windows.Media;

namespace WPC_Editor
{
    public class FileListboxItemClass
    {
        public string toolTipText {  get; set; }
        public string extensionOfFile {  get; set; }
        public Brush extensionColor { get; set; }
        public string fileNameText { get; set; }

        public string path;
        public string fileName;

        public FileListboxItemClass(string path)
        {
            this.path = path;
            this.fileName = Path.GetFileName(path);
            this.extensionOfFile = Path.GetExtension(path).Replace(".", null).ToUpper();
            this.toolTipText = $"Путь: {path}" +
                $"\nНазвание: {Path.GetFileName(path)}" +
                $"\nРасширение: {Path.GetExtension(path)}";
            switch(this.extensionOfFile)
            {
                case "JS":
                    extensionColor = Brushes.Yellow;
                    break;

                case "CSS":
                    extensionColor = Brushes.LightPink;
                    extensionOfFile = "#";
                    break;

                case "PNG": case "BMP": case "JPEG": case "JPG":
                    extensionColor = Brushes.LightBlue;
                    break;

                case "MP4": case "MOV":
                    extensionColor = Brushes.Orange;
                    break;

                case "MP3": case "WAV": case "OGG":
                    extensionColor = Brushes.MediumPurple;
                    break;

                case "WPCSAVE":
                    extensionColor = Brushes.Aqua;
                    extensionOfFile = "СТР.";
                    break;

                case "PY":
                    extensionColor = Brushes.Yellow;
                    extensionOfFile = "🐍";
                    break;

                case "JAVA":
                    extensionColor = Brushes.Orange;
                    extensionOfFile = "☕";
                    break;

                default:
                    extensionColor = Brushes.White;
                    break;
            }
            fileNameText = fileName;
        }
    }
}
