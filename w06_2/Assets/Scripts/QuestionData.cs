using UnityEngine;

/// <summary>��� ���� �������� ����� �Ǵ� �߻� ScriptableObject</summary>
public abstract class QuestionData : ScriptableObject
{
    #region Serialized Fields
    [SerializeField]
    [TextArea] // ���� �ٷ� ���ϰ� �Է��ϱ� ���� ��Ʈ����Ʈ
    private string questionText;
    #endregion


    #region Properties
    /// <summary>UI�� ǥ�õ� ���� ����</summary>
    public string QuestionText => questionText;
    #endregion


    #region Abstract Method
    /// <summary>
    /// �־��� ī�尡 �� ������ ���ǿ� �ش��ϴ��� �Ǻ��մϴ�.
    /// </summary>
    /// <param name="card">�Ǻ��� ī��</param>
    /// <returns>���ǿ� �ش��ϸ� true, �ƴϸ� false</returns>
    public abstract bool Evaluate(CardData card);
    #endregion
}