using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyBomb : MonoBehaviour
{
    // Number of bullets
    [SerializeField] 
    private int _NumberOfBullets = 10;
    // Life Time of the Crazy Bomb
    [SerializeField]
    private float _LifeTime = 1f;

    [SerializeField]
    private GameObject _BulletPrefab;

    private void Awake()
    {
        Invoke("Explosion", _LifeTime);
    }

    private void Explosion() 
    {
        // Destroy the Crazy bomb bullet
        Destroy(gameObject);
        
        // Spawn one projectile at every X degrees
        for (int i = 0; i < _NumberOfBullets; i++)
        {
            float angle = i * 1f;
            //spread rotation
            Quaternion spreadRotation = Quaternion.Euler(0f, angle, 0f);

            //add rotation to the current location
            Quaternion spawnRotation = transform.rotation * spreadRotation;

            //Spawn the bullet
            Instantiate(_BulletPrefab, transform.position, spawnRotation);
        }
    }
 
}
