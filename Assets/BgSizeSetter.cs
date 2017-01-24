using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgSizeSetter : MonoBehaviour {
    private Camera _cam;

    void Start ()
    {
        _cam = GetComponentInParent<Camera>();
    
        if (_cam == null)
        {
            Debug.LogError("Broken cam");
            Destroy(this);
        }
    }

    void Update ()
    {
        var s = Mathf.Log10(_cam.orthographicSize * 3.0f) * 10.0f;
        transform.localScale = new Vector3(s, 1.0f, s);
    }
}
