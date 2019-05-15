using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    [SerializeField]
    public Sprite[] puzzles;

    public List<Button> btns = new List<Button>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    private bool firstGuess, secondGuess;

    public int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private int firstGuessIndex, secondGuessIndex;

    private float timeToWaitBeforeTurnAround = .2f;
    private float timeToChechMatchedPuzzle = .6f;

    private string pathToEasyCards = "Sprites/cards/";
    private string category;

    public GameObject endWindow;
    private void Awake()
    {
        category = PlayerPrefs.GetString("category");
        puzzles = Resources.LoadAll<Sprite>(pathToEasyCards+category);
    }
    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzle();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;  
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for( int i = 0; i <objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddGamePuzzle()
    {
        int buttonsNumber = btns.Count;
        int index = 0;
        if (buttonsNumber > puzzles.Length)
        {
            Debug.LogError($"Liczba puzzli ({buttonsNumber}) wieksza niz liczba podwojonych obrazkow do wczytania({2*puzzles.Length})");
        }

        for (int i = 0; i < buttonsNumber; i++)
        {
            if( index == buttonsNumber / 2)
            {
                index = 0;
            }

            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    void AddListeners()
    {
        foreach(Button btn in btns)
        {   
            btn.onClick.AddListener(() => PickPuzzle());
        }
    }

    public void PickPuzzle()
    {
        if (!firstGuess) {

            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
           
        } else if (!secondGuess){

            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
            countGuesses++;
            StartCoroutine(CheckIfThePuzzlesMatch());
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(timeToChechMatchedPuzzle);

        if(firstGuessPuzzle == secondGuessPuzzle && firstGuessIndex!=secondGuessIndex)
        {
            yield return new WaitForSeconds(timeToWaitBeforeTurnAround);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckIfTheGameIsFinished();
        }
        else
        {
            yield return new WaitForSeconds(timeToWaitBeforeTurnAround);

            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }
        yield return new WaitForSeconds(timeToWaitBeforeTurnAround);

        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        countCorrectGuesses++;
        Debug.Log(countCorrectGuesses + gameGuesses);
        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("Koniec gry");
            Debug.Log($"Zgadłeś w {countGuesses} próbach.");
            endWindow.SetActive(true);

        }
    }

    public void triggerrGameController(int i)
    {
        switch (i)
        {
            default:
            case (0):
                SceneManager.LoadScene("Game");
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

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite tmp = list[i];
            int radomIndex = Random.Range(i, list.Count);
            list[i] = list[radomIndex];
            list[radomIndex] = tmp;
        }
    }

}
