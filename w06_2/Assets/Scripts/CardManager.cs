using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 게임의 카드 덱, 정답 카드, 남은 카드 목록을 총괗라는 싱글톤 매니처
/// </summary>

public class CardManager : MonoBehaviour
{
    #region Singleton Instance
    private static CardManager _instance;

    /// <summary>CardManager 싱글톤 인스턴스</summary>
    public static CardManager Instance => _instance;
    #endregion

    #region Serialized Fields
    [SerializeField]
    [Tooltip("게임에서 사용할 모든 카드 데이터 목록 (52장)")]
    private List<CardData> fullDeck;
    #endregion

    #region Private Fields
    private CardData _answerCard; // 플레이어가 맞춰야 할 정답 카드
    private List<CardData> _remainingCards; // 추리 과정에서 남은 후보 카드 목록
    #endregion

    #region Properties
    /// <summary>현재 게임의 정답 카드</summary>
    public CardData AnswerCard => _answerCard;

    /// <summary>현재 남은 후보 카드 목록</summary>
    public List<CardData> RemainingCards => _remainingCards;
    #endregion

    #region Events
    /// <summary>남은 카드 목록이 갱신될 때마다 발행되는 이벤트</summary>
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
    /// 새로운 게임을 시작합니다. 정답 카드를 뽑고 모든 카드를 후보로 설정합니다.
    /// </summary>
    public void StartNewGame()
    {
        if (fullDeck == null || fullDeck.Count == 0)
        {
            LogWarning("CardManager: 'fullDeck'에 카드 데이터가 없습니다. 게임을 시작할 수 없습니다.");
            return;
        }

        // 정답 카드를 무작위로 선택
        _answerCard = fullDeck[UnityEngine.Random.Range(0, fullDeck.Count)];
        LogMessage($"정답 카드가 설정되었습니다: {AnswerCard.CardSuit} {AnswerCard.CardRank}");

        // 남은 카드 목록을 전체 덱으로 초기화
        _remainingCards = new List<CardData>(fullDeck);

        // UI 갱신을 위해 이벤트 발행
        OnCardsUpdated?.Invoke(_remainingCards);
    }

    /// <summary>
    /// 질문의 결과에 따라 남은 카드 목록을 필터링합니다.
    /// </summary>
    /// <param name="question">사용한 질문 데이터</param>
    /// <param name="isQuestionTrueForAnswer">정답 카드가 질문에 '예'로 답했는지 여부</param>
    public void FilterCards(QuestionData question, bool isQuestionTrueForAnswer)
    {
        // 제거할 카드들을 임시로 담을 리스트
        List<CardData> cardsToRemove = new List<CardData>();

        // 남은 카드들을 하나씩 순회하며 질문과 비교
        foreach (CardData card in _remainingCards)
        {
            // 각 카드가 질문의 조건에 부합하는지 확인
            bool cardMeetsCondition = question.Evaluate(card);

            // 정답 카드의 답변과 현재 카드의 답변이 다르다면, 이 카드는 정답이 아니므로 제거 목록에 추가
            if (cardMeetsCondition != isQuestionTrueForAnswer)
            {
                cardsToRemove.Add(card);
            }
        }

        // 실제 목록에서 제거
        foreach (CardData card in cardsToRemove)
        {
            _remainingCards.Remove(card);
        }

        LogMessage($"{cardsToRemove.Count}장의 카드가 필터링되었습니다. 남은 카드: {_remainingCards.Count}장");

        // UI 갱신을 위해 이벤트 발행
        OnCardsUpdated?.Invoke(_remainingCards);
    }
    #endregion

    #region 유틸리티 및 검증
    /// <summary>싱글톤 인스턴스를 설정합니다.</summary>
    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            LogWarning($"CardManager: 중복 인스턴스 감지. 현재 인스턴스({name})를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    /// <summary>정보 로그를 파란색으로 출력합니다.</summary>
    private void LogMessage(string message)
    {
        Debug.Log($"<color=cyan>{message}</color>");
    }

    /// <summary>경고 로그를 파란색으로 출력합니다.</summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"<color=cyan>{message}</color>");
    }
    #endregion

}
