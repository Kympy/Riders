using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // When Player Enter Start Timer Trigger
    {
        if(other.gameObject.tag == "Player") // Is Player?
        {
            switch(GameManager.Instance.CurrentSceneIndex) // Which Map?
            {
                case 2: // Straight Scene
                    {
                        
                        break;
                    }
                case 3: // Dirt Scene
                    {
                        if (GameManager.Instance.IsFirstLap == false) // Start Lap
                        {
                            StartCoroutine(GameManager.Instance.LapCycleTimer());
                            GameManager.Instance.IsFirstLap = true;
                            Debug.Log("LAP START!!");
                        }
                        else // Finish Lap
                        {
                            GameManager.Instance.IsFinishLap = true;
                            Debug.Log("LAP FINISH!!");
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
