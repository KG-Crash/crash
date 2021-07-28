using System;

namespace FixMath.NET
{
    public struct FixVector2
    {
        public Fix64 x;
        public Fix64 y;

        public FixVector2(FixVector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public FixVector2(UnityEngine.Vector3 v)
        {
            this.x = (Fix64)v.x;
            this.y = (Fix64)v.y;
        }
        
        public FixVector2(double x, double y)
        {
            this.x = (Fix64)x;
            this.y = (Fix64)y;
        }

        public FixVector2(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
        }
        
        public Fix64 sqrMagnitude => (this.x * this.x) + (this.y * this.y);

        public Fix64 magnitude => (Fix64)Math.Sqrt((double)sqrMagnitude);

        public static FixVector2 operator +(FixVector2 v1, FixVector2 v2) => new FixVector2(v1.x + v2.x, v1.y + v2.y);
        
        public static FixVector2 operator -(FixVector2 v1, FixVector2 v2) => new FixVector2(v1.x - v2.x, v1.y - v2.y);

        public static FixVector2 operator *(FixVector2 v1, Fix64 v2) => new FixVector2(v1.x * v2, v1.y * v2);

        public static FixVector2 operator /(FixVector2 v1, Fix64 v2) => new FixVector2(v1.x / v2, v1.y / v2);

        public static implicit operator UnityEngine.Vector2(FixVector2 vector3) => new UnityEngine.Vector2(vector3.x, vector3.y);

        public static implicit operator FixVector2(UnityEngine.Vector3 vector3) => new FixVector2((Fix64)vector3.x, (Fix64)vector3.y);

        public static FixVector2 Min(FixVector2 a, FixVector2 b) => new FixVector2(Fix64.Min(a.x, b.x), Fix64.Min(a.y, b.y));
        
        public static FixVector2 Max(FixVector2 a, FixVector2 b) => new FixVector2(Fix64.Max(a.x, b.x), Fix64.Max(a.y, b.y));

        public static Fix64 Dot(FixVector2 a, FixVector2 b) => a.x * b.x + a.y * b.y;
    }
    
    public struct FixVector3
    {
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;

