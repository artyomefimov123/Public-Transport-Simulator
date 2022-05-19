using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Здесь мы обрабатываем пользовательский ввод из раскрывающегося списка станции.Объект пути выбранной публичной
/// транспортной линии будет отмечен и сохранен. Кроме того, запускается скрипт 
/// для сортировки информации о способе перемещения транспортного средства.

public class TranSportWayMarker : MonoBehaviour
{
    public static List<int> SelectedWays; 
    static List<int> SelectedStations; 
    public static Material PreviousMaterial; 

    public static List<Vector3> StationOrder; 
    
    static bool SelectionStarted = false; 

    private void Start()
    {
        SelectedWays = new List<int>();
        SelectedStations = new List<int>();

        StationOrder = new List<Vector3>();
    }

    /// Эта функция вызывается, как только пользователь выбирает опцию из выпадающего списка станций. Это отменяет
    ///предыдущие изменения  и помечает выбранную в данный момент линию общественного транспорта.
    /// <param name="selection">The dropdown selection of the user from the station UI</param>
    public static void SelectPublicTransportLine(string selection)
    {
        if (SelectionStarted == true || selection == "")
        {
            return;
        }

        List<GameObject> allWayObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(allWayObjects);

        for (int i = 0; i < allWayObjects.Count; i++)
        {
            if (allWayObjects[i].name == "Station(Clone)")
            {
                allWayObjects[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // Закрыть все интерфейсы станций.
                allWayObjects[i].transform.GetChild(0).GetChild(0).GetChild(6).gameObject.SetActive(true); 
            }
            else if (allWayObjects[i].name == "WayOrderManager(Clone)" || allWayObjects[i].name == "Bus(Clone)" || allWayObjects[i].name == "Tram(Clone)" || allWayObjects[i].name == "Subway(Clone)" || allWayObjects[i].name == "Train(Clone)")
            {
                allWayObjects[i].Destroy(); // удалить все текущие транспортные средства
            }
        }

        if (selection == "Global View") 
        {
            SelectionStarted = true;
            for (int j = 0; j < SelectedWays.Count; j++) // Текущая маркировка отменяется.
            {
                int index = SelectedWays[j];
                allWayObjects[index].transform.localPosition = new Vector3(allWayObjects[index].transform.localPosition.x, 0, allWayObjects[index].transform.localPosition.z);
                allWayObjects[index].gameObject.GetComponent<Renderer>().material = PreviousMaterial;
            }
            for (int k = 0; k < SelectedStations.Count; k++) 
            {
                int index = SelectedStations[k];
                GameObject StationUI = allWayObjects[index].transform.GetChild(0).GetChild(0).gameObject;
                StationUI.transform.GetChild(2).gameObject.GetComponent<Dropdown>().value = 0;
            }
            SelectionStarted = false;
        }
        else
        {
            SelectionStarted = true;

            for (int j = 0; j < SelectedWays.Count; j++) 
            {
                int index = SelectedWays[j];
                allWayObjects[index].transform.localPosition = new Vector3(allWayObjects[index].transform.localPosition.x, 0, allWayObjects[index].transform.localPosition.z);
                allWayObjects[index].gameObject.GetComponent<Renderer>().material = PreviousMaterial;
            }
            for (int k = 0; k < SelectedStations.Count; k++) 
            {
                int index = SelectedStations[k];
                GameObject StationUI = allWayObjects[index].transform.GetChild(0).GetChild(0).gameObject;
                StationUI.transform.GetChild(2).gameObject.GetComponent<Dropdown>().value = 0;
            }

            SelectedWays.Clear();
            SelectedStations.Clear();
            StationOrder.Clear();
            PreviousMaterial = null;

            // Здесь мы отмечаем выбранную линию общественного транспорта и сохраняем ее исходную информацию для последующего возврата.
            for (int i = 0; i < allWayObjects.Count; i++)
            {
                if (allWayObjects[i].name.StartsWith("New Game Object"))
                {
                    var gameObjectText = allWayObjects[i].GetComponent<Text>();
                    if (gameObjectText.text.Contains(selection)) 
                    {
                        SelectedWays.Add(i);
                        PreviousMaterial = allWayObjects[i].GetComponent<Renderer>().material;

                        allWayObjects[i].transform.position += new Vector3(0, 0.1f, 0); 
                        allWayObjects[i].GetComponent<Renderer>().material = MapBuilder.selected_way; 
                    }
                }
                else if (allWayObjects[i].name == "Station(Clone)")
                {
                    GameObject DropDown = allWayObjects[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
                    for (int j = 0; j < DropDown.GetComponent<Dropdown>().options.Count; j++)
                    {
                        if (DropDown.GetComponent<Dropdown>().options[j].text == selection) 
                        {
                            SelectedStations.Add(i);

                            GameObject StationUI = allWayObjects[i].transform.GetChild(0).GetChild(0).gameObject; 
                            StationUI.SetActive(true);
                            StationUI.transform.GetChild(6).gameObject.SetActive(false); 
                            StationUI.transform.GetChild(2).gameObject.GetComponent<Dropdown>().value = j; 
                        }
                    }
                }
            }

            SelectionStarted = false;

            // Эта часть генерирует список координат станций, которые сортируются в правильном порядке.
            // Позже это используется для перемещения транспортного средства.
            for (int i = 0; i < MapReader.relations.Count; i++)
            {
                if (MapReader.relations[i].Name == selection)
                {
                    for (int j = 0; j < MapReader.relations[i].StoppingNodeIDs.Count; j++)
                    {
                        try
                        {
                            StationOrder.Add(MapReader.nodes[MapReader.relations[i].StoppingNodeIDs[j]] - MapReader.bounds.Centre);
                        }
                        catch (KeyNotFoundException)
                        {
                            continue;
                        }
                    }
                }
            }

            // Создается экземпляр игрового объекта "WayOrderManager". Этот объект запускает скрипт "SortWay", который управляет
            // потоком транспортных средств общественного транспорта.
            GameObject WayOrderer = Instantiate(Resources.Load("WayOrderManager")) as GameObject;
        }
    }
}
