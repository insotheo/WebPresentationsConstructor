namespace WPC_BUILD_CLASS
{
    public class BUILD_DATA_CLASS
    {
        public bool removeCommentsFromJS;
        public bool zip;
        public bool makeHtmlSourceTab;
        public bool sort;
        public string pathToSave;
        public string projectName;

        public BUILD_DATA_CLASS(bool removeCommentsFromJS, bool zip, bool makeHtmlSourceTab, bool sort, string pathToSave, string projectName)
        {
            this.removeCommentsFromJS = removeCommentsFromJS;
            this.zip = zip;
            this.makeHtmlSourceTab = makeHtmlSourceTab;
            this.sort = sort;
            this.pathToSave = pathToSave;
            this.projectName = projectName;
        }
    }
}
