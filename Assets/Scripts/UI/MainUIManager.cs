using Naninovel;
using Naninovel.UI;
using UnityEngine;
using VNP.UI.Quests;

namespace VNP.UI
{
    public class MainUIManager : MonoBehaviour
    {
        [SerializeField] private QuestWindow questWindow;
        [SerializeField] private GameObject pause;
        [SerializeField] private string mapScriptName;
        [SerializeField] private string titleScriptName;

        private IUIManager uiManager;
        private void Start()
        {
            questWindow.Init();
            uiManager = Engine.GetService<IUIManager>();
        }

        private void OnDisable()
        {
            questWindow.gameObject.SetActive(false);
        }

        public void ShowMap()
        {
            Engine.GetService<IChoiceHandlerManager>().RemoveAllActors();

            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            scriptPlayer.PreloadAndPlayAsync(mapScriptName);
        }

        public void Pause()
        {
            pause.SetActive(!pause.activeSelf);
        }

        public void SaveLoad() => uiManager.GetUI<SaveLoadMenu>()?.Show();

        public void Settings() => uiManager.GetUI<ISettingsUI>()?.Show();

        public void Title()
        {
            Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync("Title");
            Pause();
        }
    }
}