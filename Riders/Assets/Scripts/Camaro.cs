using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camaro : Car // FR
{
    protected override void Init()
    {
        MaxVelocity = 288f;
        MaxWheelAngle = 45;
        MaxMotorPower = 4233f; // It means motorTorque
        MaxBrakePower = 6000f; // Brake
    }
    private void Awake()
    {
        Init();
        InitGUI();
        RigidBodySetUp();
        InitWheel();
        InitRRSkidMarks();
        InitBrakeLight();
    }
    private void FixedUpdate()
    {
        //G29Control();
        KeyBoardControl();
        RRModeMovement();

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);

        GUIUpdate();
        Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake + "  RPM : " + Wheels[0].Left_Wheel.rpm + "  V : " + rigidBody.velocity.magnitude * 3.6f);
    }
}
