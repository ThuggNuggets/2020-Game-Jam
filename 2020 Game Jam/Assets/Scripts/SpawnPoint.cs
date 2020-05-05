using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private bool _isOccupied = false;

    private void OnTriggerEnter(Collider other)
    {
         _isOccupied = true;
    }
    private void OnTriggerStay(Collider other)
    {
         _isOccupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isOccupied = false;
    }

    public bool Occupied
    {
        get { return _isOccupied; }
        set { _isOccupied = value; }
    }
}
