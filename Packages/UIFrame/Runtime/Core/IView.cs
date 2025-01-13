using UnityEngine;
namespace UIFrame.Core {
    public interface IView {
        void Awake();
        void Destroy();
        Transform RootTransform { get; }
    }
}
