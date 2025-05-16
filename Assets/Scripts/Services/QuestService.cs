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
        const string QUEST_PATH = "QuestList";
        public Dictionary<string, Quest> Quests { get; private set; } = new();

        //<id of Quest, all Quest's Tasks> for easy access
        public Dictionary<string, List<Task>> AllTasks { get; private set; } = new();

        public bool IsLoaded { get; private set; } = false;

        public async UniTask InitializeServiceAsync()
        {
            var load = await Resources.LoadAsync<QuestList>(QUEST_PATH);

            if(load == null)
            {
                Debug.LogWarning("There is no QuestList in Resources!");
                return;
            }

            if(load is QuestList questList)
            {
                if(questList.Quests.Count == 0)
                {
                    Debug.LogError("EmpyList Error: Your QuestList is empty!");
                    return;
                }
                
                if(questList.Quests.Any(q => string.IsNullOrEmpty(q.id)))
                {
                    Debug.LogError("EmptyValue Error: One of your quests contains empty ID!");
                    return;
                }

                if(questList.Quests.Any(q => q.progress.Count == 0))
                {
                    Debug.LogError("EmpyList Error: One of your quest's progress is empty!");
                    return;
                }

                HashSet<Quest> loadedQuests = new(questList.Quests);

                foreach (var loadedQuest in loadedQuests)
                {
                    Quests.Add(loadedQuest.id, loadedQuest);

                    AllTasks.Add(loadedQuest.id, new());

                    foreach(var step in loadedQuest.progress)
                    {
                        if(step.tasks.Count == 0)
                        {
                            Debug.LogError("EmptyList Error: Your QuestList contains empty steps!");
                            Quests.Clear();
                            return;
                        }

                        if(step.tasks.Any(t => string.IsNullOrEmpty(t.id)))
                        {
                            Debug.LogError("EmptyValue Error: One of your tasks contains empty ID!");
                            Quests.Clear();
                            return;
                        }

                        AllTasks[loadedQuest.id].AddRange(step.tasks);
                    }
                }

                IsLoaded = true;
            }
            else Debug.LogError("NonMatchingObjects Error: QuestList in Resources isn't QuestList asset!");
        }

        public void StartQuest(string questID)
        {
            if (!Quests.ContainsKey(questID))
            {
                Debug.LogError("NonExistingObject Error: You are trying to start non-existing quest!");
                return;
            }

            Quests[questID].Start();
        }

        public void CheckStep(string questID) => Quests[questID].CheckStep();

        public void CompleteTask(string questID, string taskID)
        {

            Task task = AllTasks[questID].First(t => t.id == taskID);

            if(task == null)
            {
                Debug.LogError("NonExistingObject Error: You are trying to complete non-existing task!");
                return;
            }

            if (task.status == QuestStatus.Active)
            {
                task.status = QuestStatus.Completed;

                CheckStep(questID);
            }
            else
            {
                Debug.LogError("FailedConditions Error: You are trying to complete inactive, completed of failed task!");
            }

        }

        public void ResetService()
        {
            foreach(var quest in Quests.Values)
                quest.status = QuestStatus.Inactive;
        }

        public void DestroyService()
        {
            Quests.Clear();
        }

        public UniTask LoadServiceStateAsync(GameStateMap state)
        {
            var data = state.GetState<List<QuestState>>();

            if (data == null) return UniTask.CompletedTask;

            foreach(var quest in data)
            {
                Quests[quest.id].status = quest.status;
                Quests[quest.id].progressCount = quest.currentProgress;

                Quests[quest.id].progress.ForEach(p =>
                {
                    p.tasks.ForEach(t => t.status = quest.tasks.First(x => x.id == t.id).status);
                });
            }

            return UniTask.CompletedTask;
        }

        public void SaveServiceState(GameStateMap state)
        {
            List<QuestState> questStates = new();

            foreach (var quest in Quests.Values)
            {
                QuestState questState = new()
                {
                    id = quest.id,
                    status = quest.status,
                    currentProgress = quest.progressCount,
                    tasks = new()
                };
                AllTasks[quest.id].ForEach(t =>
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
