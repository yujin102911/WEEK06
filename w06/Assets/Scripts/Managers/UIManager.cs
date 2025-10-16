using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>������ ��� UI ��Ҹ� �����ϰ� ������Ʈ�ϴ� �̱��� �Ŵ���</summary>
public class UIManager : MonoBehaviour
{
    #region Singleton Instance
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("���� UI �г�")]
    [SerializeField] private GameObject _interviewPanel; // ���� ��ü UI�� ��� �θ� �г�

    [Header("��� ���� UI")]
    [SerializeField] private TMP_Text _employeeCountText; // ����/�ִ� ���� ��
    [SerializeField] private TMP_Text _applicantsRemainingText; // ���� ������ ��
    [SerializeField] private TMP_Text _questionsRemainingText; // ���� ���� Ƚ��

    [Header("�̷¼� UI")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _ageText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _careerHistoryText;

    [Header("��ȣ�ۿ� UI")]
    [SerializeField] private GameObject _questionListView; // ���� ��ư���� �ִ� ��
    [SerializeField] private GameObject _answerView; // �亯�� ǥ�õǴ� ��
    [SerializeField] private TMP_Text _answerText;
    [SerializeField] private Button _backToQuestionsButton;

    [Header("���� ��ư ����")]
    [SerializeField] private GameObject _questionButtonPrefab;
    [SerializeField] private Transform _questionButtonContainer;

    [Header("���� ��ư UI")]
    [SerializeField] private Button _recruitButton;
    [SerializeField] private Button _rejectButton;

    [Header("���� ��� �г�")]
    [SerializeField] private GameObject _gameClearPanel; // ���� Ŭ���� �� �� �г�
    [SerializeField] private GameObject _gameOverPanel;  // ���� ���� �� �� �г�

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
        GameManager.Instance.OnGameClear += ShowGameClearPanel; // Ŭ���� �̺�Ʈ ����
        GameManager.Instance.OnGameOver += ShowGameOverPanel;   // ���ӿ��� �̺�Ʈ ����
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

        // ���� ���� ������ ���� UI�� ����
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
        // ���⿡ ���� ��� UI�� ���� ������ �߰��ϸ� �˴ϴ�.
        // ��: _resultPanel.SetActive(true);
    }

    private void HandleNewApplicant(ApplicantSO applicant)
    {
        _currentApplicant = applicant;
        _questionsRemaining = GameManager.Instance.QuestionsPerTurn; // �ϸ��� ���� Ƚ�� �ʱ�ȭ

        UpdateApplicantInfoUI();
        GenerateQuestionButtons();
        UpdateCounters();
        ShowQuestionView();
    }
    #endregion

    #region Public Methods

    /// <summary>���� Ŭ���� �г��� ǥ���մϴ�.</summary>
    public void ShowGameClearPanel()
    {
        _interviewPanel.SetActive(false);
        _gameClearPanel.SetActive(true);
    }

    /// <summary>���� ���� �г��� ǥ���մϴ�.</summary>
    public void ShowGameOverPanel()
    {
        _interviewPanel.SetActive(false);
        _gameOverPanel.SetActive(true);
    }

    #endregion

    #region Private Methods - UI ������Ʈ �� ����
    private void UpdateCounters()
    {
        var company = CompanyStatusManager.Instance;
        _employeeCountText.text = $"����: {company.CurrentEmployees} / {company.MaxEmployeeCapacity}";
        _applicantsRemainingText.text = $"���� ������: {EventManager.Instance.AvailableApplicantCount}";
        _questionsRemainingText.text = $"���� ��ȸ: {_questionsRemaining}";
    }

    private void UpdateApplicantInfoUI()
    {
        var info = _currentApplicant.applicantInfo;
        _nameText.text = info.applicantName;
        _ageText.text = info.age.ToString() + "��";
        _portraitImage.sprite = info.portrait;
        _careerHistoryText.text = string.Join("\n", info.careerHistory);
    }

    private void GenerateQuestionButtons()
    {
        Debug.Log($"���� ��ư ���� ����! ������: {_questionButtonPrefab.name}, �����̳�: {_questionButtonContainer.name}, ���� ����: {_currentApplicant.applicantInfo.interviewQuestions.Count}");

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
            LogMessage("���� ��ȸ�� ��� ����߽��ϴ�.");
            return;
        }

        _questionsRemaining--;
        UpdateCounters();

        _answerText.text = _currentApplicant.applicantInfo.interviewAnswers[index];
        clickedButton.interactable = false; // Ŭ���� ��ư�� ��Ȱ��ȭ

        ShowAnswerView();
    }
    #endregion

    #region Private Methods - �� ��ȯ
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

    #region Private Methods - ��ư Ŭ�� ó��
    private void OnRecruitButtonPressed()
    {
        OnRecruitClicked?.Invoke(_currentApplicant);
    }

    private void OnRejectButtonPressed()
    {
        OnRejectClicked?.Invoke();
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
        Debug.Log($"{gameObject.name}: {message}");
    }

    /// <summary>��� �α� ���</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"{gameObject.name}: {message}");
    }
    #endregion
}