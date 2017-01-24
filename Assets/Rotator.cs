using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float Speed = 1.0f;
    public bool AroundY = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var v = new Vector3(0, 0, 1);

        if (AroundY)
            v = new Vector3(0, 1, 0);

        transform.Rotate(v, Time.deltaTime * Speed);
	}
}
