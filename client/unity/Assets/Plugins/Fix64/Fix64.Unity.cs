using System;

namespace FixMath.NET
{
    public struct FixVector3
    {
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;

        public FixVector3(double x, double y)
        {
            this.x = (Fix64)x;
            this.y = (Fix64)y;
            this.z = Fix64.Zero;
        }

        public FixVector3(double x, double y, double z)
        {
            this.x = (Fix64)x;
            this.y = (Fix64)y;
            this.z = (Fix64)z;
        }

        public FixVector3(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
            this.z = Fix64.Zero;
        }

        public FixVector3(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Fix64 sqrMagnitude => (this.x * this.x) + (this.y * this.y) + (this.z * this.z);

        public Fix64 magnitude => (Fix64)Math.Sqrt((double)sqrMagnitude);

        public static FixVector3 operator +(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        
        public static FixVector3 operator -(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);

        public static FixVector3 operator *(FixVector3 v1, Fix64 v2) => new FixVector3(v1.x * v2, v1.y * v2, v1.z * v2);

        public static FixVector3 operator /(FixVector3 v1, Fix64 v2) => new FixVector3(v1.x / v2, v1.y / v2, v1.z / v2);

        public static implicit operator UnityEngine.Vector3(FixVector3 vector3) => new UnityEngine.Vector3((float)vector3.x, (float)vector3.y, (float)vector3.z);

        public static implicit operator FixVector3(UnityEngine.Vector3 vector3) => new FixVector3((Fix64)vector3.x, (Fix64)vector3.y, (Fix64)vector3.z);
    }
}
