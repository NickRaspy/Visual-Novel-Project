using Naninovel;
using UnityEngine;
using VNP.Services;

namespace VNP.Commands
{
    [CommandAlias("startMiniGame")]
    public class StartMiniGame : Command
    {
        [ParameterAlias("miniGameId"), RequiredParameter]
        public StringParameter miniGameID;

        [ParameterAlias("difficulty")]
        public IntegerParameter difficulty = 0;

        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var mgs = Engine.GetService<MiniGameService>();

            if (!mgs.IsLoaded)
            {
                Debug.LogError("ServiceLoad Error: there is no mini game loaded!");
                return;
            }

            var tcs = new UniTaskCompletionSource();

            void Finish()
            {
                mgs.OnFinish -= Finish;
                tcs.TrySetResult();
            }

            mgs.OnFinish += Finish;
            mgs.StartGame(miniGameID, difficulty);

            await tcs.Task;
        }
    }
}