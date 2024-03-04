using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadDamage : MonoBehaviour
{
    public BossEnemyController bossController;
    public Collider headCollider;
    public bool headShot;



    // Start is called before the first frame update
    void Start()
    {

        bossController = gameObject.GetComponentInParent<BossEnemyController>();
        headCollider = gameObject.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet") && !bossController.isDead)
        {
            headShot = true;
            bossController.Death();
        }

    }
}
