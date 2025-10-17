using UnityEngine;

/// <summary>ī���� ������ �����ϴ� ������</summary>
public enum Suit
{
    Spade,
    Diamond,
    Heart,
    Club
}
/// <summary>ī���� ���ڸ� �����ϴ� ������</summary>
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
/// ���� ī���� �����͸� ��� SO
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
    ///<summary>ī���� ����</summary>
    public Suit CardSuit => suit;
    ///<summary>ī���� ����</summary>
    public Rank CardRank => rank;
    ///<summary>ī���� �̹���</summary>
    public Sprite CardImage => cardImage;
    #endregion

    #region Public Methods
    ///<summary>ī���� ������ ���������� Ȯ��</summary>
    public bool IsRed()
    {
        return suit == Suit.Diamond || suit == Suit.Heart;
    }
    /// <summary>ī���� ������ ���������� Ȯ��</summary>
    public bool IsBlack()
    {
        return suit == Suit.Spade || suit == Suit.Club;
    }
    /// <summary>ī�尡 J, Q, K �� �ϳ����� Ȯ��</summary>
    public bool IsFaceCard()
    {
        return rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King;
    }

    /// <summary>ī���� ���� ���� ������ ��ȯ</summary>
    public int GetValue()
    {
        return (int)rank;
    }
    #endregion

}
