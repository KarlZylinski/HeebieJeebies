using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float MotorSpeed = 200.0f;
    public float MaxTorque = 50.0f;
    public int PlayerNum = 1;
    public bool ForwardHeld { get; set; }
    public bool BackwardHeld { get; set; }
    public bool CanonCCWHeld { get; set; }
    public bool CanonCWHeld { get; set; }
    public bool FireHeld { get; set; }
    private WheelJoint2D[] _wheels;
    private JointMotor2D _motor;
    private KeyCode _move_forward_key;
    private KeyCode _move_backward_key;
    private KeyCode _rotate_canon_ccw_key;
    private KeyCode _rotate_canon_cw_key;
    private KeyCode _fire_key;

    void Start()
    {
        ForwardHeld = false;
        BackwardHeld = false;
        CanonCCWHeld = false;
        CanonCWHeld = false;
        FireHeld = false;
        _wheels = GetComponents<WheelJoint2D>();
        _motor = new JointMotor2D();
        _motor.motorSpeed = 0;
        _motor.maxMotorTorque = MaxTorque;

        if (PlayerNum == 1)
        {
            _move_backward_key = KeyCode.DownArrow;
            _move_forward_key = KeyCode.UpArrow;
            _rotate_canon_cw_key = KeyCode.LeftArrow;
            _rotate_canon_ccw_key = KeyCode.RightArrow;
            _fire_key = KeyCode.Return;
        }
        else
        {
            _move_backward_key = KeyCode.S;
            _move_forward_key = KeyCode.W;
            _rotate_canon_cw_key = KeyCode.A;
            _rotate_canon_ccw_key = KeyCode.D;
            _fire_key = KeyCode.Space;
        }
    }

    private void ReadInput()
    {
        BackwardHeld = Input.GetKey(_move_backward_key);
        ForwardHeld = Input.GetKey(_move_forward_key);
        CanonCCWHeld = Input.GetKey(_rotate_canon_ccw_key);
        CanonCWHeld = Input.GetKey(_rotate_canon_cw_key);
        FireHeld = Input.GetKey(_fire_key);
    }

    private void ProcessMovement()
    {
        float movement_dir = 0;

        if (BackwardHeld)
            movement_dir -= 1;

        if (ForwardHeld)
            movement_dir += 1;

        foreach (var w in _wheels)
        {
            _motor.motorSpeed = movement_dir * -MotorSpeed;
            w.motor = _motor;
        }
    }

    void Update()
    {
        ReadInput();
        ProcessMovement();
    }
}
