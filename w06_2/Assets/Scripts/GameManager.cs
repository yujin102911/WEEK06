using UnityEngine;
using System; // event Action을 사용하기 위해 필요

/// <summary>게임 상태를 정의하는 열거형</summary>
public enum GameState
{
    NotStarted,
    Playing,
    GameWon,
    GameLost
}

/// <summary>
/// 게임의 전체 흐름(시작, 종료, 승리/패배 판정)을 제어하는 싱글톤 매니저
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton Instance
    private static GameManager _instance;

    /// <summary>GameManager 싱글톤 인스턴스</summary>
    public static GameManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [SerializeField]
    [Tooltip("플레이어가 질문할 수 있는 최대 횟수")]
    private int maxGuesses = 6;
    #endregion

    #region Private Fields
    private GameState _currentGameState;
    private int _remainingGuesses;

    private CardManager _cardManager;
    private UIManager _uiManager; 
    #endregion

    #region Properties
    /// <summary>현재 게임 상태</summary>
    public GameState CurrentGameState => _currentGameState;

    /// <summary>남은 질문 횟수</summary>
    public int RemainingGuesses => _remainingGuesses;
    #endregion

    #region Events
    /// <summary>게임이 시작될 때 발행되는 이벤트</summary>
    public event Action OnGameStart;

    /// <summary>게임에서 승리했을 때 발행되는 이벤트</summary>
    public event Action OnGameWon;

    /// <summary>게임에서 패배했을 때 발행되는 이벤트</summary>
    public event Action OnGameLost;

    /// <summary>남은 질문 횟수가 변경될 때 발행되는 이벤트</summary>
    public event Action<int> OnGuessesUpdated;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        SetupSingletonInstance();
        _currentGameState = GameState.NotStarted;

        // 다른 매니저 인스턴스 찾아오기
        _cardManager = CardManager.Instance;
        _uiManager = UIManager.Instance;
        if (_cardManager == null)
        {
            LogWarning("CardManager 인스턴스를 찾을 수 없습니다.");
        }
        if (_uiManager == null)
        {
            LogWarning("UIManager 인스턴스를 찾을 수 없습니다.");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 새로운 게임을 시작합니다.
    /// </summary>
    public void StartGame()
    {
        if (_currentGameState != GameState.NotStarted)
        {
            LogWarning("GameManager: 이미 게임이 진행 중입니다. 새로운 게임을 시작할 수 없습니다.");
            return;
        }

        ChangeGameState(GameState.Playing);
        _remainingGuesses = maxGuesses;

        _cardManager.StartNewGame();

        OnGameStart?.Invoke();
        OnGuessesUpdated?.Invoke(_remainingGuesses); // UI에 초기 횟수 업데이트

        //_uiManager.StartPanel.SetActive(false);
        LogMessage("새로운 게임을 시작합니다.");
    }

    /// <summary>
    /// 플레이어가 선택한 질문을 처리합니다.
    /// </summary>
    /// <param name="question">플레이어가 선택한 질문 데이터</param>
    public void SelectQuestion(QuestionData question)
    {
        if (_currentGameState != GameState.Playing)
        {
            LogWarning("GameManager: 게임이 진행 중일 때만 질문을 선택할 수 있습니다.");
            return;
        }

        // 1. 질문 횟수 차감
        _remainingGuesses--;
        OnGuessesUpdated?.Invoke(_remainingGuesses);
        LogMessage($"질문 선택: '{question.QuestionText}'. 남은 횟수: {_remainingGuesses}");

        // 2. 정답 카드가 질문에 해당하는지 판별
        bool isTrueForAnswer = question.Evaluate(_cardManager.AnswerCard);

        // 3. CardManager에 결과를 알려 카드 목록을 필터링하도록 지시
        _cardManager.FilterCards(question, isTrueForAnswer);

        // 4. 게임 종료 조건 확인
        CheckEndConditions();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 승리 또는 패배 조건을 확인하고 게임 상태를 변경합니다.
    /// </summary>
    private void CheckEndConditions()
    {
        // 승리 조건: 남은 카드가 1장일 때
        if (_cardManager.RemainingCards.Count == 1)
        {
            ChangeGameState(GameState.GameWon);
            OnGameWon?.Invoke();
            LogMessage("승리! 정답을 맞췄습니다.");
            return;
        }

        // 패배 조건: 남은 카드가 2장 이상인데 질문 횟수를 모두 소진했을 때
        if (_remainingGuesses <= 0 && _cardManager.RemainingCards.Count > 1)
        {
            ChangeGameState(GameState.GameLost);
            OnGameLost?.Invoke();
            LogMessage("패배. 질문 횟수를 모두 사용했습니다.");
            return;
        }
    }
    #endregion

    #region 유틸리티 및 검증
    /// <summary>싱글톤 인스턴스를 설정합니다.</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"GameManager: 중복 인스턴스 감지. 현재 인스턴스({name})를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    /// <summary>게임 상태를 변경하고 로그를 출력합니다.</summary>
    private void ChangeGameState(GameState newState)
    {
        if (_currentGameState == newState) return;

        GameState previousState = _currentGameState;
        _currentGameState = newState;
        LogMessage($"게임 상태 변경: [{previousState}] -> [{newState}]");
    }

    /// <summary>정보 로그를 마젠타색으로 출력합니다.</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=magenta>{message}</color>");
    }

    /// <summary>경고 로그를 마젠타색으로 출력합니다.</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=magenta>{message}</color>");
    }
    #endregion


}
