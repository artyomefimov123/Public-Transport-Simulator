using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;


/// Этот скрипт извлекает различные узлы данных из XML-файла и генерирует
/// соответствующие экземпляры на основе классов из папки сериализации.
/// Затем объекты хранятся в словарях и списках для доступа к этим данным
/// структуры из других скриптов. Как только данные были обработаны, переменная
///"isReady" становится истинной и запускается скрипт "MapBuilder".
class MapReader : MonoBehaviour
{
    public static OsmBounds bounds;  

    public static Dictionary<ulong, OsmNode> nodes;  

    public static Dictionary<ulong, OsmWay> ways;  

    public static List<OsmRelation> relations; 

    public static bool IsReady { get; private set; }  

    void Start()
    {
        nodes = new Dictionary<ulong, OsmNode>();
        ways = new Dictionary<ulong, OsmWay>();
        relations = new List<OsmRelation>();

        FileLoader.simulator_loaded = true;  

        XmlDocument doc = new XmlDocument();
        try
        {
            // Загружается XML-файл, и узлы данных извлекаются с помощью
// классы из папки сериализации.
            doc.Load(FileLoader.ResourceFilePath);
            SetBounds(doc.SelectSingleNode("/osm/bounds"));
            if(UserPreferences.PublicTransportStreets || UserPreferences.PublicTransportRailways || UserPreferences.Stations)
            {
                GetNodes(doc.SelectNodes("/osm/node"));
            }
            if(UserPreferences.PublicTransportStreets || UserPreferences.PublicTransportRailways)
            {
                GetWays(doc.SelectNodes("/osm/way"));
            }
            if(UserPreferences.PublicTransportStreets || UserPreferences.PublicTransportRailways || UserPreferences.Stations)
            {
                GetRelations(doc.SelectNodes("/osm/relation"));
            }
        }

        // Если путь ввода пользователем к XML - файлу не в формате XML, пользователь
// возвращается в Главное меню с сообщением об ошибке.
        catch
        {
            SceneManager.LoadScene(0); 
        }

        // Triggers the start of the "MapBuilder" script.
        IsReady = true;  
    }

    /// Извлекает информацию о границах из XML-файла и генерирует соответствующий
/// экземпляр.
    /// <param name="xmlNode">XML node</param>
    void SetBounds(XmlNode xmlNode)
    {
        bounds = new OsmBounds(xmlNode);
    }
/// Извлекает информацию об узлах из XML-файла и генерирует соответствующие
/// экземпляры.
    
    /// <param name="xmlNodeList">XML node</param>
    void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode n in xmlNodeList)
        {
            OsmNode node = new OsmNode(n);
            nodes[node.ID] = node;
        }
    }

    /// Извлекает информацию о путях из XML-файла и генерирует соответствующие
/// экземпляры.
    /// <param name="xmlNodeList">XML node</param>
    void GetWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        { 
            OsmWay way = new OsmWay(node);

            if (way.IsRailway == true)
            {
                if (UserPreferences.PublicTransportRailways)
                {
                    ways[way.ID] = way;
                }
            }
            else if (way.IsStreet == true)
            {
                if (UserPreferences.PublicTransportStreets)
                {
                    ways[way.ID] = way;
                }
            }
        }
    }

    
    /// Извлекает информацию о связи из XML-файла и генерирует соответствующие
/// экземпляры.
    
    /// <param name="xmlNodeList">XML node</param>
    void GetRelations(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            OsmRelation relation = new OsmRelation(node);

            if(relation.Route == true)
            {
                relations.Add(relation);

                if (UserPreferences.PublicTransportRailways || UserPreferences.PublicTransportStreets)
                {
                    TagPublicTransportWays(relation);
                }

                if (UserPreferences.Stations)
                {
                    foreach (ulong NodeID in relation.StoppingNodeIDs)
                    {
                        try
                        {
                            nodes[NodeID].TransportLines.Add(relation.Name);
                            relation.StationNames.Add(nodes[NodeID].StationName);
                        }
                        catch (KeyNotFoundException)
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }

    
    /// Если в меню опций активированы автомобильные и железные дороги общественного транспорта,
/// эта функция извлекает последние данные из этих узлов.
    
    /// <param name="r">XML node</param>
    void TagPublicTransportWays(OsmRelation r)
    {
        foreach (ulong WayID in r.WayIDs)
        {
            try
            {
                switch (r.TransportType)
                {
                    case "subway":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportRailway = true;
                        ways[WayID].TransportTypes.Add("subway");
                        break;
                    case "tram":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportRailway = true;
                        ways[WayID].TransportTypes.Add("tram");
                        break;
                    case "train":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportRailway = true;
                        ways[WayID].TransportTypes.Add("train");
                        break;
                    case "railway":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportRailway = true;
                        ways[WayID].TransportTypes.Add("railway");
                        break;
                    case "light_rail":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportRailway = true;
                        ways[WayID].TransportTypes.Add("light_rail");
                        break;
                    case "bus":
                        ways[WayID].TransportLines.Add(r.Name);
                        ways[WayID].PublicTransportStreet = true;
                        ways[WayID].TransportTypes.Add("bus");
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
        }
    }
}
