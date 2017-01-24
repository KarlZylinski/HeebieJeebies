using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDistFader : MonoBehaviour {
    private SpriteRenderer _renderer;
    private float _target_alpha;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _target_alpha = 0;
    }

    void Update()
    {
        var dist = transform.position.magnitude;

        if (dist < 45)
        {
            _target_alpha = 0.0f;
        }
        else if (dist < 50)
        {
            _target_alpha = (50 - dist) / 5.0f;
        }
        else
        {
            _target_alpha = 1.0f;
        }

        var alpha = _renderer.color.a + (_target_alpha - _renderer.color.a) * Time.deltaTime * 3.0f;
        _renderer.color = new Color(1, 1, 1, alpha);
    }
}
