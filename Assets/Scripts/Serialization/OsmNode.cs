using System.Xml;
using UnityEngine;
using System.Collections.Generic;

/// Извлекает данные узла из XML-файла OSM и сохраняет его значения.
/// Эти данные включают информацию о местоположении объекта, дорогах, железных дорогах,
/// станциях и т.д.
class OsmNode : BaseOsm
{
    public ulong ID { get; private set; }

    public float Latitude { get; private set; }

    public float Longitude { get; private set; }

    public float X { get; private set; }

    public float Y { get; private set; }

    public string StationName { get; private set; }

    public bool StationCreated { get; set; } 

    public List<string> TransportLines { get; set; }

    public static implicit operator Vector3(OsmNode node)
    {
        return new Vector3(node.X, 0, node.Y);
    }

    /// <param name="node">XML node</param>
    public OsmNode(XmlNode node)
    {
        TransportLines = new List<string>();

        ID = GetAttribute<ulong>("id", node.Attributes);

        Latitude = GetFloat("lat", node.Attributes);
        Longitude = GetFloat("lon", node.Attributes);

        X = (float)MercatorProjection.lonToX(Longitude);
        Y = (float)MercatorProjection.latToY(Latitude);

        findName(node);
    }

    /// <param name="node">XML node</param>
    public void findName(XmlNode node)
    {
        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            string key = GetAttribute<string>("k", t.Attributes);
            if (key == "name")
            {
                StationName = GetAttribute<string>("v", t.Attributes);
            }
        }
    }
}
