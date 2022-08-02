using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonControl : MonoBehaviour
{
    public void StraightSceneLoad()
    {
        SceneManager.LoadScene(1);
    }
    public void DirtSceneLoad()
    {
        SceneManager.LoadScene(2);
    }
    public void RecordSceneLoad()
    {
        SceneManager.LoadScene(3);
    }
    public void SettingSceneLoad()
    {
        SceneManager.LoadScene(4);
    }
    public void GoToMainScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
