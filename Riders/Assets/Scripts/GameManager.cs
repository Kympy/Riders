using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float timer = 0f;
    private bool start = false;

    private TextMeshProUGUI StartTimer;
    private void Awake()
    {
        StartTimer = GameObject.Find("StartTimer").GetComponent<TextMeshProUGUI>();
        StartTimer.text = 3.ToString();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(!start)
        {
            if (timer < 1.0f)
            {
                StartTimer.text = 2.ToString();
            }
            else if (timer < 2.0f)
            {
                StartTimer.text = 1.ToString();
            }
            else if (timer < 3.0f)
            {
                StartTimer.text = "GO";
            }
            else if(timer < 3.1f)
            {
                StartTimer.text = "";
                start = true;
                timer = 0f;
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartTimer.text = timer.ToString("F3") + "s";
        }
    }
}
