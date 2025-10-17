using UnityEngine;
using UnityEngine.UI; // Image ������Ʈ�� ����ϱ� ���� �ʿ�

/// <summary>
/// ȭ�鿡 ǥ�õǴ� ���� ī���� UI�� �����ϴ� ������Ʈ
/// </summary>
public class CardUI : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Image cardImage;

    [SerializeField] private Color eliminatedColor = new Color(0.5f, 0.5f, 0.5f, 0.6f); // Ż������ ���� ����
    #endregion

    #region Properties
    /// <summary>�� UI�� ��Ÿ���� ī�� ������</summary>
    public CardData AssignedCardData { get; private set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// �� UI�� ī�� �����͸� �Ҵ��ϰ� �̹����� �����մϴ�.
    /// </summary>
    public void Initialize(CardData cardData)
    {
        AssignedCardData = cardData;
        cardImage.sprite = cardData.CardImage;
    }

    /// <summary>
    /// ī�尡 ���� �ĺ����� ���ο� ���� UI ���¸� �����մϴ�.
    /// </summary>
    public void UpdateVisual(bool isRemaining)
    {
        // �ĺ��� ���������� ���� ����, Ż�������� ��ο� �������� ����
        cardImage.color = isRemaining ? Color.white : eliminatedColor;
    }
    #endregion
}