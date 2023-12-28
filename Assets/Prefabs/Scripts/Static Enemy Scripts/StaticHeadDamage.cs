using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHeadDamage : MonoBehaviour
{
    public StaticEnemyController enemyController;
    public Collider headCollider;
    public bool headShot;



    // Start is called before the first frame update
    void Start()
    {
        
        enemyController = gameObject.GetComponentInParent<StaticEnemyController>();
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
            enemyController.Death();
        }

    }
}
