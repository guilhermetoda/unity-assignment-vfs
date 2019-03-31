using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyBomb : MonoBehaviour
{

    [SerializeField] 
    private int _numberOfBullets = 10;
    [SerializeField]
    private float _lifeTime = 1f;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _velocity = 5f;

    private Rigidbody _rigidBody;
     

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();

        Invoke("Explosion", _lifeTime);
    }

    private void Explosion() 
    {
        // Destroy the Crazy bomb bullet
        Destroy(gameObject);
        
        // Spawn one projectile at every 10 degrees
        for (int i = 0; i < _numberOfBullets; i++)
        {
            float angle = i * 1f;
            //spread rotation
            Quaternion spreadRotation = Quaternion.Euler(0f, angle, 0f);

            //add rotation to the current location
            Quaternion spawnRotation = transform.rotation * spreadRotation;

            //Spawn the bullet
            Instantiate(_bulletPrefab, transform.position, spawnRotation);
        }
    }
 
}
