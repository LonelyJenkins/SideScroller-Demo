using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject deadPlayer;
    private int yBound = -50;
    private Vector3 offset = new Vector3(3, 2, -10);
    private PlayerController playerController;
    private AudioSource camAudio;
    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        camAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerController.gameOver)
        {
            transform.position = player.transform.position + offset;
        }
        else
        {
            if (deadPlayer.transform.position.y > yBound)
            {
                transform.position = deadPlayer.transform.position + offset;
            }

            camAudio.Stop();
        }

    }
}
