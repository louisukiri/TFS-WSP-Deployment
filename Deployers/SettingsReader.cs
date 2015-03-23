using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using Deployers.Abstract;
using toDev.Concrete;

namespace Deployers
{
    [Serializable, XmlRoot("Build")]
    public class BuildConfiguration
    {
        public static string ConfigFileName = "\\build.config";
        private DirectoryCollection<DirectoryEntry> _rules;
        public DirectoryCollection<DirectoryEntry> Rules
        {
            get
            {
                return _rules;
            }
            set
            {
                _rules = value;
            }
        }
        public string ConnectionString { get; set; }
        /// <summary>
        /// REturn build configuration config file
        /// </summary>
        /// <param name="basePath">The absolute path of the config file</param>
        /// <returns></returns>
        public static BuildConfiguration GetConfig(string basePath)
        {
            var config = new BuildConfiguration();
            config.Rules = new DirectoryCollection<DirectoryEntry>();
            Stream fileReader = null;
            string filePath = "";
            try
            {
                config = null;
                if ((config == null))
                {
                    filePath = basePath + (!basePath.EndsWith(".config")?ConfigFileName:string.Empty);
                    //filePath = "stamatsRewrite.config";
                    //Create a FileStream for the Config file
                    if (basePath.StartsWith("$"))
                    {

                        fileReader = DefaultFileObject.get(filePath);
                    }
                    else
                    {
                        fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }

                    var doc = new XPathDocument(fileReader);
                    config = new BuildConfiguration { Rules = new DirectoryCollection<DirectoryEntry>() };
                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("build/*"))
                    {
                        switch (nav.Name.ToLower())
                        { 
                            case "directory":
                                var rule = new DirectoryEntry();
                                //{ LookFor = nav.SelectSingleNode("LookFor").Value, SendTo = nav.SelectSingleNode("SendTo").Value, Handler=nav.SelectSingleNode("Handler").Value };
                        
                                if (nav.GetAttribute("name", "") != null)
                                    rule.Name = nav.GetAttribute("name","");
                                if (nav.GetAttribute("value", "") != null)
                                    rule.Value = nav.GetAttribute("value", "");
                                if (nav.GetAttribute("description", "") != null)
                                    rule.Description = nav.GetAttribute("description", "");

                                config.Rules.Add(rule);
                            break;
                            case "connectionstring":
                                config.ConnectionString = nav.InnerXml.Trim();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                if (fileReader != null)
                {
                    //Close the Reader
                    fileReader.Close();
                }
            }
            return config;
        }

        public static void SaveConfig(string basePath, DirectoryCollection<DirectoryEntry> rules)
        {
            if (rules != null)
            {
                var config = new BuildConfiguration { Rules = rules };

                //Create a new Xml Serializer
                var ser = new XmlSerializer(typeof(BuildConfiguration));

                //Create a FileStream for the Config file
                string filePath = basePath +ConfigFileName;
                if (File.Exists(filePath))
                {
                    //make sure file is not read-only
                    File.SetAttributes(filePath, FileAttributes.Normal);
                }
                var fileWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);

                //Open up the file to serialize
                var writer = new StreamWriter(fileWriter);

                //Serialize the stamatsRewriterConfiguration
                ser.Serialize(writer, config);

                //Close the Writers
                writer.Close();
                fileWriter.Close();

            }
        }
    }

    [Serializable]
    public class DirectoryCollection<EntryType> : CollectionBase
        where EntryType:IEntry
    {
        private IQueryable<EntryType> q
        {
            get
            {
                return InnerList.Cast<EntryType>().AsQueryable() as IOrderedQueryable<EntryType>;
            }
        }
        public virtual EntryType this[int index]
        {
            get
            {
                return (EntryType)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }
        public virtual EntryType this[string name]
        {
            get
            {
                return q.Where
                    (a => a.Name == name).FirstOrDefault();
            }
        }
        public void Add(EntryType a)
        {            
            InnerList.Add(a);
        }
    }
    [Serializable]
    public class DirectoryEntry : IEntry
    {
        private string _name;
        private string _value;
        private string _desc;


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }
    }
}
