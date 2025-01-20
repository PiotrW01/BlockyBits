using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class SphereCollider: Collider
{
    BoundingSphere sphere = new BoundingSphere();
    public Vector3 center;
    public float radius;
}

