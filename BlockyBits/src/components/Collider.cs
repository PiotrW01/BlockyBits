using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Collider: Component
{
    public virtual void Draw() { }
    public virtual void SetSize(float width, float depth, float height) { }
}