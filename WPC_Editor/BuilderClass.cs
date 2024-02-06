using System;
using System.Collections.Generic;
using System.IO;
using WPC_Editor.Widgets;
using WPC_Editor.DataWorkerClasses;
using System.Threading.Tasks;

namespace WPC_Editor
{
    public class BuilderClass
    {
        private string cacheFolder;
        private string assetsFolder;

        private bool sortFiles = false;

        public BuilderClass(string folder, string assetsFolder)
        {
            this.cacheFolder = folder;
            this.assetsFolder = assetsFolder;
        }


        private string getElementsAsTextFromGroup(ref WidgetGroup group)
        {
            string text = String.Empty;
            if (group.kids != null)
            {
                List<Widget> kids = group.kids;
                foreach (Widget kid in kids)
                {
                    text += "\n" + make(kid);
                }
            }
            return text;
        }

        private string getListContent(ref WidgetList list)
        {
            string text = String.Empty;
            if (list.content != null)
            {
                List<Widget> content = list.content;
                foreach (Widget el in content)
                {
                    text += "<li>\n" + make(el) + "\n</li>\n";
                }
            }
            return text;
        }

        private string getMarqueeContent(ref WidgetMarquee marquee)
        {
            string txt = String.Empty;
            if(marquee.elements != null)
            {
                List<Widget> content = marquee.elements;
                foreach(Widget el in content)
                {
                    txt += "\n" + make(el);
                }
            }
            return txt;
        }

        private string getMarginString(string margin)
        {
            margin = margin.Trim();
            string res = String.Empty;
            foreach(string side in margin.Split(' '))
            {
                res += " " + side + "px";
            }
            return res.Trim();
        }

