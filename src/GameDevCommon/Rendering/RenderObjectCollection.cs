using System;
using System.Collections;
using System.Collections.Generic;

namespace GameDevCommon.Rendering
{
    public class RenderObjectCollection<T> : IEnumerable<T> where T : I3DObject
    {
        protected object _lock = new object();
        protected readonly List<T> _opaqueObjects, _transparentObjects;

        public IEnumerable<T> OpaqueObjects => _opaqueObjects;
        public IEnumerable<T> TransparentObjects => _transparentObjects;

        public RenderObjectCollection()
        {
            _opaqueObjects = new List<T>();
            _transparentObjects = new List<T>();
        }

        public RenderObjectCollection(T[] objects)
        {
            _opaqueObjects = new List<T>();
            _transparentObjects = new List<T>();
            AddRange(objects);
        }

        public void AddRange(params T[] objs)
        {
            if (objs != null) {
                foreach (var obj in objs)
                    Add(obj);
            }
        }

        public virtual void Add(T obj)
        {
            lock (_lock) {
                if (obj.IsOpaque)
                    _opaqueObjects.Add(obj);
                else
                    _transparentObjects.Add(obj);
            }
        }

        public void ForEach(Action<T> method)
        {
            lock (_lock) {
                _opaqueObjects.ForEach(method);
                _transparentObjects.ForEach(method);
            }
        }

        private T GetItem(int index)
        {
            lock (_lock) {
                if (index < _opaqueObjects.Count)
                    return _opaqueObjects[index];
                else
                    return _transparentObjects[index - _opaqueObjects.Count];
            }
        }

        private void SetItem(int index, T obj)
        {
            lock (_lock) {
                if (index < _opaqueObjects.Count)
                    _opaqueObjects[index] = obj;
                else
                    _transparentObjects[index - _opaqueObjects.Count] = obj;
            }
        }

        public void Remove(T obj)
        {
            lock (_lock) {
                if (obj.IsOpaque)
                    _opaqueObjects.Remove(obj);
                else
                    _transparentObjects.Remove(obj);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock) {
                if (index < _opaqueObjects.Count)
                    _opaqueObjects.RemoveAt(index);
                else
                    _transparentObjects.RemoveAt(index - _opaqueObjects.Count);
            }
        }

        public void Clear()
        {
            _opaqueObjects.Clear();
            _transparentObjects.Clear();
        }

        public int Count => _opaqueObjects.Count + _transparentObjects.Count;

        public void Sort()
        {
            lock (_lock) {
                _transparentObjects.Sort();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var index = 0;
            while (index < _opaqueObjects.Count + _transparentObjects.Count) {
                yield return GetItem(index);
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int index]
        {
            get {
                return GetItem(index);
            }
            set {
                SetItem(index, value);
            }
        }
    }
}
