using UnityEngine;
using UnityEngine.SceneManagement;

/// Этот класс управляет пользовательским интерфейсом в симуляции.

public class IngameMenu : MonoBehaviour
{
    public GameObject Buildings;
    public Material DarkSkyBox;
    public Material LightSkyBox;
    public GameObject Sun;

    public GameObject ShowBuildingsButton;
    public GameObject HideBuildingsButton;
    public GameObject ShowLegendButton;
    public GameObject HideLegendButton;
    public GameObject LightModeButton;
    public GameObject DarkModeButton;
    public GameObject ShowMenuButton;

    public GameObject MenuWindow;
    public GameObject LegendBar;
    public GameObject NoBuildingsMessage;

    public static bool MenuHidden = false;
    public static bool DarkModeOn = false;


    public void HideUI()
    {
        MenuWindow.SetActive(false);
        ShowMenuButton.SetActive(true);
        MenuHidden = true;
    }

    public void ShowUI()
    {
        ShowMenuButton.SetActive(false);
        MenuWindow.SetActive(true);
        MenuHidden = false;
    }


    public void LightMode()
    {
        DarkModeButton.SetActive(true);
        LightModeButton.SetActive(false);
        RenderSettings.skybox = LightSkyBox;
        Sun.SetActive(true);
        DarkModeOn = false;
    }

    public void DarkMode()
    {
        LightModeButton.SetActive(true);
        DarkModeButton.SetActive(false);
        RenderSettings.skybox = DarkSkyBox;
        Sun.SetActive(false);
        DarkModeOn = true;
    }


    public void ShowLegend()
    {
        ShowLegendButton.gameObject.SetActive(false);
        HideLegendButton.SetActive(true);
        LegendBar.SetActive(true);
    }


    public void HideLegend()
    {
        HideLegendButton.gameObject.SetActive(false);
        ShowLegendButton.SetActive(true);
        LegendBar.SetActive(false);
    }


    public void ShowBuildings()
    {
        ShowBuildingsButton.SetActive(false);
        HideBuildingsButton.SetActive(true);
        Buildings.SetActive(true);
    }


    public void HideBuildings()
    {
        if (UserPreferences.Buildings)
        {
            HideBuildingsButton.SetActive(false);
            ShowBuildingsButton.SetActive(true);
            Buildings = GameObject.Find("3D-Buildings(Clone)");
            Buildings.SetActive(false);
        }
        else
        {
            NoBuildingsMessage.SetActive(true);
            MenuWindow.SetActive(false);
        }
    }
    
    public void OKButton()
    {
        NoBuildingsMessage.SetActive(false);
        MenuWindow.SetActive(true);
    }

    public void ReturnToMenu()
    {
        FileLoader.simulator_loaded = false;
        SceneManager.LoadScene(0);
        UserPreferences.PublicTransportRailways = true;
        UserPreferences.PublicTransportStreets = true;
        UserPreferences.AllStreets = true;
        UserPreferences.Stations = true;
        UserPreferences.Buildings = true;
        UserPreferences.Subways = true;
        UserPreferences.Trams = true;
        UserPreferences.Trains = true;
        UserPreferences.Railways = true;
        UserPreferences.LightRails = true;
    }
}
