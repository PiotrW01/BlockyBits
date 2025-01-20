using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Gravity: Component
{
    private float g = 0.81f;

    public override void Update(float deltaTime)
    {
        owner.pos.Y -= g * deltaTime;
    }
}
