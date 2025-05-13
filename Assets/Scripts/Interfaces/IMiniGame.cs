using UnityEngine.Events;

namespace VNP.Interfaces
{
    public interface IMiniGame
    {
        UnityAction OnGameFinish { get; set; }
        void StartGame();
        void StopGame();
        void FinishGame();
    }

    public enum Difficulty
    {
        Easy, Medium, Hard
    }
}
