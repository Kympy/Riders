using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CarControl : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;
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
    private float Steering = 0;
    private float Motor = 0;
    private float Brake = 0;

    private int MaxWheelAngle = 45;
    private float MaxMotorPower = 1000f;
    private float MaxBrakePower = 3000f;


    private int MaxHandleAngle = 450; // G29 휠 한쪽 최대 각도
    private float Int2HandleAngle;
    private float Handle2WheelAngle;
    private float Int2Throttle; // Int => Throttle Pedal
    private float Int2Brake;

    private float myVeloctiy = 0f;
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

    LogitechGSDK.DIJOYSTATE2ENGINES controller;
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
        // Foward Wheels
        Wheels[0].Left_Wheel = GameObject.FindGameObjectWithTag("LFW").GetComponent<WheelCollider>();
        Wheels[0].Right_Wheel = GameObject.FindGameObjectWithTag("RFW").GetComponent<WheelCollider>();
        // Backward Wheels
        Wheels[1].Left_Wheel = GameObject.FindGameObjectWithTag("LBW").GetComponent<WheelCollider>();
        Wheels[1].Right_Wheel = GameObject.FindGameObjectWithTag("RBW").GetComponent<WheelCollider>();
        // Foward Wheels Skid Marks
        Skids[0].Left_Skid = GameObject.FindGameObjectWithTag("LFW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Right_Skid = GameObject.FindGameObjectWithTag("RFW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Left_Skid.emitting = false;
        Skids[0].Right_Skid.emitting = false;
        // Brake BackLight
        brakeLight = GameObject.FindGameObjectWithTag("BackLight");
        brakeLight.SetActive(false);

        Int2HandleAngle = 32767 / MaxHandleAngle; // 32767 / 450 >> Convert Int to Handle Degree
        Handle2WheelAngle = MaxHandleAngle / MaxWheelAngle; // 450 / 45 >> Convert Handle Degree to Wheel Degree
        Int2Throttle = 32767 / MaxMotorPower; // Convert Int to Throttle pedal value
        Int2Brake = 32767 / MaxBrakePower; // Convert Int to Brake pedal value
    }

    private void FixedUpdate()
    {
        controller = LogitechGSDK.LogiGetStateUnity(0); // Logitech G 29 Wheel

        LogitechGSDK.LogiPlaySpringForce(0, 0, 40, 30); // ForceFeedback Setting

        Steering = controller.lX / Int2HandleAngle / Handle2WheelAngle; // Handle
        Motor = Mathf.Round(-controller.lY / Int2Throttle + MaxMotorPower); // Throttle
        Brake = Mathf.Round(-controller.lRz / Int2Brake + MaxBrakePower); // Brake

        for (int i = 0; i < 128; i++) // Gear Button Input
        {
            if (controller.rgbButtons[i] == 128)
            {
                if(i == 12) // 1 gear
                {
                    Debug.Log("1 st Gear Input");
                }
                else if(i == 18) // Back gear
                {
                    Debug.Log("Backward Gear Input");
                    Motor = -Motor;
                }
            }
        }
        if(Brake > 1) // If Brake ON
        {
            brakeLight.SetActive(true); // BackLight ON
        }
        else brakeLight.SetActive(false);

        //Steering = Input.GetAxis("Horizontal") * MaxSteeringAngle;
        //Motor = Input.GetAxis("Vertical") * MaxMotorPower;

        /*
        if (Input.GetKey(KeyCode.Space))
        {
            brakePower = 3500f;
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
        */

        Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake);
        // ========== FF ========= //
        // Steer
        Wheels[0].Left_Wheel.steerAngle = Steering;
        Wheels[0].Right_Wheel.steerAngle = Steering;
        // Motor
        Wheels[0].Left_Wheel.motorTorque = Motor;
        Wheels[0].Right_Wheel.motorTorque = Motor;
        // Brake
        Wheels[0].Left_Wheel.brakeTorque = Brake;
        Wheels[0].Right_Wheel.brakeTorque = Brake;

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);

        myVeloctiy = (transform.position - lastPosition).magnitude / Time.deltaTime * 3.6f; // Convert m/s -> km/s with multiply 3.6
        speedUI.text = myVeloctiy.ToString("F0") + "KM/H"; // Truncate
        lastPosition = transform.position; // To Calculate Next Velocity
    }
    private void MoveVisualWheel(WheelCollider wheel) // Move Visual Real Wheel
    {
        wheel.GetWorldPose(out colliderWorldPos, out colliderWorldRot);
        visualWheel = wheel.transform.GetChild(0).gameObject;
        visualWheel.transform.position = colliderWorldPos;
        visualWheel.transform.rotation = colliderWorldRot;
    }
}
