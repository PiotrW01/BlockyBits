using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

    public virtual void LoadContent(ContentManager cm) { }

    public virtual void HandleInput(float deltaTime) { }

    public virtual void HandleMouseMove(float deltaTime, Vector2 mouseVec) { }

    public virtual void HandleMouseClick() { }

    public void SetOwner(Object owner)
    {
        this.owner = owner;
    }

    public virtual void OnDelete()
    {

    }
}
