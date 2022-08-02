using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CarControl : Car
{
    private void Awake()
    {
        base.Init();
        base.RigidBodySetUp();
        base.InitWheel();
        base.InitFFSkidMarks();
        base.InitBrakeLight();
    }
    private void FixedUpdate()
    {
        //base.G29Control();
        base.KeyBoardControl();
        base.FFModeMovement();

        base.MoveVisualWheel(Wheels[0].Left_Wheel);
        base.MoveVisualWheel(Wheels[0].Right_Wheel);
        base.MoveVisualWheel(Wheels[1].Left_Wheel);
        base.MoveVisualWheel(Wheels[1].Right_Wheel);

        base.GUIUpdate();
        Debug.Log("SteeringAngle : " + Steering + "  MotorTorque : " + Motor + "  BrakePower : " + Brake + "  RPM : " + Wheels[0].Left_Wheel.rpm + "  V : " + rigidBody.velocity.magnitude * 3.6f);
    }
}