        public FixVector3(FixVector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public FixVector3(UnityEngine.Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public FixVector3(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = Fix64.Zero;
        }
        
        public FixVector3(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.z = Fix64.Zero;
        }

        public FixVector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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

        public FixVector2 xy => new FixVector2(this.x, this.y);
        public FixVector2 yx => new FixVector2(this.y, this.x);
        public FixVector2 yz => new FixVector2(this.y, this.z);
        public FixVector2 zy => new FixVector2(this.z, this.y);
        public FixVector2 zx => new FixVector2(this.z, this.x);
        public FixVector2 xz => new FixVector2(this.x, this.z);

        public Fix64 sqrMagnitude => (this.x * this.x) + (this.y * this.y) + (this.z * this.z);

        public Fix64 magnitude => (Fix64)Math.Sqrt((double)sqrMagnitude);

        public FixVector3 normalized => new FixVector3(this.x, this.y, this.z) / magnitude;

        public void Normalize()
        {
            var mag = magnitude;
            
            this.x /= magnitude;
            this.y /= magnitude;
            this.z /= magnitude;
        }
        
        public static FixVector3 operator +(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        
        public static FixVector3 operator -(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);

        public static FixVector3 operator *(FixVector3 v1, Fix64 v2) => new FixVector3(v1.x * v2, v1.y * v2, v1.z * v2);
        
        public static FixVector3 operator *(Fix64 v1, FixVector3 v2) => new FixVector3(v1 * v2.x, v1 * v2.y, v1 * v2.z);
        
        public static FixVector3 operator *(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        
        public static FixVector3 operator /(FixVector3 v1, Fix64 v2) => new FixVector3(v1.x / v2, v1.y / v2, v1.z / v2);
        
        public static FixVector3 operator /(Fix64 v1, FixVector3 v2) => new FixVector3(v1 / v2.x, v1 / v2.y, v1 / v2.z);
        
        public static FixVector3 operator /(FixVector3 v1, FixVector3 v2) => new FixVector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);

        public static implicit operator UnityEngine.Vector3(FixVector3 vector3) => new UnityEngine.Vector3(vector3.x, vector3.y, vector3.z);

        public static implicit operator FixVector3(UnityEngine.Vector3 vector3) => new FixVector3((Fix64)vector3.x, (Fix64)vector3.y, (Fix64)vector3.z);

        public static FixVector3 Min(FixVector3 a, FixVector3 b) => new FixVector3(Fix64.Min(a.x, b.x), Fix64.Min(a.y, b.y), Fix64.Min(a.z, b.z));
        
        public static FixVector3 Max(FixVector3 a, FixVector3 b) => new FixVector3(Fix64.Max(a.x, b.x), Fix64.Max(a.y, b.y), Fix64.Max(a.z, b.z));

        public static Fix64 Dot(FixVector3 a, FixVector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static FixVector3 Cross(FixVector3 a, FixVector3 b) =>
            new FixVector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);

        public static FixVector3 One => new FixVector3(Fix64.One, Fix64.One, Fix64.One);
        public static FixVector3 Zero => new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    }

    public struct FixRect
    {
        public Fix64 minX;
        public Fix64 minY;
        public Fix64 maxX;
        public Fix64 maxY;
        
        public FixRect(FixRect r)
		{
            this.minX = r.minX;
            this.minY = r.minY;
            this.maxX = r.maxX;
            this.maxY = r.maxY;
        }
        public FixRect(UnityEngine.Rect r)
		{
            this.minX = r.xMin;
            this.minY = r.yMin;
            this.maxX = r.xMax;
            this.maxY = r.yMax;
        }
        public FixRect(FixVector2 minV, FixVector2 maxV)
		{
            this.minX = minV.x;
            this.minY = minV.y;
            this.maxX = maxV.x;
            this.maxY = maxV.y;
        }
        public FixRect(FixVector3 minV, FixVector3 maxV)
		{
            this.minX = minV.x;
            this.minY = minV.z;
            this.maxX = maxV.x;
            this.maxY = maxV.z;
        }
        public FixRect(double minX, double minY, double maxX, double maxY)
		{
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
        public FixRect(int minX, int minY, int maxX, int maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public bool Contains(FixVector2 point)
		{
            return (point.x >= this.minX && point.x <= this.maxX) && (point.y >= this.minY && point.y <= this.maxY);
		}
        public bool Contains(FixVector3 point)
        {
            return (point.x >= this.minX && point.x <= this.maxX) && (point.z >= this.minY && point.z <= this.maxY);
        }        
        public bool Contains(FixRect rect)
        {
            return (rect.minX >= this.minX && rect.minY >= this.minY) && (rect.maxX <= this.maxX && rect.maxY <= this.maxY);
        }

        public static implicit operator UnityEngine.Rect(FixRect rect) => new UnityEngine.Rect(rect.minX, rect.minY, rect.maxX, rect.maxY);
        public static implicit operator FixRect(UnityEngine.Rect rect) => new FixRect((Fix64)rect.xMin, (Fix64)rect.yMin, (Fix64)rect.xMax, (Fix64)rect.yMax);

        public static FixRect Zero => new FixRect(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
    }

    public struct FixBounds2
    {
        public FixVector2 center;
        public FixVector2 extents;
        
        public FixBounds2(FixVector2 center, FixVector2 size)
        {
            this.center = new FixVector2(center);
            this.extents = new FixVector2(size / 2);
        }

        public static bool Intersect(FixBounds2 b0, FixBounds2 b1)
        {
            if (b0.center.x - b0.extents.x < b1.center.x + b1.extents.x ||
                b0.center.x + b0.extents.x < b1.center.x - b1.extents.x)
            {
                return false;
            }
            
            if (b0.center.y - b0.extents.y < b1.center.y + b1.extents.y ||
                b0.center.y + b0.extents.y < b1.center.y - b1.extents.y)
            {
                return false;
            }
            
            return true;
        }
    }
    
    public struct FixBounds3
    {
        public FixVector3 center;
        public FixVector3 extents;

        public FixBounds2 xy => new FixBounds2(this.center.xy, this.extents.xy);
        public FixBounds2 yx => new FixBounds2(this.center.yx, this.extents.yx);
        public FixBounds2 yz => new FixBounds2(this.center.yz, this.extents.yz);
        public FixBounds2 zy => new FixBounds2(this.center.zy, this.extents.zy);
        public FixBounds2 zx => new FixBounds2(this.center.zx, this.extents.zx);
        public FixBounds2 xz => new FixBounds2(this.center.xz, this.extents.xz);
        
        public FixBounds3(UnityEngine.Bounds bounds)
        {
            this.center = new FixVector3(bounds.center);
            this.extents = new FixVector3(bounds.extents);
        }
        
        public FixBounds3(FixVector3 center, FixVector3 size)
        {
            this.center = new FixVector3(center);
            this.extents = new FixVector3(size / (Fix64)2);
        }

        public static bool Intersect(FixBounds3 b0, FixBounds3 b1)
        {
            if (b0.center.x - b0.extents.x < b1.center.x + b1.extents.x ||
                b0.center.x + b0.extents.x < b1.center.x - b1.extents.x)
            {
                return false;
            }
            
            if (b0.center.y - b0.extents.y < b1.center.y + b1.extents.y ||
                b0.center.y + b0.extents.y < b1.center.y - b1.extents.y)
            {
                return false;
            }
            
            if (b0.center.z - b0.extents.z < b1.center.z + b1.extents.z ||
                b0.center.z + b0.extents.z < b1.center.z - b1.extents.z)
            {
                return false;
            }
            
            return true;
        }
        
        // https://medium.com/@bromanz/another-view-on-the-classic-ray-aabb-intersection-algorithm-for-bvh-traversal-41125138b525
        public bool Intersect(FixRay r)
        {
            FixVector3 invD = FixVector3.One / r.direction;
            FixVector3 t0s = (center - extents - r.origin) * invD;
            FixVector3 t1s = (center + extents - r.origin) * invD;
    
            FixVector3 tsmaller = FixVector3.Min(t0s, t1s);
            FixVector3 tbigger  = FixVector3.Max(t0s, t1s);
    
            Fix64 tmin = Fix64.Max(tsmaller.x, Fix64.Max(tsmaller.y, tsmaller.z));
            Fix64 tmax = Fix64.Min(tbigger.x, Fix64.Min(tbigger.y, tbigger.z));

            return (tmin < tmax);
        }
    }

    public struct FixRay
    {
        public FixVector3 origin;
        public FixVector3 direction;

        public FixRay(FixVector3 origin, FixVector3 direction)
        {
            this.origin = origin;
            this.direction = direction.normalized;
        }

        public FixVector3 GetPoint(Fix64 t)
        {
            return origin + direction * t;
        }
    }
}
