using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public abstract class Component
{
    protected GameObject owner = null;


/*    public Component(GameObject self)
    {
        this.self = self;
    }*/

    public virtual void Start() { }

    public virtual void Update(float deltaTime) { }

    public virtual void LoadContent() { }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }
}
