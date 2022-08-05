using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            switch(GameManager.Instance.MySceneIndex)
            {
                case 2: // Straight Scene
                    {
                        
                        break;
                    }
                case 3: // Dirt Scene
                    {
                        if (GameManager.Instance.isFirstLap == false)
                        {
                            StartCoroutine(GameManager.Instance.LapCycleTimer());
                            GameManager.Instance.isFirstLap = true;
                            Debug.Log("LAP START!!");
                        }
                        else
                        {
                            GameManager.Instance.isFinishLap = true;
                            Debug.Log("LAP FINISH!!");
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
