using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject LookTarget;
    private Camera car_Camera;
    private void Awake()
    {
        car_Camera = GetComponent<Camera>();
    }
    private void Update()
    {
        car_Camera.transform.LookAt(LookTarget.transform);
        car_Camera.transform.position = LookTarget.transform.position + new Vector3(0f, 4f, -7f);
        //car_Camera.transform.Rotate()
    }
}
