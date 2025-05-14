using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VNP.MiniGames.CardPair
{
    [RequireComponent(typeof(Image))]
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private CardPair cardPair;

        private CardFront currentCardFront;

        private Sprite currentCardBack;

        private Image cardImage;

        private Outline outline;

        private bool isFront = false;

        public void Init(CardPair cardPair, CardFront cardFront, Sprite cardBack)
        {
            this.cardPair = cardPair;
            currentCardFront = cardFront;
            currentCardBack = cardBack;

            cardImage = GetComponent<Image>();

            cardImage.sprite = cardBack;

            if(TryGetComponent(out Outline outLine)) outline = outLine;
        }

        public string GetCardFrontName() => currentCardFront.name;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!cardPair.CanClickOnCards) return;

            ShowFront();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (outline == null || isFront) return;
            outline.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (outline == null || isFront) return;
            outline.enabled = false;
        }

        private void ShowFront()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.SetAutoKill(true)
                .OnStart(() => cardPair.CanClickOnCards = false)
                .Append(transform.DORotate(90f * Vector3.up, 0.5f))
                .AppendCallback(() => cardImage.sprite = currentCardFront.frontView)
                .Append(transform.DORotate(90f * Vector3.up, 0.5f))
                .OnComplete(() =>
                {
                    isFront = true;
                    cardPair.CanClickOnCards = true;
                    cardPair.CompareCards(this);
                });
        }

        public void ShowBack()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.SetAutoKill(true)
                .Append(transform.DORotate(-90f * Vector3.up, 0.5f))
                .AppendCallback(() => cardImage.sprite = currentCardBack)
                .Append(transform.DORotate(-90f * Vector3.up, 0.5f))
                .OnComplete(() =>
                {
                    isFront = false;
                    cardPair.CanClickOnCards = true;
                });
        }
    }

    [Serializable]
    public class CardFront
    {
        public string name;
        public Sprite frontView;
    }
}
