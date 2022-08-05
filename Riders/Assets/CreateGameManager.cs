using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGameManager : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.FindObjectOfType(typeof(GameManager)) == null)
        {
            GameObject obj = new GameObject(typeof(GameManager).ToString(), typeof(GameManager));
            obj.GetComponent<GameManager>().InitAwake();
        }
        else return;
    }
}
