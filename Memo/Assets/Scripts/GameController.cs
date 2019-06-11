using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using static System.Math;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    [SerializeField]
    public Sprite[] puzzles;

    public List<Button> btns = new List<Button>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    private bool firstGuess, secondGuess;

    public int countGuesses = 0;
    private int countCorrectGuesses = 0;
    private int gameGuesses;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private int firstGuessIndex, secondGuessIndex;

    private float timeToWaitBeforeTurnAround = .2f;
    private float timeToChechMatchedPuzzle = .5f;

    private string pathToEasyCards = "Sprites/cards/";
    private string category;
    private int level;

    public GameObject endWindow;
    public GameObject quiteWindow;
    public Text movesNumber;


    private PlayerEmotions playerEmotions;
    private Text emotionField;
    EmotionAccumulator emotionAccumulator;

    private const double reactionThreshold = 20.0;
    private const double minEmotionDuration = 10.0;

    private double time = 0.0;
    private Text timeField;

    private Text faceField;

    public Color disabledCardColor = new Color(0, 0, 0, 0);

    public bool isPreviewAvailable = false;
    GameObject previewCardsButton;

    Text middleText;
    private double gameTime = 1.4;

    private void Awake()
    {
        category = PlayerPrefs.GetString("category");
        level = PlayerPrefs.GetInt("level");
        puzzles = Resources.LoadAll<Sprite>(pathToEasyCards + category);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        playerEmotions = player.GetComponent<PlayerEmotions>();

        Transform emotion = GameObject.FindGameObjectWithTag("CurrentEmotion").transform;
        emotionField = emotion.GetComponent<Text>();
        emotionAccumulator = new EmotionAccumulator();

        Transform timer = GameObject.FindGameObjectWithTag("Timer").transform;
        timeField = timer.GetComponent<Text>();

        Transform faceDetectionField = GameObject.FindGameObjectWithTag("FaceDetectionField").transform;
        faceField = faceDetectionField.GetComponent<Text>();
    }

    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzle();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;

        Transform textInfo = GameObject.FindGameObjectWithTag("MiddleText").transform;
        middleText = textInfo.GetComponent<Text>();
        middleText.enabled = false;

        previewCardsButton = GameObject.FindGameObjectWithTag("PreviewCardsButton");
        setPreviewCardsButtonVisibility(true);
    }

    private void Update()
    {
        if (playerEmotions.faceOn)
        {
            faceField.color = Color.green;
            faceField.text = "Face: ON";
        }
        else
        {
            faceField.color = Color.red;
            faceField.text = "Face: OFF";
        }
           
        double deltaTime = Time.deltaTime;
        if (playerEmotions.currentSmile > reactionThreshold)
        {
            emotionAccumulator.update("positive", deltaTime);
            double positiveDuration = emotionAccumulator.getEmotionDuration("positive");
            emotionField.color = Color.green;
            emotionField.text = "Nastrój pozytywny";
            if (positiveDuration > minEmotionDuration)
            {
                StartCoroutine(PreviewCards());
                emotionAccumulator.reset("positive");
            }
        }
        else if (playerEmotions.currentAnger > reactionThreshold || playerEmotions.currentContempt > reactionThreshold)
        {
            emotionAccumulator.update("negative", deltaTime);
            double negativeDuration = emotionAccumulator.getEmotionDuration("negative");
            emotionField.color = Color.red;
            emotionField.text = "Nastrój negatywny";
            if (negativeDuration > minEmotionDuration)
            {
                ShuffleRemainingCards();
                emotionAccumulator.reset("negative");
            }
        }
        else
        {
            emotionField.color = Color.grey;
            emotionField.text = "Nastrój neutralny";
            emotionAccumulator.update("neutral", deltaTime);
        }
        time = (time + deltaTime) % double.MaxValue;
        string minute = ((int)(time) / 60).ToString();
        string second = ((int)(time) % 60).ToString().PadLeft(2, '0');
        timeField.text = "Time: " + minute + ":" + second;
    }
    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
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
            Debug.LogError($"Liczba puzzli ({buttonsNumber}) wieksza niz liczba podwojonych obrazkow do wczytania({2 * puzzles.Length})");
        }

        for (int i = 0; i < buttonsNumber; i++)
        {
            if (index == buttonsNumber / 2)
            {
                index = 0;
            }

            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickPuzzle());
        }
    }

    public void PickPuzzle()
    {
        if (!firstGuess)
        {

            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];

        }
        else if (!secondGuess)
        {
            int secondGuessIndexTmp = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            if (firstGuessIndex != secondGuessIndexTmp)
            {
                secondGuess = true;
                secondGuessIndex = secondGuessIndexTmp;
                secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
                btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
                countGuesses++;
                StartCoroutine(CheckIfThePuzzlesMatch());
            }
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(timeToChechMatchedPuzzle);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(timeToWaitBeforeTurnAround);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = disabledCardColor;
            btns[secondGuessIndex].image.color = disabledCardColor;

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
           movesNumber.text = "Liczba prób : " + countGuesses;
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
                MenuBehavior.nickWindowVisible = false;
                break;
            case (4): //save
                MenuBehavior.database.InsertGame(countGuesses, gameTime, category, level, System.DateTime.Today.ToShortDateString(), MenuBehavior.playerName);
                Debug.Log("Game saved.");
                MenuBehavior.nickWindowVisible = false;
                SceneManager.LoadScene("Menu");
                break;

        }
    }

    public void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite tmp = list[i];
            int radomIndex = Random.Range(i, list.Count);
            list[i] = list[radomIndex];
            list[radomIndex] = tmp;
        }
    }

    public void ShuffleRemainingCards()
    {
        StartCoroutine(ShowBigMessage("Tasowanie kart", 2));
        int randomIndex;
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < gamePuzzles.Count; i++)
            {
                randomIndex = Random.Range(i, gamePuzzles.Count);
                if (btns[i].image.color != disabledCardColor &&
                    btns[randomIndex].image.color != disabledCardColor)
                {
                    Sprite tmp = gamePuzzles[i];
                    gamePuzzles[i] = gamePuzzles[randomIndex];
                    gamePuzzles[randomIndex] = tmp;
                }
            }
        }
    }

    public IEnumerator PreviewCards()
    {
        setPreviewCardsButtonVisibility(false);

        List<int> cardsToPreview = new List<int>();

        List<int> remainingCards = new List<int>();
        for (int i = 0; i < gamePuzzles.Count; i++)
        {
            if(btns[i].image.color != disabledCardColor)
            {
                remainingCards.Add(i);
            }
        }
        
        int showCards = Max(1, (int)(remainingCards.Count*0.2));
        while (showCards > 0)
        {
            int randomIndexRemainingCard = Random.Range(0, remainingCards.Count - 1);
            int randomIndexCardObject = remainingCards[randomIndexRemainingCard];
            cardsToPreview.Add(randomIndexCardObject);
            remainingCards.Remove(randomIndexRemainingCard);

            showCards--;
        }

        foreach(int index in cardsToPreview)
        {
            btns[index].image.sprite = gamePuzzles[index];
        }

        yield return new WaitForSeconds(2);

        foreach (int index in cardsToPreview)
        {
            btns[index].image.sprite = bgImage;
        }
    }

    public void PreviewCardsButton()
    {
        StartCoroutine(PreviewCards());
    }

    void setPreviewCardsButtonVisibility(bool state)
    {
        isPreviewAvailable = state;
        previewCardsButton.SetActive(isPreviewAvailable);
    }

    public void ExitGameButton()
    {
        Debug.Log("Exit Game");
        quiteWindow.SetActive(true);
    }

    public void PopUpQuite(int i)
    {
        switch (i)
        {
            default:
            case (0):
                quiteWindow.SetActive(false);
                break;
            case (1):
                Debug.Log("Quit Game");
                Application.Quit();
                break;
        }
    }

    IEnumerator ShowBigMessage(string message, float delay)
    {
        middleText.text = message;
        middleText.enabled = true;
        yield return new WaitForSeconds(delay);
        middleText.enabled = false;
    }

}
