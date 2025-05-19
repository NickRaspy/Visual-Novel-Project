using Naninovel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VNP.Services;

namespace VNP.UI.Quests
{
    public class QuestWindow : MonoBehaviour
    {
        [SerializeField] private MainQuestBlock mainQuestBlockTemplate;
        [SerializeField] private Transform questsContainer;

        private Dictionary<string, MainQuestBlock> mainQuestBlocks = new();

        public void Init()
        {
            var questService = Engine.GetService<QuestService>();

            foreach (var quest in questService.Quests.Values)
            {
                MainQuestBlock newMainQuestBlock = Instantiate(mainQuestBlockTemplate, questsContainer);

                newMainQuestBlock.Init(quest, new(questService.AllTasks[quest.id]));

                mainQuestBlocks.Add(quest.id, newMainQuestBlock);
            }
        }

        private void UpdateQuests()
        {
            foreach (var questBlock in mainQuestBlocks.Values.Where(q => q.AssignedQuest.status != Data.QuestStatus.Inactive))
                questBlock.Activate();
        }

        private void OnEnable() => UpdateQuests();
    }
}