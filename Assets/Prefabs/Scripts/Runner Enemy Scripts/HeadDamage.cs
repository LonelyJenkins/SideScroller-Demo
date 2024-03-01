﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDamage : MonoBehaviour
{
    public EnemyController enemyController;
    public Collider headCollider;
    public bool headShot;

    // Start is called before the first frame update
    void Start()
    { 
        enemyController = gameObject.GetComponentInParent<EnemyController>();
        headCollider = gameObject.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bullet") && !enemyController.isDead)
        {
            headShot = true;
            Destroy(other.gameObject);
            enemyController.Death();
        }

    }
}
