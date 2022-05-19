using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;


/// После выбора линии поезда/автобуса будет создан экземпляр объекта WayOrderManager Gameobject, который содержит этот скрипт. Этот сценарий принимает
/// путевые узлы железнодорожной/автобусной линии и сортирует их таким образом, чтобы транспортное средство могло перемещаться от узла к узлу, используя список координат. Наконец, это
/// создает экземпляры объектов транспортного средства.

public class SortWay : MonoBehaviour
{
    // первое измерение = Объекты пути, второе измерение = координаты узлов объектов пути.
    public List<List<Vector3>> allWayCoordinates;

    // первое измерение = пути (пути, которые соединены вместе), второе измерение = объекты пути, которые отсортированы по путям, третье измерение = координаты узлов объектов пути.
    public static List<List<List<Vector3>>> SortedPathAndWays; 
                                                     
    public static List<List<Vector3>> SplittedinPaths;
    public static List<List<Vector3>> PathsInRightOrder;

    public static List<Vector3> MoveToTarget;
    public static List<Vector3> PathLastNode;

    private void Start()
    {
        allWayCoordinates = new List<List<Vector3>>();
        SortedPathAndWays = new List<List<List<Vector3>>>();
        SplittedinPaths = new List<List<Vector3>>();
        PathsInRightOrder = new List<List<Vector3>>();

        MoveToTarget = new List<Vector3>();
        PathLastNode = new List<Vector3>();

        List<GameObject> allObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(allObjects);

        // Этот цикл for возвращает список "allWayCoordinates", который содержит больше списков. Каждый список из них-это объекты пути
        // которые содержат координаты узла
        for (int i = 0; i < TranSportWayMarker.SelectedWays.Count; i++)
        {
            string ObjectName = allObjects[TranSportWayMarker.SelectedWays[i]].name;
            string ObjectID = ObjectName.Substring(15); // Идентификатор пути определяется для того, чтобы найти его в классе MapReader.

            List<Vector3> WayNodes = new List<Vector3>();
            for (int j = 0; j < MapReader.ways[Convert.ToUInt64(ObjectID)].UnityCoordinates.Count; j++)
            {
                WayNodes.Add(MapReader.ways[Convert.ToUInt64(ObjectID)].UnityCoordinates[j]);
            }
            allWayCoordinates.Add(WayNodes);
            // allWayCoordinates содержит списки, которые представляют объекты пути. Каждый из этих списков содержит координаты узлов в качестве значений.
        }

        // Далее мы создаем новый список (SortedPathAndWays). Этот список объединяет пути, принадлежащие общему пути.
        // Например, если автобусная линия заканчивается на одном краю карты и продолжается на другом краю карты, соответствующие пути будут сохранены в разных списках.
        List<List<Vector3>> localList = new List<List<Vector3>>();
        localList.Add(allWayCoordinates[0]);
        SortedPathAndWays.Add(localList);
        allWayCoordinates.RemoveAt(0);

        // Вызывается для сортировки SortedPathAndWays по отношению к allWayCoordinates.
        SortList(allWayCoordinates, 0);

        // Здесь мы преобразуем трехмерный список SortedPathAndWays в двумерный список. Мы делаем это путем объединения внутренних списков
        // потому что они уже отсортированы.
        List<Vector3> temporaryList = new List<Vector3>();
        for(int i = 0; i < SortedPathAndWays.Count; i++) 
        {
            temporaryList = SortedPathAndWays[i].SelectMany(x => x).ToList();
            SplittedinPaths.Add(temporaryList);
        }

        for(int i = 0; i < TranSportWayMarker.StationOrder.Count; i++)
        {
            for(int j = 0; j < SplittedinPaths.Count; j++)
            {
                for(int k = 0; k < SplittedinPaths[j].Count; k++)
                {
                    if(TranSportWayMarker.StationOrder[i] == SplittedinPaths[j][k])
                    {
                        if (PathsInRightOrder.Contains(SplittedinPaths[j]))
                        {
                            break;
                        }
                        else
                        {
                            PathsInRightOrder.Add(SplittedinPaths[j]);
                        }
                    }
                }
            }
        }

        // Добавление путей, которые не содержат никакой станции в конце.
        for (int i = 0; i < SplittedinPaths.Count; i++)
        {
            if (!PathsInRightOrder.Contains(SplittedinPaths[i]))
            {
                PathsInRightOrder.Add(SplittedinPaths[i]);
            }
        }

        // Переключение направления значений в пределах путей с помощью станций.
        int firstIndex = -1;
        int secondIndex = -1;
        for(int i = 0; i < TranSportWayMarker.StationOrder.Count; i++)
        {
            for(int k = 0; k < PathsInRightOrder.Count; k++)
            {
                for(int j = 0; j < PathsInRightOrder[k].Count; j++)
                {
                    if(TranSportWayMarker.StationOrder[i] == PathsInRightOrder[k][j])
                    {
                        if(firstIndex == -1)
                        {
                            firstIndex = j;
                            break;
                        }
                        else
                        {
                            secondIndex = j;
                            break;
                        }
                    }
                }
                if(firstIndex != -1 && secondIndex != -1)
                {
                    if(firstIndex > secondIndex)
                    {
                        PathsInRightOrder[k].Reverse();
                        break;
                    }
                }
            }
        }

        for(int i = 0; i < PathsInRightOrder.Count; i++)
        {
            for(int j = 0; j < SortWay.PathsInRightOrder[i].Count; j++)
            {
                MoveToTarget.Add(SortWay.PathsInRightOrder[i][j]);
            }
        }

        if(SortWay.PathsInRightOrder.Count > 1)
        {
            for(int i = 0; i < PathsInRightOrder.Count; i++)
            {
                PathLastNode.Add(PathsInRightOrder[i][PathsInRightOrder[i].Count - 1]);
            }
        }

        IdentifyVehicle();
    }


