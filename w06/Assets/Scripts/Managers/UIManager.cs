using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>게임의 모든 UI 요소를 관리하고 업데이트하는 싱글톤 매니저</summary>
public class UIManager : MonoBehaviour
{
    #region Singleton Instance
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("메인 UI 패널")]
    [SerializeField] private GameObject _interviewPanel; // 면접 전체 UI를 담는 부모 패널

    [Header("상단 정보 UI")]
    [SerializeField] private TMP_Text _employeeCountText; // 현재/최대 직원 수
    [SerializeField] private TMP_Text _applicantsRemainingText; // 남은 지원자 수
    [SerializeField] private TMP_Text _questionsRemainingText; // 남은 질문 횟수

    [Header("이력서 UI")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _ageText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _careerHistoryText;

    [Header("상호작용 UI")]
    [SerializeField] private GameObject _questionListView; // 질문 버튼들이 있는 뷰
    [SerializeField] private GameObject _answerView; // 답변이 표시되는 뷰
    [SerializeField] private TMP_Text _answerText;
    [SerializeField] private Button _backToQuestionsButton;

    [Header("질문 버튼 관련")]
    [SerializeField] private GameObject _questionButtonPrefab;
    [SerializeField] private Transform _questionButtonContainer;

    [Header("결정 버튼 UI")]
    [SerializeField] private Button _recruitButton;
    [SerializeField] private Button _rejectButton;

    [Header("게임 결과 패널")]
    [SerializeField] private GameObject _gameClearPanel; // 게임 클리어 시 켤 패널
    [SerializeField] private GameObject _gameOverPanel;  // 게임 오버 시 켤 패널

    #endregion

    #region Private Fields
    private ApplicantSO _currentApplicant;
    private int _questionsRemaining;
    private List<Button> _generatedQuestionButtons = new List<Button>();
    #endregion

    #region Events
    public event Action<ApplicantSO> OnRecruitClicked;
    public event Action OnRejectClicked;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += HandleGameStart;
        EventManager.Instance.OnNewApplicantDrawn += HandleNewApplicant;
        CompanyStatusManager.Instance.OnStatusUpdated += UpdateCounters;
        GameManager.Instance.OnGameClear += ShowGameClearPanel; // 클리어 이벤트 구독
        GameManager.Instance.OnGameOver += ShowGameOverPanel;   // 게임오버 이벤트 구독
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= HandleGameStart;
            GameManager.Instance.OnGameClear -= ShowGameClearPanel;
            GameManager.Instance.OnGameOver -= ShowGameOverPanel;
        }
        if (EventManager.Instance != null) EventManager.Instance.OnNewApplicantDrawn -= HandleNewApplicant;
        if (CompanyStatusManager.Instance != null) CompanyStatusManager.Instance.OnStatusUpdated -= UpdateCounters;
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        SetupSingletonInstance();
        _recruitButton.onClick.AddListener(OnRecruitButtonPressed);
        _rejectButton.onClick.AddListener(OnRejectButtonPressed);
        _backToQuestionsButton.onClick.AddListener(ShowQuestionView);

        // 게임 시작 전에는 면접 UI를 숨김
        _interviewPanel.SetActive(false);
        _answerView.SetActive(false);
        _gameClearPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
    }
    #endregion

    #region Event Handlers
    private void HandleGameStart()
    {
        _interviewPanel.SetActive(true);
        UpdateCounters();
    }

    private void HandleGameEnd()
    {
        _interviewPanel.SetActive(false);
        // 여기에 게임 결과 UI를 띄우는 로직을 추가하면 됩니다.
        // 예: _resultPanel.SetActive(true);
    }

    private void HandleNewApplicant(ApplicantSO applicant)
    {
        _currentApplicant = applicant;
        _questionsRemaining = GameManager.Instance.QuestionsPerTurn; // 턴마다 질문 횟수 초기화

        UpdateApplicantInfoUI();
        GenerateQuestionButtons();
        UpdateCounters();
        ShowQuestionView();
    }
    #endregion

    #region Public Methods

    /// <summary>게임 클리어 패널을 표시합니다.</summary>
    public void ShowGameClearPanel()
    {
        _interviewPanel.SetActive(false);
        _gameClearPanel.SetActive(true);
    }

    /// <summary>게임 오버 패널을 표시합니다.</summary>
    public void ShowGameOverPanel()
    {
        _interviewPanel.SetActive(false);
        _gameOverPanel.SetActive(true);
    }

    #endregion

    #region Private Methods - UI 업데이트 및 제어
    private void UpdateCounters()
    {
        var company = CompanyStatusManager.Instance;
        _employeeCountText.text = $"직원: {company.CurrentEmployees} / {company.MaxEmployeeCapacity}";
        _applicantsRemainingText.text = $"남은 지원자: {EventManager.Instance.AvailableApplicantCount}";
        _questionsRemainingText.text = $"질문 기회: {_questionsRemaining}";
    }

    private void UpdateApplicantInfoUI()
    {
        var info = _currentApplicant.applicantInfo;
        _nameText.text = info.applicantName;
        _ageText.text = info.age.ToString() + "세";
        _portraitImage.sprite = info.portrait;
        _careerHistoryText.text = string.Join("\n", info.careerHistory);
    }

    private void GenerateQuestionButtons()
    {
        Debug.Log($"질문 버튼 생성 시작! 프리팹: {_questionButtonPrefab.name}, 컨테이너: {_questionButtonContainer.name}, 질문 개수: {_currentApplicant.applicantInfo.interviewQuestions.Count}");

        foreach (Button button in _generatedQuestionButtons)
        {
            Destroy(button.gameObject);
        }
        _generatedQuestionButtons.Clear();

        var questions = _currentApplicant.applicantInfo.interviewQuestions;
        for (int i = 0; i < questions.Count; i++)
        {
            GameObject buttonGO = Instantiate(_questionButtonPrefab, _questionButtonContainer);
            buttonGO.GetComponentInChildren<TMP_Text>().text = questions[i];

            Button button = buttonGO.GetComponent<Button>();
            int questionIndex = i;
            button.onClick.AddListener(() => OnQuestionButtonClicked(questionIndex, button));
            _generatedQuestionButtons.Add(button);
        }
    }

    private void OnQuestionButtonClicked(int index, Button clickedButton)
    {
        if (_questionsRemaining <= 0)
        {
            LogMessage("질문 기회를 모두 사용했습니다.");
            return;
        }

        _questionsRemaining--;
        UpdateCounters();

        _answerText.text = _currentApplicant.applicantInfo.interviewAnswers[index];
        clickedButton.interactable = false; // 클릭된 버튼은 비활성화

        ShowAnswerView();
    }
    #endregion

    #region Private Methods - 뷰 전환
    private void ShowQuestionView()
    {
        _questionListView.SetActive(true);
        _answerView.SetActive(false);
    }

    private void ShowAnswerView()
    {
        _questionListView.SetActive(false);
        _answerView.SetActive(true);
    }
    #endregion

    #region Private Methods - 버튼 클릭 처리
    private void OnRecruitButtonPressed()
    {
        OnRecruitClicked?.Invoke(_currentApplicant);
    }

    private void OnRejectButtonPressed()
    {
        OnRejectClicked?.Invoke();
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
        Debug.Log($"{gameObject.name}: {message}");
    }

    /// <summary>경고 로그 출력</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"{gameObject.name}: {message}");
    }
    #endregion
}