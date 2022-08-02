using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject LookTarget;
    private float smoothSpeed =10f;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Vector3 offset = new Vector3(0f, 3f, 0f);

    private void Start()
    {
        LookTarget = GameObject.FindGameObjectWithTag("Player").gameObject;
    }
    private void FixedUpdate()
    {
        desiredPosition = LookTarget.transform.TransformPoint(0f, 2f, -5f);
        desiredRotation = Quaternion.LookRotation(LookTarget.transform.position + offset - transform.position);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}
