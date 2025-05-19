using Naninovel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VNP.Data
{
    [Serializable]
    public class InGameQuest
    {
        public Quest quest;
        public QuestStatus status = QuestStatus.Inactive;
        public int currentProgress = 0;

        public void SetStatus(QuestStatus newStatus)
        {
            status = newStatus;
        }
    }

    [Serializable]
    public class Quest
    {
        public string id;
        public string title;

        [NonSerialized]
        public QuestStatus status = QuestStatus.Inactive;

        public List<Step> progress;

        private int progressCount = 0;

        public int ProgressCount
        {
            get => progressCount;
            set
            {
                progressCount = value;
                UpdateQuestProgress();
            }
        }

        public Step GetCurrentProgress() => progress[progressCount];

        public void Start()
        {
            status = QuestStatus.Active;

            UpdateQuestProgress();

            progress[progressCount].StartTasks();
        }

        public void CheckStep()
        {
            if (progress[progressCount].IsFailed())
            {
                status = QuestStatus.Failed;
                return;
            }

            if (progress[progressCount].IsCompleted()) NextStep();
        }

        public void NextStep()
        {
            ProgressCount++;

            progress[progressCount].StartTasks();

            if (progressCount == progress.Count) status = QuestStatus.Completed;
        }

        private void UpdateQuestProgress()
        {
            Engine.GetService<ICustomVariableManager>().SetVariableValue($"{id}_progress", (progressCount + 1).ToString());
        }
    }

    [Serializable]
    public class Task
    {
        public string id;
        public string title;

        [NonSerialized]
        public QuestStatus status = QuestStatus.Inactive;
    }

    [Serializable]
    public class Step
    {
        public List<Task> tasks;

        public void StartTasks() => tasks.ForEach(t => t.status = QuestStatus.Active);

        public bool IsCompleted() => tasks.All(t => t.status == QuestStatus.Completed);

        public bool IsFailed() => tasks.Any(t => t.status == QuestStatus.Failed);
    }

    [Serializable]
    public enum QuestStatus
    {
        Inactive,
        Active,
        Completed,
        Failed
    }
}