using UnityEngine;

[CreateAssetMenu(fileName = "New Applicant Profile", menuName = "Talent Tangle/Applicant Profile")]
public class ApplicantSO : ScriptableObject
{
    [Header("면접 화면에 표시될 정보")]
    public ApplicantData applicantInfo;

    [Header("내부 로직 데이터")]
    [Range(-5f, 5f)]
    public float impact;

}
