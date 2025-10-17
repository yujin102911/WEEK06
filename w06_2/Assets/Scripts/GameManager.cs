using UnityEngine;
using System; // event Action�� ����ϱ� ���� �ʿ�

/// <summary>���� ���¸� �����ϴ� ������</summary>
public enum GameState
{
    NotStarted,
    Playing,
    GameWon,
    GameLost
}

/// <summary>
/// ������ ��ü �帧(����, ����, �¸�/�й� ����)�� �����ϴ� �̱��� �Ŵ���
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton Instance
    private static GameManager _instance;

    /// <summary>GameManager �̱��� �ν��Ͻ�</summary>
    public static GameManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [SerializeField]
    [Tooltip("�÷��̾ ������ �� �ִ� �ִ� Ƚ��")]
    private int maxGuesses = 6;
    #endregion

    #region Private Fields
    private GameState _currentGameState;
    private int _remainingGuesses;

    private CardManager _cardManager;
    private UIManager _uiManager; 
    #endregion

    #region Properties
    /// <summary>���� ���� ����</summary>
    public GameState CurrentGameState => _currentGameState;

    /// <summary>���� ���� Ƚ��</summary>
    public int RemainingGuesses => _remainingGuesses;
    #endregion

    #region Events
    /// <summary>������ ���۵� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameStart;

    /// <summary>���ӿ��� �¸����� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameWon;

    /// <summary>���ӿ��� �й����� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameLost;

    /// <summary>���� ���� Ƚ���� ����� �� ����Ǵ� �̺�Ʈ</summary>
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

        // �ٸ� �Ŵ��� �ν��Ͻ� ã�ƿ���
        _cardManager = CardManager.Instance;
        _uiManager = UIManager.Instance;
        if (_cardManager == null)
        {
            LogWarning("CardManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
        if (_uiManager == null)
        {
            LogWarning("UIManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// ���ο� ������ �����մϴ�.
    /// </summary>
    public void StartGame()
    {
        if (_currentGameState != GameState.NotStarted)
        {
            LogWarning("GameManager: �̹� ������ ���� ���Դϴ�. ���ο� ������ ������ �� �����ϴ�.");
            return;
        }

        ChangeGameState(GameState.Playing);
        _remainingGuesses = maxGuesses;

        _cardManager.StartNewGame();

        OnGameStart?.Invoke();
        OnGuessesUpdated?.Invoke(_remainingGuesses); // UI�� �ʱ� Ƚ�� ������Ʈ

        //_uiManager.StartPanel.SetActive(false);
        LogMessage("���ο� ������ �����մϴ�.");
    }

    /// <summary>
    /// �÷��̾ ������ ������ ó���մϴ�.
    /// </summary>
    /// <param name="question">�÷��̾ ������ ���� ������</param>
    public void SelectQuestion(QuestionData question)
    {
        if (_currentGameState != GameState.Playing)
        {
            LogWarning("GameManager: ������ ���� ���� ���� ������ ������ �� �ֽ��ϴ�.");
            return;
        }

        // 1. ���� Ƚ�� ����
        _remainingGuesses--;
        OnGuessesUpdated?.Invoke(_remainingGuesses);
        LogMessage($"���� ����: '{question.QuestionText}'. ���� Ƚ��: {_remainingGuesses}");

        // 2. ���� ī�尡 ������ �ش��ϴ��� �Ǻ�
        bool isTrueForAnswer = question.Evaluate(_cardManager.AnswerCard);

        // 3. CardManager�� ����� �˷� ī�� ����� ���͸��ϵ��� ����
        _cardManager.FilterCards(question, isTrueForAnswer);

        // 4. ���� ���� ���� Ȯ��
        CheckEndConditions();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// �¸� �Ǵ� �й� ������ Ȯ���ϰ� ���� ���¸� �����մϴ�.
    /// </summary>
    private void CheckEndConditions()
    {
        // �¸� ����: ���� ī�尡 1���� ��
        if (_cardManager.RemainingCards.Count == 1)
        {
            ChangeGameState(GameState.GameWon);
            OnGameWon?.Invoke();
            LogMessage("�¸�! ������ ������ϴ�.");
            return;
        }

        // �й� ����: ���� ī�尡 2�� �̻��ε� ���� Ƚ���� ��� �������� ��
        if (_remainingGuesses <= 0 && _cardManager.RemainingCards.Count > 1)
        {
            ChangeGameState(GameState.GameLost);
            OnGameLost?.Invoke();
            LogMessage("�й�. ���� Ƚ���� ��� ����߽��ϴ�.");
            return;
        }
    }
    #endregion

    #region ��ƿ��Ƽ �� ����
    /// <summary>�̱��� �ν��Ͻ��� �����մϴ�.</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"GameManager: �ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ�({name})�� �ı��մϴ�.");
            Destroy(gameObject);
        }
    }

    /// <summary>���� ���¸� �����ϰ� �α׸� ����մϴ�.</summary>
    private void ChangeGameState(GameState newState)
    {
        if (_currentGameState == newState) return;

        GameState previousState = _currentGameState;
        _currentGameState = newState;
        LogMessage($"���� ���� ����: [{previousState}] -> [{newState}]");
    }

    /// <summary>���� �α׸� ����Ÿ������ ����մϴ�.</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=magenta>{message}</color>");
    }

    /// <summary>��� �α׸� ����Ÿ������ ����մϴ�.</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=magenta>{message}</color>");
    }
    #endregion


}
