using System.Collections.Generic;
using System.Xml;
using UnityEngine;


/// <summary>
/// Этот класс использует способ получения данных из XML-файлов и хранит свою информацию.
/// Он содержит информацию о дорогах и железных дорогах.
/// </summary>
class OsmWay : BaseOsm
{
    public ulong ID { get; private set; }

    public List<ulong> NodeIDs { get; private set; }

    public bool IsRailway { get; private set; }

    public bool PublicTransportRailway { get; set; }

    public bool IsStreet { get; private set; }

    public bool PublicTransportStreet { get; set; }

    public List<string> TransportTypes { get; set; }

    public List<string> TransportLines { get; set; }

    public List<Vector3> UnityCoordinates { get; set; }

    /// <param name="node">XML node</param>
    public OsmWay(XmlNode node)
    {
        NodeIDs = new List<ulong>();
        TransportTypes = new List<string>();
        TransportLines = new List<string>();
        UnityCoordinates = new List<Vector3>();

        ID = GetAttribute<ulong>("id", node.Attributes);

        Tagger(node);

        if ((UserPreferences.PublicTransportRailways && IsRailway == true) || (UserPreferences.PublicTransportStreets && IsStreet == true))
        {
            NodeIDsCreator(node);
        }
    }


    /// <param name="node">XML node</param>
    void Tagger(XmlNode node)
    {
        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            string key = GetAttribute<string>("k", t.Attributes);
            if (key == "railway")
            {
                IsRailway = true;
            }
            else if (key == "highway")
            {
                string value = GetAttribute<string>("v", t.Attributes);
                List<string> AllowedTags = new List<string> { "motorway", "trunk", "primary", "secondary", "tertiary", "unclassified", "residential", "motorway_link", "trunk_link", "primary_link", "secondary_link", "tertiary_link", "road", "living_street", "service" };
                if (AllowedTags.Contains(value))
                {
                    IsStreet = true;
                }
            }
        }
    }

    /// <param name="node">XML node</param>
    void NodeIDsCreator(XmlNode node)
    {
        XmlNodeList nds = node.SelectNodes("nd");
        foreach (XmlNode n in nds)
        {
            ulong refNo = GetAttribute<ulong>("ref", n.Attributes);
            NodeIDs.Add(refNo);
        }
    }
}

