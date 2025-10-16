using System;
using UnityEngine;

/// <summary>���� ���� ������</summary>
public enum GameState
{
    NotStarted,
    Playing,
    GameOver,
    GameCleared
}

/// <summary>���� ��ü �帧 ���� �� �ٸ� �Ŵ������� ��ȸ�ϴ� �̱��� �Ŵ���</summary>
public class GameManager : MonoBehaviour
{
    #region Singleton Instance
    private static GameManager _instance;

    public static GameManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("���� ��Ģ ����")]
    [SerializeField] private int questionsPerTurn = 2;
    [Header("���� ���� ����")]
    [SerializeField] private int maxTurns = 20;
    [SerializeField] private float gameClearScore = 80f;



    #endregion

    #region Pirvate Fields
    private GameState _currentGameState;
    private int _currentTurn;
    #endregion

    #region Properties
    ///<summary>���� ���� ����</summary>
    public GameState CurrentGameState => _currentGameState;
    ///<summary>���� �� ��ȣ</summary>
    public int CurrentTurn => _currentTurn;
    ///<summary>�� �� ���� ������ Ƚ��</summary>
    public int QuestionsPerTurn => questionsPerTurn;
    #endregion

    #region Events
    ///<summary>���� ���� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameStart;
    /// <summary>���� ���� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameOver;
    /// <summary>���� Ŭ���� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnGameClear;
    /// <summary>���ο� ���� ���۵� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action<int> OnTurnStart;
    /// <summary>���� ���� ����� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action<int> OnTurnEnd;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        LateInitialize();
    }
    private void OnDestroy()
    {
        // �޸� ���� ������ ���� �����ߴ� �̺�Ʈ�� ��� �����մϴ�.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnRecruitClicked -= HandleRecruitChoice;
            UIManager.Instance.OnRejectClicked -= HandleRejectChoice;
        }
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEventProcessingCompleted -= EndCurrentTurn;
        }
    }
    #endregion

    #region Initialization
    /// <summary>�������� �ʿ� ���� ���� �ʱ�ȭ</summary>
    private void Initialize()
    {
        SetupSingletonInstance();
        _currentGameState = GameState.NotStarted;
    }
    private void LateInitialize()
    {
        // UI �Ŵ����κ��� ä��/�ݷ� ��ư Ŭ�� �̺�Ʈ�� �����մϴ�.
        UIManager.Instance.OnRecruitClicked += HandleRecruitChoice;
        UIManager.Instance.OnRejectClicked += HandleRejectChoice;

        // Event �Ŵ����κ��� �̺�Ʈ ó�� �Ϸ�(���� ����) �̺�Ʈ�� �����մϴ�.
        EventManager.Instance.OnEventProcessingCompleted += EndCurrentTurn;
    }

    #endregion

    #region Public Methods - ���� �帧 ����
    ///<summary>���� ����</summary>
    public void StartGame()
    {
        if (_currentGameState != GameState.NotStarted)
        {
            LogWarning("������ �̹� ���۵Ǿ��ų� ����Ǿ����ϴ�.");
            return;
        }
        _currentGameState = GameState.Playing;
        _currentTurn = 0;

        CompanyStatusManager.Instance.ResetStatusToInitial();

        LogMessage("���� ����");
        OnGameStart?.Invoke();

        StartNextTurn();
    }
    #endregion

    #region Event Handlers - �ٸ� �Ŵ����� �̺�Ʈ ó��

    /// <summary>UI �Ŵ����κ��� 'ä��' ������ �޾��� �� ó���մϴ�.</summary>
    private void HandleRecruitChoice(ApplicantSO applicant)
    {
        LogMessage($"����: {applicant.applicantInfo.applicantName} ä��");
        CompanyStatusManager.Instance.RecruitApplicant(applicant);

        // ������ �������Ƿ� �� ���� ������ �����մϴ�.
        EndCurrentTurn();
    }

    /// <summary>UI �Ŵ����κ��� '�ݷ�' ������ �޾��� �� ó���մϴ�.</summary>
    private void HandleRejectChoice()
    {
        LogMessage("����: �ݷ�");

        // ������ �������Ƿ� �� ���� ������ �����մϴ�.
        EndCurrentTurn();
    }
    #endregion

    #region Private Methods - �� �� ���� ����
    ///<summary>���� ���� ����</summary>
    private void StartNextTurn()
    {
        if (_currentGameState != GameState.Playing) return;

        _currentTurn++;
        LogMessage($"--- {_currentTurn}��° �� ���� ---");

        OnTurnStart?.Invoke(_currentTurn);

        EventManager.Instance.RequestNewApplicant();
    }

    ///<summary>���� �� ���� �� ���� ���� Ȯ��</summary>
    /// <summary>���� ���� �����ϰ� ���� ������ Ȯ���մϴ�.</summary>
    private void EndCurrentTurn()
    {
        if (_currentGameState != GameState.Playing) return;

        LogMessage($"--- {_currentTurn}��° �� ���� ---");
        OnTurnEnd?.Invoke(_currentTurn);

        CheckEndingConditions();
    }

    /// <summary>���� Ŭ����/���� ������ Ȯ���ϰ� ���¸� �����մϴ�.</summary>
    private void CheckEndingConditions()
    {
        var company = CompanyStatusManager.Instance;
        var eventManager = EventManager.Instance;

        // ������ ������ �ϴ� ��Ȳ���� ���� Ȯ�� (������ �� á�ų�, �����ڰ� ���ų�)
        bool isGameEnding = (company.CurrentEmployees >= company.MaxEmployeeCapacity) ||
                            (eventManager.AvailableApplicantCount == 0);

        if (isGameEnding)
        {
            LogMessage("���� ���� ���� ����! ���� ������ ����մϴ�.");

            // ���� ���� ���
            float finalScore = company.CurrentEmployees * company.CurrentStatus;
            LogMessage($"���� ����: {company.CurrentEmployees}(���� ��) * {company.CurrentStatus}(����) = {finalScore}");

            // ������ ���� Ŭ����/���� ����
            if (finalScore >= gameClearScore)
            {
                TriggerGameClear();
            }
            else
            {
                TriggerGameOver();
            }
            return; // �Լ� ����
        }

        // ������ ������ �ʾҴٸ� ���� �� ����
        StartNextTurn();
    }

    /// <summary>���� Ŭ���� ���·� ��ȯ�մϴ�.</summary>
    private void TriggerGameClear()
    {
        if (_currentGameState != GameState.Playing) return; // �ߺ� ȣ�� ����
        _currentGameState = GameState.GameCleared;
        LogMessage("--- ���� Ŭ����! ---");
        OnGameClear?.Invoke(); // UIManager�� �˸�
    }

    /// <summary>���� ���� ���·� ��ȯ�մϴ�.</summary>
    private void TriggerGameOver()
    {
        if (_currentGameState != GameState.Playing) return; // �ߺ� ȣ�� ����
        _currentGameState = GameState.GameOver;
        LogMessage("--- ���� ���� ---");
        OnGameOver?.Invoke(); // UIManager�� �˸�
    }
    #endregion

    #region Private Methods - ��ƿ��Ƽ
    /// <summary>�̱��� �ν��Ͻ� ���� �� �ߺ� ����</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"�ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ��� �ı��մϴ�. (����: {_instance.name}, �ű�: {name})");
            Destroy(gameObject);
        }
    }

    /// <summary>�α� ���</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=magenta>{gameObject.name}: {message}</color>");
    }

    /// <summary>��� �α� ���</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=magenta>{gameObject.name}: {message}</color>");
    }
    #endregion
}

