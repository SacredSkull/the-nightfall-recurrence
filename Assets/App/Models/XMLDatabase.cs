using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Karma.Metadata;
using Level;
using Level.Entity;

namespace Models {
    [Service]
	public class XMLDatabase : IDatabase {
        private const string ROOT = @"./Assets/Resources";
		protected List<HackTool> hackTools;
		protected List<Sentry> sentries;
		protected List<MapItem> mapItems;
        protected Map map;

        protected virtual T deserialiseXML<T>(string path = @"/Entities/Tools.xml", string rootElement = "software") {
            XmlSerializer toolSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));
            TextReader toolReader = new StreamReader(ROOT + path);
            T software = (T)toolSerializer.Deserialize(toolReader);

            //TODO: Handle/throw necessary exceptions

            toolReader.Close();
            return software;
        }

        protected virtual void serialiseXML<T>(T elements, string path = "/.", string rootElement = "software") {
            XmlSerializer toolSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));
            using (TextWriter writer = new StreamWriter(ROOT + path + "/" + typeof(T).Name + @".serialised.xml")) {
                toolSerializer.Serialize(writer, elements);
            }
            //TODO: Handle/throw necessary exceptions
        }

        public virtual List<HackTool> loadHackTools() {
            return hackTools = deserialiseXML<List<HackTool>>();
        }

        public virtual void dumpHackTools() {
            serialiseXML(hackTools);
        }

        public virtual List<Sentry> loadSentries() {
            return sentries = deserialiseXML<List<Sentry>>(@"/Entities/Sentries.xml");
        }

        public virtual void dumpSentries() {
            serialiseXML(sentries);
        }

        public virtual List<MapItem> loadMapItems() {
            return mapItems = deserialiseXML<FeaturesXMLRoot>(@"/Entities/Features.xml", "features").features;
        }

        public virtual void dumpMapItems() {
            serialiseXML(new FeaturesXMLRoot {features = this.mapItems}, "/." , "features");
        }

        public virtual Map loadLevel(string path) {
            return map = deserialiseXML<Map>(path, "map");
        }

        public virtual void dumpMap() {
            serialiseXML(map);
        }
    }
}
