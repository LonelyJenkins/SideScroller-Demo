using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileMotion : MonoBehaviour
{
    public float speed = 10;
    public int bulletDespawn = 4;
    public ParticleSystem ricochet;
    public ParticleSystem bulletHit;


    // Update is called once per frame
    void Update()
    {

        StartCoroutine(BulletDespawnCountdown());
        transform.Translate(Vector3.forward * speed * Time.deltaTime);


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(ricochet, gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(180, 0, 0));
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(bulletHit, gameObject.transform.position, bulletHit.transform.rotation);
            Destroy(gameObject);
        }

    }

    IEnumerator BulletDespawnCountdown()
    {
        yield return new WaitForSeconds(bulletDespawn);
        Destroy(gameObject);
    }

}
