#if UNITY_EDITOR
    using RedTipDebug.Editor;
#endif
using RedTipHelper.Config;
using UnityEngine;
using RedTipHelper.Core;
using RedTipPart.Config;
using RedTipPart.Part;
using System;

namespace RedTipPart {
    public class RedTipTest : MonoBehaviour {

        RedTipService _service;
        void Awake() {
            _service = new RedTipService();
            
            var source = new RedTipConstImp();
            source.BindCreator(_service);
            source.Root = RedTipConstImp.Main;
            
            var relation = new RedTipRelationImp();
            _service.SetSource(source, relation);
            
            _service.Init();
            _service.Start();
            
            _service.AddRedTip(RedTipConstImp.IslandHero, RedTipConstImp.IslandHeroDict);
            _service.RemoveRedTipDict(RedTipConstImp.IslandHeroDict, 1);
            _service.AddRedTipDict(RedTipConstImp.IslandHeroDict, 1);
            _service.RemoveRedTip(RedTipConstImp.IslandHeroDict);

            
#if UNITY_EDITOR
            RedTipDebugWindow.GetShowGraphStr += () => {
                return _service.DumpActive();
            };
#endif
        }

        void Update() {
            _service.RefreshSchedule.Schedule();
        }

        void OnDestroy() {
            _service?.UnInit();
        }
    }
}
