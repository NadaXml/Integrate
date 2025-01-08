using System.Collections;
namespace AppFrame {
    public interface IService {
        void Awake();
        void Destroy();
        
        IEnumerator Start();
    }
}
