using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// ������ ī�� ��, ���� ī��, ���� ī�� ����� �тD��� �̱��� �Ŵ�ó
/// </summary>

public class CardManager : MonoBehaviour
{
    #region Singleton Instance
    private static CardManager _instance;

    /// <summary>CardManager �̱��� �ν��Ͻ�</summary>
    public static CardManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [SerializeField]
    [Tooltip("���ӿ��� ����� ��� ī�� ������ ��� (52��)")]
    private List<CardData> fullDeck;
    #endregion

    #region Private Fields
    private CardData _answerCard; // �÷��̾ ����� �� ���� ī��
    private List<CardData> _remainingCards; // �߸� �������� ���� �ĺ� ī�� ���
    #endregion

    #region Properties
    /// <summary>���� ������ ���� ī��</summary>
    public CardData AnswerCard => _answerCard;

    /// <summary>���� ���� �ĺ� ī�� ���</summary>
    public List<CardData> RemainingCards => _remainingCards;
    #endregion

    #region Events
    /// <summary>���� ī�� ����� ���ŵ� ������ ����Ǵ� �̺�Ʈ</summary>
    public event Action<List<CardData>> OnCardsUpdated;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// ���ο� ������ �����մϴ�. ���� ī�带 �̰� ��� ī�带 �ĺ��� �����մϴ�.
    /// </summary>
    public void StartNewGame()
    {
        if (fullDeck == null || fullDeck.Count == 0)
        {
            LogWarning("CardManager: 'fullDeck'�� ī�� �����Ͱ� �����ϴ�. ������ ������ �� �����ϴ�.");
            return;
        }

        // ���� ī�带 �������� ����
        _answerCard = fullDeck[UnityEngine.Random.Range(0, fullDeck.Count)];
        LogMessage($"���� ī�尡 �����Ǿ����ϴ�: {AnswerCard.CardSuit} {AnswerCard.CardRank}");

        // ���� ī�� ����� ��ü ������ �ʱ�ȭ
        _remainingCards = new List<CardData>(fullDeck);

        // UI ������ ���� �̺�Ʈ ����
        OnCardsUpdated?.Invoke(_remainingCards);
    }

    /// <summary>
    /// ������ ����� ���� ���� ī�� ����� ���͸��մϴ�.
    /// </summary>
    /// <param name="question">����� ���� ������</param>
    /// <param name="isQuestionTrueForAnswer">���� ī�尡 ������ '��'�� ���ߴ��� ����</param>
    public void FilterCards(QuestionData question, bool isQuestionTrueForAnswer)
    {
        // ������ ī����� �ӽ÷� ���� ����Ʈ
        List<CardData> cardsToRemove = new List<CardData>();

        // ���� ī����� �ϳ��� ��ȸ�ϸ� ������ ��
        foreach (CardData card in _remainingCards)
        {
            // �� ī�尡 ������ ���ǿ� �����ϴ��� Ȯ��
            bool cardMeetsCondition = question.Evaluate(card);

            // ���� ī���� �亯�� ���� ī���� �亯�� �ٸ��ٸ�, �� ī��� ������ �ƴϹǷ� ���� ��Ͽ� �߰�
            if (cardMeetsCondition != isQuestionTrueForAnswer)
            {
                cardsToRemove.Add(card);
            }
        }

        // ���� ��Ͽ��� ����
        foreach (CardData card in cardsToRemove)
        {
            _remainingCards.Remove(card);
        }

        LogMessage($"{cardsToRemove.Count}���� ī�尡 ���͸��Ǿ����ϴ�. ���� ī��: {_remainingCards.Count}��");

        // UI ������ ���� �̺�Ʈ ����
        OnCardsUpdated?.Invoke(_remainingCards);
    }
    #endregion

    #region ��ƿ��Ƽ �� ����
    /// <summary>�̱��� �ν��Ͻ��� �����մϴ�.</summary>
    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"CardManager: �ߺ� �ν��Ͻ� ����. ���� �ν��Ͻ�({name})�� �ı��մϴ�.");
            Destroy(gameObject);
        }
    }

    /// <summary>���� �α׸� �Ķ������� ����մϴ�.</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=cyan>{message}</color>");
    }

    /// <summary>��� �α׸� �Ķ������� ����մϴ�.</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=cyan>{message}</color>");
    }
    #endregion

}
