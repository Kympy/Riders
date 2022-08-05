using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porsche911 : Car // RR
{
    protected override void Init() // This Car's Own Values
    {
        MaxVelocity = 293f;
        MaxWheelAngle = 45;
        MaxMotorPower = 3413f; // It means motorTorque
        MaxBrakePower = 5000f; // Brake
    }
    private void Awake()
    {
        Init();
        //InitKey();
        InitGUI();
        RigidBodySetUp();
        InitWheel();
        InitConstValue();
        InitRRSkidMarks();
        InitBrakeLight();
    }
    private void FixedUpdate()
    {
        //G29Control();
        //KeyBoardControl();
        Movement();
        RRModeMovement();

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);

        GUIUpdate();
    }
}
