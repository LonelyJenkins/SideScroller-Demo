using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winStateUI;
    public GameObject loseStateUI;
    public GameObject winGameUI;


    public void WinState()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != "Level Three")
        {
            winStateUI.SetActive(true);
        }

        else
        {
            winGameUI.SetActive(true);
        }
    }

    public void LoseState()
    {
        loseStateUI.SetActive(true);
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
    }

    public void Continue()
    {
        Scene scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.buildIndex + 1);

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
