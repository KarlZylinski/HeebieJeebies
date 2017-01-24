using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : MonoBehaviour {
    enum CanonFireState
    {
        Ready,
        Cooldown,
        Charging
    };

    public List<AmmoType> Ammo;
    public GameObject AmmoPile;
    private PlayerControl _control;
    public FollowObject ObjectFollower;
    public float RotateSpeed = 1000.0f;
    public GameObject BallStart;
    private CanonFireState _fire_state;
    private float _fire_cooldown_end;
    public float MaxFireStrength = 2000.0f;
    public float MinFireStrength = 200.0f;
    public float FireStrengthIncreaseRate = 400.0f;
    public float FireCooldownTime = 0.0f;
    public float RecoilMultiplier = 4.0f;
    private float _fire_strength;
    private Vector2 _original_pivot;
    public GameObject HeebieAmmo;
    public GameObject JeebieAmmo;
    private float _shake_time;
    private AudioSource _audio_source;
    public List<AudioClip> FireSounds;

    void Start ()
    {
        _control = GetComponentInParent<PlayerControl>();
        _fire_state = CanonFireState.Ready;
        _fire_strength = 0;
        _shake_time = 0;
        _original_pivot = GetComponent<SpriteRenderer>().sprite.pivot;
        _audio_source = GetComponent<AudioSource>();
    }

    float GetNewRotation()
    {
        var rotate_dir = 0.0f;

        if (_control.CanonCCWHeld)
            rotate_dir -= 1;

        if (_control.CanonCWHeld)
            rotate_dir += 1;

        var cur_z_rot = transform.localRotation.eulerAngles.z;
        var delta_rot = RotateSpeed * rotate_dir * Time.deltaTime;

        if (cur_z_rot + delta_rot > 180)
            return 180;
        else if (cur_z_rot + delta_rot < 0)
            return 0;

        return cur_z_rot + delta_rot;
    }

    private void ShakeCannon()
    {
        var s = _fire_strength / MaxFireStrength;
        _shake_time += Time.deltaTime * 100.0f * s;
        transform.localPosition = new Vector3(Mathf.Cos(_shake_time) * 0.1f * s, Mathf.Sin(_shake_time) * 0.1f * s, 0);
    }

    void CheckFire()
    {
        if (_control.FireHeld && _fire_state == CanonFireState.Cooldown && Time.time > _fire_cooldown_end)
            _fire_state = CanonFireState.Ready;

        if (_control.FireHeld && _fire_state == CanonFireState.Ready && Ammo.Count > 0)
        {
            _fire_state = CanonFireState.Charging;
            _fire_strength = MinFireStrength;
        }

        if (_control.FireHeld && _fire_state == CanonFireState.Charging)
        {
            _fire_strength += FireStrengthIncreaseRate * Time.deltaTime;
            _fire_strength = Mathf.Clamp(_fire_strength, MinFireStrength, MaxFireStrength);
        }

        if (!_control.FireHeld && _fire_state == CanonFireState.Charging)
        {
            Fire();
            _fire_state = CanonFireState.Cooldown;
            _fire_cooldown_end = Time.time + FireCooldownTime;
        }
    }

    private GameObject InstatiateAmmo(AmmoType type)
    {
        if (type == AmmoType.Heebie)
            return Instantiate(HeebieAmmo);

        if (type == AmmoType.Jeebie)
            return Instantiate(JeebieAmmo);

        return null;
    }

    private void Fire()
    {
        if (Ammo.Count == 0)
            return;

        var n = Ammo.Count - 1;
        var obj_to_inst = Ammo[n];
        Ammo.RemoveAt(n);

        AmmoPile.transform.FindChild((n + 1).ToString()).GetComponent<SpriteRenderer>().enabled = false;
        var fired_ball = InstatiateAmmo(obj_to_inst);
        fired_ball.transform.position = BallStart.transform.position;
        var ball_rb = fired_ball.GetComponent<Rigidbody2D>();
        var fire_dir = (BallStart.transform.position - transform.position).normalized;
        var force = fire_dir * _fire_strength;
        _fire_strength = 0;
        _shake_time = 0;
        ball_rb.AddForce(force);
        _control.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * RecoilMultiplier);

        var angle = Mathf.Atan2(fire_dir.y, fire_dir.x) * Mathf.Rad2Deg - 90.0f;
        fired_ball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        ObjectFollower.Add(fired_ball);

        if (FireSounds.Count != 0)
        {
            _audio_source.PlayOneShot(FireSounds[Random.Range(0, FireSounds.Count - 1)]);
            _audio_source.time = 2f;
        }

        var ball_collision_checker = fired_ball.GetComponent<CannonBallCollisionChecker>();
        if (ball_collision_checker != null)
            ball_collision_checker.ObjectFollower = ObjectFollower;
    }

    void Update()
    {
        ShakeCannon();
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, GetNewRotation()));
        CheckFire();
    }
}
