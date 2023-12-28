using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemyController : MonoBehaviour
{
    public Collider mainCollider;
    public int maxHits = 2;
    public int bulletForce;
    public bool isDead = false;
    public bool isAlerted = false;
    public bool alreadyAttacked = false;
    public float attackDelay = 0.75f;
    public GameObject deadSoldier;
    public GameObject hitPoint;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    public GameObject gunMuzzle;
    public ParticleSystem muzzleFlash;
    public AudioClip gunShot;
    public PlayerController playerController;
    public StaticEnemyAlarm enemyAlarm;
    public Transform playerPosition;
    public Vector3 offset;

    private Rigidbody enemyRb;
    private StaticHeadDamage headDamage;
    private DeadSoldier deadSoldierController;
    private Animator enemyAnim;
    private AudioSource enemyAudio;
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
        headDamage = gameObject.GetComponentInChildren<StaticHeadDamage>();
        playerController = playerPrefab.GetComponent<PlayerController>();
        enemyAlarm = gameObject.GetComponentInChildren<StaticEnemyAlarm>();
        enemyAudio = gameObject.GetComponent<AudioSource>();
        deadSoldierController = deadSoldier.GetComponent<DeadSoldier>();
        deadSoldier.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timesHit >= maxHits)
        {
            Death();
        }

        if (isAlerted)
        {
            enemyAnim.SetBool("isAlerted", true);
        }
    }

    private void FixedUpdate()
    {
        if (isAlerted && !playerController.gameOver && !isDead)
        {

            enemyRb.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(playerPosition.transform.position.x - transform.position.x), 0)));
            Shoot();
            gunMuzzle.transform.LookAt(playerPosition.position + offset);
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

        if (enemyAlarm.alarmCollider.gameObject.CompareTag("Player"))
        {
            isAlerted = true;
        }

    }

    private void OnAnimatorIK()
    {
        //weapon aim at target IK
        if (isAlerted == true)
        enemyAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        enemyAnim.SetIKPosition(AvatarIKGoal.RightHand, playerPrefab.transform.position);
    }

    private void Shoot()
    {
        if (!alreadyAttacked)
        {
            Instantiate(projectilePrefab, gunMuzzle.transform.position, gunMuzzle.transform.rotation);
            enemyAudio.PlayOneShot(gunShot);
            muzzleFlash.Play();
            enemyAnim.SetTrigger("shootTrig");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackDelay);
        }

    }

    public void Death()
    {
        isDead = true;
        CopyTransformData(gameObject.transform, deadSoldier.transform, enemyRb.velocity);
        deadSoldier.SetActive(true);
        bulletForce = (Random.Range(200, 600));
        deadSoldier.GetComponentInChildren<Rigidbody>().AddForceAtPosition(Vector3.right * bulletForce, hitPoint.transform.position, ForceMode.Impulse);
        Destroy(deadSoldier, deadSoldierController.despawnTimer);
        if (headDamage.headShot == true)
        {
            deadSoldierController.HeadShot();
        }
        gameObject.SetActive(false);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
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
