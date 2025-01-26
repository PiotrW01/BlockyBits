using Microsoft.Xna.Framework;
using System;

namespace BlockyBitsClient.src
{
    public class Transform
    {
        Vector3 _globalPosition = Vector3.Zero;
        Vector3 _eulerAngles = Vector3.Zero;
        Quaternion _quaternion = Quaternion.Identity;

        public Vector3 Position = Vector3.Zero;
        public Vector3 GlobalPosition
        {
            get
            {
                return _globalPosition;
            }
            set
            {
                //Position += value - GlobalPosition;
                _globalPosition = value;
            }
        }
        public Quaternion Quaternion
        {
            get
            {
                return _quaternion;
            }
            set
            {
                _eulerAngles = ToEulerAngles(value);
                _quaternion = value;
            }
        }
        public Vector3 EulerAngles
        {
            get
            {
                return _eulerAngles;
            }
            set
            {
                Vector3 vec = new(WrapAngle(value.X), WrapAngle(value.Y), WrapAngle(value.Z));
                _quaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);
                _eulerAngles = vec;
            }
        }
        public Vector3 Scale = Vector3.One;


        public Vector3 GetEulerDegrees()
        {
            return new Vector3(Utils.RadToDeg(EulerAngles.X), Utils.RadToDeg(EulerAngles.Y), Utils.RadToDeg(EulerAngles.Z));
        }

        private static float WrapAngle(float angle)
        {
            angle = angle % (float)(2 * Math.PI);  // Ensure the angle is within -2π to 2π
            if (angle > Math.PI)
                angle -= 2f * (float)Math.PI;         // Wrap to -π to π
            if (angle < -Math.PI)
                angle += 2f * (float)Math.PI;         // Wrap to -π to π

            return angle;
        }

        private static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

    }
}
