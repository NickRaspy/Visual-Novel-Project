using UnityEngine.Events;

namespace VNP.MiniGames
{
    public interface IMiniGame
    {
        UnityAction OnGameFinish { get; set; }
        void StartGame(Difficulty difficulty);
        void StopGame();
        void FinishGame();
    }

    public enum Difficulty
    {
        Easy, Medium, Hard
    }
}
