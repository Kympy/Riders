using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porsche911 : Car // RR
{
    protected override void Init()
    {
        MaxVelocity = 293f;
        MaxWheelAngle = 45;
        MaxMotorPower = 3413f; // It means motorTorque
        MaxBrakePower = 5000f; // Brake
    }
    private void Awake()
    {
        Init();
        InitKey();
        InitGUI();
        RigidBodySetUp();
        InitWheel();
        InitRRSkidMarks();
        InitBrakeLight();
    }
    private void FixedUpdate()
    {
        //G29Control();
        //KeyBoardControl();
        RRModeMovement();

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);

        GUIUpdate();
        //Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake + "  RPM : " + Wheels[0].Left_Wheel.rpm + "  V : " + rigidBody.velocity.magnitude * 3.6f);
    }
}
