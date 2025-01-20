using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public abstract class Object
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 scale = Vector3.One;
    public GameObject parent = null;
    public List<GameObject> children = new List<GameObject>();
    public List<Component> components = new List<Component>();
    public Collider collider;


    public virtual void Start() { }
    public virtual void Render() { }
    public virtual void Update(float deltaTime) { }
    public virtual void HandleInput(float deltaTime) { }

    // mouseVec is the relative position of the mouse to the screen center
    public virtual void HandleMouseInput(float deltaTime, Vector2 mouseVec) { }

    public virtual void LoadContent(ContentManager cm)
    {

    }

}
