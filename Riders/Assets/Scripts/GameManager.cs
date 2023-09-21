using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : SingleTon<GameManager>
{
    #region 게임 플레이 시간 관련
    private float timer = 0f; // Normal Timer
    private WaitForSeconds OneSecond = new WaitForSeconds(1.0f); // 1 sec
    private int threeTime = 3;
    private int min = 0;
    private int sec = 0;
    private int ms = 0;
    #endregion

    #region 게임 플레이 관련
    public bool IsGaming { get; private set; } = false; // Use To Control Key Input Update

    public bool IsFirstLap = false; // Use to when finish lap
    public bool IsFinishLap = false; // Use to when finish lap
    // ===========ID Info ========== //
    public int CurrentSceneIndex = 0; // Get Current Scene Build Index
    [SerializeField]
    private int SelectedCarID = 1; // My car index
    public int MyCarID { get { return SelectedCarID; } set { SelectedCarID = value; } }
    [SerializeField]
    private int SelectedMapID = 0; // My map Index
    public int MyMapID { get { return SelectedMapID; } set { SelectedMapID = value; } }
    // The Record Management Script
    private RecordManager record = new RecordManager();
    
    [SerializeField]
    private Car player = null; // Current Player Script
    private GameObject PlayerPrefab = null; // Selected Player Prefab
    private GameObject StartPosition = null; // Player Start Position
    #endregion

    #region UI 관련
    // ========== Play Info ========== //
    private TextMeshProUGUI StartTimer; // 3 2 1 Go Timer Text
    private TextMeshProUGUI LapTimer; // Count Lap Time
    #endregion

    public void InitAwake()
    {
        InitDPI(); // Window Size
        SceneManager.activeSceneChanged -= WhenSceneChanged;
        SceneManager.activeSceneChanged += WhenSceneChanged; // Scene Change Event Called Once
    }

    #region 씬 변경 시
    private void WhenSceneChanged(Scene previous, Scene now) // When Scene Changed, Called only once.
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Get current scene build index
        Debug.Log("SceneChanged! : " + CurrentSceneIndex);
        IsGaming = false; // Init Values to NULL
        player = null; // Init Values to NULL
        switch (CurrentSceneIndex) // Loaded Scene Initialize (Buttons, UI, Selected Car Prefab)
        {
            case 0:
                {
                    ButtonManager.Instance.InitMainSceneButton(); // Button Initialize
                    Debug.Log("Main Scene Loaded");
                    break;
                }
            case 1: // Car Select Scene
                {
                    ButtonManager.Instance.InitCarSelectButton();  // Button Initialize
                    break;
                }
            case 2: // Straight Course Scene
                {
                    ButtonManager.Instance.InitGameSceneButton(); // Button Initialize
                    InstantiatePrefabAndPosition(); // Create Player and Set Position
                    InitStraightScene(); // Init Scene
                    break;
                }
            case 3: // Dirt Course Scene
                {
                    ButtonManager.Instance.InitGameSceneButton(); // Button Initialize
                    InstantiatePrefabAndPosition();
                    InitDirtScene();
                    break;
                }
            case 4: // Record Scene
                {
                    ButtonManager.Instance.InitRecordSceneButton(); // Button Initialize
                    record.ShowRecord();
                    break;
                }
            case 5: // Setting Scene
                {
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
        StartCoroutine(CountDown()); // 3 2 1 GO
        StartCoroutine(Zero100Timer()); // start calculate zero 100
    }
    private void InitDirtScene() // Initialize Dirt Scene Game Timer
    {  // Find UI
        StartTimer = GameObject.Find("StartTimer").GetComponent<TextMeshProUGUI>();
        StartTimer.text = "";
        // Find UI
        ms = 0; sec = 0; min = 0;
        LapTimer = GameObject.Find("LapTimer").GetComponent<TextMeshProUGUI>();
        LapTimer.text = "LAP : " + min.ToString("00") + ":" + sec.ToString("00") + ":" + ms.ToString("00");
        // Find Player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
        timer = 0f; // Init Value
        IsFirstLap = false; IsFinishLap = false;
        StartCoroutine(CountDown()); // 3 2 1
    }
    #endregion

    #region 게임 플레이 타이머 코루틴
    private IEnumerator CountDown() // 3, 2, 1, Go Timer
    {
        while(true)
        {
            yield return OneSecond; // wait One Second
            if (threeTime == 0)
            {
                StartTimer.text = "GO";
                IsGaming = true; // Enable Key Input
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
            if (player != null && player.GetRigidbody.velocity.magnitude * 3.6f >= 100.0f) // Stop When Player velocity over 100km/h
            {
                StartTimer.text = (timer - 4f).ToString("F3") + "s"; // Display Time - 4 because of the CountDown Timer
                StopCoroutine(Zero100Timer());
                break;
            }
        }
    }
    public IEnumerator LapCycleTimer()
    {
        while(true)
        {
            if (IsFinishLap) // When Finish Lap
            {
                StopCoroutine(LapCycleTimer()); // Stop Lap Time Coroutine
                record.RecordCount++; // And Plus 1 Record Count
                record.DirtRecord.Add(record.RecordCount + "  " + LapTimer.text + "  / " + player.gameObject.name); // Save Record at RecordManager Class
                break; // Break While
            }
            yield return null;
            timer += Time.deltaTime;
            ms = Mathf.FloorToInt((timer - Mathf.FloorToInt(timer)) * 100); // ex) 3.546321 >> 0.546321 >> 54.6321 >> 54
            sec = Mathf.FloorToInt(timer); // second
            if(sec == 60)
            {
                sec = 0;
                min++; // Miniute
                timer = 0f; // Reset timer
            }
            LapTimer.text = "LAP : " + min.ToString("00") + ":" + sec.ToString("00") + ":" + ms.ToString("00"); // Display Lap Time on UI
        }
    }
    #endregion

    private void InstantiatePrefabAndPosition() // Create Player Object at Start Position
    {
        StartPosition = GameObject.Find("StartPosition").gameObject;
        switch (MyCarID) // Get My Model ID
        {
            case 0: // Audi A3
                {
                    PlayerPrefab = Resources.Load("Prefabs/Audi A3") as GameObject;
                    break;
                }
            case 1: // Porsche 911
                {
                    PlayerPrefab = Resources.Load("Prefabs/Porsche 911 Carrera") as GameObject;
                    break;
                }
            case 2:
                {
                    PlayerPrefab = Resources.Load("Prefabs/Camaro RS") as GameObject;
                    break;
                }
            default: break;
        }
        if (PlayerPrefab != null && GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(PlayerPrefab, StartPosition.transform.position, StartPosition.transform.rotation);
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
            player.gameObject.name = player.gameObject.name.Remove(player.gameObject.name.LastIndexOf("("));
            return;
        }
        else if (PlayerPrefab == null) Debug.LogError("Player Prefab is Null!!");
        else if (GameObject.FindGameObjectWithTag("Player") != null) Debug.Log("Player is already created.");
    }
    public void InitDPI() // Set the window size
    {
        switch(GameSetting.Instance.CurrentDPI) // Get from Game Setting Class
        {
            case 0: // 1920 1080
                {
                    Screen.SetResolution(1920, 1080, GameSetting.Instance.IsFull); // FHD
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1600, 900, GameSetting.Instance.IsFull); // HD+
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(1280, 720, GameSetting.Instance.IsFull); // HD
                    break;
                }
            default:
                {
                    Screen.SetResolution(1920, 1080, GameSetting.Instance.IsFull); // Default Value
                    break;
                }
        }
    }
}
