using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    private SpriteRenderer _step;

    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        _step = gameObject.transform.FindChild("ChargeBarStep").GetComponent<SpriteRenderer>();
    }

    public void SetFill(float fill)
    {
        if (_step == null)
            return;

        _step.transform.localScale = new Vector3(1, fill, 1);
    }
}
