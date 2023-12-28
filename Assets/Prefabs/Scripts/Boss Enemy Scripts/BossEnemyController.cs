using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyController : MonoBehaviour
{
    public Collider mainCollider;
    public int maxHits = 3;
    public int bulletForce;
    public bool isDead = false;
    public bool isAlerted = false;
    public bool alreadyAttacked = false;
    public float attackDelay = 0.75f;
    public GameObject deadEnemy;
    public GameObject hitPoint;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    public GameObject gunMuzzle;
    public GameObject trigger;
    public GameObject handle;
    public AudioClip gunShot;
    public ParticleSystem muzzleFlash;
    public DeadBoss deadBossController;
    public PlayerController playerController;
    public Transform playerPosition;
    public Transform chest;
    public Vector3 offset;

    private BossEnemyAlarm bossAlarm;
    private Rigidbody enemyRb;
    private BossHeadDamage headDamage;
    private AudioSource enemyAudio;
    private Animator enemyAnim;
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
        headDamage = gameObject.GetComponentInChildren<BossHeadDamage>();
        playerController = playerPrefab.GetComponent<PlayerController>();
        bossAlarm = gameObject.GetComponentInChildren<BossEnemyAlarm>();
        enemyAudio = gameObject.GetComponent<AudioSource>();
        deadBossController = deadEnemy.GetComponent<DeadBoss>();
        deadEnemy.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

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
        }
    }

    private void LateUpdate()
    { if ( isAlerted && !isDead && !playerController.gameOver)
        {
            chest.LookAt(playerPosition);
            chest.rotation = chest.rotation * Quaternion.Euler(offset);
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

        if (bossAlarm.alarmCollider.gameObject.CompareTag("Player"))
        {
            isAlerted = true;
        }

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

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnAnimatorIK()
    {
        // enemy's hands are placed on weapon
        enemyAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        enemyAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        enemyAnim.SetIKPosition(AvatarIKGoal.RightHand, trigger.transform.position);
        enemyAnim.SetIKPosition(AvatarIKGoal.LeftHand, handle.transform.position);
    }

    public void Death()
    {
        isDead = true;
        CopyTransformData(gameObject.transform, deadEnemy.transform, enemyRb.velocity);
        deadEnemy.SetActive(true);
        bulletForce = (Random.Range(200, 600));
        deadEnemy.GetComponentInChildren<Rigidbody>().AddForceAtPosition(Vector3.right * bulletForce, hitPoint.transform.position, ForceMode.Impulse);
        Destroy(deadEnemy, deadBossController.despawnTimer);
        if (headDamage.headShot == true)
        {
            deadBossController.HeadShot();
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
