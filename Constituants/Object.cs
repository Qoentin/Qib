using Qib.EFFECTS;
using Qib.OPENGL;
using Qib.TEXTURES;
using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.CONSTITUANTS
{
    class Object : IRenderable {
        public Transform Transform { get; set; }
        public Mesh Mesh { get; set; }
        public Shader Shader { get; set; }
        public Texture Texture { get; set; }

        public Action UniformUpload { get; set; }

        private Dictionary<Type,IEffect> EffectMap = new();
        public T Effects<T>() { return (T)EffectMap[typeof(T)]; }
        public void AddEffect(IEffect Effect) { Effect.Parent = this; EffectMap.Add(Effect.GetType(), Effect); }
        public void UpdateEffects() { foreach ( IEffect Effect in EffectMap.Values ) Effect.Update(); }

        public Object( Transform Transform, Mesh Mesh, Shader Shader, Texture Texture = null ) {
            this.Transform = Transform;
            this.Mesh = Mesh;
            this.Shader = Shader;
            this.Texture = Texture ?? NullTexture2.Get();

            UniformUpload = () => { };
        }

        public Object( Mesh Mesh, Shader Shader, Texture Texture ) : this(new(), Mesh, Shader, Texture) { }

        public virtual void Update() { }
    }
}
