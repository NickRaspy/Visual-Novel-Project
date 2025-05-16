using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace VNP.MiniGames.CardPair
{
    public class CardPair : MiniGame
    {
        [Header("Settings")]
        [SerializeField] private Sprite cardBack;
        [SerializeField] private List<CardFront> cardFronts;
        [Space]
        [Tooltip("Amount will be doubled")]
        [SerializeField] private int easyCardPairAmount;
        [Space]
        [Tooltip("Amount will be doubled")]
        [SerializeField] private int mediumCardPairAmount;
        [Space]
        [Tooltip("Amount will be doubled")]
        [SerializeField] private int hardCardPairAmount;
        [Space]
        [SerializeField] private float cardCooldown = 1f;

        [Header("Insides")]
        [SerializeField] private Card originCard;
        [SerializeField] private GridLayoutGroup gridLayout;

        private HashSet<Card> cards = new();

        private Card firstComparingCard;
        private Card secondComparingCard;

        public bool CanClickOnCards { get; set; } = true;

        public override void FinishGame()
        {
            OnGameFinish();
            base.FinishGame();
        }

        public override void StartGame(Difficulty difficulty)
        {
            if (cardFronts.Count == 0)
            {
                Debug.LogError("There are no Card Fronts");
                return;
            }

            int currentCardAmount = difficulty switch
            {
                Difficulty.Easy => easyCardPairAmount,
                Difficulty.Medium => mediumCardPairAmount,
                Difficulty.Hard => hardCardPairAmount,
                _ => easyCardPairAmount
            };

            List<CardFront> cardFrontBag = new(cardFronts);

            for(int i = 0; i < currentCardAmount; i++)
            {
                Card firstCard = Instantiate(originCard, transform);

                CardFront cardFront = cardFrontBag[UnityEngine.Random.Range(0, cardFrontBag.Count)];
                firstCard.Init(this, cardFront, cardBack);

                Card secondCard = Instantiate(firstCard, transform);
                secondCard.Init(this, cardFront, cardBack);

                cards.Add(firstCard);
                cards.Add(secondCard);

                cardFrontBag.Remove(cardFront);

                if(cardFrontBag.Count == 0) cardFrontBag = new(cardFronts);
            }

            var shuffledCards = cards.OrderBy(x => Random.value);

            cards = new(shuffledCards);

            foreach(var card in cards)
            {
                card.transform.SetParent(gridLayout.transform);
                card.gameObject.SetActive(true);
            }
        }

        public override void StopGame()
        {
            cards.Clear();
            Destroy(gameObject);
        }

        public void CompareCards(Card card)
        {
            if(firstComparingCard == null)
            {
                firstComparingCard = card;
                return;
            }

            secondComparingCard = card;

            if(firstComparingCard.GetCardFrontName() == secondComparingCard.GetCardFrontName())
            {
                firstComparingCard.enabled = false;
                secondComparingCard.enabled= false;

                cards.Remove(firstComparingCard);
                cards.Remove(secondComparingCard);

                ComparingCardClear();

                if(cards.Count == 0) FinishGame();
            }
            else
            {
                StartCoroutine(CardCooldown());
            }
        }

        private IEnumerator CardCooldown()
        {
            CanClickOnCards = false;
            yield return new WaitForSeconds(cardCooldown);
            firstComparingCard.ShowBack();
            secondComparingCard.ShowBack();

            ComparingCardClear();
        }

        private void ComparingCardClear()
        {
            firstComparingCard = null;
            secondComparingCard = null;
        }
    }
}
