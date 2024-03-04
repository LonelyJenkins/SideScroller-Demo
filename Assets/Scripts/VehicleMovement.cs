using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public float speed = 0.5f;
    public AudioClip carHit;

    private float leftBound = -20;
    private Rigidbody vehicleRb;
    private AudioSource vehicleAudio;
    // Start is called before the first frame update
    void Start()
    {
        vehicleRb = gameObject.GetComponent<Rigidbody>();
        vehicleAudio = gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        vehicleRb.velocity = Vector3.left * speed;

        if (transform.position.x <= leftBound)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            vehicleAudio.PlayOneShot(carHit);
        }
    }

}
