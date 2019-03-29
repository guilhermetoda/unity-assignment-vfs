using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Position;
    private void Update()
    {
        transform.position = _Position.position;
    }
}