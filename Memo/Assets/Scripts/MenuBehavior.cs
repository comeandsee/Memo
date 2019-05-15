using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    public GameObject quiteWindow;
    public void quite()
    {
        quiteWindow.SetActive(true);
    }
    public void triggerrMenuBehavior(int i)
    {
        switch (i)
        {
            default:
            case (0):
                SceneManager.LoadScene("Level");
                break;
            case (1):
                Debug.Log("Quit Game");
                Application.Quit();
                break;
            case (2):
                SceneManager.LoadScene("Menu");
                break;
        }
    }
    
}
