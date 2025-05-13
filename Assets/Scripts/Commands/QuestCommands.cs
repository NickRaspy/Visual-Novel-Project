using Naninovel;
using VNP.Services;

namespace VNP.Commands
{
    [CommandAlias("startQuest")]
    public class QuestStart : Command
    {
        public StringParameter questId;
        public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            Engine.GetService<QuestService>().StartQuest(questId);
            return UniTask.CompletedTask;
        }
    }

    [CommandAlias("completeTask")]
    public class TaskComplete : Command
    {
        public StringParameter questId;
        public StringParameter taskId;
        public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            Engine.GetService<QuestService>().CompleteTask(questId, taskId);
            return UniTask.CompletedTask;
        }
    }
}
