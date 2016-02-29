using Scripts.Action.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "tool")]
public class SoftwareTool : Attackable {
    [XmlAttribute("maxsize")]
    public short maxsize;
    [XmlAttribute("cost")]
    public short cost;
    [XmlAttribute("level")]
    public short level;
    [XmlAttribute("governor")]
    public string governor_string;
    [XmlIgnore]
    public bool isEnemy { get; set; }
    [XmlIgnore]
    public Governor governor;

    [XmlArray("attacks")]
    [XmlArrayItem(ElementName = "attackbasic", Type = typeof(AttackBasic)),
        XmlArrayItem(ElementName = "attributemodifier", Type = typeof(AttributeModifier)),
        XmlArrayItem(ElementName = "mapmodifier", Type = typeof(MapModifier))]
    public List<Attack> attacks { get; set; }

    public SoftwareTool(){
        var assembly = Assembly.GetExecutingAssembly();
        var type = assembly.GetTypes().FirstOrDefault(t => t.Name == governor_string);
        if (type != null)
            governor = Activator.CreateInstance(type) as Governor;
        else
            governor = new Governor();

        this.gridPosition = new Vector2();
    }
}