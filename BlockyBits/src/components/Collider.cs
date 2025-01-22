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

    }

    public void Draw()
    {
        BoundingBox box = this.box;
        Vector3 counterCorner = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);

        //bottom ring
        Debugger.DrawLine(box.Min, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Red);
        Debugger.DrawLine(box.Min, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Red);


        Debugger.DrawLine(counterCorner, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Red);
        Debugger.DrawLine(counterCorner, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Red);

        //top ring
        Debugger.DrawLine(box.Max, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Red);
        Debugger.DrawLine(box.Max, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Red);

        counterCorner = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
        Debugger.DrawLine(counterCorner, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Red);
        Debugger.DrawLine(counterCorner, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Red);

    }

    public void SetSize(float width, float depth, float height)
    {
        offsetMin = new Vector3(width / 2, height / 2, depth / 2);
        offsetMax = new Vector3(width / 2, height / 2, depth / 2);
    }
}