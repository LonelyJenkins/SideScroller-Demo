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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            headShot = true;
            Destroy(other.gameObject);
            bossController.Death();
        }

    }
}
