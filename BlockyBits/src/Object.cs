using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public abstract class Object
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public Vector3 localPos = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 scale = Vector3.One;
    public Object parent = null;
    public List<Object> children = new List<Object>();
    public List<Component> components = new List<Component>();
    
    public virtual void Start() { }
    public virtual void Render() { }
    public virtual void Update(float deltaTime) { }

    public void RenderChildren()
    {
        foreach(var child in children)
        {
            child.Render();
            child.RenderChildren();
        }
    }

    public void UpdateChildrenAndComponents(float deltaTime)
    {
        foreach (var child in children)
        {
            child.pos = pos + child.localPos;
            child.UpdateChildrenAndComponents(deltaTime);
        }
        foreach (var c in components)
        {
            c.Update(deltaTime);
        }
    }

    public void ChildrenStart(ContentManager cm)
    {
        foreach(var child in children)
        {
            child.LoadContent(cm);
            child.Start();
            child.ChildrenStart(cm);
        }
    }

    public void ComponentsStart(ContentManager cm)
    {
        foreach(Component c in components)
        {
            c.LoadContent(cm);
            c.Start();
        }

        foreach(var child in children)
        {
            child.ComponentsStart(cm);
        }
    }

    public void HandleComponentInput(float deltaTime)
    {
        foreach(var component in components)
        {
            component.HandleInput(deltaTime);
        }
    }

    public void HandleComponentMouseInput(float deltaTime, Vector2 mouseVec)
    {
        foreach (var component in components)
        {
            component.HandleMouseInput(deltaTime, mouseVec);
        }
    }

    public void HandleChildrenMouseInput(float deltaTime, Vector2 mouseVec)
    {
        foreach( var child in children)
        {
            child.HandleMouseInput(deltaTime, mouseVec);
            child.HandleChildrenMouseInput(deltaTime, mouseVec);
        }
    }

    public virtual void HandleInput(float deltaTime) { }

    // mouseVec is the relative position of the mouse to the screen center
    public virtual void HandleMouseInput(float deltaTime, Vector2 mouseVec) { }

    public virtual void LoadContent(ContentManager cm)
    {

    }

    public void HandleChildrenInput(float delta)
    {
        foreach(var child in children)
        {
            child.HandleInput(delta);
            child.HandleChildrenInput(delta);
        }
    }

    public void OnDeleteChildrenAndComponents()
    {
        foreach( var c in components)
        {
            c.OnDelete();
        }
        foreach( var child in children)
        {
            child.OnDelete();
            child.OnDeleteChildrenAndComponents();
        }
    }

    public virtual void OnDelete()
    {

    }

    public void AddComponent(Component c)
    {
        if(components.Any(component => component.GetType() == c.GetType()))
        {
            Debug.WriteLine($"Trying to assign component to object {GetType()} that already has this kind of component: {c.GetType()}");
            return;
        }

        c.SetOwner(this);
        components.Add(c);
    }

    public void AddComponent<T>() where T : Component, new()
    {
        if (components.Any(component => component is T))
        {
            Debug.WriteLine($"Trying to assign component to object {GetType()} that already has this kind of component: {typeof(T)}");
            return;
        }

        T c = new T();
        c.SetOwner(this);
        components.Add(c);
    }

    public T GetComponent<T>() where T : Component
    {
        foreach(Component c in components)
        {
            if (c.GetType() == typeof(T)) return (T)c;
        }
        return default;
    }
}
