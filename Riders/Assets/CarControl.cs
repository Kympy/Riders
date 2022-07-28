using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public class WheelInfo
    {
        public WheelCollider Left_Wheel;
        public WheelCollider Right_Wheel;
    }
    private List<WheelInfo> Wheels = new List<WheelInfo>();

    private float Steering;
    private float Motor;

    public float MaxSteeringAngle = 45;
    public float MaxMotorPower = 300;

    private Vector3 colliderWorldPos;
    private Quaternion colliderWorldRot;

    private GameObject visualWheel = null;

    private void Awake()
    {
        WheelInfo Front = new WheelInfo();
        WheelInfo Back = new WheelInfo();
        Wheels.Add(Front);
        Wheels.Add(Back);
    }
    private void Start()
    {
        Wheels[0].Left_Wheel = GameObject.FindGameObjectWithTag("LFW").GetComponent<WheelCollider>();
        Wheels[0].Right_Wheel = GameObject.FindGameObjectWithTag("RFW").GetComponent<WheelCollider>();

        Wheels[1].Left_Wheel = GameObject.FindGameObjectWithTag("LBW").GetComponent<WheelCollider>();
        Wheels[1].Right_Wheel = GameObject.FindGameObjectWithTag("RBW").GetComponent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        Steering = Input.GetAxis("Horizontal") * MaxSteeringAngle;
        Motor = Input.GetAxis("Vertical") * MaxMotorPower;

        if(Input.GetKey(KeyCode.Space))
        {
            Wheels[1].Left_Wheel.motorTorque = 0;
            Wheels[1].Right_Wheel.motorTorque = 0;
        }

        Debug.Log("Steering : " + Steering + "  Motor : " + Motor);

        Wheels[0].Left_Wheel.steerAngle = Steering;
        Wheels[0].Right_Wheel.steerAngle = Steering;

        Wheels[1].Left_Wheel.motorTorque = Motor;
        Wheels[1].Right_Wheel.motorTorque = Motor;

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);
    }
    private void MoveVisualWheel(WheelCollider wheel)
    {
        wheel.GetWorldPose(out colliderWorldPos, out colliderWorldRot);
        visualWheel = wheel.transform.GetChild(0).gameObject;
        visualWheel.transform.position = colliderWorldPos;
        visualWheel.transform.rotation = colliderWorldRot;
    }
}
