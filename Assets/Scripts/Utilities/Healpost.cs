using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healpost : MonoBehaviour
{
    // Public Static List to return the HealPost
    public static List<Healpost> HealpostList = new List<Healpost>();

    [SerializeField] private float _HealingPoints = 5f;

    private float _CountSeconds = 0f;

    private void OnEnable()
    {
        HealpostList.Add(this);
    }

    private void OnDisable()
    {
        HealpostList.Remove(this);
    }

    private void OnTriggerStay(Collider other)
    {
        // Increasing the seconds to the heal post stay only heal every second, and not every frame
        _CountSeconds += Time.deltaTime;

        if (_CountSeconds >= 1f)
        {
            AUnit aunit = other.gameObject.GetComponent<AUnit>();
            if (aunit != null)
            {
                aunit.Heal(_HealingPoints);
            }
            _CountSeconds = 0f;
        }

    }
}
