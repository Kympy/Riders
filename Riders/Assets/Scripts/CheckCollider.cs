using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // When Player Enter Start Timer Trigger
    {
        if(other.gameObject.tag == "Player") // Is Player?
        {
            switch(GameManager.Instance.MySceneIndex) // Which Map?
            {
                case 2: // Straight Scene
                    {
                        
                        break;
                    }
                case 3: // Dirt Scene
                    {
                        if (GameManager.Instance.isFirstLap == false) // Start Lap
                        {
                            StartCoroutine(GameManager.Instance.LapCycleTimer());
                            GameManager.Instance.isFirstLap = true;
                            Debug.Log("LAP START!!");
                        }
                        else // Finish Lap
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
