using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAffected : MonoBehaviour {
    private List<ConstantForce2D> _constant_forces;

    void Start()
    {
        _constant_forces = new List<ConstantForce2D>();
        var child_rigid_bodies = GetComponentsInChildren<Rigidbody2D>();

        foreach (var rb in child_rigid_bodies)
        {
            _constant_forces.Add(rb.gameObject.AddComponent<ConstantForce2D>());
        }
    }

    void Update()
    {
        foreach (var cf in _constant_forces)
        {
            cf.force = (-transform.position).normalized * cf.gameObject.GetComponent<Rigidbody2D>().mass * 9.8f;
        }
    }
}
