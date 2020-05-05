using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    public PlayerController playerController;
    public AudioSource dying;
    public AudioSource boo;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy.HasBeenKicked)
                playerController.KilledEnemy();

            Destroy(other.gameObject);
        }

        if(other.CompareTag(playerController.gameObject.tag))
        {
            boo.Play();
            dying.Play();
            playerController.HasDied();
        }
    }
}
