using UnityEngine;
using Naninovel;
using VNP.Data;
using System.Collections.Generic;
using System.Linq;

namespace VNP.Services
{
    public class QuestService : IEngineService, IStatefulService<GameStateMap>
    {
        public Dictionary<string, Quest> quests = new();

        public UniTask InitializeServiceAsync()
        {
            throw new System.NotImplementedException();
        }

        public void ResetService()
        {

        }

        public void DestroyService()
        {

        }

        public UniTask LoadServiceStateAsync(GameStateMap state)
        {
            throw new System.NotImplementedException();
        }

        public void SaveServiceState(GameStateMap state)
        {
            throw new System.NotImplementedException();
        }
    }


}
