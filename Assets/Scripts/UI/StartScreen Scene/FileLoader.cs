using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Этот класс управляет Главным меню симуляции и всей
/// сценой "Стартовый экран". Он также обрабатывает ввод пользователем пути к данным
///  (XML-файл OSM) и переключается на экран загрузки.

public class FileLoader : MonoBehaviour
{
    public GameObject StartScreen;
    public GameObject MainMenu;
    public GameObject SimulationSource;
    public GameObject OptionsScreen;
    public GameObject QuitScreen;
    public GameObject LoadingScreen;
    public GameObject FilePath;
    public GameObject Instructions1;
    public GameObject Instructions2;
    public GameObject Instructions3;
    public GameObject ErrorMessage;
    public GameObject ErrorMessageOptions;
    public GameObject ErrorMessageOptions2;

    // Здесь будет храниться путь к OSM XML файлу.
    public static string ResourceFilePath;

    bool user_input = false;

    public static bool simulator_loaded = false;
    
    private void Update()
    {
        // Проверить правильность пути и запустить загрузочный экран
        if (user_input == true)
        {
            StartScreen.SetActive(false);
            LoadingScreen.SetActive(true);
            user_input = false;
            if (File.Exists(ResourceFilePath) && ResourceFilePath.EndsWith(".txt"))
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                LoadingScreen.SetActive(false);
                StartScreen.SetActive(true);
                ErrorMessage.SetActive(true);
            }
        }

        if (simulator_loaded)
        {
            simulator_loaded = false;
        }
    }

    public void StartSimulation()
    {
        MainMenu.SetActive(false);
        SimulationSource.SetActive(true);
    }

        public void ConfirmPath()
    {
        ResourceFilePath = FilePath.GetComponent<Text>().text;
        if (ResourceFilePath != "")
        {
            user_input = true;
        }
    }


    public void ReturnToSimulationStart()
    {
        Instructions1.SetActive(false);
        Instructions2.SetActive(false);
        Instructions3.SetActive(false);
        SimulationSource.SetActive(true);
    }

    public void NextPage1()
    {
        Instructions1.SetActive(false);
        Instructions2.SetActive(true);
    }
    public void ReturnPage()
    {
        Instructions1.SetActive(true);
        Instructions2.SetActive(false);
    }

    public void NexPage2()
    {
        Instructions2.SetActive(false);
        Instructions3.SetActive(true);
    }

    public void Return2()
    {
        Instructions3.SetActive(false);
        Instructions2.SetActive(true);
    }

    public void Options()
    {
        MainMenu.SetActive(false);
        OptionsScreen.SetActive(true);
        
    }

    public void ReturnfromOptions()
    {
        if (!UserPreferences.Buildings && !UserPreferences.Stations && !UserPreferences.AllStreets && !UserPreferences.PublicTransportStreets && !UserPreferences.PublicTransportRailways)
        {
            ErrorMessageOptions2.SetActive(false);
            ErrorMessageOptions.SetActive(true);
        }
        else if(UserPreferences.Stations && !UserPreferences.Subways && !UserPreferences.Trams && !UserPreferences.Trains && !UserPreferences.Railways && !UserPreferences.LightRails && !UserPreferences.Busses)
        {
            ErrorMessageOptions.SetActive(false);
            ErrorMessageOptions2.SetActive(true);
        }
        else
        {
            ErrorMessageOptions.SetActive(false);
            ErrorMessageOptions2.SetActive(false);
            OptionsScreen.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
    
    public void Close()
    {
        MainMenu.SetActive(false);
        QuitScreen.SetActive(true);
    }    public void Quit_yes()
    {
        Application.Quit();
    }    public void Quit_no()
    {
        QuitScreen.SetActive(false);
        MainMenu.SetActive(true);
    }

        public void Return()
    {   OptionsScreen.SetActive(false);
        SimulationSource.SetActive(false);
        MainMenu.SetActive(true);
        ErrorMessage.SetActive(false);
    }
}
