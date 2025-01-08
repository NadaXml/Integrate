using System.Collections;
namespace UIDocument.Script.App {
    public interface IApp {
        void Awake();
        void Destroy();
        IEnumerator Start();

        void Play();
    }
}
