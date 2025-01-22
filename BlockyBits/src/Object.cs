using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public abstract class Object
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public Vector3 localPos = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 scale = Vector3.One;
    public Object parent = null;
    public List<Object> children = new List<Object>();
    public List<Component> components = new List<Component>();
    public Collider collider = null;


    public virtual void Start() { }
    public virtual void Render() { }
    public virtual void Update(float deltaTime) { }

    public void ForceUpdate(float deltaTime)
    {
        foreach (var child in children)
        {
            child.pos = pos + child.localPos;
        }
        foreach (var c in components)
        {
            c.Update(deltaTime);
        }
        if (collider != null)
        {
            collider.Update(deltaTime);
        }
    }

    public void ComponentStart()
    {
        foreach(var c in components)
        {
            c.Start();
        }
        if(collider != null)
        {
            collider.Start();
        }
    }
    public void HandleComponentInput(float deltaTime)
    {
        foreach(var component in components)
        {
            component.HandleInput(deltaTime);
        }
        if (collider != null)
        {
            collider.HandleInput(deltaTime);
        }
    }

    public void HandleComponentMouseInput(float deltaTime, Vector2 mouseVec)
    {
        foreach (var component in components)
        {
            component.HandleMouseInput(deltaTime, mouseVec);
        }
        if (collider != null)
        {
            collider.HandleMouseInput(deltaTime, mouseVec);
        }
    }

    public virtual void HandleInput(float deltaTime) { }

    // mouseVec is the relative position of the mouse to the screen center
    public virtual void HandleMouseInput(float deltaTime, Vector2 mouseVec) { }

    public virtual void LoadContent(ContentManager cm)
    {

    }

}
