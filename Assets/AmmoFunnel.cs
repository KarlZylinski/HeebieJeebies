using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoFunnel : MonoBehaviour {
    private GameObject _currently_tractoring;
    private GameObject _potential_tractorer;
    public GameObject AmmoPile;
    public CannonControl CannonControl;
    public float TractorSpeed = 3.0f;

    void Start()
    {
        _currently_tractoring = null;
        _potential_tractorer = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CannonControl.Ammo.Count == 3 || _currently_tractoring != null || other.gameObject.tag != "CannonBallGrounded")
        {
            if (other.gameObject.tag == "CannonBall" || other.gameObject.tag == "CannonBallGrounded")
            {
                _potential_tractorer = other.gameObject;
                return;
            }

            return;
        }

        StartTractoring(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_potential_tractorer != null && _potential_tractorer == other.gameObject)
            _potential_tractorer = null;
    }

    private void StartTractoring(GameObject obj)
    {
        _currently_tractoring = obj;
        obj.tag = "CannonBall";
        obj.GetComponent<CircleCollider2D>().enabled = false;
        obj.GetComponent<Rigidbody2D>().simulated = false;
    }

    private void TakeAmmo()
    {
        var index = CannonControl.Ammo.Count + 1;
        var ammogfx_obj = AmmoPile.transform.FindChild(index.ToString()).gameObject;

        if (ammogfx_obj == null)
        {
            Debug.LogError("TAKING TOO MUCH AMMO.");
            return;
        }

        var ammogfx = ammogfx_obj.GetComponent<SpriteRenderer>();

        if (ammogfx == null)
        {
            Debug.LogError("Bad ammo gfx");
            return;
        }

        var tractor_gfx = _currently_tractoring.GetComponent<SpriteRenderer>();

        if (tractor_gfx == null)
        {
            Debug.LogError("Bad tractor gfx");
            return;
        }

        ammogfx.sprite = tractor_gfx.sprite;
        ammogfx.enabled = true;
        ammogfx_obj.SetActive(true);
        var prefab_storer = _currently_tractoring.GetComponent<AmmoPrefabStorer>();

        if (prefab_storer == null)
        {
            Debug.LogError("Bad prefab storer.");
            return;
        }

        CannonControl.Ammo.Add(prefab_storer.AmmoType);
        Destroy(_currently_tractoring);
        _currently_tractoring = null;
    }

    void Update()
    {
        if (_potential_tractorer != null && _potential_tractorer.tag == "CannonBallGrounded" && CannonControl.Ammo.Count < 3 && _currently_tractoring == null)
        {
            StartTractoring(_potential_tractorer);
            _potential_tractorer = null;
        }

        if (_currently_tractoring == null)
            return;

        var delta_pos = transform.position - _currently_tractoring.transform.position;
        var ball_to_parent = Mathf.Abs((transform.parent.transform.position - _currently_tractoring.transform.position).magnitude);
        var funnel_to_parent = Mathf.Abs((transform.parent.transform.position - transform.position).magnitude);

        if (delta_pos.magnitude < 0.25f || ball_to_parent < funnel_to_parent)
        {
            TakeAmmo();
            return;
        }

        _currently_tractoring.transform.position += delta_pos * Time.deltaTime * TractorSpeed;
    }
}
