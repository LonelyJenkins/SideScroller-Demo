﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBoss : MonoBehaviour
{
    public GameObject livingEnemy;
    public GameObject head;
    public ParticleSystem bloodFountain;
    public ParticleSystem headPop;
    public int despawnTimer = 6;

    private HeadDamage headDamage;
    private BossEnemyController bossEnemyController;

    // Start is called before the first frame update
    void Start()
    {
        headDamage = livingEnemy.GetComponent<HeadDamage>();
        bossEnemyController = livingEnemy.GetComponent<BossEnemyController>();
    }


    public void HeadShot()
    {
        headPop.Play();
        bloodFountain.Play();
        Destroy(head);
    }
}
