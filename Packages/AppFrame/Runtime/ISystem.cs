using System.Collections;
namespace AppFrame {
    public interface ISystem {
        void Awake();
        void Destroy();
        
        IEnumerator Start();
    }
}
