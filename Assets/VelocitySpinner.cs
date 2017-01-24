using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocitySpinner : MonoBehaviour
{
    public float SpinMultiplier = 20.0f;
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

    void Update ()
    {
        transform.Rotate(new Vector3(0, 0, -_rigidbody.velocity.magnitude * SpinMultiplier * Time.deltaTime));
    }
}
