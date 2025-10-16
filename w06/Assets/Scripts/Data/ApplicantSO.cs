using UnityEngine;

[CreateAssetMenu(fileName = "New Applicant Profile", menuName = "Talent Tangle/Applicant Profile")]
public class ApplicantSO : ScriptableObject
{
    [Header("���� ȭ�鿡 ǥ�õ� ����")]
    public ApplicantData applicantInfo;

    [Header("���� ���� ������")]
    [Range(-5f, 5f)]
    public float impact;

}
