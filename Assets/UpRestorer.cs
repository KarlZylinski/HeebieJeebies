using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpRestorer : MonoBehaviour
{
    public float Speed = 5.0f;
    public float RotOffset = -90.0f;
    private float _paused_till;

    void Start()
    {
        _paused_till = 0;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10, 1 << 12);

        if (hit.collider != null)
            return;

        if (Time.time < _paused_till)
            return;

        var wanted_up_dir = transform.position.normalized;
        transform.up = wanted_up_dir;
    }

    public void Pause(float time)
    {
        _paused_till = Time.time + time;
    }
}
