using AppFrame;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core.HUD {
    public class HUDStencilOrderSystem<T> : ISystem 
        where T : MonoBehaviour {
        static readonly int Stencil = Shader.PropertyToID("_Stencil");
        static readonly int MainTex = Shader.PropertyToID("_MainTex");

        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Update() {
            // throw new System.NotImplementedException();
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
        }
        public IEnumerator Start() {
            throw new System.NotImplementedException();
        }

        List<IHUD<T>> _huds;

        Camera _uiCamera;

        public HUDStencilOrderSystem(IEnumerable<IHUD<T>> huds, Camera camera) {
            _huds = new List<IHUD<T>>();
            _huds.AddRange(huds);
            _uiCamera = camera;
        }

        public async void StencilOrder() {
            for (int i = 0; i < _huds.Count; i++) {
                IHUD<T> hud = _huds[i];
                await UniTask.WaitUntil(() => hud.Asset.InstantiatedGameObject != null);
                
                if (hud.Asset.Binder is HUDBinder) {
                    HUDBinder binder = hud.Asset.Binder as HUDBinder;
                    var spriteRender = binder.hp.GetComponent<SpriteRenderer>();
                    spriteRender.material.SetFloat(Stencil, i);
                    spriteRender.material.SetTexture(MainTex, spriteRender.sprite.texture);
                }
            }
        }
        
    }
}
