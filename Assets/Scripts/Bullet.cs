using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _damage = 10f;

    private Rigidbody _rigidBody; //rigidbody on the projectile

    private float _lifeTime = 3f;

    private float _velocity = 20f;

    private void Awake()
    {
        // get the rigidbody and apply velocity
        _rigidBody = GetComponent<Rigidbody>();
        
        //destroy after life time ends
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        AUnit aunit = other.gameObject.GetComponent<AUnit>();
        if (aunit != null) 
        {
            aunit.OnHit(_damage);
        }
    }

}
