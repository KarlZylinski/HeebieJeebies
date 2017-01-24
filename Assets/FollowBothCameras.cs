using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBothCameras : MonoBehaviour
{
    public GameObject Camera1;
    public GameObject Camera2;

    void Update ()
    {
        transform.position = (Camera1.transform.position + Camera2.transform.position) / 2.0f;
    }
}
