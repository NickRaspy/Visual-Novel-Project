using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VNP.Data;

namespace VNP.UI.Quests
{
    public class TaskBlock : MonoBehaviour
    {
        public Task AssignedTask { get; private set; }

        [SerializeField] private TMP_Text taskTitle;
        [SerializeField] private GameObject completionMark;
        [SerializeField] private GameObject failedMark;

        public void Init(Task task)
        {
            AssignedTask = task;

            taskTitle.text = task.title;
        }

        private void SetMark()
        {
            switch (AssignedTask.status)
            {
                case QuestStatus.Completed:
                    completionMark.SetActive(true);
                    break;
                case QuestStatus.Failed:
                    failedMark.SetActive(true);
                    break;
            }
        }

        public void Activate()
        {
            gameObject.SetActive(true);

            SetMark();
        }
    }
}
