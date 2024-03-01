using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSoldier : MonoBehaviour
{
    public GameObject livingEnemy;
    public GameObject head;
    public ParticleSystem bloodFountain;
    public ParticleSystem headPop;
    public int despawnTimer = 6;

    private StaticHeadDamage headDamage;
    private StaticEnemyController staticEnemyController;

    // Start is called before the first frame update
    void Start()
    {
        headDamage = livingEnemy.GetComponent<StaticHeadDamage>();
        staticEnemyController = livingEnemy.GetComponent<StaticEnemyController>();
    }


    public void HeadShot()
    {
        headPop.Play();
        bloodFountain.Play();
        head.SetActive(false);
    }
}
