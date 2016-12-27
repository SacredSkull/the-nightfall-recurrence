using System.Collections.Generic;
using System.Xml.Serialization;

namespace Level {
    [XmlRoot("map")]
    public class Map {
	    [XmlAttribute("width", typeof(int))]
	    public int width;
	    [XmlAttribute("height", typeof(int))]
	    public int height;
	    [XmlElement("tileset")]
        public List<TileSet> tilesets = new List<TileSet>();
        [XmlElement("layer")]
        public List<Layer> layers = new List<Layer>();
    }

// START TileSet Classes //

    public class TileSet {
        [XmlAttribute("firstgid")]
        public int firstgid;
        [XmlAttribute("name")]
        public string name;

        [XmlElement("tile")]
        public List<Tile> tiles = new List<Tile>();
    }

    public class Tile {
        [XmlAttribute("id")]
        public int id;

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public TileSetProperty[] properties;
    }

    public class TileSetProperty {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("value")]
        public string value;
    }

// END TileSet Classes //



// START Layer Classes //

    public class Layer{
        [XmlAttribute("width")]
        public int width;
        [XmlAttribute("height")]
        public int height;
        [XmlAttribute("name")]
        public string name;

        [XmlArray("data")]
        [XmlArrayItem("tile")]
        public LayerTile[] tiles;
    }

    public class LayerTile {
        [XmlAttribute("gid")]
        public int gid;
    }

// END Layer Classes //
}