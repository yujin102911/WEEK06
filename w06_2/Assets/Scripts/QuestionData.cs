using UnityEngine;

/// <summary>모든 질문 데이터의 기반이 되는 추상 ScriptableObject</summary>
public abstract class QuestionData : ScriptableObject
{
    #region Serialized Fields
    [SerializeField]
    [TextArea] // 여러 줄로 편하게 입력하기 위한 어트리뷰트
    private string questionText;
    #endregion


    #region Properties
    /// <summary>UI에 표시될 질문 내용</summary>
    public string QuestionText => questionText;
    #endregion


    #region Abstract Method
    /// <summary>
    /// 주어진 카드가 이 질문의 조건에 해당하는지 판별합니다.
    /// </summary>
    /// <param name="card">판별할 카드</param>
    /// <returns>조건에 해당하면 true, 아니면 false</returns>
    public abstract bool Evaluate(CardData card);
    #endregion
}