    //Эта функция сортирует объекты в правильном порядке.
    void SortList(List<List<Vector3>> WaysToBeSorted, int index)
    {
        if(WaysToBeSorted.Count <= 0)
        {
            return;
        }

        List<List<Vector3>> LeftItems = WaysToBeSorted; 
        bool found_something = false;

        for(int i = 0; i < WaysToBeSorted.Count; i++)
        {
            if(WaysToBeSorted[i][0] == SortedPathAndWays[index][SortedPathAndWays[index].Count - 1][SortedPathAndWays[index][SortedPathAndWays[index].Count - 1].Count - 1])
            {
                SortedPathAndWays[index].Add(WaysToBeSorted[i]);
                LeftItems.RemoveAt(i);
                found_something = true;
            }
            else if (SortedPathAndWays[index][0][0] == WaysToBeSorted[i][WaysToBeSorted[i].Count - 1])
            {
               SortedPathAndWays[index].Insert(0, WaysToBeSorted[i]);
                LeftItems.RemoveAt(i);
                found_something = true;
            }
            else if(WaysToBeSorted[i][0] == SortedPathAndWays[index][0][0])
            {
                WaysToBeSorted[i].Reverse();
                SortedPathAndWays[index].Insert(0, WaysToBeSorted[i]);
                LeftItems.RemoveAt(i);
                found_something = true;
            }
            else if (WaysToBeSorted[i][WaysToBeSorted[i].Count - 1] == SortedPathAndWays[index][SortedPathAndWays[index].Count - 1][SortedPathAndWays[index][SortedPathAndWays[index].Count - 1].Count - 1])
            {
                WaysToBeSorted[i].Reverse();
                SortedPathAndWays[index].Add(WaysToBeSorted[i]);
                LeftItems.RemoveAt(i);
                found_something = true;
            }
            else
            {
                continue;
            }
        }

        if(found_something == false)
        {
            List<List<Vector3>> localList = new List<List<Vector3>>();
            localList.Add(WaysToBeSorted[0]);
            SortedPathAndWays.Add(localList);
            LeftItems.RemoveAt(0);
            SortList(LeftItems, index + 1);
        }
        else
        {
            SortList(LeftItems, index);
        }
    }

    /// Эта функция может определить, какое транспортное средство необходимо, используя сохраненное значение цвета пути. С помощью
    /// эта информация может вызвать функцию VehicleSpawner, которая создает экземпляр правильного транспортного средства.

    void IdentifyVehicle()
    {
        switch (ColorUtility.ToHtmlStringRGB(TranSportWayMarker.PreviousMaterial.color))
        {
            case "1400FF":
                StartCoroutine(VehicleSpawner("Subway"));
                break;
            case "FF0000":
                print("bus");
                StartCoroutine(VehicleSpawner("Bus"));
                break;
            case "00C5FF":
                StartCoroutine(VehicleSpawner("Tram"));
                break;
            case "FFFC00":
                StartCoroutine(VehicleSpawner("Train"));
                break;
            case "08FF00":
                StartCoroutine(VehicleSpawner("Tram"));
                break;
            case "FF7800":
                StartCoroutine(VehicleSpawner("Tram"));
                break;
        }
    }

    int wagons = 0;
    /// Эта программа контролирует нерест транспортных средств. 
    /// <param name="Vehicle">name of the transport vehicle</param>
    /// <returns></returns>
    public IEnumerator VehicleSpawner(string Vehicle)
    {
        bool flag = true;

        while (flag)
        {
            if(Vehicle == "Bus")
            {
                GameObject Bus = Instantiate(Resources.Load(Vehicle)) as GameObject; // Создайте экземпляр транспортного средства автобуса.
            }
            else if(wagons == 0 && Vehicle != "Bus") 
            {
                wagons = 1;
                GameObject Tram = Instantiate(Resources.Load(Vehicle)) as GameObject; // Создайте экземпляр ведущего вагона железнодорожного транспортного средства.
                if (Vehicle == "Train")
                {
                    yield return new WaitForSeconds(0.8f);
                }
                else
                {
                    yield return new WaitForSeconds(0.6f);
                }
            }
            if (wagons < 4 && Vehicle != "Bus") 
            {
                wagons += 1;
                GameObject Wagon = Instantiate(Resources.Load(Vehicle + " Wagon")) as GameObject; // Создайте экземпляр других вагонов железнодорожного транспортного средства.
                if (Vehicle == "Train")
                {
                    yield return new WaitForSeconds(0.8f);
                }
                else
                {
                    yield return new WaitForSeconds(0.6f);
                }
            }
            else 
            {
                yield return new WaitForSeconds(20f); // После создания 3 вагонов подождите 20 секунд и создайте следующие.
                wagons = 0;
            }
        }
    }
}
