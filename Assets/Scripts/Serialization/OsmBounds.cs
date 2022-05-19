using UnityEngine;
using System.Globalization;
using System.Xml;


/// <summary>
/// Берет граничные данные из XML-файла OSM и вычисляет центр карты. 
/// Созданные вручную объекты затем центрируются с использованием этих значений центра. Кроме того, этот класс
/// также отвечает за создание ссылочных значений для объектов, созданных с помощью SDK Mapbox.
/// Эти объекты центрируются с использованием значений из этого класса.
/// </summary>
class OsmBounds : BaseOsm
{
    public float MinLat { get; private set; }

    public float MaxLat { get; private set; }

    public float MinLon { get; private set; }

    public float MaxLon { get; private set; }
	
    public Vector3 Centre { get; private set; } 

    public string BuildingCentre { get; private set; } 

    public static int LenghtFactor { get; private set; }

    public static int WidthFactor { get; private set; } 

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">XML node parameter</param>
    public OsmBounds(XmlNode node)
    {
        MinLat = GetFloat("minlat", node.Attributes);
        MaxLat = GetFloat("maxlat", node.Attributes);
        MinLon = GetFloat("minlon", node.Attributes);
        MaxLon = GetFloat("maxlon", node.Attributes);

        float x = (float)((MercatorProjection.lonToX(MaxLon) + MercatorProjection.lonToX(MinLon)) / 2);
        float y = (float)((MercatorProjection.latToY(MaxLat) + MercatorProjection.latToY(MinLat)) / 2);

        Centre = new Vector3(x, 0, y);

        string x_2 = ((MaxLat + MinLat) / 2).ToString(CultureInfo.InvariantCulture);
        string y_2 = ((MaxLon + MinLon) / 2).ToString(CultureInfo.InvariantCulture);

        BuildingCentre = x_2 + ", " + y_2;

        SetMapboxFactor(MaxLon, MinLon, MaxLat, MinLat);
    }

    /// <param name="MaxLon">Highest Longitude value</param>
    /// <param name="MinLon">Lowest Longitude value</param>
    /// <param name="MaxLat">Highest Latitude value</param>
    /// <param name="MinLat">Lowest Latitude value</param>
    void SetMapboxFactor(float MaxLon, float MinLon, float MaxLat, float MinLat)
    {
        float lenght = MaxLon - MinLon;
        float width = MaxLat - MinLat;

        int lenghtValue = Mathf.FloorToInt(lenght / 0.0237f);
        int widthValue = Mathf.FloorToInt(width / 0.017f) + 1;

        if(lenghtValue == 0)
        {
            lenghtValue = 1;
        }
        if(widthValue == 0)
        {
            widthValue = 1;
        }

        LenghtFactor = lenghtValue + 1;
        WidthFactor = widthValue + 1;
    }
}

