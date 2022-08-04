using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : SingleTon<GameManager>
{
    // ========= Timer ============ //
    private float timer = 0f; // Normal Timer
    private WaitForSeconds OneSecond = new WaitForSeconds(1.0f); // 1 sec
    private int threeTime = 3;
    // ========= Game Control Variable ===== //
    private bool isGaming = false; // Use To Control Key Input Update
    // ========== Play Info ========== //

    private TextMeshProUGUI StartTimer; // 3 2 1 Go Timer Text
    private TextMeshProUGUI LapTimer; // Count Lap Time
    private Car player; // Current Player Script
    private GameObject PlayerPrefab = null;
    private GameObject StartPosition = null;

    // ===========ID Info ========== //
    private int CurrentSceneIndex = 0; // Get Current Scene Build Index
    public int MySceneIndex { get { return CurrentSceneIndex; } set { CurrentSceneIndex = value; } }

    private int SelectedCarID = 1; // My car index
    public int MyCarID { get { return SelectedCarID; } set { SelectedCarID = value; } }

    private int SelectedMapID = 0; // My map Index
    public int MyMapID { get { return SelectedMapID; } set { SelectedMapID = value; } }

    private void Awake()
    {
        DontDestroyOnLoad(this); // Keep GameManager On Load
        SceneManager.activeSceneChanged -= WhenSceneChanged;
        SceneManager.activeSceneChanged += WhenSceneChanged; // Scene Change Event
    }
    private void Update()
    {
        if (isGaming) // Update Input Only In Game Scene
        {
            InputManager.Instance.OnUpdate(); // Input Update
        }
    }
    private void WhenSceneChanged(Scene previous, Scene now) // When Scene Changed, Called only once.
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Get current scene build index
        Debug.Log("SceneChanged! : " + CurrentSceneIndex);
        switch (CurrentSceneIndex) // Loaded Scene Initialize (Buttons, UI, Selected Car Prefab)
        {
            case 0:
                {
                    isGaming = false;
                    ButtonManager.Instance.InitMainSceneButton(); // Button Initialize
                    Debug.Log("Main Scene Loaded");
                    break;
                }
            case 1: // Car Select Scene
                {
                    isGaming = false;
                    ButtonManager.Instance.InitCarSelectButton();  // Button Initialize
                    break;
                }
            case 2: // Straight Course Scene
                {
                    isGaming = false;
                    ButtonManager.Instance.InitGameSceneButton(); // Button Initialize
                    InstantiatePrefabAndPosition();
                    InitStraightScene();
                    break;
                }
            case 3: // Dirt Course Scene
                {
                    isGaming = false;
                    ButtonManager.Instance.InitGameSceneButton(); // Button Initialize
                    InstantiatePrefabAndPosition();
                    InitDirtScene();
                    break;
                }
            case 4: // Record Scene
                {
                    isGaming = false;
                    ButtonManager.Instance.InitRecordSceneButton(); // Button Initialize
                    break;
                }
            case 5: // Setting Scene
                {
                    isGaming = false;
                    ButtonManager.Instance.InitSettingSceneButton();
                    break;
                }
            default: { break; }
        }
    }
    private void InitStraightScene() // Initialize Straight Scene Game TImer
    {
        StartTimer = GameObject.Find("StartTimer").GetComponent<TextMeshProUGUI>();
        StartTimer.text = "";
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
        timer = 0f;
        StartCoroutine(CountDown());
        StartCoroutine(Zero100Timer());
    }
    private void InitDirtScene() // Initialize Dirt Scene Game Timer
    {
        StartTimer = GameObject.Find("StartTimer").GetComponent<TextMeshProUGUI>();
        StartTimer.text = "";

        LapTimer = GameObject.Find("LapTimer").GetComponent<TextMeshProUGUI>();
        LapTimer.text = "";

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
        timer = 0f;
        StartCoroutine(CountDown());
    }
    private IEnumerator CountDown() // 3, 2, 1, Go Timer
    {
        while(true)
        {
            yield return OneSecond; // wait One Second
            if (threeTime == 0)
            {
                StartTimer.text = "GO";
                isGaming = true; // Enable Key Input
                threeTime--;
            }
            else if(threeTime < 0)
            {
                StartTimer.text = ""; // Erase "GO"
                threeTime = 3; // Reset
                StopCoroutine(CountDown()); // Total 4 Second
                break;
            }
            else
            {
                StartTimer.text = threeTime.ToString();
                threeTime--;
            }
        }
    }
    private IEnumerator Zero100Timer() // Used by Straight Scene
    {
        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (player.GetRigidbody.velocity.magnitude * 3.6f >= 100.0f) // Stop When Player velocity over 100km/h
            {
                StartTimer.text = (timer - 4f).ToString("F3") + "s"; // Display Time - 4 because of the CountDown Timer
                StopCoroutine(Zero100Timer());
                break;
            }
        }
    }
    private IEnumerator LapCycleTimer()
    {
        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            
        }
    }
    private void InstantiatePrefabAndPosition()
    {
        StartPosition = GameObject.Find("StartPosition").gameObject;
        switch (MyCarID)
        {
            case 0: // Audi A3
                {
                    PlayerPrefab = Resources.Load("Prefabs/AudiA3") as GameObject;
                    break;
                }
            case 1: // Porsche 911
                {
                    PlayerPrefab = Resources.Load("Prefabs/Porsche911") as GameObject;
                    break;
                }
            case 2:
                {
                    PlayerPrefab = Resources.Load("Prefabs/Camaro") as GameObject;
                    break;
                }
            default: break;

        }
        if (PlayerPrefab != null)
        {
            Instantiate(PlayerPrefab, StartPosition.transform.position, StartPosition.transform.rotation);
        }
        else Debug.LogError("Player Prefab is Null!!");
    }
}
