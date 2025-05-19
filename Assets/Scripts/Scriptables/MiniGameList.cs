using System;
using System.Collections.Generic;
using UnityEngine;
using VNP.MiniGames;

namespace VNP.Scriptables
{
    [CreateAssetMenu(fileName = "MiniGameList", menuName = "VNP/Mini Game List")]
    public class MiniGameList : ScriptableObject
    {
        public List<SerializedMiniGame> miniGamesList;
    }

    [Serializable]
    public class SerializedMiniGame
    {
        public string id;
        public MiniGame miniGame;
    }
}