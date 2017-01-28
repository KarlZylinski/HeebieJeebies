using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallCollisionChecker : MonoBehaviour
{
    public FollowObject ObjectFollower;
    private bool _removed_from_follower;
    private float _created_at;

    void Start()
    {
        _removed_from_follower = false;
        _created_at = Time.time;
    } 

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider.gameObject.tag == "Ground")
            {
                Remove();
                return;
            }

            if (contact.collider.gameObject.tag == "Player" && gameObject.tag == "CannonBall")
            {
                Remove();

                var dt = contact.collider.GetComponent<DamageTaker>();

                if (dt != null && collision.relativeVelocity.magnitude > 10)
                {
                    dt.TakeDamage(contact.point);
                    return;
                }
            }

            if (contact.collider.gameObject.tag == "HeebieGod" || contact.collider.gameObject.tag == "JeebieGod")
            {
                Remove();

                var dt = contact.collider.GetComponent<DamageTaker>();
                var ur = contact.collider.GetComponent<UpRestorer>();

                if (ur != null)
                {
                    ur.Pause(5.0f);
                }

                if (dt != null && collision.relativeVelocity.magnitude > 10)
                {
                    dt.TakeDamage(contact.point);
                    return;
                }
            }
        }
    }

    private void Remove()
    {
        gameObject.tag = "CannonBallGrounded";
    }

    void OnDestroy()
    {
        ObjectFollower.Remove(gameObject, 0.0f);
    }

    void Update()
    {
        if (_removed_from_follower)
            return;

        var rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        if (rb.velocity.magnitude < 2 && Time.time > _created_at + 2.0f)
        {
            ObjectFollower.Remove(gameObject, 0.9f);
            _removed_from_follower = true;
        }
    }
}