        private string make(Widget widget)
        {
            string line = String.Empty;

            if (widget != null)
            {
                if (widget is WidgetText && !(widget is WidgetLink))
                {
                    var text = widget as WidgetText;
                    string s = String.Empty;
                    if (!widget.useStyle)
                    {
                        s = $"style=\"font-size: {text.fontSize.ToString()}px; font-family: '{text.fontFamily}'; font-weight: {text.fontWeight}; color: {text.fontColorHEX.ToLower()}; background-color: {text.backgroundColorHEX}; border-radius: {text.backgroundRad}%; margin: {getMarginString(text.margin)};\"";
                    }
                    line = $"<{widget.HTML_TAG} id=\"{text.name}\" {s}>{(widget as WidgetText).content}</{widget.HTML_TAG}>";
                }
                else if (widget is WidgetLink)
                {
                    var link = widget as WidgetLink;
                    string s = String.Empty;
                    if (!widget.useStyle)
                    {
                        s = $"style=\"font-size: {link.fontSize.ToString()}px; font-family: '{link.fontFamily}'; font-weight: {link.fontWeight}; color: {link.fontColorHEX.ToLower()}; background-color: {link.backgroundColorHEX}; border-radius: {link.backgroundRad}%; margin: {getMarginString(link.margin)};\"";
                    }
                    line = $"<{link.HTML_TAG} id=\"{link.name}\" href=\"{link.href}\" {s}>{link.content}</{link.HTML_TAG}>";
                }
                else if (widget is WidgetButton)
                {
                    var button = widget as WidgetButton;
                    string s = String.Empty;
                    string oc = String.Empty;
                    if (!button.useStyle)
                    {
                        s = $"style=\"font-size: {button.fontSize.ToString()}px; font-family: {button.fontFamily}; font-weight: {button.fontWeight}; color: {button.fontColorHEX}; background-color: {button.backgroundColorHEX}; border-color: {button.borderColorHEX}; border-radius: {button.borderRadius}%; cursor: {button.cursor}; margin: {getMarginString(button.margin)};\"";
                    }
                    if (button.onclick != null)
                    {
                        if (button.onclick != String.Empty)
                        {
                            oc = $"onclick=\"{button.onclick.Trim()}({button.arguments})\"";
                        }
                    }
                    line = $"<{button.HTML_TAG} id=\"{button.name}\" {oc} {s}>{button.content}</{button.HTML_TAG}>";
                }
                else if (widget is WidgetNextLine)
                {
                    var nextLine = widget as WidgetNextLine;
                    for (int i = 0; i < nextLine.repeatTime; i++)
                    {
                        if (i + 1 == nextLine.repeatTime)
                        {
                            line += $"<{(widget as WidgetNextLine).HTML_TAG}>";
                        }
                        else
                        {
                            line += $"<{(widget as WidgetNextLine).HTML_TAG}>\n";
                        }
                    }

                }
                else if (widget is WidgetImage)
                {
                    var img = widget as WidgetImage;
                    string s = String.Empty;
                    if (!img.useStyle)
                    {
                        s = $"style=\"height: {img.height}%; width: {img.width}%; rotate: {img.rotationAngle}deg; border-radius: {img.radius}%; margin: {getMarginString(img.margin)}; filter: blur({img.blur}px);\"";
                    }
                    if (img.contentType == 'f')
                    {
                        if (sortFiles)
                        {
                            img.href = @"Resources\" + img.href;
                        }
                        if (File.Exists(Path.Combine(assetsFolder, img.href)))
                        {
                            File.Copy(Path.Combine(assetsFolder, img.href), Path.Combine(cacheFolder, img.href), true);
                        }
                    }
                    line = $"<{img.HTML_TAG} id=\"{img.name}\" src=\"{img.href}\" {s}>";
                }
                else if (widget is WidgetVideo)
                {
                    var vid = widget as WidgetVideo;
                    string s = String.Empty;
                    string ap = String.Empty;
                    string l = String.Empty;
                    string c = String.Empty;
                    if (!vid.useStyle)
                    {
                        s = $"style=\"height: {vid.height}%; width: {vid.width}%; margin: {getMarginString(vid.margin)};\"";
                    }
                    if (vid.showControls)
                    {
                        c = "controls=\"controls\"";
                    }
                    if (vid.isLoop)
                    {
                        l = "loop=\"loop\"";
                    }
                    if (vid.src != String.Empty)
                    {
                        if (sortFiles)
                        {
                            vid.src = @"Resources\" + vid.src;
                        }
                        if (File.Exists(Path.Combine(assetsFolder, vid.src)))
                        {
                            File.Copy(Path.Combine(assetsFolder, vid.src), Path.Combine(cacheFolder, vid.src), true);
                        }
                    }
                    line = $"<{vid.HTML_TAG} id=\"{vid.name}\" src=\"{vid.src}\" {c} {ap} {l} {s}></{vid.HTML_TAG}>";
                }
                else if (widget is WidgetInput)
                {
                    var inp = widget as WidgetInput;
                    string s = String.Empty;
                    string ro = String.Empty;
                    string Type = WidgetInput.types[Array.IndexOf(WidgetInput.rus_types, inp.type)];
                    if (!inp.useStyle)
                    {
                        s = $"style=\"font-size: {inp.fontSize.ToString()}px; font-family: {inp.fontFamily}; font-weight: {inp.fontWeight}; color: {inp.fontColorHEX.ToLower()}; background-color: {inp.backgroundColorHEX}; border-radius: {inp.backgroundRad}%; margin: {getMarginString(inp.margin)};\"";
                    }
                    if (inp.isReadonly)
                    {
                        ro = "readonly=\"readonly\"";
                    }
                    line = $"<{inp.HTML_TAG} id=\"{inp.name}\" placeholder=\"{inp.placeholder}\" value=\"{inp.content}\" type=\"{Type}\" {ro} {s}>";
                }
                else if (widget is WidgetGroup)
                {
                    var grp = widget as WidgetGroup;
                    string s = String.Empty;
                    string els = getElementsAsTextFromGroup(ref grp);
                    if (!grp.useStyle)
                    {
                        string flex = String.Empty;
                        if (grp.isFlex)
                        {
                            flex = $"display: flex; justify-content: {WidgetGroup.justifying[Array.IndexOf(WidgetGroup.justifying_rus, grp.justifyContent)]}; flex-direction: {WidgetGroup.flexDir[Array.IndexOf(WidgetGroup.flexDir_rus, grp.flexDirection)]};";
                        }
                        s = $"style=\"{flex} background-color: {grp.backgroundColorHEX}; border-radius: {grp.radius}%; margin: {getMarginString(grp.margin)}; position: {WidgetGroup.position[Array.IndexOf(WidgetGroup.position_rus, grp.pos)]}; {WidgetGroup.positionVector[Array.IndexOf(WidgetGroup.positionVector_rus, grp.posVector)]}: {grp.posVectorPx}px;\"";
                    }
                    line = $"<{grp.HTML_TAG} class=\"{grp.name}\" {s}>" +
                        $"{els}" +
                        $"\n</{grp.HTML_TAG}>";
                }
                else if(widget is WidgetList)
                {
                    var list = widget as WidgetList;
                    string s = String.Empty;
                    string content = getListContent(ref list);
                    if (!list.useStyle)
                    {
                        s = $"style=\"font-size: {list.fontSize.ToString()}px; font-family: {list.fontFamily}; font-weight: {list.fontWeight}; color: {list.fontColor.ToLower()}; background-color: {list.backgroundColor}; border-radius: {list.borderRadius}%; margin: {getMarginString(list.margin)};\"";
                    }
                    line = $"<{list.getHTML_TAG()} id=\"{list.name}\" {s}>\n" +
                        $"{content}" +
                        $"</{list.getHTML_TAG()}>";
                }
                else if(widget is WidgetHtmlSource)
                {
                    var html = widget as WidgetHtmlSource;
                    if(html.type == WidgetHtmlSource.ContentTypeOfHtmlSource.text)
                    {
                        line = html.content;
                    }
                    else
                    {
                        if(html.content != null)
                        {
                            if (File.Exists(Path.Combine(assetsFolder, html.content)))
                            {
                                line = File.ReadAllText(Path.Combine(assetsFolder, html.content));
                            }
                        }
                    }
                }
                else if(widget is WidgetMarquee)
                {
                    var marq = widget as WidgetMarquee;
                    string s = String.Empty;
                    string content = getMarqueeContent(ref marq);
                    if (!marq.useStyle)
                    {
                        s = $"behavior=\"{WidgetMarquee.behaviorOptions[Array.IndexOf(WidgetMarquee.behaviorOptions_rus, marq.behavior)]}\" bgcolor=\"{marq.backgroundColor}\" direction=\"{WidgetMarquee.direction[Array.IndexOf(WidgetMarquee.direction_rus, marq.dir)]}\" loop=\"{marq.loop}\" scrollamount=\"{marq.scrollAmount}\" style=\"margin: {getMarginString(marq.margin)};\"";
                    }
                    line = $"<{marq.HTML_TAG} id=\"{marq.name}\" {s}>" +
                        $"{content}" +
                        $"\n</{marq.HTML_TAG}>";
                }
                else if(widget is WidgetSquareShape)
                {
                    var shape = widget as WidgetSquareShape;
                    string s = String.Empty;
                    if (!shape.useStyle)
                    {
                        s = $"style=\"height: {shape.height}px; width: {shape.width}px; background-color: {shape.color}; border-radius: {shape.radius}%; transform: skew({shape.skew}deg); margin: {getMarginString(shape.margin)};\"";
                    }
                    line = $"<{shape.HTML_TAG} id=\"{shape.name}\" class=\"{shape.classHTML}\" {s}></{shape.HTML_TAG}>";
                }
            }
            return line;
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

        public void fastBuild(List<WidgetsTreeItem> list, ref ConfigWorker config, WidgetBody body, bool allowMobile = true, string fileTitle = "INDEX.HTML", bool recreateDir = true, bool setIcon = false)
        {
            if (recreateDir)
            {
                Directory.Delete(cacheFolder, true);
                Directory.CreateDirectory(cacheFolder);
            }
            FileStream indexHTML = File.Create(Path.Combine(cacheFolder, fileTitle));
            indexHTML.Close();

            string blockMobiles = !allowMobile ? "<style>\r\n        .modal {\r\n            display: none;\r\n            position: fixed;\r\n            top: 50%;\r\n            left: 50%;\r\n            transform: translate(-50%, -50%);\r\n            padding: 20px;\r\n            background-color: white;\r\n            border: 2px solid red;\r\n            z-index: 9999;\r\n        }\r\n    </style>\r\n    <script>\r\n        window.addEventListener('DOMContentLoaded', function() {\r\n            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {\r\n                var modal = document.createElement('div');\r\n                modal.className = 'modal';\r\n                modal.innerText = 'Данную страницу нельзя просмотреть на мобильном устройстве';\r\n                document.body.appendChild(modal);\r\n                modal.style.display = 'block';\r\n            }\r\n        });\r\n    </script>" : String.Empty;
            string webIcon = String.Empty;
            if (setIcon)
            {
                string subString = sortFiles ? @"Resources\" : String.Empty;
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "build_icon.svg"), Path.Combine(cacheFolder, "build_icon.svg"), true);
                webIcon = $"<link rel=\"icon\" type=\"image/svg\" href=\"{subString}build_icon.svg\">";
            }
            string finalText = $"<!DOCTYPE html>\n<!-- Made with WebPresentationsConstructor {DateTime.Now.Year} -->\n<!-- https://github.com/insotheo/WebPresentationsConstructor -->" +
                $"\n<html lang=\"{config.language}\">" +
                "\n<head>" +
                $"\n<title>{config.title}</title>" +
                $"\n<meta charset=\"{config.charset.ToLower()}\">" +
                $"\n{webIcon}" +
                $"\n{blockMobiles}";

