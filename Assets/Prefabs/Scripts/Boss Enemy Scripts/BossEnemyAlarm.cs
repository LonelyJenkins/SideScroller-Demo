using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAlarm : MonoBehaviour
{
    public Collider alarmCollider;
    private BossEnemyController bossController;

    // Start is called before the first frame update
    void Start()
    {
        alarmCollider = gameObject.GetComponent<Collider>();
        bossController = gameObject.GetComponentInParent<BossEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bossController.isAlerted = true;
        }
    }
}
