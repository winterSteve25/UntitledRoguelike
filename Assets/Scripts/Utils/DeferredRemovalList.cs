using System.Collections.Generic;

namespace Utils
{
    public class DeferredRemovalList<T> : List<T>
    {
        private List<T> _removeQueue = new();
        
        public new void Remove(T item)
        {
            _removeQueue.Add(item);
        }

        public void Flush()
        {
            RemoveAll(x => _removeQueue.Contains(x));
            _removeQueue.Clear();
        }
    }
}