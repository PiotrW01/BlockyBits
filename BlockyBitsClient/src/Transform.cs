using Microsoft.Xna.Framework;
using System;

namespace BlockyBitsClient.src
{
    public class Transform
    {
        Object owner;
        Vector3 _eulerAngles = Vector3.Zero;
        Vector3 _globalEulerAngles = Vector3.Zero;
        Quaternion _quaternion = Quaternion.Identity;
        Quaternion _globalQuaternion = Quaternion.Identity;
        Vector3 _globalPosition = Vector3.Zero;
        Vector3 _position = Vector3.Zero;

        public Transform(Object owner)
        {
            this.owner = owner;
        }

        public Vector3 Position
        {
            get { return _position; }
            set 
            {
                Object parent = owner.GetParent();
                if (parent == null)
                {
                    _globalPosition = value;
                    _position = value;
                } else
                {
                    _globalPosition = parent.Transform.GlobalPosition + value;
                    _position = value;
                }
                owner.UpdateChildPositions();
            }
        }
        public Vector3 GlobalPosition
        {
            get { return _globalPosition; }
            set 
            {
                Object parent = owner.GetParent();
                if (parent != null)
                {
                    _position = value - parent.Transform.GlobalPosition;
                } else
                {
                    _position = value;
                }
                _globalPosition = value;
                owner.UpdateChildPositions();
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
                Quaternion temp = _quaternion;
                Object parent = owner.GetParent();
                if (parent == null)
                {
                    _eulerAngles = ToEulerAngles(value);
                    _quaternion = value;
                    _globalEulerAngles = ToEulerAngles(value);
                    _globalQuaternion = value;
                }
                else
                {
                    _quaternion = value;
                    _eulerAngles = ToEulerAngles(value);
                    _globalQuaternion = parent.Transform.GlobalQuaternion * value;
                    _globalEulerAngles = ToEulerAngles(_globalQuaternion);
                }
                owner.UpdateChildRotation(value * Quaternion.Inverse(temp));
            }
        }

        public Quaternion GlobalQuaternion
        {
            get
            {
                return _globalQuaternion;
            }
            set
            {
                Quaternion temp = _quaternion;
                Object parent = owner.GetParent();
                if (parent == null)
                {
                    _globalQuaternion = value;
                    _quaternion = value;
                    _globalEulerAngles = ToEulerAngles(value);
                    _eulerAngles = ToEulerAngles(value);
                }
                else
                {
                    _globalQuaternion = value;
                    //_quaternion = Quaternion.Inverse(parent.Transform.Quaternion) * value;
                    _globalEulerAngles = ToEulerAngles(value);
                    //_eulerAngles = ToEulerAngles(_quaternion);
                }
                
                owner.UpdateChildRotation(value * Quaternion.Inverse(temp));
            }
        }

        public Vector3 GlobalEulerAngles
        {
            get
            {
                return _globalEulerAngles;
            }
            set
            {
                Quaternion temp = _quaternion;
                Vector3 vec = new(WrapAngle(value.X), WrapAngle(value.Y), WrapAngle(value.Z));
                var parent = owner.GetParent();
                if (parent == null)
                {
                    _globalQuaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);
                    _quaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);
                    _globalEulerAngles = vec;
                    _eulerAngles = vec;
                }
                else
                {
                    _globalQuaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);
                    _globalEulerAngles = vec;

                    _quaternion = Quaternion.Inverse(parent.Transform.Quaternion) * _globalQuaternion;
                    _eulerAngles = ToEulerAngles(_quaternion);
                }
                
                owner.UpdateChildRotation(_quaternion * Quaternion.Inverse(temp));
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
                Quaternion temp = _quaternion;
                Vector3 vec = new(WrapAngle(value.X), WrapAngle(value.Y), WrapAngle(value.Z));
                _eulerAngles = vec;

                var parent = owner.GetParent();

                if (parent == null)
                {
                    _quaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);
                    _globalQuaternion = _quaternion;
                    _globalEulerAngles = _eulerAngles;
                }
                else
                {
                    _quaternion = Quaternion.CreateFromYawPitchRoll(vec.Y, vec.X, vec.Z);

                    _globalQuaternion = parent.Transform.Quaternion * _quaternion;
                    _globalEulerAngles = ToEulerAngles(_globalQuaternion);
                }
                
                owner.UpdateChildRotation(_quaternion * Quaternion.Inverse(temp));
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
