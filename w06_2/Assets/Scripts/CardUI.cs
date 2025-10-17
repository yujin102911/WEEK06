using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 필요

/// <summary>
/// 화면에 표시되는 개별 카드의 UI를 제어하는 컴포넌트
/// </summary>
public class CardUI : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Image cardImage;

    [SerializeField] private Color eliminatedColor = new Color(0.5f, 0.5f, 0.5f, 0.6f); // 탈락했을 때의 색상
    #endregion

    #region Properties
    /// <summary>이 UI가 나타내는 카드 데이터</summary>
    public CardData AssignedCardData { get; private set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// 이 UI에 카드 데이터를 할당하고 이미지를 설정합니다.
    /// </summary>
    public void Initialize(CardData cardData)
    {
        AssignedCardData = cardData;
        cardImage.sprite = cardData.CardImage;
    }

    /// <summary>
    /// 카드가 남은 후보인지 여부에 따라 UI 상태를 갱신합니다.
    /// </summary>
    public void UpdateVisual(bool isRemaining)
    {
        // 후보로 남아있으면 원래 색상, 탈락했으면 어두운 색상으로 변경
        cardImage.color = isRemaining ? Color.white : eliminatedColor;
    }
    #endregion
}