            if(body.type == WidgetBody.CommonType.photo && body.photoType == WidgetBody.PhotoType.image)
            {
                if (File.Exists(Path.Combine(assetsFolder, body.imageHref)))
                {
                    File.Copy(Path.Combine(assetsFolder, body.imageHref), Path.Combine(cacheFolder, body.imageHref), true);
                }
            }

            string bodyString = String.Empty;

            if (body.useStyle)
            {
                bodyString = "<body>";
            }
            else
            {
                string s = String.Empty;

                if(body.type == WidgetBody.CommonType.color)
                {
                    s = $"style=\"background-color: {body.color}\"";
                }
                else
                {
                    s = $"style=\"background-image: url('{body.imageHref}'); backdrop-filter: blur({body.blurRadius}px) invert({body.invertRadius}); background-size: {WidgetBody.imageSize_eng[Array.IndexOf(WidgetBody.imageSize_rus, body.imageSize)]}; background-repeat: {WidgetBody.imageRepeat_eng[Array.IndexOf(WidgetBody.imageRepeat_rus, body.imageRepeat)]};\"";
                }

                bodyString = $"<body {s}>";
            }

            if (config.usingStyles.Count > 0)
            {
                foreach (var style in config.usingStyles)
                {
                    if (File.Exists(Path.Combine(assetsFolder, style)))
                    {
                        File.Copy(Path.Combine(assetsFolder, style), Path.Combine(cacheFolder, style), true);
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", style)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "styles", style), Path.Combine(cacheFolder, style), true);
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    string sortString = sortFiles ? @"Styles\" : String.Empty;
                    finalText += $"\n<link rel=\"stylesheet\" href=\"{sortString}{style}\">";
                }
            }
            if (config.usingScripts.Count > 0)
            {
                foreach (var script in config.usingScripts)
                {
                    if (File.Exists(Path.Combine(assetsFolder, script)))
                    {
                        File.Copy(Path.Combine(assetsFolder, script), Path.Combine(cacheFolder, script), true);
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", script)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "scripts", script), Path.Combine(cacheFolder, script), true);
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    string sortString = sortFiles ? @"Scripts\" : String.Empty;
                    finalText += $"\n<script src=\"{sortString}{script}\"></script>";
                }
            }

            finalText += $"\n</head>\n{bodyString}";
            foreach (WidgetsTreeItem el in list)
            {
                string line = "";

                var widget = el.widget;

                line = make(widget);

                finalText += "\n" + line;
            }
            finalText += "\n</body>\n</html>\n";

            if(config.additionalFilesForBuilding.Count > 0)
            {
                foreach (var file in config.additionalFilesForBuilding)
                {
                    if (File.Exists(Path.Combine(assetsFolder, file)))
                    {
                        File.Copy(Path.Combine(assetsFolder, file), Path.Combine(cacheFolder, file), true);
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", file)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "styles", file), Path.Combine(cacheFolder, file), true);
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                }
            }

            File.WriteAllText(Path.Combine(cacheFolder, fileTitle), finalText);
        }

        public void build(bool sortFiles, ConfigWorker config, bool allowMobile)
        {
            cacheFolder = cacheFolder.Replace("cache", ".build");
            this.sortFiles = sortFiles;
            List<List<WidgetsTreeItem>> pagesData = new List<List<WidgetsTreeItem>>();
            List<string> names = new List<string>();
            foreach(string page in FilesWorker.getAllFilesByExt(assetsFolder, ".wpcsave"))
            {
                pagesData.Add(DataWorker.staticLoad(assetsFolder, page));
                names.Add(page.Replace(".wpcsave", ".html"));
            }
            for(int i = 0; i < pagesData.Count; i++)
            {
                fastBuild(pagesData[i][0].widgetsOfScene,
                    ref config,
                    pagesData[i][0].widget as WidgetBody,
                    allowMobile,
                    names[i],
                    false,
                    true);
                Task.Delay(10).Wait();
            }
            cacheFolder = cacheFolder.Replace(".build", "cache");
            this.sortFiles = false;
        }
    }
}
