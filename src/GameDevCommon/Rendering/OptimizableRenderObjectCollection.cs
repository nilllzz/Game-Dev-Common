using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GameDevCommon.Rendering
{
    public sealed class OptimizableRenderObjectCollection : RenderObjectCollection
    {
        private List<I3DObject> _originalObjects;

        public IEnumerable<I3DObject> OriginalObjects
        {
            get
            {
                if (Optimized)
                    return _originalObjects;
                else
                    return _opaqueObjects.Concat(_transparentObjects);
            }
        }

        public bool Optimized { get; private set; }

        public OptimizableRenderObjectCollection()
            : base()
        { }

        public OptimizableRenderObjectCollection(I3DObject[] objects)
            : base(objects)
        { }

        public override void Add(I3DObject obj)
        {
            if (Optimized)
            {
                lock (_lock)
                {
                    _originalObjects.Add(obj);
                }
            }
            base.Add(obj);
        }

        private struct ObjectCategory
        {
            public bool IsVisible;
            public int BlendStateId;
            public bool IsVisualObject;
            public float Alpha;
            public bool IsOpaque;
            public string TextureName;

            private static int GetBlendStateId(BlendState blendState)
            {
                if (blendState == null)
                    return 0;

                if (blendState.Name == BlendState.Additive.Name)
                    return 1;
                if (blendState.Name == BlendState.AlphaBlend.Name)
                    return 2;
                if (blendState.Name == BlendState.NonPremultiplied.Name)
                    return 3;
                if (blendState.Name == BlendState.Opaque.Name)
                    return 4;

                return -1;
            }

            public static ObjectCategory Create(I3DObject obj)
            {
                return new ObjectCategory
                {
                    IsVisible = obj.IsVisible,
                    BlendStateId = GetBlendStateId(obj.BlendState),
                    IsVisualObject = obj.IsVisualObject,
                    Alpha = obj.Alpha,
                    IsOpaque = obj.IsOpaque,
                    TextureName = obj.Texture.Name,
                };
            }
        }

        public void Optmimize<VertexType>() where VertexType : struct
        {
            lock (_lock)
            {
                _originalObjects = _opaqueObjects.Concat(_transparentObjects).ToList();

                _opaqueObjects.Clear();
                _transparentObjects.Clear();

                // categorize objects:
                var categories = new Dictionary<ObjectCategory, List<I3DObject>>();
                foreach (var obj in _originalObjects)
                {
                    if (!obj.IsOpaque || !obj.IsOptimizable)
                    {
                        Add(obj);
                    }
                    else
                    {
                        var category = ObjectCategory.Create(obj);
                        if (category.BlendStateId == -1) // unknown/custom blendstate
                        {
                            Add(obj);
                        }
                        else
                        {
                            if (categories.ContainsKey(category))
                                categories[category].Add(obj);
                            else
                                categories.Add(category, new List<I3DObject>() { obj });
                        }
                    }
                }

                // morph categories of objects into a single geometry object:
                foreach (var category in categories.Keys)
                {
                    var objects = categories[category];
                    if (objects.Count == 1)
                    {
                        AddRange(objects.ToArray());
                    }
                    else
                    {
                        var morphed = new Morphed3DObject<VertexType>(objects);
                        morphed.LoadContent();
                        Add(morphed);
                    }
                }

                Optimized = true;
            }
        }

        public void Restore()
        {
            lock (_lock)
            {
                _opaqueObjects.Clear();
                _transparentObjects.Clear();
                AddRange(_originalObjects.ToArray());
                _originalObjects = null;

                Optimized = false;
            }
        }
    }
}
