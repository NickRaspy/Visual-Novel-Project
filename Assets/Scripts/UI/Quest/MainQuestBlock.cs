using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VNP.Data;

namespace VNP.UI.Quests
{
    public class MainQuestBlock : MonoBehaviour
    {
        public Quest AssignedQuest { get; private set; }

        private Dictionary<string, TaskBlock> taskBlocks = new();

        [SerializeField] private TMP_Text title;
        [SerializeField] private TaskBlock taskTemplate;

        public void Init(Quest quest, HashSet<Task> tasks)
        {
            AssignedQuest = quest;

            title.text = quest.title;

            foreach (var task in tasks)
            {
                TaskBlock newTaskBlock = Instantiate(taskTemplate, transform);
                newTaskBlock.Init(task);
                taskBlocks.Add(task.id, newTaskBlock);
            }
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            UpdateTaskBlocks();
        }

        private void UpdateTaskBlocks()
        {
            foreach (var task in taskBlocks.Values.Where(t => t.AssignedTask.status != QuestStatus.Inactive)) task.Activate();
        }
    }
}