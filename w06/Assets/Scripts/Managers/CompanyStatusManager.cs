using System;
using UnityEngine;

/// <summary>회사의 상태를 관리하는 싱글톤 매니저</summary>
public class CompanyStatusManager : MonoBehaviour
{
    #region Singleton Instance
    private static CompanyStatusManager _instance;

    public static CompanyStatusManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("회사 초기 설정")]
    [SerializeField] private int maxEmployeeCapacity = 10;
    [SerializeField] private int initialEmployeeCount = 0;
    [SerializeField] private float initialStatus = 0f;


    [Header("상태값 범위 설정")]
    [SerializeField] private int maxStatusValue = 100;
    [SerializeField] private int minStatusValue = -100;
    #endregion

    #region Private Fields
    private int _currentEmployees;
    private float _currentStatus;
    #endregion

    #region Properties
    ///<summary>현재 직원 수</summary>
    public int CurrentEmployees => _currentEmployees;
    /// <summary>현재 회사 상태</summary>
    public float CurrentStatus => _currentStatus;
    /// <summary>최대 고용 가능 수</summary>
    public int MaxEmployeeCapacity => maxEmployeeCapacity;
    #endregion

    #region Events
    public event Action OnStatusUpdated; //회사 상태가 업데이트될 때 발행되는 이벤트
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    /// <summary>의존성이 필요 없는 내부 초기화</summary>
    private void Initialize()
    {
        SetupSingletonInstance(); //싱글톤
        ResetStatusToInitial(); //상태 초기화
    }
    #endregion

    #region Public Methods
    ///<summary>
    ///새로운 지원자를 채용, 그 영향력을 회사 상태에 반영
    ///</summary>
    public void RecruitApplicant(ApplicantSO applicant)
    {
        if (!CanRecruit(applicant)) { return; } //고용 가능한 상태가 아니면 return
        
        _currentEmployees++; //현 고용 인원 추가
        _currentStatus += applicant.impact; //회사에 지원자 영향력 적용

        ClampAllStatus(); //상태 값이 설정된 범위를 벗어나지 않도록 제한
        LogMessage($"{applicant.applicantInfo.applicantName}채용 완료 | 현재 직원 수: {_currentEmployees} | 현재 회사 상태: {_currentStatus}");

        OnStatusUpdated?.Invoke(); //이벤트를 발행하여 상태가 업데이트 되었음을 알림

        return; //고용 성공

    }
    ///<summary>
    ///모든 회사 상태를 초기값으로 리셋
    ///</summary>
    public void ResetStatusToInitial()
    {
        _currentEmployees = initialEmployeeCount;
        _currentStatus = initialStatus;

        LogMessage("회사 상태가 초기값으로 리셋");

        OnStatusUpdated?.Invoke();
    }
    #endregion


    #region Private Methods
    ///<summary>고용 가능 여부 검증</summary>
    private bool CanRecruit(ApplicantSO applicant)
    {
        if (_currentEmployees >= maxEmployeeCapacity)
        {
            LogWarning($"{applicant.applicantInfo.applicantName} 고용 불가");
            return false;
        }
        return true;
    }

    #endregion

    #region 상태 검증
    ///<summary>모든 상태 변수를 Min/Max 사이로 유지</summary>
    private void ClampAllStatus()
    {
        _currentStatus = Mathf.Clamp(_currentStatus, minStatusValue, maxStatusValue);

    }

    ///<summary>싱글톤 인스턴스 설정 및 중복 방지</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"CompanyStatusManager: 중복 인스턴스 감지. 현재 인스턴스를 파괴합니다. (기존: {_instance.name}, 신규: {name})");
            Destroy(gameObject);
        }
    }

    ///<summary>로그 출력</summary>
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
