using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButtons : MonoBehaviour
{
    [SerializeField]
    private Transform puzzleField;

    [SerializeField]
    private GameObject btn;

    private int howManyButtons = 8;
        private void Awake()
    {
        howManyButtons = PlayerPrefs.GetInt("level");
        for (int i = 0; i < howManyButtons; i++)
        {

            GameObject button = Instantiate(btn);
            button.name = "" + i;
            button.transform.SetParent(puzzleField, false);

        }
    }
}
