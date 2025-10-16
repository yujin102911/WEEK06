using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>면접 이벤트를 관리하고 제공하는 싱글톤 매니저</summary>
public class EventManager : MonoBehaviour
{
    #region Singleton Instance
    private static EventManager _instance;

    public static EventManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [Header("이벤트 풀 설정")]
    [SerializeField] private List<ApplicantSO> _applicantPool;
    #endregion

    #region Private Fields
    ///<summary>현재 게임 세션에서 사용 가능한 지원자 목록(중복 방지용)</summary>
    private List<ApplicantSO> _availableApplicants;

    ///<summary>현재 진행 중인 면접의 지원자 SO</summary>
    private ApplicantSO _currentApplicant;
    #endregion

    #region Properties
    ///<summary>현재 진행 중인 면접의 지원자 정보</summary>
    public ApplicantSO CurrentApplicant => _currentApplicant;
    ///<summary>사용 가능한 풀에 남아있는 지원자 수</summary>
    public int AvailableApplicantCount => _availableApplicants.Count;

    #endregion

    #region Events
    ///<summary>새로운 지원자를 뽑았을 때 발행되는 이벤트</summary>
    public event Action<ApplicantSO> OnNewApplicantDrawn;

    ///<summary>현재 이벤트 처리가 완료되었을 때 발행되는 이벤트</summary>
    public event Action OnEventProcessingCompleted;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    ///<summary>의존성이 필요 없는 내부 초기화</summary>
    private void Initialize()
    {
        SetupSingletonInstance();
        //사용 가능한 지원자 풀을 초기화하고 마스터 풀의 모든 데이터를 복사(원본 보호를 위해 복사본을 사용)
        _availableApplicants = new List<ApplicantSO>(_applicantPool);
    }
    #endregion

    #region Public Methods
    ///<summary>사용 가능한 풀에서 새로운 지원자를 랜덤으로 요청하고 이벤트를 발행</summary>
    public void RequestNewApplicant()
    {
        if (_availableApplicants.Count == 0)
        {
            if(_applicantPool.Count == 0)
            {
                LogWarning("마스터 지원자 풀이 비어있어 새로운 지원자를 뽑을 수 없습니다.");
                return;
            }
            LogMessage("모든 지원자를 다 만났습니다. 지원자 풀을 초기화합니다.");
            _availableApplicants = new List<ApplicantSO>(_applicantPool);
        }
        //랜덤으로 지원자 선택
        int randomIndex = UnityEngine.Random.Range(0, _availableApplicants.Count);
        _currentApplicant = _availableApplicants[randomIndex];

        //중복 등장을 막기 위해 한 번 선택된 지원자는 리스트에서 제거
        _availableApplicants.RemoveAt(randomIndex);

        LogMessage($"새로운 지원자 '{_currentApplicant.applicantInfo.applicantName}'");

        OnNewApplicantDrawn?.Invoke(_currentApplicant);

    }
    
    ///<summary>현재 이벤트 처리가 완료되었음을 시스템에 알림</summary>
    public void NotifyEventProcessingCompleted()
    {
        LogMessage($"'{_currentApplicant.applicantInfo.applicantName}'의 면접 끝");
        OnEventProcessingCompleted?.Invoke();
    }
    #endregion

    #region Private Methods
    #endregion

    #region 검증 및 유틸리티
    ///<summary>싱글톤 인스턴스 설정 및 중복 방지</summary>
    private void SetupSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }else if( _instance != this)
        {
            LogWarning($"중복 인스턴스 감지. 현재 인스턴스를 파괴합니다. (기존: {_instance.name}, 신규: {name})");
            Destroy(gameObject);
        }
    }

    ///<summary>로그 출력</summary>
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
