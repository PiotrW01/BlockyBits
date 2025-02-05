using System.Numerics;

namespace BlockyBitsClient.src.components
{
    internal class Rotation: Component
    {
        public override void Update(float deltaTime)
        {
            owner.Transform.Quaternion *= Quaternion.CreateFromYawPitchRoll(Microsoft.Xna.Framework.MathHelper.PiOver4 * deltaTime,0,0);
        }
    }
}
