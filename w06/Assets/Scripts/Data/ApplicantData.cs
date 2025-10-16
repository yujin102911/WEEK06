using System.Collections.Generic;
using UnityEngine;

//�������� ������ ��� ����ü
[System.Serializable]
public struct ApplicantData
{
    #region ������ �⺻ ����
    //������ �̸�
    public string applicantName;
    //������ ����
    public int age;
    //������ ��
    public Sprite portrait;
    #endregion

    #region ������ ���
    //������ ���
    [TextArea(3, 5)]
    public List<string> careerHistory;
    #endregion

    #region ������ ����
    //�����ڿ��� �� �� �ִ� ����
    public List<string> interviewQuestions;
    //�����ڰ� �ϴ� �亯(4�� 1:1 ����)
    public List<string> interviewAnswers;
    #endregion
}