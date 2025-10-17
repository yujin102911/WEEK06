using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// ������ ��� UI ���(ī��, ��ư, �ؽ�Ʈ)�� �����ϴ� �̱��� �Ŵ���
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton Instance
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("UI Prefabs & Parents")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private List<Button> questionButtons;
    [SerializeField] private List<QuestionData> questionDatas;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI guessesText;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    #endregion

    #region Private Fields
    private List<CardUI> _spawnedCardUIs = new List<CardUI>();
    private GameManager _gameManager;
    private CardManager _cardManager;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();

        // Start()���� ���� ����Ǵ� Awake()���� �ٸ� �Ŵ��� �ν��Ͻ��� ã�ƿɴϴ�.
        _gameManager = GameManager.Instance;
        _cardManager = CardManager.Instance;
    }

    private void Start()
    {
        // UI �ʱ� ���� ������ Start()���� �մϴ�.
        SetupInitialUI();
    }

    private void OnEnable()
    {
        // Awake()���� �̹� _gameManager�� ã�ƿ����Ƿ�, ���⼭ �̺�Ʈ ������ �����ϰ� ����˴ϴ�.
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    /// <summary>�ʱ� UI ���¿� ��ư�� �����մϴ�.</summary>
    private void SetupInitialUI()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        startPanel.SetActive(true);

        SetupQuestionButtons();
    }

    /// <summary>���� ��ư���� Ŭ�� �̺�Ʈ�� �����մϴ�.</summary>
    private void SetupQuestionButtons()
    {
        if (questionButtons.Count != questionDatas.Count)
        {
            LogWarning("���� ��ư�� ���� �������� ������ ��ġ���� �ʽ��ϴ�.");
            return;
        }

        for (int i = 0; i < questionButtons.Count; i++)
        {
            int index = i;
            questionButtons[index].onClick.AddListener(() =>
            {
                _gameManager.SelectQuestion(questionDatas[index]);
            });
            var buttonText = questionButtons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = questionDatas[index].QuestionText;
            }
        }
    }
    #endregion

    #region Private Methods - Event Handlers
    private void HandleGameStart()
    {
        Debug.Log("���� ���� ��ȣ ����! UI�� �����մϴ�. ī�� ��: " + _cardManager.RemainingCards.Count);

        startPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        _spawnedCardUIs.Clear();

        foreach (var cardData in _cardManager.RemainingCards)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardContainer);
            CardUI cardUI = cardObject.GetComponent<CardUI>();
            cardUI.Initialize(cardData);
            _spawnedCardUIs.Add(cardUI);
        }
    }

    private void HandleCardsUpdated(List<CardData> remainingCards)
    {
        foreach (var cardUI in _spawnedCardUIs)
        {
            bool isRemaining = remainingCards.Contains(cardUI.AssignedCardData);
            cardUI.UpdateVisual(isRemaining);
        }
    }

    private void HandleGuessesUpdated(int remainingGuesses)
    {
        guessesText.text = $"���� ����: {remainingGuesses}";
    }

    private void HandleGameWon()
    {
        winPanel.SetActive(true);
    }

    private void HandleGameLost()
    {
        losePanel.SetActive(true);
    }
    #endregion

    #region Private Methods - Event Subscription
    private void SubscribeToEvents()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameStart += HandleGameStart;
            _gameManager.OnGameWon += HandleGameWon;
            _gameManager.OnGameLost += HandleGameLost;
            _gameManager.OnGuessesUpdated += HandleGuessesUpdated;
        }
        if (_cardManager != null)
        {
            _cardManager.OnCardsUpdated += HandleCardsUpdated;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameStart -= HandleGameStart;
            _gameManager.OnGameWon -= HandleGameWon;
            _gameManager.OnGameLost -= HandleGameLost;
            _gameManager.OnGuessesUpdated -= HandleGuessesUpdated;
        }
        if (_cardManager != null)
        {
            _cardManager.OnCardsUpdated -= HandleCardsUpdated;
        }
    }
    #endregion

    #region Private Methods - Singleton and Logging
    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"UIManager: �ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ�({name})�� �ı��մϴ�.");
            Destroy(gameObject);
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=orange>{message}</color>");
    }
    #endregion
}