using Naninovel;
using UnityEngine;
using VNP.Services;

namespace VNP.Commands
{
    [CommandAlias("startQuest")]
    public class QuestStart : Command
    {
        [ParameterAlias("questId"), RequiredParameter]
        public StringParameter questId;
        public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            if(!Engine.GetService<QuestService>().IsLoaded)
            {
                Debug.LogError("ServiceLoad Error: there is no quest loaded!");
                return UniTask.CompletedTask;
            }

            Engine.GetService<QuestService>().StartQuest(questId);
            return UniTask.CompletedTask;
        }
    }

    [CommandAlias("completeTask")]
    public class TaskComplete : Command
    {
        [ParameterAlias("questId"), RequiredParameter]
        public StringParameter questId;
        [ParameterAlias("taskId"), RequiredParameter]
        public StringParameter taskId;
        public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            if (!Engine.GetService<QuestService>().IsLoaded)
            {
                Debug.LogError("ServiceLoad Error: there is no quest loaded!");
                return UniTask.CompletedTask;
            }

            Engine.GetService<QuestService>().CompleteTask(questId, taskId);
            return UniTask.CompletedTask;
        }
    }
}
