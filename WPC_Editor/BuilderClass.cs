using System;
using System.Collections.Generic;
using System.IO;
using WPC_Editor.Widgets;
using static System.Net.Mime.MediaTypeNames;

namespace WPC_Editor
{
    class BuilderClass
    {
        private string cacheFolder;
        private string assetsFolder;

        public BuilderClass(string folder, string assetsFolder)
        {
            this.cacheFolder = folder;
            this.assetsFolder = assetsFolder;
        }

        public void Init(ref ConfigWorker config)
        {
            foreach (string file in config.usingStyles)
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", file.Trim())) &&
                    !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                {
                    throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                }
            }
            foreach (string file in config.usingScripts)
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", file.Trim())) &&
                    !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                {
                    throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                }
            }
            if (!File.Exists(Path.Combine(cacheFolder, "INDEX.html")))
            {
                FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                indexHTML.Close();
            }
            else
            {
                Directory.Delete(cacheFolder, true);
                Directory.CreateDirectory(cacheFolder);
                FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                indexHTML.Close();
            }
        }

        public void fastBuild(List<WidgetsTreeItem> list, ref ConfigWorker config)
        {
            Directory.Delete(cacheFolder, true);
            Directory.CreateDirectory(cacheFolder);
            FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
            indexHTML.Close();

            string finalText = "<!DOCTYPE html>" +
                $"\n<html lang=\"{config.language}\">" +
                "\n<head>" +
                $"\n<title>{config.title}</title>" +
                $"\n<meta charset=\"{config.charset.ToLower()}\">";

            if (config.usingStyles.Count > 0)
            {
                foreach (var style in config.usingStyles)
                {
                    if (File.Exists(Path.Combine(assetsFolder, style)))
                    {
                        File.Copy(Path.Combine(assetsFolder, style), Path.Combine(cacheFolder, style));
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", style)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "styles", style), Path.Combine(cacheFolder, style));
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    finalText += $"\n<link rel=\"stylesheet\" href=\"{style}\">";
                }
            }
            if (config.usingScripts.Count > 0)
            {
                foreach (var script in config.usingScripts)
                {
                    if (File.Exists(Path.Combine(assetsFolder, script)))
                    {
                        File.Copy(Path.Combine(assetsFolder, script), Path.Combine(cacheFolder, script));
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", script)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "scripts", script), Path.Combine(cacheFolder, script));
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    finalText += $"\n<script src=\"{script}\"></script>";
                }
            }

            finalText += "\n</head>\n<body>";
            foreach (WidgetsTreeItem el in list)
            {
                string line = "";

                var widget = el.widget;
                if (widget is WidgetText && !(widget is WidgetLink))
                {
                    var text = widget as WidgetText;
                    string s = String.Empty;
                    if (widget.useStyle == false)
                    {
                        s = $"style=\"font-size: {text.fontSize.ToString()}px; font-family: {text.fontFamily}; font-weight: {text.fontWeight}; color: {text.fontColorHEX.ToLower()}; background-color: {text.backgroundColorHEX}; border-radius: {text.backgroundRad}%;\"";
                    }
                    line = $"<{widget.HTML_TAG} id=\"{text.name}\" {s}>{(widget as WidgetText).content}</{widget.HTML_TAG}>";
                }
                else if(widget is WidgetLink)
                {
                    var link = widget as WidgetLink;
                    string s = String.Empty;
                    if(widget.useStyle == false)
                    {
                        s = $"style=\"font-size: {link.fontSize.ToString()}px; font-family: {link.fontFamily}; font-weight: {link.fontWeight}; color: {link.fontColorHEX.ToLower()}; background-color: {link.backgroundColorHEX}; border-radius: {link.backgroundRad}%;\"";
                    }
                    line = $"<{link.HTML_TAG} id=\"{link.name}\" href=\"{link.href}\" {s}>{link.content}</{link.HTML_TAG}>";
                }
                else if(widget is WidgetButton)
                {
                    var button = widget as WidgetButton;
                    string s = String.Empty;
                    if(button.useStyle == false)
                    {
                        s = $"style=\"font-size: {button.fontSize.ToString()}px; font-family: {button.fontFamily}; font-weight: {button.fontWeight}; color: {button.fontColorHEX}; background-color: {button.backgroundColorHEX}; border-color: {button.borderColorHEX}; border-radius: {button.borderRadius}%; cursor: {button.cursor};\"";
                    }
                    line = $"<{button.HTML_TAG} id=\"{button.name}\" onclick=\"{button.onclick}({button.arguments})\" {s}>{button.content}</{button.HTML_TAG}>";
                }
                else if(widget is WidgetNextLine)
                {
                    line = $"<{(widget as WidgetNextLine).HTML_TAG}>";
                }
                else if(widget is WidgetImage)
                {
                    var img = widget as WidgetImage;
                    string s = String.Empty;
                    if(img.useStyle == false)
                    {
                        s = $"style=\"height: {img.height}%; width: {img.width}%; rotate: {img.rotationAngle}deg; border-radius: {img.radius}%;\"";
                    }
                    if(img.contentType == 'f')
                    {
                        if(File.Exists(Path.Combine(assetsFolder, img.href)))
                        {
                            File.Copy(Path.Combine(assetsFolder, img.href), Path.Combine(cacheFolder, img.href));
                        }
                    }
                    line = $"<{img.HTML_TAG} id=\"{img.name}\" src=\"{img.href}\" {s}>";
                }
                else if(widget is WidgetVideo)
                {
                    var vid = widget as WidgetVideo;
                    string s = String.Empty;
                    string ap = String.Empty;
                    string l = String.Empty;
                    string c = String.Empty;
                    if(vid.useStyle == false)
                    {
                        s = $"style=\"height: {vid.height}%; width: {vid.width}%;\"";
                    }
                    if (vid.showControls)
                    {
                        c = "controls=\"controls\"";
                    }
                    if (vid.isLoop)
                    {
                        l = "loop=\"loop\"";
                    }
                    if(vid.src != String.Empty)
                    {
                        if(File.Exists(Path.Combine(assetsFolder, vid.src)))
                        {
                            File.Copy(Path.Combine(assetsFolder, vid.src), Path.Combine(cacheFolder, vid.src));
                        }
                    }
                    line = $"<{vid.HTML_TAG} id=\"{vid.name}\" src=\"{vid.src}\" {c} {ap} {l} {s}></{vid.HTML_TAG}>";
                }
                else if(widget is WidgetInput)
                {
                    var inp = widget as WidgetInput;
                    string s = String.Empty;
                    string ro = String.Empty;
                    string Type = WidgetInput.types[Array.IndexOf(WidgetInput.rus_types, inp.type)];
                    if(inp.useStyle == false)
                    {
                        s = $"style=\"font-size: {inp.fontSize.ToString()}px; font-family: {inp.fontFamily}; font-weight: {inp.fontWeight}; color: {inp.fontColorHEX.ToLower()}; background-color: {inp.backgroundColorHEX}; border-radius: {inp.backgroundRad}%;\"";
                    }
                    if (inp.isReadonly)
                    {
                        ro = "readonly=\"readonly\"";
                    }
                    line = $"<{inp.HTML_TAG} id=\"{inp.name}\" placeholder=\"{inp.placeholder}\" value=\"{inp.content}\" type=\"{Type}\" {ro} {s}>";
                }
                finalText += "\n" + line;
            }
            finalText += "\n</body>\n</html>\n";

            File.WriteAllText(Path.Combine(cacheFolder, "INDEX.html"), finalText);
        }
    }
}
