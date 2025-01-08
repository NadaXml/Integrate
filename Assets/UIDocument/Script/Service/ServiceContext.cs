using AppFrame;
using System.Collections.Generic;
namespace UIDocument.Script.Service {
    public class ServiceContext : IServiceContext {

        Dictionary<string, object> _handles = new();

        public void SetHandleObject(string name, object obj) {
            _handles.Add(name, obj);
        }
    }
}
