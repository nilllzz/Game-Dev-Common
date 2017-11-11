using System;
using System.Collections;
using System.Collections.Generic;

namespace GameDevCommon.Rendering
{
    public class RenderObjectCollection : IEnumerable<I3DObject>
    {
        protected object _lock = new object();
        protected readonly List<I3DObject> _opaqueObjects, _transparentObjects;

        public IEnumerable<I3DObject> OpaqueObjects => _opaqueObjects;
        public IEnumerable<I3DObject> TransparentObjects => _transparentObjects;

        public RenderObjectCollection()
        {
            _opaqueObjects = new List<I3DObject>();
            _transparentObjects = new List<I3DObject>();
        }

        public RenderObjectCollection(I3DObject[] objects)
        {
            _opaqueObjects = new List<I3DObject>();
            _transparentObjects = new List<I3DObject>();
            AddRange(objects);
        }

        public void AddRange(params I3DObject[] objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                    Add(obj);
            }
        }

        public virtual void Add(I3DObject obj)
        {
            lock (_lock)
            {
                if (obj.IsOpaque)
                    _opaqueObjects.Add(obj);
                else
                    _transparentObjects.Add(obj);
            }
        }

        public void ForEach(Action<I3DObject> method)
        {
            lock (_lock)
            {
                _opaqueObjects.ForEach(method);
                _transparentObjects.ForEach(method);
            }
        }

        private I3DObject GetItem(int index)
        {
            lock (_lock)
            {
                if (index < _opaqueObjects.Count)
                    return _opaqueObjects[index];
                else
                    return _transparentObjects[index - _opaqueObjects.Count];
            }
        }

        private void SetItem(int index, I3DObject obj)
        {
            lock (_lock)
            {
                if (index < _opaqueObjects.Count)
                    _opaqueObjects[index] = obj;
                else
                    _transparentObjects[index - _opaqueObjects.Count] = obj;
            }
        }

        public void Remove(I3DObject obj)
        {
            lock (_lock)
            {
                if (obj.IsOpaque)
                    _opaqueObjects.Remove(obj);
                else
                    _transparentObjects.Remove(obj);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
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
            lock (_lock)
            {
                _transparentObjects.Sort();
            }
        }

        public IEnumerator<I3DObject> GetEnumerator()
        {
            var index = 0;
            while (index < _opaqueObjects.Count + _transparentObjects.Count)
            {
                yield return GetItem(index);
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public I3DObject this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }
    }
}
