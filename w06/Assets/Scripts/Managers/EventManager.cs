using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>���� �̺�Ʈ�� �����ϰ� �����ϴ� �̱��� �Ŵ���</summary>
public class EventManager : MonoBehaviour
{
    #region Singleton Instance
    private static EventManager _instance;

    public static EventManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("�̺�Ʈ Ǯ ����")]
    [SerializeField] private List<ApplicantSO> _applicantPool;
    #endregion

    #region Private Fields
    ///<summary>���� ���� ���ǿ��� ��� ������ ������ ���(�ߺ� ������)</summary>
    private List<ApplicantSO> _availableApplicants;

    ///<summary>���� ���� ���� ������ ������ SO</summary>
    private ApplicantSO _currentApplicant;
    #endregion

    #region Properties
    ///<summary>���� ���� ���� ������ ������ ����</summary>
    public ApplicantSO CurrentApplicant => _currentApplicant;
    ///<summary>��� ������ Ǯ�� �����ִ� ������ ��</summary>
    public int AvailableApplicantCount => _availableApplicants.Count;

    #endregion

    #region Events
    ///<summary>���ο� �����ڸ� �̾��� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action<ApplicantSO> OnNewApplicantDrawn;

    ///<summary>���� �̺�Ʈ ó���� �Ϸ�Ǿ��� �� ����Ǵ� �̺�Ʈ</summary>
    public event Action OnEventProcessingCompleted;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    ///<summary>�������� �ʿ� ���� ���� �ʱ�ȭ</summary>
    private void Initialize()
    {
        SetupSingletonInstance();
        //��� ������ ������ Ǯ�� �ʱ�ȭ�ϰ� ������ Ǯ�� ��� �����͸� ����(���� ��ȣ�� ���� ���纻�� ���)
        _availableApplicants = new List<ApplicantSO>(_applicantPool);
    }
    #endregion

    #region Public Methods
    ///<summary>��� ������ Ǯ���� ���ο� �����ڸ� �������� ��û�ϰ� �̺�Ʈ�� ����</summary>
    public void RequestNewApplicant()
    {
        if (_availableApplicants.Count == 0)
        {
            if(_applicantPool.Count == 0)
            {
                LogWarning("������ ������ Ǯ�� ����־� ���ο� �����ڸ� ���� �� �����ϴ�.");
                return;
            }
            LogMessage("��� �����ڸ� �� �������ϴ�. ������ Ǯ�� �ʱ�ȭ�մϴ�.");
            _availableApplicants = new List<ApplicantSO>(_applicantPool);
        }
        //�������� ������ ����
        int randomIndex = UnityEngine.Random.Range(0, _availableApplicants.Count);
        _currentApplicant = _availableApplicants[randomIndex];

        //�ߺ� ������ ���� ���� �� �� ���õ� �����ڴ� ����Ʈ���� ����
        _availableApplicants.RemoveAt(randomIndex);

        LogMessage($"���ο� ������ '{_currentApplicant.applicantInfo.applicantName}'");

        OnNewApplicantDrawn?.Invoke(_currentApplicant);

    }
    
    ///<summary>���� �̺�Ʈ ó���� �Ϸ�Ǿ����� �ý��ۿ� �˸�</summary>
    public void NotifyEventProcessingCompleted()
    {
        LogMessage($"'{_currentApplicant.applicantInfo.applicantName}'�� ���� ��");
        OnEventProcessingCompleted?.Invoke();
    }
    #endregion

    #region Private Methods
    #endregion

    #region ���� �� ��ƿ��Ƽ
    ///<summary>�̱��� �ν��Ͻ� ���� �� �ߺ� ����</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }else if( _instance != this)
        {
            LogWarning($"�ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ��� �ı��մϴ�. (����: {_instance.name}, �ű�: {name})");
            Destroy(gameObject);
        }
    }

    ///<summary>�α� ���</summary>
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
