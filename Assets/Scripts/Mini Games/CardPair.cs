using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VNP.MiniGames.CardPair
{
    public class CardPair : MonoBehaviour, IMiniGame
    {
        public UnityAction OnGameFinish { get; set; }

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

        private HashSet<Card> cards;

        private Card firstComparingCard;
        private Card secondComparingCard;

        public bool CanClickOnCards { get; set; } = true;

        public void FinishGame()
        {
            StopGame();
            OnGameFinish();
        }

        public void StartGame(Difficulty difficulty)
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

                cards.Add(firstCard);
                cards.Add(secondCard);

                cardFrontBag.Remove(cardFront);

                if(cardFrontBag.Count == 0) cardFrontBag = new(cardFronts);
            }

            var shuffledCards = cards.OrderBy(x => UnityEngine.Random.value);

            cards = new(shuffledCards);

            foreach(var card in cards) card.transform.SetParent(gridLayout.transform);
        }

        public void StopGame()
        {
            foreach(var card in cards) Destroy(card);
            cards.Clear();
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

                firstComparingCard = null;
                secondComparingCard = null;
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
        }
    }
}
