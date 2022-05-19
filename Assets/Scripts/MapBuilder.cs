using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


///Этот скрипт генерирует все железные дороги, станции и дороги, используя информацию
///из скрипта MapReader. Здесь также создается экземпляр поколения 3D-зданий.
///Прогресс отображается в окне загрузки в процентах.
class MapBuilder : MonoBehaviour
{
    public GameObject StationPrefab; // Объекты станции.
    public GameObject StationUIPrefab; // UI станции.
    public GameObject buildings; // Mapbox 3d здания.
    public GameObject roads; // Mapbox карта.
    public GameObject InGameLoadingScreen; // Показ процента загрузки.
    public GameObject InGameLoadingMessage; // Показ загрузочного сообщения.
    public GameObject InGameLoadingWindow; // Показ загрузочного окна.

    // Здесь хранятся различные цвета общественного транспорта, которые используются
    // при создании автомобильных и железных дорог.
    public Material bus_streets;
    public Material public_transport_railways;
    public Material subways;
    public Material trams;
    public Material trains;
    public Material railway;
    public Material light_rails;
    public static Material selected_way;

    Material inUse; 

    bool StationsCreated = false;

    // Это используется для окна процент загрузки.
    float processed_items = 0f;
    int percentageAmount = 0;

    /// Эта функция ожидает, пока скрипт MapReader обработает всю информацию.
    /// Как только это будет сделано, скрипт MapReader запустит эту функцию, чтобы
    /// запустить процесс построения сцены.
    /// <returns></returns>
    IEnumerator Start()
    {
        if (!UserPreferences.Stations)
        { 
            StationsCreated = true;
        }
        else
        {
            StationBuilder(); // Строятся станции.
        }

        while (!MapReader.IsReady || !StationsCreated)
        {
            yield return null;
        }

        if (UserPreferences.Buildings)
        {
            // Создаются экземпляры 3D - зданий.
            GameObject MapboxBuildings = Instantiate(buildings);
            MapboxBuildings.transform.localScale = new Vector3(1.223f, 1.22264f, 1.219f);
            MapboxBuildings.transform.localPosition = new Vector3(0, -0.1f, 0);
        }
        if (UserPreferences.AllStreets)
        {
            // Создается экземпляр карты.
            GameObject MapboxRoads = Instantiate(roads);
            MapboxRoads.transform.localScale = new Vector3(1.223f, 1.22264f, 1.219f);
            MapboxRoads.transform.localPosition = new Vector3(0, -0.1f, 0);
        }
        if(UserPreferences.PublicTransportRailways || UserPreferences.PublicTransportStreets)
        {
            StartCoroutine(WayBuilder()); // Создаются экземпляры автомобильных и железных дорог.
        }
    }

