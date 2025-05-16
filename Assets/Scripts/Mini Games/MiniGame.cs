using UnityEngine;
using UnityEngine.Events;

namespace VNP.MiniGames
{
    public abstract class MiniGame : MonoBehaviour, IMiniGame
    {
        public UnityAction OnGameFinish { get; set; }

        public virtual void FinishGame() => StopGame();

        public abstract void StartGame(Difficulty difficulty);

        public abstract void StopGame();
    }
    public interface IMiniGame
    {
        UnityAction OnGameFinish { get; set; }
        void StartGame(Difficulty difficulty);
        void StopGame();
        void FinishGame();
    }

    public enum Difficulty
    {
        Easy = 1, Medium = 2, Hard = 3
    }
}
