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
    public Text records;
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
                rankingWindow.SetActive(true);

                Ranking ranking = new Ranking();
                var recordsList = ranking.getRecords();
                var sortedList = recordsList.OrderBy(p => p.MoveNumber).ToList();
                int x = 1;
                string recordsString ="";
                foreach(var rec in sortedList)
                {
                    recordsString += x + ". Użytkownik: " + rec.user + ", liczba ruchów: " + rec.MoveNumber + ",data: " + rec.Date + "\n";
                    x++;
                }
                records.text = recordsString;
                break;
            case (3):
                rankingWindow.SetActive(false);
                break;

        }
    }
    
}
