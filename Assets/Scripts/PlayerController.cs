using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float speed = 8;
    public float jumpForce = 10;
    public float shootVolume = 0.75f;
    public int health = 3;
    public int timesHit;
    public int leftbound = -3;
    public bool isOnGround = true;
    public bool gameOver = false;
    public bool hasWon = false;
    public GameObject projectilePrefab;
    public GameObject gunMuzzle;
    public GameObject deadPlayer;
    public ParticleSystem muzzleFlash;
    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public AudioClip gunShot;
    public AudioClip spikeHit;
    public AudioClip healthPickup;
    public AudioClip landing;
    public AudioClip running;

    private float playerMovement;
    private float stepRate = 0.33f;
    private float nextStep = 0;
    private GameManager gameManager;
    private HealthText healthText;
    private Camera mainCamera;
    private Vector3 offset = new Vector3(-0.11f, 0, 0);
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;

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
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<HealthText>();
        playerRb = gameObject.GetComponent<Rigidbody>();
        playerAnim = gameObject.GetComponent<Animator>();
        playerAudio = gameObject.GetComponent<AudioSource>();
        deadPlayer.SetActive(false);
        hasWon = false;
        Time.timeScale = 1;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement = Input.GetAxisRaw("Horizontal");

        if ((playerMovement >= 0.1f || playerMovement <= -0.1f) && isOnGround && Time.time > nextStep)
        {
            nextStep = Time.time + stepRate;
            playerAudio.pitch = Random.Range(0.8f, 1f);
            playerAudio.PlayOneShot(running, Random.Range(0.8f, 1f));
        }


        healthText.healthValue = timesHit;

        gunMuzzle.transform.LookAt(new Vector3(targetTransform.transform.position.x, targetTransform.transform.position.y, targetTransform.transform.position.z));


        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.transform.position = hit.point;
        }

        // Player always faces the x direction of the target game object
        playerRb.MoveRotation(Quaternion.Euler(new Vector3(0, 90*Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));


        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            Jump(); 
        }

        if (Input.GetMouseButtonDown(0) && !hasWon)
        {
            Shoot();
        }

        if (transform.position.x < leftbound)
        {
            transform.position = new Vector3(leftbound, transform.position.y, transform.position.z);
        }

        if (timesHit >= health)
        {
            GameOver();
        }

 
    }

    private void FixedUpdate()
    {
       playerRb.velocity = new Vector3(playerMovement * speed, playerRb.velocity.y, 0);
       playerAnim.SetFloat("walkSpeed", facingSign * playerMovement);
    }

    private void Jump()
    {
       playerRb.AddForce(Vector3.up * jumpForce);
        isOnGround = false;
        playerAnim.SetTrigger("jumpTrig");
        playerAnim.ResetTrigger("landTrig");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            playerAnim.SetTrigger("landTrig");
            playerAudio.PlayOneShot(landing);
        }

        if (collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("Enemy"))

        {
            timesHit += 1;
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            playerAudio.PlayOneShot(spikeHit);
            GameOver();
        }

        if (collision.gameObject.CompareTag("Vehicle"))
        {
            GameOver();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HealthPickup"))
        {
            timesHit = 0;
            playerAudio.PlayOneShot(healthPickup);
            Debug.Log("Healed!! For now...");
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("WinGate"))
        {
            LevelWin();
        }

        if (other.gameObject.CompareTag("Fall Gate"))
        {
            GameOver();
        }
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, gunMuzzle.transform.position + offset, gunMuzzle.transform.rotation);
        playerAudio.pitch = Random.Range(0.7f, 1f);
        playerAudio.PlayOneShot(gunShot, shootVolume);
        muzzleFlash.Play();
        playerAnim.SetTrigger("shootTrig");

    }

    private void OnAnimatorIK()
    {
        if (!hasWon)
        {
            //weapon aim at target IK
            playerAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            playerAnim.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.transform.position);

        }


    }

    public void GameOver()
    {
        gameOver = true;
        CopyTransformData(gameObject.transform, deadPlayer.transform, playerRb.velocity);
        deadPlayer.SetActive(true);
        gameObject.SetActive(false);
        Time.timeScale = 0.5f;
        gameManager.LoseState();
        Debug.Log("You Just Got GOT FUCKER");
    }

    public void LevelWin()
    {
        hasWon = true;
        gameManager.WinState();
        Time.timeScale = 0;

        Debug.Log("You actually made it, Soldier");

    }

    private void CopyTransformData(Transform sourceTransform, Transform destinationTransform, Vector3 bodyVelocity)
        //void that allows deadPlayer prefab to mimic player prefab's current state on death (for consistent ragdoll)
    {
        if(sourceTransform.childCount != destinationTransform.childCount)
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
