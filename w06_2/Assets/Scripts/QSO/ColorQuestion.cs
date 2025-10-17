using UnityEngine;

[CreateAssetMenu(fileName = "Question_Color_", menuName = "Card Game/Questions/Color Question")]
/// <summary>카드의 색상에 대한 질문을 처리하는 ScriptableObject</summary>
public class ColorQuestion : QuestionData
{
    #region Serialized Fields
    [SerializeField]
    private bool isRedQuestion; // true이면 '빨간색', false이면 '검은색' 질문
    #endregion


    #region Public Override Method
    /// <summary>
    /// 카드의 색상이 설정된 색상(빨강/검정)과 일치하는지 판별합니다.
    /// </summary>
    public override bool Evaluate(CardData card)
    {
        if (isRedQuestion)
        {
            return card.IsRed();
        }
        else
        {
            return card.IsBlack();
        }
    }
    #endregion
}