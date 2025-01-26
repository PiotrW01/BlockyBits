using BlockyBitsClient.src;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public abstract class Object
{
    public Transform Transform = new();

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
        // adjusts each childs global pos relative to parent based on their local pos
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

    public void HandleComponentMouseMove(float deltaTime, Vector2 mouseVec)
    {
        foreach (var component in components)
        {
            component.HandleMouseMove(deltaTime, mouseVec);
        }
    }

    public void HandleChildrenMouseMove(float deltaTime, Vector2 mouseVec)
    {
        foreach( var child in children)
        {
            child.HandleMouseMove(deltaTime, mouseVec);
            child.HandleChildrenMouseMove(deltaTime, mouseVec);
        }
    }

    public virtual void HandleInput(float deltaTime) { }

    // mouseVec is the relative position of the mouse to the screen center
    public virtual void HandleMouseMove(float deltaTime, Vector2 mouseVec) { }

    public virtual void HandleMouseClick() { }

    public virtual void HandleScrollInput() { }

    public void HandleChildrenScrollInput()
    {
        foreach(var child in children)
        {
            child.HandleScrollInput();
            child.HandleChildrenScrollInput();
        }
    }

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

    public void HandleChildrenMouseClick()
    {
        foreach(var child in children)
        {
            child.HandleMouseClick();
            child.HandleChildrenMouseClick();
        }
    }

    public void HandleComponentMouseClick()
    {
        foreach (var component in components)
        {
            component.HandleMouseClick();
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

    public void AddChild(Object c)
    {
        c.parent = this;
        children.Add(c);
    }

    public void AddComponent(Component c)
    {
/*        if(components.Any(component => component.GetType() == c.GetType()))
        {
            Debug.WriteLine($"Trying to assign component to object {GetType()} that already has this kind of component: {c.GetType()}");
            return;
        }*/

        c.SetOwner(this);
        components.Add(c);
    }

    public void AddComponent<T>() where T : Component, new()
    {
/*        if (components.Any(component => component is T))
        {
            Debug.WriteLine($"Trying to assign component to object {GetType()} that already has this kind of component: {typeof(T)}");
            return;
        }*/

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
