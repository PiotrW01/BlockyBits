using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public abstract class Component
{
    protected Object owner = null;

    public virtual void Start() { }

    public virtual void Update(float deltaTime) { }

    public virtual void LoadContent() { }

    public virtual void HandleInput(float deltaTime) { }

    public virtual void HandleMouseInput(float deltaTime, Vector2 mouseVec) { }

    public void SetOwner(Object owner)
    {
        this.owner = owner;
    }
}
