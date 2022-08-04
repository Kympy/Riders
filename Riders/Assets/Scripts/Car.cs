using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Car : MonoBehaviour // Normal Base Car Class
{
    protected LogitechGSDK.LogiControllerPropertiesData properties; // Logitech Controller
    protected LogitechGSDK.DIJOYSTATE2ENGINES controller;
    protected class WheelInfo // Left and Right Wheels
    {
        public WheelCollider Left_Wheel;
        public WheelCollider Right_Wheel;
    }
    protected class SkidMark // Left and Right Skid Marks
    {
        public TrailRenderer Left_Skid;
        public TrailRenderer Right_Skid;
    }
    protected float Steering = 0f; // Handle Angle
    protected float Motor = 0f; // Motor Power
    protected float Brake = 0f; // Brake Power
    protected bool FrontGear = false;
    protected bool BackGear = false;
    
    protected int MaxWheelAngle = 45; // Wheels can rotate between maximum value
    protected float MaxMotorPower = 1901f; // It means motorTorque
    protected float MaxBrakePower = 3000f; // Brake Maximum power

    protected int MaxHandleAngle = 450; // G29 Real Controller Max Angle (One Side)
    protected float Int2HandleAngle; // Int => Handle Angle
    protected float Handle2WheelAngle; // Handle Angle => Wheel Angle
    protected float Int2Throttle; // Int => Throttle Pedal value
    protected float Int2Brake; // Int => Brake pedal value

    protected float myVeloctiy = 0f; // current speed
    protected Vector3 colliderWorldPos; // Wheel collider position
    protected Quaternion colliderWorldRot; // Wheel collider rotation
    protected float MaxVelocity = 210f; // Max speed km/s

    protected float speedFactor = 0f; // current speed / max speed => percentage
    protected float rotationAngle = 0f; // speedometer arrow pointer angle
    protected Image arrowPointer; // GUI Arrow
    protected TextMeshProUGUI speedUI; // Display Velocity GUI

    protected Rigidbody rigidBody; // Car rigidbody
    public Rigidbody GetRigidbody { get { return rigidBody; } }
    protected GameObject centerOfMass; // Car center of mass

    protected List<WheelInfo> Wheels = new List<WheelInfo>(); // Wheels List
    protected List<SkidMark> Skids = new List<SkidMark>(); // Skid Marks List
    protected GameObject brakeLight = null; // Back light object
    protected GameObject visualWheel = null; // visual wheels

    protected virtual void Init()
    {
        MaxVelocity = 210f;
        MaxWheelAngle = 45;
        MaxMotorPower = 1901f; // It means motorTorque
        MaxBrakePower = 3000f; // Brake
    } // Init State Value
    protected virtual void InitKey() 
    {
        // Init KeyBoard
        //InputManager.Instance.KeyAction -= KeyBoardControl;
        //InputManager.Instance.KeyAction += KeyBoardControl;
        // Init Controller
        InputManager.Instance.KeyAction -= G29Control;
        InputManager.Instance.KeyAction += G29Control;
    }// Initialize Control Method
    protected virtual void InitGUI()
    {
        speedUI = GameObject.Find("Speed").GetComponent<TextMeshProUGUI>();
        arrowPointer = GameObject.Find("Arrow").GetComponent<Image>();
    } //Init GUI
    protected virtual void RigidBodySetUp()
    {
        rigidBody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.FindGameObjectWithTag("CM").gameObject;
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    } // Get Rigidbody and center of mass
    protected virtual void InitWheel()
    {
        LogitechGSDK.LogiSteeringInitialize(false);

        WheelInfo Front = new WheelInfo();
        WheelInfo Back = new WheelInfo();

        Wheels.Add(Front);
        Wheels.Add(Back);

        // Foward Wheels
        Wheels[0].Left_Wheel = GameObject.FindGameObjectWithTag("LFW").GetComponent<WheelCollider>();
        Wheels[0].Right_Wheel = GameObject.FindGameObjectWithTag("RFW").GetComponent<WheelCollider>();
        // Backward Wheels
        Wheels[1].Left_Wheel = GameObject.FindGameObjectWithTag("LBW").GetComponent<WheelCollider>();
        Wheels[1].Right_Wheel = GameObject.FindGameObjectWithTag("RBW").GetComponent<WheelCollider>();
    } // Initialize 4 Wheels
    protected virtual void InitFFSkidMarks()
    {
        SkidMark FrontSkid = new SkidMark();
        Skids.Add(FrontSkid);
        // Foward Wheels Skid Marks
        Skids[0].Left_Skid = GameObject.FindGameObjectWithTag("LFW").GetComponentInChildren<TrailRenderer>(); // Left
        Skids[0].Right_Skid = GameObject.FindGameObjectWithTag("RFW").GetComponentInChildren<TrailRenderer>(); // Right
        Skids[0].Left_Skid.emitting = false; // Visible false on start
        Skids[0].Right_Skid.emitting = false;
    } // Initialize FF Type Skid marks
    protected virtual void InitRRSkidMarks()
    {
        SkidMark BackSkid = new SkidMark();
        Skids.Add(BackSkid);

        // Foward Wheels Skid Marks
        Skids[0].Left_Skid = GameObject.FindGameObjectWithTag("LBW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Right_Skid = GameObject.FindGameObjectWithTag("RBW").GetComponentInChildren<TrailRenderer>();
        Skids[0].Left_Skid.emitting = false;
        Skids[0].Right_Skid.emitting = false;
    }// Initialize RR Type Skid marks
    protected virtual void InitBrakeLight()
    {
        // Brake BackLight
        brakeLight = GameObject.FindGameObjectWithTag("BackLight");
        brakeLight.SetActive(false);
    } // Initialize Brake Light
    protected virtual void InitConstValue()
    {
        Int2HandleAngle = 32767f / MaxHandleAngle; // 32767 / 450 >> Convert Int to Handle Degree
        Handle2WheelAngle = MaxHandleAngle / MaxWheelAngle; // 450 / 45 >> Convert Handle Degree to Wheel Degree
        Int2Throttle = 65534 / MaxMotorPower; // Convert Int to Throttle pedal value
        Int2Brake = 65534 / MaxBrakePower; // Convert Int to Brake pedal value
    } // Initalize some values used on G29 Wheel
    protected virtual void MoveVisualWheel(WheelCollider wheel)
    {
        wheel.GetWorldPose(out colliderWorldPos, out colliderWorldRot);
        visualWheel = wheel.transform.GetChild(0).gameObject;
        visualWheel.transform.position = colliderWorldPos;
        visualWheel.transform.rotation = colliderWorldRot;
    } // Move Visual Real Wheel
    protected virtual void KeyBoardControl()
    {
        Steering = Input.GetAxis("Horizontal") * MaxWheelAngle;
        Motor = Input.GetAxis("Vertical") * MaxMotorPower;

        if (Input.GetKey(KeyCode.Space))
        {
            Brake = MaxBrakePower;
            Motor = 0f;
            Skids[0].Left_Skid.emitting = true;
            Skids[0].Right_Skid.emitting = true;
            brakeLight.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.S)) brakeLight.SetActive(true);
        else
        {
            Brake = 0f;
            Skids[0].Left_Skid.emitting = false;
            Skids[0].Right_Skid.emitting = false;
            brakeLight.SetActive(false);
        }

    } // Keyboard Input
    protected virtual void G29Control()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            controller = LogitechGSDK.LogiGetStateUnity(0); // Logitech G 29 Wheel

            LogitechGSDK.LogiPlaySpringForce(0, 0, 40, 30); // ForceFeedback Setting

            if (FrontGear)
            {
                Motor = Mathf.Round((-controller.lY + Mathf.Abs(controller.lY)) / Int2Throttle); // Throttle
            }
            else if (BackGear)
            {
                Motor = -Mathf.Round((-controller.lY + Mathf.Abs(controller.lY)) / Int2Throttle); // Throttle
            }
            else Motor = 0f;

            Steering = controller.lX / Int2HandleAngle / Handle2WheelAngle; // Handle
            Brake = Mathf.Round((-controller.lRz + Mathf.Abs(controller.lRz)) / Int2Brake); // Brake

            for (int i = 0; i < 128; i++) // Gear Button Input
            {
                if (controller.rgbButtons[i] == 128)
                {
                    if (i == 12) // Forward gear
                    {
                        FrontGear = true;
                        BackGear = false;
                        Debug.Log("1 st Gear Input");
                    }
                    else if (i == 18) // Back gear
                    {
                        FrontGear = false;
                        BackGear = true;
                        Debug.Log("Backward Gear Input");
                    }
                }
            }

            if (controller.rgbButtons[12] != 128 && controller.rgbButtons[18] != 128) // Gear N
            {
                FrontGear = false;
                BackGear = false;
            }

            if (Brake > 0.1f) // If Brake ON
            {
                brakeLight.SetActive(true); // BackLight ON
            }
            else brakeLight.SetActive(false);
        }
        else if (!LogitechGSDK.LogiIsConnected(0))
        {
            Debug.Log("LOGITECH DEVICE NOT CONNECTED");
        }
        else
        {
            Debug.Log("Device Connected, but some errors occured");
        }
    } // G29 Input
    protected virtual void FFModeMovement()
    {
        if (rigidBody.velocity.magnitude * 3.6f > MaxVelocity)
        {
            Debug.Log("max");
            Motor = 0;
        }
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
    } // FF Car Setting
    protected virtual void RRModeMovement()
    {
        if (rigidBody.velocity.magnitude * 3.6f > MaxVelocity)
        {
            Debug.Log("max");
            Motor = 0;
        }
        // ========== RR ========= //
        // Steer
        Wheels[0].Left_Wheel.steerAngle = Steering;
        Wheels[0].Right_Wheel.steerAngle = Steering;
        // Motor
        Wheels[1].Left_Wheel.motorTorque = Motor;
        Wheels[1].Right_Wheel.motorTorque = Motor;
        // Brake
        Wheels[1].Left_Wheel.brakeTorque = Brake;
        Wheels[1].Right_Wheel.brakeTorque = Brake;
    }// RR Car Setting
    protected virtual void GUIUpdate()
    {
        myVeloctiy = rigidBody.velocity.magnitude * 3.6f; // Convert m/s -> km/s with multiply 3.6
        speedUI.text = myVeloctiy.ToString("000"); // Truncate
        speedFactor = Mathf.Abs(rigidBody.velocity.magnitude * 3.6f / MaxVelocity);
        rotationAngle = Mathf.Lerp(0, 315, speedFactor);

        arrowPointer.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -rotationAngle));
        Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake + "  RPM : " + Wheels[0].Left_Wheel.rpm + "  Velocity : " + rigidBody.velocity.magnitude * 3.6f);
    } // Speedometer Update
    private void OnDestroy()
    {
        if(LogitechGSDK.LogiIsConnected(0))
        {
            Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
        }
    }
}
