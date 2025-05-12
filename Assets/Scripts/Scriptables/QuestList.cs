using UnityEngine;
using System.Collections.Generic;
using VNP.Data;

namespace VNP.Scriptables
{
    [CreateAssetMenu(fileName = "QuestList", menuName = "VNP/Quest List")]
    public class QuestList : ScriptableObject
    {
        public List<Quest> Quests = new();
    }
}
