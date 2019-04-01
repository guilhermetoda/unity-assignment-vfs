using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _Damage = 10f; // Damage taken 
    private float _LifeTime = 3f; // Lifetime of the bullet

    private void Awake()
    {
        //destroy after life time ends
        Destroy(gameObject, _LifeTime);
    }
    //Damages if a bullet hits a player
    private void OnCollisionEnter(Collision other)
    {
        AUnit aunit = other.gameObject.GetComponent<AUnit>();
        if (aunit != null) 
        {
            // If the collider is a Unit, take damage
            aunit.OnHit(_Damage);
        }
    }

}
