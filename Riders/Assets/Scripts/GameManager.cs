using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float timer = 0f;
    private bool start = false;
    private bool zero100 = false;

    private TextMeshProUGUI StartTimer;
    private Car player;
    private void Awake()
    {
        StartTimer = GameObject.Find("StartTimer").GetComponent<TextMeshProUGUI>();
        StartTimer.text = 3.ToString();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
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
            else if(timer > 3.1f)
            {
                StartTimer.text = "";
                start = true;
                timer = 0f;
            }
        }
        if(zero100 == false)
        {
            if (player.GetRigidbody.velocity.magnitude * 3.6f >= 100f)
            {
                StartTimer.text = timer.ToString("F3") + "s";
                zero100 = true;
            }
        }

    }
}
