using System.Collections;
using System.Collections.Generic;

namespace LAPI.Test.Mocks
{
    public class Deque<TElement> : ICollection<TElement>
    {
        private LinkedList<TElement> _linkedList = new LinkedList<TElement>();

        public void Enqueue(TElement element)
        {
            _linkedList.AddLast(element);
        }

        public TElement Dequeue()
        {
            var node = _linkedList.First;
            _linkedList.RemoveFirst();
            return node.Value;
        }

        public void PushFront(TElement element)
        {
            _linkedList.AddFirst(element);
        }

        public TElement PopBack()
        {
            var node = _linkedList.Last;
            _linkedList.RemoveLast();
            return node.Value;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TElement item)
        {
            _linkedList.AddLast(item);
        }

        public void Clear()
        {
            _linkedList.Clear();
        }

        public bool Contains(TElement item)
        {
            return _linkedList.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        public bool Remove(TElement item)
        {
            return _linkedList.Remove(item);
        }

        public int Count => _linkedList.Count;
        public bool IsReadOnly => false;
    }
}