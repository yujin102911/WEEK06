using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 게임의 모든 UI 요소(카드, 버튼, 텍스트)를 제어하는 싱글톤 매니저
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

        // Start()보다 먼저 실행되는 Awake()에서 다른 매니저 인스턴스를 찾아옵니다.
        _gameManager = GameManager.Instance;
        _cardManager = CardManager.Instance;
    }

    private void Start()
    {
        // UI 초기 상태 설정은 Start()에서 합니다.
        SetupInitialUI();
    }

    private void OnEnable()
    {
        // Awake()에서 이미 _gameManager를 찾아왔으므로, 여기서 이벤트 구독이 안전하게 실행됩니다.
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    /// <summary>초기 UI 상태와 버튼을 설정합니다.</summary>
    private void SetupInitialUI()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        startPanel.SetActive(true);

        SetupQuestionButtons();
    }

    /// <summary>질문 버튼들의 클릭 이벤트를 설정합니다.</summary>
    private void SetupQuestionButtons()
    {
        if (questionButtons.Count != questionDatas.Count)
        {
            LogWarning("질문 버튼과 질문 데이터의 개수가 일치하지 않습니다.");
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
        Debug.Log("게임 시작 신호 수신! UI를 설정합니다. 카드 수: " + _cardManager.RemainingCards.Count);

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
        guessesText.text = $"남은 질문: {remainingGuesses}";
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
            LogWarning($"UIManager: 중복 인스턴스 감지. 현재 인스턴스({name})를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=orange>{message}</color>");
    }
    #endregion
}