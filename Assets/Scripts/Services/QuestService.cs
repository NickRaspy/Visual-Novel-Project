using UnityEngine;
using Naninovel;
using VNP.Data;
using System.Collections.Generic;
using System.Linq;
using VNP.Scriptables;
using System;

namespace VNP.Services
{
    [InitializeAtRuntime]
    public class QuestService : IEngineService, IStatefulService<GameStateMap>
    {
        const string QUEST_PATH = "Quest List";

        private Dictionary<string, Quest> quests = new();

        //<id of Quest, all Quest's Tasks> for easy access
        private Dictionary<string, List<Task>> allTasks = new();

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

                foreach (var loadedQuest in loadedQuests)
                {
                    quests.Add(loadedQuest.id, loadedQuest);

                    allTasks.Add(loadedQuest.id, new());

                    loadedQuest.progress.ForEach(p => 
                    {
                        allTasks[loadedQuest.id].AddRange(p.tasks);
                    });
                }

            }
            else
            {
                Debug.LogError("Quest List in Resources isn't QuestList asset");
            }
        }

        public void StartQuest(string questID) => quests[questID].Start();

        public void CheckStep(string questID) => quests[questID].CheckStep();

        public void CompleteTask(string questID, string taskID)
        {
            Task task = allTasks[questID].First(t => t.id == taskID);

            if (task.status == QuestStatus.Active)
            {
                allTasks[questID].First(t => t.id == taskID).status = QuestStatus.Completed;

                CheckStep(questID);
            }
            else
            {
                Debug.LogError("You are trying to complete inactive, completed of failed task");
            }

        }

        public void ResetService()
        {
            foreach(var quest in quests.Values)
                quest.status = QuestStatus.Inactive;
        }

        public void DestroyService()
        {
            quests.Clear();
        }

        public UniTask LoadServiceStateAsync(GameStateMap state)
        {
            var data = state.GetState<List<QuestState>>();

            if (data == null) return UniTask.CompletedTask;

            foreach(var quest in data)
            {
                quests[quest.id].status = quest.status;
                quests[quest.id].progressCount = quest.currentProgress;

                quests[quest.id].progress.ForEach(p =>
                {
                    p.tasks.ForEach(t => t.status = quest.tasks.First(x => x.id == t.id).status);
                });
            }

            return UniTask.CompletedTask;
        }

        public void SaveServiceState(GameStateMap state)
        {
            List<QuestState> questStates = new();

            foreach (var quest in quests.Values)
            {
                QuestState questState = new()
                {
                    id = quest.id,
                    status = quest.status,
                    currentProgress = quest.progressCount,
                    tasks = new()
                };
                allTasks[quest.id].ForEach(t =>
                {
                    questState.tasks.Add(new()
                    {
                        id = t.id,
                        status = t.status
                    });
                });
            }

            state.SetState(questStates);
        }
    }

    public class QuestState
    {
        public string id;
        public QuestStatus status;
        public int currentProgress;
        public List<TaskState> tasks;
    }

    [Serializable]
    public class TaskState
    {
        public string id;
        public QuestStatus status;
    }
}
