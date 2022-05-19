using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// Используется для функции поиска в моделируемой сцене (управляет строкой поиска).

public class SearchBar : MonoBehaviour
{
    public GameObject TextBar;
    public GameObject DropDown;

    public List<string> FoundStations; 
    public List<string> AllStation; 

    void Start()
    {
             transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Поиск станций";
        

        FoundStations = new List<string>();
        AllStation = new List<string>();

        List<GameObject> allObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(allObjects);

        for(int i = 0; i < allObjects.Count; i++)
        {
            if(allObjects[i].name == "Station(Clone)")
            {
                if (AllStation.Contains(allObjects[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text))
                {
                    continue;
                }
                AllStation.Add(allObjects[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text);
            }
        }
    }

    /// <param name="searchInput">User input from the search bar</param>
    public void SearchForStation(string searchInput)
    {
        if(searchInput == "")
        {
            return;
        }

        DropDown.SetActive(true);
        FoundStations.Clear();                       FoundStations.Add("Результаты поиска:");
       
        
        for(int i = 0; i < AllStation.Count; i++)
        {
            if (AllStation[i].ToLower().Contains(searchInput.ToLower()))
            {
                FoundStations.Add(AllStation[i]);
            }
        }

        DropDown.GetComponent<Dropdown>().options.Clear();
        DropDown.GetComponent<Dropdown>().AddOptions(FoundStations);
    }
}
