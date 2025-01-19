using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Utils
{
    static public float DegToRad(float angle)
    {
        return (((float)Math.PI) / 180) * angle;
    }
    static public float RadToDeg(float angle)
    {
        return angle * 180 / ((float)Math.PI);
    }
}

