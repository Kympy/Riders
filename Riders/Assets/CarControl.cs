using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CarControl : MonoBehaviour
{
    public class WheelInfo
    {
        public WheelCollider Left_Wheel;
        public WheelCollider Right_Wheel;
    }
    public class SkidMark
    {
        public TrailRenderer Left_Skid;
        public TrailRenderer Right_Skid;
    }
    private float Steering;
    private float Motor;

    public float MaxSteeringAngle = 30;
    public float MaxMotorPower = 1000;
    public float brakePower = 0;

    private Vector3 colliderWorldPos;
    private Quaternion colliderWorldRot;

    private GameObject visualWheel = null;

    private Rigidbody rigidBody;
    private GameObject centerOfMass;

    private List<WheelInfo> Wheels = new List<WheelInfo>();
    private List<SkidMark> Skids = new List<SkidMark>();
    
    private GameObject brakeLight = null;

    private Vector3 lastPosition;
    private TextMeshProUGUI speedUI;
    private void Awake()
    {
        WheelInfo Front = new WheelInfo();
        WheelInfo Back = new WheelInfo();
        
        Wheels.Add(Front);
        Wheels.Add(Back);

        SkidMark FrontSkid = new SkidMark();
        Skids.Add(FrontSkid);
        speedUI = GameObject.Find("Speed").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.FindGameObjectWithTag("CM").gameObject;
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;

        Wheels[0].Left_Wheel = GameObject.FindGameObjectWithTag("LFW").GetComponent<WheelCollider>();
        Wheels[0].Right_Wheel = GameObject.FindGameObjectWithTag("RFW").GetComponent<WheelCollider>();

        Wheels[1].Left_Wheel = GameObject.FindGameObjectWithTag("LBW").GetComponent<WheelCollider>();
        Wheels[1].Right_Wheel = GameObject.FindGameObjectWithTag("RBW").GetComponent<WheelCollider>();

        Skids[0].Left_Skid = GameObject.FindGameObjectWithTag("LFW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Right_Skid = GameObject.FindGameObjectWithTag("RFW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Left_Skid.emitting = false;
        Skids[0].Right_Skid.emitting = false;

        brakeLight = GameObject.FindGameObjectWithTag("BackLight");
        brakeLight.SetActive(false);
    }

    private void FixedUpdate()
    {
        Steering = Input.GetAxis("Horizontal") * MaxSteeringAngle;
        Motor = Input.GetAxis("Vertical") * MaxMotorPower;
        var s = (transform.position - lastPosition).magnitude / Time.deltaTime * 3.6f + " km/h";
        speedUI.text = s;
        lastPosition = transform.position;
        if (Input.GetKey(KeyCode.Space))
        {
            brakePower = 4000f;
            Motor = 0f;
            Skids[0].Left_Skid.emitting = true;
            Skids[0].Right_Skid.emitting = true;
            brakeLight.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.S)) brakeLight.SetActive(true);
        else
        {
            brakePower = 0f;
            Skids[0].Left_Skid.emitting = false;
            Skids[0].Right_Skid.emitting = false;
            brakeLight.SetActive(false);
        }


        //Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Wheels[0].Right_Wheel.motorTorque);
        // ========== 전륜 FF ========= //
        // 조향
        Wheels[0].Left_Wheel.steerAngle = Steering;
        Wheels[0].Right_Wheel.steerAngle = Steering;
        // 구동
        Wheels[0].Left_Wheel.motorTorque = Motor;
        Wheels[0].Right_Wheel.motorTorque = Motor;
        // 브레이크
        Wheels[0].Left_Wheel.brakeTorque = brakePower;
        Wheels[0].Right_Wheel.brakeTorque = brakePower;

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
