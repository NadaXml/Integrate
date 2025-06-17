using adt;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace game_service.pool_service {
    public class PoolService : GameService {

        public ReferencePool<IReference> poolEventData;
        
        public async UniTask<GameProcedure> Init(CancellationTokenSource cts) {
            poolEventData = new ReferencePool<IReference>();
            return GameProcedure.Success;
        }

        protected override void OnDestroy() {
            poolEventData.Destroy();
        }
    }
}
