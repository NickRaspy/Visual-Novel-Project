using Naninovel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VNP.Data;
using VNP.Scriptables;

namespace VNP.Services
{
    [InitializeAtRuntime]
    public class QuestService : IEngineService, IStatefulService<GameStateMap>
    {
        private const string QUEST_PATH = "QuestList";
        private const string QUESTS_STATE_KEY = "QuestsState";

        public Dictionary<string, Quest> Quests { get; private set; } = new();
        public Dictionary<string, List<Task>> AllTasks { get; private set; } = new();
        public Dictionary<string, Dictionary<string, Task>> TaskById { get; private set; } = new();
        public bool IsLoaded { get; private set; } = false;

        private ICustomVariableManager customVariableManager;

        public async UniTask InitializeServiceAsync()
        {
            customVariableManager = Engine.GetService<ICustomVariableManager>();

            var load = await Resources.LoadAsync<QuestList>(QUEST_PATH);

            if (load is not QuestList questList)
            {
                Debug.LogWarning(load == null
                    ? "There is no QuestList in Resources!"
                    : "NonMatchingObjects Error: QuestList in Resources isn't QuestList asset!");
                return;
            }

            if (questList.Quests.Count == 0)
            {
                Debug.LogError("EmptyList Error: Your QuestList is empty!");
                return;
            }

            foreach (var quest in questList.Quests)
            {
                if (string.IsNullOrEmpty(quest.id))
                {
                    Debug.LogError("EmptyValue Error: One of your quests contains empty ID!");
                    return;
                }
                if (quest.progress == null || quest.progress.Count == 0)
                {
                    Debug.LogError($"EmptyList Error: Quest '{quest.id}' progress is empty!");
                    return;
                }
                foreach (var step in quest.progress)
                {
                    if (step.tasks == null || step.tasks.Count == 0)
                    {
                        Debug.LogError($"EmptyList Error: Quest '{quest.id}' contains empty steps!");
                        return;
                    }
                    if (step.tasks.Any(t => string.IsNullOrEmpty(t.id)))
                    {
                        Debug.LogError($"EmptyValue Error: Quest '{quest.id}' has a task with empty ID!");
                        return;
                    }
                }
            }

            foreach (var quest in questList.Quests)
            {
                Quests[quest.id] = quest;
                var allTasks = new List<Task>();
                var taskDict = new Dictionary<string, Task>();
                foreach (var step in quest.progress)
                {
                    allTasks.AddRange(step.tasks);
                    foreach (var task in step.tasks)
                    {
                        if (!string.IsNullOrEmpty(task.id))
                            taskDict[task.id] = task;
                    }
                }
                AllTasks[quest.id] = allTasks;
                TaskById[quest.id] = taskDict;
            }

            IsLoaded = true;
        }

        public void StartQuest(string questID)
        {
            if (!Quests.TryGetValue(questID, out var quest))
            {
                Debug.LogError($"NonExistingObject Error: You are trying to start non-existing quest '{questID}'!");
                return;
            }

            if (quest.status != QuestStatus.Inactive)
            {
                Debug.LogError($"FailedConditions Error: You are trying to start a quest ('{questID}') that is not inactive!");
                return;
            }

            quest.Start();
        }

        public void CheckStep(string questID)
        {
            if (Quests.TryGetValue(questID, out var quest))
                quest.CheckStep();
            else
                Debug.LogError($"NonExistingObject Error: Quest '{questID}' does not exist!");
        }

        public void CompleteTask(string questID, string taskID)
        {
            if (!TaskById.TryGetValue(questID, out var taskDict))
            {
                Debug.LogError($"NonExistingObject Error: Quest '{questID}' does not exist!");
                return;
            }

            if (!taskDict.TryGetValue(taskID, out var task))
            {
                Debug.LogError($"NonExistingObject Error: Task '{taskID}' does not exist in quest '{questID}'!");
                return;
            }

            if (task.status == QuestStatus.Active)
            {
                task.status = QuestStatus.Completed;
                CheckStep(questID);
            }
            else
            {
                Debug.LogError($"FailedConditions Error: Task '{taskID}' in quest '{questID}' is not active!");
            }
        }

        public void FailTask(string questID, string taskID)
        {
            if (!TaskById.TryGetValue(questID, out var taskDict))
            {
                Debug.LogError($"NonExistingObject Error: Quest '{questID}' does not exist!");
                return;
            }

            if (!taskDict.TryGetValue(taskID, out var task))
            {
                Debug.LogError($"NonExistingObject Error: Task '{taskID}' does not exist in quest '{questID}'!");
                return;
            }

            if (task.status == QuestStatus.Active)
            {
                task.status = QuestStatus.Failed;
                CheckStep(questID);
            }
            else
            {
                Debug.LogError($"FailedConditions Error: Task '{taskID}' in quest '{questID}' is not active!");
            }
        }

        public void ResetService()
        {
            foreach (var quest in Quests.Values)
            {
                quest.status = QuestStatus.Inactive;
                quest.ProgressCount = 0;
            }

            foreach (var tasks in AllTasks.Values)
                foreach (var task in tasks)
                    task.status = QuestStatus.Inactive;
        }

        public void DestroyService()
        {
            Quests.Clear();
            AllTasks.Clear();
            TaskById.Clear();
            IsLoaded = false;
        }

        public UniTask LoadServiceStateAsync(GameStateMap state)
        {
            var questStatesList = state.GetState<QuestStatesList>(QUESTS_STATE_KEY);
            var data = questStatesList?.QuestStates;
            if (data == null) return UniTask.CompletedTask;

            foreach (var questState in data)
            {
                if (!Quests.TryGetValue(questState.id, out var quest))
                    continue;

                quest.status = questState.status;
                quest.ProgressCount = questState.currentProgress;

                var taskStateDict = questState.tasks?.ToDictionary(ts => ts.id, ts => ts.status) ?? new();
                foreach (var step in quest.progress)
                {
                    foreach (var task in step.tasks)
                    {
                        if (taskStateDict.TryGetValue(task.id, out var status))
                            task.status = status;
                        else
                            task.status = QuestStatus.Inactive;
                    }
                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveServiceState(GameStateMap state)
        {
            var questStates = new List<QuestState>(Quests.Count);

            foreach (var quest in Quests.Values)
            {
                var questState = new QuestState
                {
                    id = quest.id,
                    status = quest.status,
                    currentProgress = quest.ProgressCount,
                    tasks = AllTasks.TryGetValue(quest.id, out var tasks)
                        ? tasks.Select(t => new TaskState { id = t.id, status = t.status }).ToList()
                        : new List<TaskState>()
                };
                questStates.Add(questState);
            }

            var questStatesList = new QuestStatesList { QuestStates = questStates };
            state.SetState(questStatesList, QUESTS_STATE_KEY);
        }
    }

    [Serializable]
    public class QuestStatesList
    {
        public List<QuestState> QuestStates = new();
    }

    [Serializable]
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