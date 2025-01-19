using Microsoft.Xna.Framework;

internal abstract class Object
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);

    public virtual void Render(Camera camera) { }
    public virtual void Update(float deltaTime) { }
    public virtual void HandleInput(float deltaTime) { }

    // mouseVec is the relative position of the mouse to the screen center
    public virtual void HandleMouseInput(float deltaTime, Vector2 mouseVec) { }





}
