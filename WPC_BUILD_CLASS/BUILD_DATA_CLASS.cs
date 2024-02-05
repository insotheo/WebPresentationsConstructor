namespace WPC_BUILD_CLASS
{
    public class BUILD_DATA_CLASS
    {
        public bool allowMobileDevices;
        public bool removeCommentsFromJS;
        public bool zip;
        public bool makeHtmlSourceTab;
        public bool sort;
        public string pathToSave;

        public BUILD_DATA_CLASS(bool allowMobileDevices, bool removeCommentsFromJS, bool zip, bool makeHtmlSourceTab, bool sort, string pathToSave)
        {
            this.allowMobileDevices = allowMobileDevices;
            this.removeCommentsFromJS = removeCommentsFromJS;
            this.zip = zip;
            this.makeHtmlSourceTab = makeHtmlSourceTab;
            this.sort = sort;
            this.pathToSave = pathToSave;
        }
    }
}
