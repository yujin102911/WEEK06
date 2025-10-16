using System.Collections.Generic;
using UnityEngine;

//지원자의 정보를 담는 구조체
[System.Serializable]
public struct ApplicantData
{
    #region 지원자 기본 정보
    //지원자 이름
    public string applicantName;
    //지원자 나이
    public int age;
    //지원자 얼굴
    public Sprite portrait;
    #endregion

    #region 지원자 경력
    //지원자 경력
    [TextArea(3, 5)]
    public List<string> careerHistory;
    #endregion

    #region 지원자 질문
    //지원자에게 할 수 있는 질문
    public List<string> interviewQuestions;
    //지원자가 하는 답변(4와 1:1 대응)
    public List<string> interviewAnswers;
    #endregion
}