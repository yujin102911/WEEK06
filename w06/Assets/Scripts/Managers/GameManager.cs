using System;
using UnityEngine;

/// <summary>게임 상태 열거형</summary>
public enum GameState
{
    NotStarted,
    Playing,
    GameOver,
    GameCleared
}

/// <summary>게임 전체 흐름 제어 및 다른 매니저들을 지회하는 싱글톤 매니저</summary>
public class GameManager : MonoBehaviour
{
    #region Singleton Instance
    private static GameManager _instance;

    public static GameManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("게임 규칙 설정")]
    [SerializeField] private int questionsPerTurn = 2;
    [Header("엔딩 조건 설정")]
    [SerializeField] private int maxTurns = 20;
    [SerializeField] private float gameClearScore = 80f;



    #endregion

    #region Pirvate Fields
    private GameState _currentGameState;
    private int _currentTurn;
    #endregion

    #region Properties
    ///<summary>현재 게임 상태</summary>
    public GameState CurrentGameState => _currentGameState;
    ///<summary>현재 턴 번호</summary>
    public int CurrentTurn => _currentTurn;
    ///<summary>턴 당 질문 가능한 횟수</summary>
    public int QuestionsPerTurn => questionsPerTurn;
    #endregion

    #region Events
    ///<summary>게임 시작 시 발행되는 이벤트</summary>
    public event Action OnGameStart;
    /// <summary>게임 오버 시 발행되는 이벤트</summary>
    public event Action OnGameOver;
    /// <summary>게임 클리어 시 발행되는 이벤트</summary>
    public event Action OnGameClear;
    /// <summary>새로운 턴이 시작될 때 발행되는 이벤트</summary>
    public event Action<int> OnTurnStart;
    /// <summary>현재 턴이 종료될 때 발행되는 이벤트</summary>
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
        // 메모리 누수 방지를 위해 구독했던 이벤트를 모두 해제합니다.
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
    /// <summary>의존성이 필요 없는 내부 초기화</summary>
    private void Initialize()
    {
        SetupSingletonInstance();
        _currentGameState = GameState.NotStarted;
    }
    private void LateInitialize()
    {
        // UI 매니저로부터 채용/반려 버튼 클릭 이벤트를 구독합니다.
        UIManager.Instance.OnRecruitClicked += HandleRecruitChoice;
        UIManager.Instance.OnRejectClicked += HandleRejectChoice;

        // Event 매니저로부터 이벤트 처리 완료(면접 종료) 이벤트를 구독합니다.
        EventManager.Instance.OnEventProcessingCompleted += EndCurrentTurn;
    }

    #endregion

    #region Public Methods - 게임 흐름 제어
    ///<summary>게임 시작</summary>
    public void StartGame()
    {
        if (_currentGameState != GameState.NotStarted)
        {
            LogWarning("게임이 이미 시작되었거나 종료되었습니다.");
            return;
        }
        _currentGameState = GameState.Playing;
        _currentTurn = 0;

        CompanyStatusManager.Instance.ResetStatusToInitial();

        LogMessage("게임 시작");
        OnGameStart?.Invoke();

        StartNextTurn();
    }
    #endregion

    #region Event Handlers - 다른 매니저의 이벤트 처리

    /// <summary>UI 매니저로부터 '채용' 결정을 받았을 때 처리합니다.</summary>
    private void HandleRecruitChoice(ApplicantSO applicant)
    {
        LogMessage($"결정: {applicant.applicantInfo.applicantName} 채용");
        CompanyStatusManager.Instance.RecruitApplicant(applicant);

        // 면접이 끝났으므로 턴 종료 절차를 시작합니다.
        EndCurrentTurn();
    }

    /// <summary>UI 매니저로부터 '반려' 결정을 받았을 때 처리합니다.</summary>
    private void HandleRejectChoice()
    {
        LogMessage("결정: 반려");

        // 면접이 끝났으므로 턴 종료 절차를 시작합니다.
        EndCurrentTurn();
    }
    #endregion

    #region Private Methods - 턴 및 엔딩 관리
    ///<summary>다음 턴을 시작</summary>
    private void StartNextTurn()
    {
        if (_currentGameState != GameState.Playing) return;

        _currentTurn++;
        LogMessage($"--- {_currentTurn}번째 턴 시작 ---");

        OnTurnStart?.Invoke(_currentTurn);

        EventManager.Instance.RequestNewApplicant();
    }

    ///<summary>현재 턴 종료 및 엔딩 조건 확인</summary>
    /// <summary>현재 턴을 종료하고 엔딩 조건을 확인합니다.</summary>
    private void EndCurrentTurn()
    {
        if (_currentGameState != GameState.Playing) return;

        LogMessage($"--- {_currentTurn}번째 턴 종료 ---");
        OnTurnEnd?.Invoke(_currentTurn);

        CheckEndingConditions();
    }

    /// <summary>게임 클리어/오버 조건을 확인하고 상태를 변경합니다.</summary>
    private void CheckEndingConditions()
    {
        var company = CompanyStatusManager.Instance;
        var eventManager = EventManager.Instance;

        // 게임이 끝나야 하는 상황인지 먼저 확인 (직원이 꽉 찼거나, 지원자가 없거나)
        bool isGameEnding = (company.CurrentEmployees >= company.MaxEmployeeCapacity) ||
                            (eventManager.AvailableApplicantCount == 0);

        if (isGameEnding)
        {
            LogMessage("게임 종료 조건 충족! 최종 점수를 계산합니다.");

            // 최종 점수 계산
            float finalScore = company.CurrentEmployees * company.CurrentStatus;
            LogMessage($"최종 점수: {company.CurrentEmployees}(직원 수) * {company.CurrentStatus}(상태) = {finalScore}");

            // 점수에 따라 클리어/오버 결정
            if (finalScore >= gameClearScore)
            {
                TriggerGameClear();
            }
            else
            {
                TriggerGameOver();
            }
            return; // 함수 종료
        }

        // 게임이 끝나지 않았다면 다음 턴 시작
        StartNextTurn();
    }

    /// <summary>게임 클리어 상태로 전환합니다.</summary>
    private void TriggerGameClear()
    {
        if (_currentGameState != GameState.Playing) return; // 중복 호출 방지
        _currentGameState = GameState.GameCleared;
        LogMessage("--- 게임 클리어! ---");
        OnGameClear?.Invoke(); // UIManager에 알림
    }

    /// <summary>게임 오버 상태로 전환합니다.</summary>
    private void TriggerGameOver()
    {
        if (_currentGameState != GameState.Playing) return; // 중복 호출 방지
        _currentGameState = GameState.GameOver;
        LogMessage("--- 게임 오버 ---");
        OnGameOver?.Invoke(); // UIManager에 알림
    }
    #endregion

    #region Private Methods - 유틸리티
    /// <summary>싱글톤 인스턴스 설정 및 중복 방지</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"중복 인스턴스 감지. 현재 인스턴스를 파괴합니다. (기존: {_instance.name}, 신규: {name})");
            Destroy(gameObject);
        }
    }

    /// <summary>로그 출력</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=magenta>{gameObject.name}: {message}</color>");
    }

    /// <summary>경고 로그 출력</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=magenta>{gameObject.name}: {message}</color>");
    }
    #endregion
}

