using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlarm : MonoBehaviour
{
    public Collider alarmCollider;
    private EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        alarmCollider = gameObject.GetComponent<Collider>();
        enemyController = gameObject.GetComponentInParent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemyController.isAlerted = true;
        }
    }
}
