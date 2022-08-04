using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AudiA3 : Car // FF
{
    protected override void Init()
    {
        MaxVelocity = 210f;
        MaxWheelAngle = 45;
        MaxMotorPower = 1901f; // It means motorTorque
        MaxBrakePower = 3000f; // Brake
    }
    private void Awake()
    {
        Init();
        InitKey();
        InitGUI();
        RigidBodySetUp();
        InitWheel();
        InitFFSkidMarks();
        InitBrakeLight();
    }
    private void FixedUpdate()
    {
        //G29Control();
        //KeyBoardControl();
        FFModeMovement();

        MoveVisualWheel(Wheels[0].Left_Wheel);
        MoveVisualWheel(Wheels[0].Right_Wheel);
        MoveVisualWheel(Wheels[1].Left_Wheel);
        MoveVisualWheel(Wheels[1].Right_Wheel);

        GUIUpdate();
        //Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake + "  RPM : " + Wheels[0].Left_Wheel.rpm + "  V : " + rigidBody.velocity.magnitude * 3.6f);
    }
}
