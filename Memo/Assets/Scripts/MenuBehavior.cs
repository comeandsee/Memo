using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    public GameObject quiteWindow;
    public GameObject rankingWindow;
    public GameObject nickWindow;
    public InputField nickInput;
    public static bool nickWindowVisible = true;


    public Text records;
    public static SqliteControler database;
    public static string playerName;

    public void quite()
    {
        quiteWindow.SetActive(true);
    }
    void Start()
    {
        database = new SqliteControler();
    }
    private void Update()
    {
        if(!nickWindowVisible) nickWindow.SetActive(false); ;
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
            case (2): //ranking wyswietlanie
                rankingWindow.SetActive(true);
                records.text = database.ReadFromGameTable(playerName);;
                break;
            case (3):
                rankingWindow.SetActive(false);
                break;
            case (4): //nick
                playerName = nickInput.text;
                database.InsertUser(playerName);
                nickWindow.SetActive(false);
                break;
            case (5):
                nickWindowVisible = false;
                SceneManager.LoadScene("Menu");
                break;

        }
    }
    
}
