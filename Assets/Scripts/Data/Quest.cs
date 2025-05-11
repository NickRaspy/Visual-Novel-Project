using System.Collections.Generic;
using System.Linq;

namespace VNP.Data
{
    public class MainQuest : Quest
    {
        private readonly List<Quest> _subQuests = new();

        public MainQuest(string id, string title) : base(id, title) { }

        public void AddSubQuest(Quest quest)
        {
            _subQuests.Add(quest);
        }

        public void CheckCompletion()
        {
            if (Status == QuestStatus.Completed) return;

            bool completed = _subQuests.All(q => q.Status == QuestStatus.Completed);

            if (completed)
                Status = QuestStatus.Completed;
        }
    }

    public class Quest
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public QuestStatus Status { get; protected set; }

        public Quest(string id, string title)
        {
            Id = id;
            Title = title;
            Status = QuestStatus.Inactive;
        }

        public virtual void SetStatus(QuestStatus newStatus)
        {
            Status = newStatus;
        }
    }

    public enum QuestStatus
    {
        Inactive,
        Active,
        Completed,
        Failed
    }
}
