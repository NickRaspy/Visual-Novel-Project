using Naninovel;
using UnityEngine;
using VNP.UI.Quests;

namespace VNP.UI
{
    public class MainUIManager : MonoBehaviour
    {
        [SerializeField] private QuestWindow questWindow;
        [SerializeField] private string mapNaniTag;

        private void Start()
        {
            questWindow.Init();
        }

        private void OnDisable()
        {
            questWindow.gameObject.SetActive(false);
        }

        public void ShowMap()
        {
            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            scriptPlayer.PreloadAndPlayAsync(mapNaniTag);
        }
    }
}
