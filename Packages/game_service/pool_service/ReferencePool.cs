using System;
using System.Collections.Generic;
namespace game_service.pool_service {
    
    
    /// <summary>
    /// no support multi thread
    /// </summary>
    public class ReferencePool<T> where T : IReference {
        Queue<T> pool;
        Func<T> creator;
        
        public ReferencePool() {
            pool = new Queue<T>();
        }

        public T Acquire()  {
            if (pool.Count > 0) {
                return pool.Dequeue();
            }
            return creator.Invoke();
        }

        public void Release(T reference) {
            reference.ClearForPool();
            pool.Enqueue(reference);
        }

        public void RegCreator(Func<T> creator) {
            this.creator = creator;
        }

        public void Destroy() {
            creator = null;
            // TODO 这里是不是没有释放干净
            foreach (T temp in pool) {
                temp.ClearForPool();
            }
            pool.Clear();
        }
    }
}
