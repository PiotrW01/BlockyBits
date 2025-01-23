using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Collider: Component
{
    Vector3 offsetMin, offsetMax = Vector3.Zero;


    public BoundingBox box
    {
        get
        {
            return new BoundingBox((owner.pos - offsetMin * owner.scale), (owner.pos + offsetMax * owner.scale));
        }
    }




    public override void Update(float deltaTime)
    {
        BoundingBox box = this.box;
        Debugger.QueueDraw(box);
    }

    public void SetSize(float width, float depth, float height)
    {
        offsetMin = new Vector3(width / 2f, height / 2f, depth / 2f);
        offsetMax = new Vector3(width / 2f, height / 2f, depth / 2f);
    }
}