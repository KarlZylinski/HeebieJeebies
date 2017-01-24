using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageTaker : MonoBehaviour
{
    private int Health;
    private float _win_time;
    private PlayerType _winner;
    private bool _victory;

    void Start()
    {
        Health = 3;
        _victory = false;
    }

    void Update()
    {
        if (!_victory)
            return;

        if (Time.time < _win_time)
            return;

        Data.Winner = _winner;
        SceneManager.LoadScene("End");
    }

    private void Shock(Vector2 ContactPoint)
    {
        var r = GetComponent<Rigidbody2D>();

        if (r == null)
            return;

        Vector2 pos = transform.position;
        Vector2 damage_dir = (pos - ContactPoint).normalized;
        r.AddForce(damage_dir * r.mass * 200.0f);
    }

    public void TakeDamage(Vector2 ContactPoint)
    {
        Shock(ContactPoint);
        --Health;
        
        if (Health <= 0)
        {
            if (gameObject.tag == "HeebieGod" || gameObject.tag == "Heebie")
            {
                _win_time = Time.time + 3.0f;
                _winner = PlayerType.Jeebie;
                Data.Victory = true;
                _victory = true;
                return;
            }
            else if (gameObject.tag == "JeebieGod" || gameObject.tag == "Jeebie")
            {
                _win_time = Time.time + 3.0f;
                _winner = PlayerType.Heebie;
                Data.Victory = true;
                _victory = true;
                return;
            }
        }
    }
}
