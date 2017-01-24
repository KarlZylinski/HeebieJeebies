using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityRotator : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_rigidbody == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        var dir = _rigidbody.velocity.normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f;
        var cur = transform.rotation.eulerAngles.z;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4.0f);
    }
}
