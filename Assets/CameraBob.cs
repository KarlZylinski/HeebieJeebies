using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public float Speed = 1.0f;

    void Update ()
    {
        var speed = Time.time * 0.25f * Speed;
        transform.position = new Vector3(Mathf.Sin(Mathf.Cos(speed) * 5.0f), Mathf.Cos(speed) * 1.4f, -10);
    }
}
