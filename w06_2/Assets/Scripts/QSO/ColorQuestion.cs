using UnityEngine;

[CreateAssetMenu(fileName = "Question_Color_", menuName = "Card Game/Questions/Color Question")]
/// <summary>ī���� ���� ���� ������ ó���ϴ� ScriptableObject</summary>
public class ColorQuestion : QuestionData
{
    #region Serialized Fields
    [SerializeField]
    private bool isRedQuestion; // true�̸� '������', false�̸� '������' ����
    #endregion


    #region Public Override Method
    /// <summary>
    /// ī���� ������ ������ ����(����/����)�� ��ġ�ϴ��� �Ǻ��մϴ�.
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