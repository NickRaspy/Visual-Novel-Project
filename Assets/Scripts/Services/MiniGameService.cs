using Naninovel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using VNP.MiniGames;
using VNP.Scriptables;

namespace VNP.Services
{
    [InitializeAtRuntime]
    public class MiniGameService : IEngineService
    {
        private const string MINI_GAMES_PATH = "MiniGameList";

        private MiniGame currentMiniGame;

        private Dictionary<string, MiniGame> miniGames = new();

        public bool IsLoaded { get; private set; } = false;

        public UnityAction OnFinish { get; set; }

        private IStateManager stateManager;

        public async UniTask InitializeServiceAsync()
        {
            var load = await Resources.LoadAsync(MINI_GAMES_PATH);

            if (load == null)
            {
                Debug.LogWarning("There is no MiniGamesList in Resources folder");
                return;
            }

            if (load is MiniGameList miniGameList)
            {
                if (miniGameList.miniGamesList.Count == 0)
                {
                    Debug.LogError("EmptyList Error: Your MiniGamesList is empty!");
                }

                if (miniGameList.miniGamesList.Any(mg => string.IsNullOrEmpty(mg.id) || mg.miniGame == null))
                {
                    Debug.LogError("EmptyValue Error: One of your minigames has no id or MiniGame object!");
                    return;
                }

                foreach (SerializedMiniGame serializedMiniGame in miniGameList.miniGamesList)
                    miniGames.Add(serializedMiniGame.id, serializedMiniGame.miniGame);

                stateManager = Engine.GetService<IStateManager>();

                stateManager.OnRollbackStarted += ResetService;

                IsLoaded = true;
            }
            else Debug.LogError("NonMatchingObjects Error: MiniGamesList in Resources isn't MiniGamesList asset!");
        }

        public void StartGame(string gameID, int difficulty)
        {
            currentMiniGame = Object.Instantiate(miniGames[gameID]);

            currentMiniGame.StartGame((Difficulty)difficulty);

            currentMiniGame.OnGameFinish += FinishGame;
        }

        public void FinishGame()
        {
            if (currentMiniGame != null) currentMiniGame.OnGameFinish -= FinishGame;

            OnFinish();
        }

        public void ResetService() => StopGame();

        public void DestroyService()
        {
            stateManager.OnRollbackStarted -= ResetService;
            StopGame();
        }

        private void StopGame()
        {
            if (currentMiniGame == null) return;

            currentMiniGame.OnGameFinish -= FinishGame;
            currentMiniGame.StopGame();
            currentMiniGame = null;
        }
    }
}