    /// Мы перебираем все экземпляры отношений, чтобы сгенерировать объекты станции.
    void StationBuilder()
    {
        for(int i=0; i<MapReader.relations.Count; i++)
        {
            switch (MapReader.relations[i].TransportType)
            {
                case "subway":
                    if (UserPreferences.Subways == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
                case "tram":
                    if (UserPreferences.Trams == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
                case "train":
                    if (UserPreferences.Trains == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
                case "railway":
                    if (UserPreferences.Railways == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
                case "light_rail":
                    if (UserPreferences.LightRails == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
                case "bus":
                    if (UserPreferences.Busses == true)
                    {
                        CreateStations(MapReader.relations[i]);
                    }
                    break;
            }
        }
        StationsCreated = true;
    }

    /// Мы перебираем экземпляры отношений, чтобы сгенерировать объекты станции и
    /// соответствующие UI станции.
    /// <param name="r">relation instance</param>
    void CreateStations(OsmRelation r)
    {       
        foreach (ulong NodeID in r.StoppingNodeIDs)
        {
            GameObject station_object;
            GameObject stationUI_object;

            try
            {
                // Если станция уже была создана с использованием позиции узла, новая станция не будет создана.
                // будет создан там, но существующий будет дополнен новой информацией. Это
                // позволяет избежать случая множественного перекрытия объекта станции в одной точке.
                if (MapReader.nodes[NodeID].StationCreated == true)
                {
                    List<GameObject> allObjects = new List<GameObject>();
                    Scene scene = SceneManager.GetActiveScene();
                    scene.GetRootGameObjects(allObjects);

                    // Мы перебираем все объекты Unity, чтобы найти станцию, которая
                    // уже была сгенерирована в определенной точке.
                    for (int i = 5; i < allObjects.Count; i++)
                    {
                        if(allObjects[i].transform.position == MapReader.nodes[NodeID] - MapReader.bounds.Centre)
                        {
                            bool doubleFound = false;

                            // Здесь мы проверяем, добавлена ли новая информация к объекту станции,
                            // еще не был сохранен внутри объекта станции. Эту операцию можно проследить
                            // вернемся к ошибке, с которой я столкнулся во время разработки, где та же информация
                            // сохранялся несколько раз. Причина этого до сих пор неясна, это обходной путь.
                            GameObject Dropdown = allObjects[i].transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject;
                            var dropOptions = Dropdown.GetComponent<Dropdown>();
                            for(int j = 0; j < dropOptions.options.Count; j++)
                            {
                                for(int k = 0; k < MapReader.nodes[NodeID].TransportLines.Count; k++)
                                {
                                    if(dropOptions.options[j].text == MapReader.nodes[NodeID].TransportLines[k])
                                    {
                                        doubleFound = true;
                                        continue;
                                    }
                                }
                            }
                            if (!doubleFound)  // Если информация не найдена, мы добавим новую информацию здесь.
                            {
                                dropOptions.AddOptions(MapReader.nodes[NodeID].TransportLines);
                                if (r.TransportType == "bus")
                                {
                                    // Активирует символ шины в пользовательском интерфейсе.
                                    allObjects[i].transform.GetChild(0).GetChild(0).transform.GetChild(4).gameObject.SetActive(true);
                                }
                                else
                                {
                                    // Активирует символ поезда в пользовательском интерфейсе.
                                    allObjects[i].transform.GetChild(0).GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
                                }
                                continue;
                            }
                        }
                    }
                    continue;
                }

                station_object = Instantiate(StationPrefab) as GameObject;
                OsmNode new_station = MapReader.nodes[NodeID];
                Vector3 new_station_position = new_station - MapReader.bounds.Centre;
                station_object.transform.position = new_station_position;

                // Устанавливается таким образом, чтобы на этой позиции не создавалось никаких новых станций.
                MapReader.nodes[NodeID].StationCreated = true;


                stationUI_object = Instantiate(StationUIPrefab) as GameObject;
                stationUI_object.transform.SetParent(station_object.transform.GetChild(0));
                stationUI_object.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                stationUI_object.transform.localPosition = new Vector3(-15, 0, 0);

                GameObject station_text = stationUI_object.transform.GetChild(1).gameObject;
                station_text.transform.localPosition = new Vector3(0, 7, 0);
                Text OnScreenText = station_text.GetComponent<Text>();
                OnScreenText.text = MapReader.nodes[NodeID].StationName;

                GameObject TransportLineDropdown = stationUI_object.transform.GetChild(2).gameObject;
                var dropDownOptions = TransportLineDropdown.GetComponent<Dropdown>();
                dropDownOptions.AddOptions(MapReader.nodes[NodeID].TransportLines);

                stationUI_object.SetActive(false);

                if(r.TransportType == "bus")
                {
                    // Активирует символ шины в пользовательском интерфейсе.
                    stationUI_object.transform.GetChild(4).gameObject.SetActive(true);
                }
                else
                {
                    // Активирует символ поезда в пользовательском интерфейсе.
                    stationUI_object.transform.GetChild(5).gameObject.SetActive(true);
                }
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
        }
    }

    /// Эта функция генерирует автомобильные и железнодорожные грузы. Он также показывает загрузку
    /// экран с процентом выполнения.
    /// <returns></returns>
    IEnumerator WayBuilder()
    {
        float TargetCount = MapReader.ways.Count;
        Text PercentageDisplayer = InGameLoadingScreen.GetComponent<Text>();

        InGameLoadingWindow.SetActive(true);

        foreach (KeyValuePair<ulong, OsmWay> w in MapReader.ways)
        {
            // Простая логика выполнения загрузки, которая возвращает сумму
            // обработанных изделий, деленное на общее количество изделий.
            processed_items += 1;
            float loadingPercentage = processed_items / TargetCount * 100;

            if(loadingPercentage < 99) 
            {
                if(loadingPercentage > percentageAmount)
                {
                    percentageAmount += 1;
                    PercentageDisplayer.text = percentageAmount.ToString() + "%";
                }
            }
            else
            {
                InGameLoadingScreen.SetActive(false);
                InGameLoadingMessage.SetActive(false);
                InGameLoadingWindow.SetActive(false);
            }

            // Здесь мы начинаем процесс создания экземпляра автомобильной/железнодорожной дороги.
            if (w.Value.PublicTransportStreet)
            {
                if (UserPreferences.PublicTransportStreets)
                {
                    inUse = bus_streets;
                }
                else
                {
                    inUse = null;
                    continue;
                }
            }
            else if (w.Value.PublicTransportRailway)
            {
                if (UserPreferences.PublicTransportRailways)
                {
                    if(!UserPreferences.Subways && !UserPreferences.Trams && !UserPreferences.Trains && !UserPreferences.Railways && !UserPreferences.LightRails)
                    {
                        inUse = public_transport_railways;
                    }
                    else if (w.Value.TransportTypes.Contains("subway"))
                    {
                        if (UserPreferences.Subways)
                        {
                            inUse = subways;
                        }
                        else
                        {
                            inUse = null;
                            continue;
                        }
                    }
                    else if (w.Value.TransportTypes.Contains("tram"))
                    {
                        if (UserPreferences.Trams)
                        {
                            inUse = trams;
                        }
                        else
                        {
                            inUse = null;
                            continue;
                        }
                    }
                    else if (w.Value.TransportTypes.Contains("train"))
                    {
                        if (UserPreferences.Trains)
                        {
                            inUse = trains;
                        }
                        else
                        {
                            inUse = null;
                            continue;
                        }
                    }
                    else if (w.Value.TransportTypes.Contains("railway"))
                    {
                        if (UserPreferences.Railways)
                        {
                            inUse = railway;
                        }
                        else
                        {
                            inUse = null;
                            continue;
                        }
                    }
                    else if (w.Value.TransportTypes.Contains("light_rail"))
                    {
                        if (UserPreferences.LightRails)
                        {
                            inUse = light_rails;
                        }
                        else
                        {
                            inUse = null;
                            continue;
                        }
                    }
                    else
                    {
                        inUse = null;
                        continue;
                    }
                    
                }
            }
            else
            {
                inUse = null;
                continue;
            }
                                             
            GameObject go = new GameObject();
            var waytext = go.AddComponent<Text>();
            foreach (string tramline in w.Value.TransportLines)
            {
                waytext.text += tramline + ", ";
            }
            Vector3 localOrigin = GetCentre(w.Value);
            go.transform.position = localOrigin - MapReader.bounds.Centre;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            mr.material = inUse;

            // Здесь мы храним векторы, нормали и индексы.
            List<Vector3> vectors = new List<Vector3>();  
            List<Vector3> normals = new List<Vector3>();
            List<int> indicies = new List<int>();

            for (int i = 1; i < w.Value.NodeIDs.Count; i++)
            {
                OsmNode p1 = MapReader.nodes[w.Value.NodeIDs[i - 1]];
                OsmNode p2 = MapReader.nodes[w.Value.NodeIDs[i]];

                Vector3 s1 = p1 - localOrigin;  
                Vector3 s2 = p2 - localOrigin;

                Vector3 diff = (s2 - s1).normalized;

                // Ширина автомобильных и железных дорог установлена в 1 метр.
                var cross = Vector3.Cross(diff, Vector3.up) * 1.0f; 

                Vector3 v1 = s1 + cross;
                Vector3 v2 = s1 - cross;
                Vector3 v3 = s2 + cross;
                Vector3 v4 = s2 - cross;

                vectors.Add(v1);  
                vectors.Add(v2);
                vectors.Add(v3);
                vectors.Add(v4);

                normals.Add(Vector3.up);  
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                int idx1, idx2, idx3, idx4;
                idx4 = vectors.Count - 1;
                idx3 = vectors.Count - 2;
                idx2 = vectors.Count - 3;
                idx1 = vectors.Count - 4;

                indicies.Add(idx1);
                indicies.Add(idx3);
                indicies.Add(idx2);

                indicies.Add(idx3);
                indicies.Add(idx4);
                indicies.Add(idx2);
            }
            go.name += w.Value.ID;
            mf.mesh.vertices = vectors.ToArray();
            mf.mesh.normals = normals.ToArray();
            mf.mesh.triangles = indicies.ToArray();

            yield return null;

            // Наконец, мы храним координаты единства каждого сгенерированного пути. Это затем используется позже
            // когда мы хотим переместить транспортные средства через пути.
            for (int i = 0; i < w.Value.NodeIDs.Count; i++)
            {
                OsmNode p1 = MapReader.nodes[w.Value.NodeIDs[i]];
                w.Value.UnityCoordinates.Add(p1 - MapReader.bounds.Centre);
            }
        }
    }

    /// Возвращает центральную точку объекта. Эта информация используется в качестве справочной
    /// для размещения объекта внутри мира единства.
    /// <param name="way">way instance</param>
    /// <returns></returns>
    protected Vector3 GetCentre(OsmWay way)  
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total += MapReader.nodes[id];  
        }
        return total / way.NodeIDs.Count;
    }  
}