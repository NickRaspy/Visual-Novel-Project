using UnityEngine;
using Naninovel;
using VNP.Data;
using System.Collections.Generic;
using System.Linq;
using VNP.Scriptables;

namespace VNP.Services
{
    public class QuestService : IEngineService, IStatefulService<GameStateMap>
    {
        const string QUEST_PATH = "Quest List";

        public Dictionary<string, InGameQuest> quests = new();

        public async UniTask InitializeServiceAsync()
        {
            var load = await Resources.LoadAsync<QuestList>(QUEST_PATH);

            if(load == null)
            {
                Debug.LogError("There is no Quest List in Resources");
                return;
            }

            if(load is QuestList questList)
            {
                HashSet<Quest> loadedQuests = new(questList.Quests);

                foreach (var loadedQuest in loadedQuests) quests.Add(loadedQuest.id, new() { quest = loadedQuest });
            }
            else
            {
                Debug.LogError("Quest List in Resources isn't QuestList asset");
            }
        }

        public void StartQuest(string id)
        {
            quests[id].status = QuestStatus.Active;
        }

        public void ResetService()
        {
            foreach(var quest in quests.Values)
                quest.SetStatus(QuestStatus.Inactive);
        }

        public void DestroyService()
        {
            quests.Clear();
        }

        public UniTask LoadServiceStateAsync(GameStateMap state)
        {
            var data = state.GetState<InGameQuestState>();

            if (data == null) return UniTask.CompletedTask;

            quests = data.quests;

            return UniTask.CompletedTask;
        }

        public void SaveServiceState(GameStateMap state)
        {
            InGameQuestState inGameQuestState = new() { quests = quests };
            state.SetState(inGameQuestState);
        }
    }

    public class InGameQuestState
    {
        public Dictionary<string, InGameQuest> quests;
    }
}
