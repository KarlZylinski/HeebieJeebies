using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallCollisionChecker : MonoBehaviour
{
    public FollowObject ObjectFollower;

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
        ObjectFollower.Remove(gameObject, 0.9f);
    }
}
