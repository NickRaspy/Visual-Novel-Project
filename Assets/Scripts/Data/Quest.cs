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
        public List<Step> progress;
    }

    [Serializable]
    public class Step
    {
        public string task;
    }

    public enum QuestStatus
    {
        Inactive,
        Active,
        Completed,
        Failed
    }
}
