using UnityEngine;

/// <summary>카드의 문양을 정의하는 열거형</summary>
public enum Suit
{
    Spade,
    Diamond,
    Heart,
    Club
}
/// <summary>카드의 숫자를 정의하는 열거형</summary>
public enum Rank
{
    Ace = 1,
    Two, 
    Three, 
    Four, 
    Five, 
    Six, 
    Seven, 
    Eight, 
    Nine, 
    Ten, 
    Jack, 
    Queen, 
    King
}

/// <summary>
/// 개별 카드의 데이터를 담는 SO
/// </summary>
[CreateAssetMenu(fileName = "Card_", menuName = "Card Game/Card Data")]
public class CardData : ScriptableObject
{
    #region Serialized Fields
    [SerializeField] private Suit suit;
    [SerializeField] private Rank rank;
    [SerializeField] private Sprite cardImage;
    #endregion

    #region Properties
    ///<summary>카드의 문양</summary>
    public Suit CardSuit => suit;
    ///<summary>카드의 숫자</summary>
    public Rank CardRank => rank;
    ///<summary>카드의 이미지</summary>
    public Sprite CardImage => cardImage;
    #endregion

    #region Public Methods
    ///<summary>카드의 색상이 빨간색인지 확인</summary>
    public bool IsRed()
    {
        return suit == Suit.Diamond || suit == Suit.Heart;
    }
    /// <summary>카드의 색상이 검은색인지 확인</summary>
    public bool IsBlack()
    {
        return suit == Suit.Spade || suit == Suit.Club;
    }
    /// <summary>카드가 J, Q, K 중 하나인지 확인</summary>
    public bool IsFaceCard()
    {
        return rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King;
    }

    /// <summary>카드의 숫자 값을 정수로 반환</summary>
    public int GetValue()
    {
        return (int)rank;
    }
    #endregion

}
