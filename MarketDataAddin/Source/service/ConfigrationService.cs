using System.Xml;

namespace MarketDataAddin
{
    class ConfigrationService
    {
        private Macro macro = Macro.instance;
        private Logger logger = Logger.instance;
        public void readXML()
        {
            string file = "d:\\Addin\\config.xml";
            XmlDocument dom = new XmlDocument();
            dom.Load(file);
            logger.WriteInfo(file);
            foreach (XmlElement xmlElement in dom.DocumentElement.ChildNodes)
            {
                if (xmlElement.Name == "interval") macro.interval = int.Parse(xmlElement.InnerText);
                if (xmlElement.Name == "hostname") macro.hostname = xmlElement.InnerText;
                if (xmlElement.Name == "hostport") macro.hostport = int.Parse(xmlElement.InnerText);
                if (xmlElement.Name == "username") macro.username = xmlElement.InnerText;
                if (xmlElement.Name == "password") macro.password = xmlElement.InnerText;
                if (xmlElement.Name == "debug") macro.debug = xmlElement.InnerText;
            }
        }
    }
}
