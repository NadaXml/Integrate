using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using YooAsset;

public class ElementBinder : MonoBehaviour {

    public UIDocument Document;
    // Start is called before the first frame update
    IEnumerator Start() {
        
        yield return null;
        
        Button button = Document.rootVisualElement.Q<Button>();
        
        var defaultPackage = YooAssets.GetPackage("DefaultPackage");
        //
        var assetHandle = defaultPackage.LoadAssetAsync<Sprite>("Assets/UIDocument/Res/SpriteAtlas/Common/CJ_LV14.png");
        yield return assetHandle;
        //
        // var assetHandle2 = defaultPackage.LoadAssetAsync<SpriteAtlas>("Assets/UIDocument/Res/Common.spriteatlas");
        // yield return assetHandle2;
        //
        // SpriteAtlas atlas = assetHandle2.AssetObject as SpriteAtlas;
        button.style.backgroundImage = new StyleBackground(assetHandle.AssetObject as Sprite);

        button.clicked += () => {
            Debug.Log("clicked");
        };
        
        // assetHandle.Release();
        // assetHandle2.Release();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
