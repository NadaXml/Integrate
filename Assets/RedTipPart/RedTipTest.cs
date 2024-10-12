using RedTipDebug.Editor;
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
            
            RedTipService.BindCreate(RedTipConstImp.Main, (_key, _service) => {
                return new RedTipMain(_key, _service);
            });
            
            RedTipService.BindCreate(RedTipConstImp.IslandHero, (_key, _service) => {
                return new RedTipIslandHero(_key, _service);
            });
            
            RedTipService.BindCreate(RedTipConstImp.IslandHeroDict, (_key, _service) => {
                return new RedTipIslandHeroDict(_key, _service);
            });
            
            RedTipService.BindCreate(RedTipConstImp.IslandHeroRef, (_key, _service) => {
                return new RedTipIslandHeroRef(_key, _service);
            });

            var source = new RedTipConstImp();
            var relation = new RedTipRelationImp();
            _service.SetSource(source, relation);
            
            _service.Init();
            _service.Start();

            
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
