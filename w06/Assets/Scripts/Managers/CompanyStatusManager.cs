using System;
using UnityEngine;

/// <summary>ȸ���� ���¸� �����ϴ� �̱��� �Ŵ���</summary>
public class CompanyStatusManager : MonoBehaviour
{
    #region Singleton Instance
    private static CompanyStatusManager _instance;

    public static CompanyStatusManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("ȸ�� �ʱ� ����")]
    [SerializeField] private int maxEmployeeCapacity = 10;
    [SerializeField] private int initialEmployeeCount = 0;
    [SerializeField] private float initialStatus = 0f;


    [Header("���°� ���� ����")]
    [SerializeField] private int maxStatusValue = 100;
    [SerializeField] private int minStatusValue = -100;
    #endregion

    #region Private Fields
    private int _currentEmployees;
    private float _currentStatus;
    #endregion

    #region Properties
    ///<summary>���� ���� ��</summary>
    public int CurrentEmployees => _currentEmployees;
    /// <summary>���� ȸ�� ����</summary>
    public float CurrentStatus => _currentStatus;
    /// <summary>�ִ� ��� ���� ��</summary>
    public int MaxEmployeeCapacity => maxEmployeeCapacity;
    #endregion

    #region Events
    public event Action OnStatusUpdated; //ȸ�� ���°� ������Ʈ�� �� ����Ǵ� �̺�Ʈ
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    /// <summary>�������� �ʿ� ���� ���� �ʱ�ȭ</summary>
    private void Initialize()
    {
        SetupSingletonInstance(); //�̱���
        ResetStatusToInitial(); //���� �ʱ�ȭ
    }
    #endregion

    #region Public Methods
    ///<summary>
    ///���ο� �����ڸ� ä��, �� ������� ȸ�� ���¿� �ݿ�
    ///</summary>
    public void RecruitApplicant(ApplicantSO applicant)
    {
        if (!CanRecruit(applicant)) { return; } //��� ������ ���°� �ƴϸ� return
        
        _currentEmployees++; //�� ��� �ο� �߰�
        _currentStatus += applicant.impact; //ȸ�翡 ������ ����� ����

        ClampAllStatus(); //���� ���� ������ ������ ����� �ʵ��� ����
        LogMessage($"{applicant.applicantInfo.applicantName}ä�� �Ϸ� | ���� ���� ��: {_currentEmployees} | ���� ȸ�� ����: {_currentStatus}");

        OnStatusUpdated?.Invoke(); //�̺�Ʈ�� �����Ͽ� ���°� ������Ʈ �Ǿ����� �˸�

        return; //��� ����

    }
    ///<summary>
    ///��� ȸ�� ���¸� �ʱⰪ���� ����
    ///</summary>
    public void ResetStatusToInitial()
    {
        _currentEmployees = initialEmployeeCount;
        _currentStatus = initialStatus;

        LogMessage("ȸ�� ���°� �ʱⰪ���� ����");

        OnStatusUpdated?.Invoke();
    }
    #endregion


    #region Private Methods
    ///<summary>��� ���� ���� ����</summary>
    private bool CanRecruit(ApplicantSO applicant)
    {
        if (_currentEmployees >= maxEmployeeCapacity)
        {
            LogWarning($"{applicant.applicantInfo.applicantName} ��� �Ұ�");
            return false;
        }
        return true;
    }

    #endregion

    #region ���� ����
    ///<summary>��� ���� ������ Min/Max ���̷� ����</summary>
    private void ClampAllStatus()
    {
        _currentStatus = Mathf.Clamp(_currentStatus, minStatusValue, maxStatusValue);

    }

    ///<summary>�̱��� �ν��Ͻ� ���� �� �ߺ� ����</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"CompanyStatusManager: �ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ��� �ı��մϴ�. (����: {_instance.name}, �ű�: {name})");
            Destroy(gameObject);
        }
    }

    ///<summary>�α� ���</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"{gameObject.name}:{message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"{gameObject.name}: {message}");
    }
    #endregion

}
