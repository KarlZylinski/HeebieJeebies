using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public GameObject StartPos;
    public GameObject HeebieGod;
    public GameObject JeebieGod;
    public GameObject JeebiesWinsText;
    public GameObject HeebiesWinsText;
    public MeshRenderer EndWhite;
    public Camera Cam;
    private GameObject _god;
    private float _shake_speed;
    private float _amp;
    private float _t2;
    private float _a;

    private GameObject GetWinnerGod()
    {
        if (Data.Winner == PlayerType.Heebie)
            return HeebieGod;

        return JeebieGod;
    }

    private GameObject GetWinnerGodText()
    {
        if (Data.Winner == PlayerType.Heebie)
            return HeebiesWinsText;

        return JeebiesWinsText;
    }

    void Start()
    {
        _god = Instantiate(GetWinnerGod(), StartPos.transform.position, HeebieGod.transform.rotation);
        _shake_speed = 1;
        _amp = 1;
        _a = 0;
        EndWhite.material.SetColor("_Color", new Color(1, 1, 1, 0));
    }
    
    void Update()
    {
        _god.transform.position -= _god.transform.position * Time.deltaTime * 0.9f;
        var t = Time.timeSinceLevelLoad;

        if (t > 15.3f)
        {
            var text = GetWinnerGodText();
            text.SetActive(true);
        }
        else if (t > 11.5f)
        {
            _shake_speed -= Time.deltaTime * 10.0f;
            _amp -= Time.deltaTime * 1.0f;
            _t2 -= Time.deltaTime;
            _amp = Mathf.Max(0, _amp);
            _shake_speed = Mathf.Max(0, _shake_speed);
            _t2 = Mathf.Max(0, _t2);
        }
        else if (t > 9.0f)
        {
            _a += Time.deltaTime * 0.5f;
            _a = Mathf.Min(_a, 1);
            EndWhite.material.SetColor("_Color", new Color(1, 1, 1, _a));
            _shake_speed += Time.deltaTime * 45.0f;
            _amp += Time.deltaTime * 0.3f;
            _t2 += Time.deltaTime;
        }
        else if (t > 6.0f)
        {
            _shake_speed += Time.deltaTime * 45.0f;
            _amp += Time.deltaTime * 0.3f;
            _t2 += Time.deltaTime;
        }
        else if (t > 5.0f)
        {
            _shake_speed += Time.deltaTime * 15f;
            _amp += Time.deltaTime * 0.2f;
            _t2 += Time.deltaTime;
        }
        else if (t > 3.0f)
        {
            _shake_speed += Time.deltaTime * 5.0f;
        }

        var speed = t * 0.25f * _shake_speed;
        Cam.transform.position = new Vector3(Mathf.Cos(speed + _t2) * _amp, Mathf.Sin(Mathf.Cos(speed) * 1.4f) * _amp, -10);
    }
}
