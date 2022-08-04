using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonManager : SingleTon<ButtonManager>
{
     /*
     *  -- Scene Build Index --
     *  0 : MainScene
     *  1 : SelectScene
     *  2 : StraightScene
     *  3 : DirtScene
     *  4 : Record
     *  5 : Setting
    
      -- Car Index --
     0 : Audi A3
     1 : Porsche 911 Carrera
     2 : Camaro RS
      
      */
    private Button mainButton = null;
    private Button exitButton = null;
    private Button recordButton = null;
    private Button settingButton = null;
    private Button carButton = null;
    private Button mapButton = null;
    // ================= Init Button =========== //
    private void InitMainButton()
    {
        mainButton = GameObject.Find("Main").GetComponent<Button>();
        mainButton.onClick.AddListener(() => Load(0));
    }
    private void InitExitButton()
    {
        exitButton = GameObject.Find("Exit").GetComponent<Button>();
        exitButton.onClick.AddListener(() => ExitGame());
    }
    private void InitMapButton() // Initialize Map Selection Buttons
    {   // First Map : Straight Course
        mapButton = GameObject.Find("Straight").GetComponent<Button>();
        mapButton.onClick.AddListener(() => SelectMap(2));
        mapButton.onClick.AddListener(() => Load(1));
        // Second Map : Dirt Course
        mapButton = GameObject.Find("Dirt").GetComponent<Button>();
        mapButton.onClick.AddListener(() => SelectMap(3));
        mapButton.onClick.AddListener(() => Load(1));
    }
    private void InitRecordButton()
    {
        recordButton = GameObject.Find("RecordBackGround").GetComponent<Button>();
        recordButton.onClick.AddListener(() => Load(4));
    }
    private void InitSettingButton()
    {
        settingButton = GameObject.Find("Setting").GetComponent<Button>();
        settingButton.onClick.AddListener(() => Load(5));
    }
    // ==================== Called at GameManager When Scene Changed ====== //
    public void InitMainSceneButton() // Initialize Main Scene's Buttons
    {
        InitMapButton(); // Map Button
        InitRecordButton(); // Record Button
        InitSettingButton(); // Setting Button
        InitExitButton(); // Exit Button
    }
    public void InitCarSelectButton() // Initialize Car Selection Buttons
    {// Audi Button
        carButton = GameObject.Find("AudiA3").GetComponent<Button>();
        carButton.onClick.AddListener(() => SelectCar(0)); // Save Car ID
        carButton.onClick.AddListener(() => Load(GameManager.Instance.MyMapID)); // Load Chosen Map
        // Porsche Button
        carButton = GameObject.Find("Porsche911Carrera").GetComponent<Button>();
        carButton.onClick.AddListener(() => SelectCar(1));
        carButton.onClick.AddListener(() => Load(GameManager.Instance.MyMapID));
        // Camaro Button
        carButton = GameObject.Find("CamaroRS").GetComponent<Button>();
        carButton.onClick.AddListener(() => SelectCar(2));
        carButton.onClick.AddListener(() => Load(GameManager.Instance.MyMapID));
        // Main Button
        mainButton = GameObject.Find("Main").GetComponent<Button>();
        mainButton.onClick.AddListener(() => Load(0));
    }
    public void InitRecordSceneButton() // Initialize Record Scene's Button
    {
        InitMainButton();
    }
    public void InitSettingSceneButton() // Initialize Setting Scene's Button
    {
        InitMainButton();
    }
    public void InitGameSceneButton() // Initialize Whole Game Scene's Button
    {

    }
    public void SelectCar(int carID) // When Car Selected, the car ID is saved at the GameManager
    {
        GameManager.Instance.MyCarID = carID;
        Debug.Log("Car : " +carID + " Selected");
    }
    public void SelectMap(int mapID) // When Map Selected, the map ID is saved
    {
        GameManager.Instance.MyMapID = mapID;
        Debug.Log("Map : " + mapID + " Selected");
    }
    public void Load(int SceneID) // Load the scene by build index
    {
        SceneManager.LoadScene(SceneID);
    }
    public void ExitGame() // Application Quit
    {
        Application.Quit();
    }
}
