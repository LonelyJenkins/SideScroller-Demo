using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public int startDelay = 2;
    public int repeatRate = 2;

    private Vector3 spawnPos = new Vector3(230, 1, 0);
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        InvokeRepeating("SpawnCar", startDelay, repeatRate);
    }

    private void SpawnCar()
    {
        if (!playerController.gameOver)
        {
            int carIndex = Random.Range(0, carPrefabs.Length);
            Instantiate(carPrefabs[carIndex], spawnPos, carPrefabs[carIndex].transform.rotation);
        }
    }
}
