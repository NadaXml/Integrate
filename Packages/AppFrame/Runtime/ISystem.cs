using System.Collections;
namespace AppFrame {
    public interface ISystem : IUpdater {
        void Awake();
        void Destroy();
        
        IEnumerator Start();
    }
}
