using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Collider mainCollider;
    public float speed = 6;
    public int maxHits = 2;
    public int bulletForce;
    public bool isDead = false;
    public bool isAlerted = false;
    public GameObject hitPoint;
    public GameObject playerPrefab;
    public GameObject deadEnemy;
    public AudioClip running;
    public PlayerController playerController;
    public EnemyAlarm enemyAlarm;
    public Transform playerPosition;

    private Rigidbody enemyRb;
    private HeadDamage headDamage;
    private Animator enemyAnim;
    private DeadEnemy deadEnemyController;
    private AudioSource enemyAudio;
    private float stepRate = 0.33f;
    private float nextStep = 0f;
    private int timesHit; 
    private int facingSign
    {
        get
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0f ? 1 : 0;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        enemyRb = gameObject.GetComponent<Rigidbody>();
        enemyAnim = gameObject.GetComponent<Animator>();
        mainCollider = gameObject.GetComponent<Collider>();
        headDamage = gameObject.GetComponentInChildren<HeadDamage>();
        playerController = playerPrefab.GetComponent<PlayerController>();
        enemyAlarm = gameObject.GetComponentInChildren<EnemyAlarm>();
        enemyAudio = gameObject.GetComponent<AudioSource>();
        deadEnemyController = deadEnemy.GetComponent<DeadEnemy>();
        deadEnemy.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timesHit >= maxHits)
        {
            Death();
        }

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        if (isAlerted && !playerController.gameOver)
        {
            enemyRb.velocity = new Vector3(speed * facingSign, enemyRb.velocity.y, 0);
            enemyAnim.SetBool("isRunning", true);
        enemyRb.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(playerPosition.transform.position.x - transform.position.x), 0)));

        }

        if (isAlerted && !isDead && Time.time > nextStep)
        {
            nextStep = Time.time + stepRate;
            enemyAudio.pitch = Random.Range(0.5f, 0.7f);
            enemyAudio.PlayOneShot(running, Random.Range(0.8f, 1f));
        }



    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            timesHit += 1;
            if (timesHit == maxHits)
            {
                Instantiate(hitPoint, collision.transform);
            }
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            Death();
        }

        if (collision.gameObject.CompareTag("Vehicle"))
        {
            Death();
        }

    }

    public void Death()
    {
        isDead = true;
        CopyTransformData(gameObject.transform, deadEnemy.transform, enemyRb.velocity);
        deadEnemy.SetActive(true);
        bulletForce = (Random.Range(200, 600));
        deadEnemy.GetComponentInChildren<Rigidbody>().AddForceAtPosition(Vector3.right * bulletForce, hitPoint.transform.position, ForceMode.Impulse);
        Destroy(deadEnemy, deadEnemyController.despawnTimer);
        if (headDamage.headShot == true)
        {
            deadEnemyController.HeadShot();
        }
        gameObject.SetActive(false);

    }

    private void CopyTransformData(Transform sourceTransform, Transform destinationTransform, Vector3 bodyVelocity)
    //void that allows deadPlayer prefab to mimic player prefab's current state on death (for consistent ragdoll)
    {
        if (sourceTransform.childCount != destinationTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy, they need to match transform heirarchies");
            return;
        }

        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destinationTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            var rb = destination.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bodyVelocity;
            }
            CopyTransformData(source, destination, bodyVelocity);
        }
    }

}
