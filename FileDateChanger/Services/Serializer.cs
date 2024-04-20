using System;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace FileDateChanger.Services
{
    [Serializable]
    public class Properties
    {
        public Point Location;
        public bool DarkTheme;
    }

    public class Serializer
    {
        private readonly string name = "Config.xml";
        private readonly string fullFileName = null;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(Properties));
        public Properties Properties { get; private set; } = new Properties();

        public Serializer() : this(null) { }

        public Serializer(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fullFileName = Path.GetFullPath(name);
            }
            else
            {
                fullFileName = Path.GetFullPath(fileName);
            }
        }

        public void Open()
        {
            if (File.Exists(fullFileName))
            {
                using (FileStream stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
                {
                    Properties = serializer.Deserialize(stream) as Properties;
                }
            }
        }

        public void Save()
        {
            using (FileStream stream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, Properties);
            }
        }
    }
}
