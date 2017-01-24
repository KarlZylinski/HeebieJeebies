using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlupp : MonoBehaviour {
    public GameObject ThisPlupp;
    public GameObject ThisWheel;
    public GameObject OtherPlupp;
    public GameObject OtherWheel;
    public GameObject Bar;

    void Update()
    {
        ThisPlupp.transform.position = transform.position;
        OtherPlupp.transform.position = OtherWheel.transform.position + (transform.position - ThisWheel.transform.position);
        Bar.transform.position = ThisWheel.transform.position + (transform.position - ThisWheel.transform.position);
    }
}
