using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    public ParticleSystem spikeHit;

    private Collider limbCollider;
    private Rigidbody limbRb;
    // Start is called before the first frame update
    void Start()
    {
        limbCollider = gameObject.GetComponent<Collider>();
        limbRb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            Instantiate(spikeHit, gameObject.transform);
            limbRb.velocity = limbRb.velocity - limbRb.velocity;
            limbRb.constraints = RigidbodyConstraints.FreezeAll;
        }

    }
}
