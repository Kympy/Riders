using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camaro : Car // FR
{
    protected override void Init() // This Car's Own Values
    {
        MaxVelocity = 288f;
        MaxWheelAngle = 45;
        MaxMotorPower = 4233f; // It means motorTorque
        MaxBrakePower = 6000f; // Brake
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
    } // Initialize
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
    } // Movement
}
