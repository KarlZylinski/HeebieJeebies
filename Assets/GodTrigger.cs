using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodTrigger : MonoBehaviour {
    public string GodTag;
    public float NewMotorSpeed;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != GodTag)
            return;

        var wheels = other.gameObject.GetComponentsInChildren<WheelJoint2D>();
        JointMotor2D motor = new JointMotor2D();
        motor.maxMotorTorque = 100;
        motor.motorSpeed = NewMotorSpeed;

        foreach (var w in wheels)
            w.motor = motor;
    }
}
