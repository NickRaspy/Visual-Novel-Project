using Naninovel;
using UnityEngine;
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
}
