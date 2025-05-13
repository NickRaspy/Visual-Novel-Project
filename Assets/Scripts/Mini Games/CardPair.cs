using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VNP.Interfaces;

namespace VNP.MiniGames.CardPair
{
    public class CardPair : MonoBehaviour, IMiniGame
    {
        public UnityAction OnGameFinish { get; set; }

        [Header("Settings")]
        [SerializeField] private Sprite cardBack;
        [SerializeField] private List<Sprite> cardFronts;
        [SerializeField] private List<CardPairSetting> cardPairSettings;


        [Header("Insides")]
        [SerializeField] private Card originCard;

        public void FinishGame()
        {

        }

        public void StartGame()
        {

        }

        public void StopGame()
        {

        }
    }

    [Serializable]
    public class CardPairSetting
    {
        public Difficulty difficulty;
        public int cardAmount;
    }
}
