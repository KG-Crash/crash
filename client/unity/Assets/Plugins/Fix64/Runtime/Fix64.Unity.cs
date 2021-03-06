using System;
using System.Linq;

namespace FixMath.NET
{
    [System.Serializable]
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
        
        public FixVector2(int x, int y)
        {
            this.x = (Fix64)x;
            this.y = (Fix64)y;
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

        public FixVector2 yx => new FixVector2(y, x);

        public static FixVector2 zero => new FixVector2(Fix64.Zero, Fix64.Zero);

        public static FixVector2 one => new FixVector2(Fix64.One, Fix64.One);

        public static FixVector2 operator +(FixVector2 v1, FixVector2 v2) => new FixVector2(v1.x + v2.x, v1.y + v2.y);
        
        public static FixVector2 operator -(FixVector2 v1, FixVector2 v2) => new FixVector2(v1.x - v2.x, v1.y - v2.y);

        public static FixVector2 operator *(FixVector2 v1, Fix64 v2) => new FixVector2(v1.x * v2, v1.y * v2);

        public static FixVector2 operator /(FixVector2 v1, Fix64 v2) => new FixVector2(v1.x / v2, v1.y / v2);

        public static implicit operator UnityEngine.Vector2(FixVector2 vector3) => new UnityEngine.Vector2(vector3.x, vector3.y);

        public static implicit operator FixVector2(UnityEngine.Vector3 vector3) => new FixVector2((Fix64)vector3.x, (Fix64)vector3.z);

        public static implicit operator FixVector3(FixVector2 vector2) => new FixVector3(vector2.x, Fix64.Zero, vector2.y);

        public static implicit operator UnityEngine.Vector3(FixVector2 vector2) => new UnityEngine.Vector3(vector2.x, 0.0f, vector2.y);

        public static FixVector2 Min(FixVector2 a, FixVector2 b) => new FixVector2(Fix64.Min(a.x, b.x), Fix64.Min(a.y, b.y));
        
        public static FixVector2 Max(FixVector2 a, FixVector2 b) => new FixVector2(Fix64.Max(a.x, b.x), Fix64.Max(a.y, b.y));

        public static Fix64 Dot(FixVector2 a, FixVector2 b) => a.x * b.x + a.y * b.y;

        public override string ToString()
        {
            return $"{nameof(FixVector2)}({x.ToString()}, {y.ToString()})";
        }
        public string ToString(string format, bool abbr = false)
        {
            var prefix = !abbr ? nameof(FixVector2) : "FV2";
            return $"{prefix}({x.ToString(format)}, {y.ToString(format)})";
        }
    }
    
    [System.Serializable]
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

        public FixVector3(FixVector2 vector2)
        {
            this.x = vector2.x;
            this.y = vector2.y;
            this.z = Fix64.Zero;
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

        public static bool operator ==(FixVector3 v1, FixVector3 v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;

        public static bool operator !=(FixVector3 v1, FixVector3 v2) => !(v1 == v2);

        public static implicit operator UnityEngine.Vector3(FixVector3 vector3) => new UnityEngine.Vector3(vector3.x, vector3.y, vector3.z);

        public static implicit operator FixVector3(UnityEngine.Vector3 vector3) => new FixVector3((Fix64)vector3.x, (Fix64)vector3.y, (Fix64)vector3.z);

        public static implicit operator FixVector2(FixVector3 vector3) => new FixVector2(vector3.x, vector3.z);

        public static FixVector3 Min(FixVector3 a, FixVector3 b) => new FixVector3(Fix64.Min(a.x, b.x), Fix64.Min(a.y, b.y), Fix64.Min(a.z, b.z));
        
        public static FixVector3 Max(FixVector3 a, FixVector3 b) => new FixVector3(Fix64.Max(a.x, b.x), Fix64.Max(a.y, b.y), Fix64.Max(a.z, b.z));

        public static Fix64 Dot(FixVector3 a, FixVector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static FixVector3 Cross(FixVector3 a, FixVector3 b) =>
            new FixVector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);

        public static FixVector3 One => new FixVector3(Fix64.One, Fix64.One, Fix64.One);
        public static FixVector3 Zero => new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static FixVector3 Up => new FixVector3(Fix64.Zero, Fix64.One, Fix64.Zero);
        public static FixVector3 Down => new FixVector3(Fix64.Zero, -Fix64.One, Fix64.Zero);
        public static FixVector3 Right => new FixVector3(Fix64.One, Fix64.Zero, Fix64.Zero);
        public static FixVector3 Left => new FixVector3(-Fix64.One, Fix64.Zero, Fix64.Zero);
        public static FixVector3 Forward => new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.One);
        public static FixVector3 Backword => new FixVector3(Fix64.Zero, Fix64.Zero, -Fix64.One);
        
        public override string ToString()
        {
            return $"{nameof(FixVector3)}({x.ToString()}, {y.ToString()}, {z.ToString()})";
        }
        public string ToString(string format, bool abbr = false)
        {
            var prefix = !abbr ? nameof(FixVector2) : "FV3";
            return $"{prefix}({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)})";
        }
    }

    public struct FixRect
    {
        public Fix64 minX { get; set; }
        public Fix64 minY { get; set; }
        public Fix64 maxX { get; set; }
        public Fix64 maxY { get; set; }
        public Fix64 width => maxX - minX;
        public Fix64 height => maxY - minY;
        public bool empty => width == Fix64.Zero || height == Fix64.Zero;

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

        public FixRect(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
        {
            this.minX = x;
            this.minY = y;
            this.maxX = x + width;
            this.maxY = y + height;
        }

        public FixRect(FixVector2 position, FixVector2 size) : this(position.x, position.y, size.x, size.y)
        { }

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
            {
                var points = new[]
                {
                    new FixVector2(this.minX, this.minY), // left  top
                    new FixVector2(this.maxX, this.minY), // right top
                    new FixVector2(this.minX, this.maxY), // left  bottom
                    new FixVector2(this.maxX, this.maxY), // right bottom
                };
                
                if (points.Any(point => rect.Contains(point)))
                    return true;
            }

            {
                var points = new[]
                {
                    new FixVector2(rect.minX, rect.minY), // left  top
                    new FixVector2(rect.maxX, rect.minY), // right top
                    new FixVector2(rect.minX, rect.maxY), // left  bottom
                    new FixVector2(rect.maxX, rect.maxY), // right bottom
                };

                var self = this;
                if (points.Any(point => self.Contains(point)))
                    return true;
            }

            return false;
        }

        public FixRect Padding(Fix64 value) => new FixRect(minX - value, minY - value, width + (value * Fix64.One * 2), height + (value * Fix64.One * 2));

        public static implicit operator UnityEngine.Rect(FixRect rect) => new UnityEngine.Rect(rect.minX, rect.minY, rect.maxX, rect.maxY);
        public static implicit operator FixRect(UnityEngine.Rect rect) => new FixRect((Fix64)rect.xMin, (Fix64)rect.yMin, (Fix64)rect.xMax, (Fix64)rect.yMax);

        public static FixRect Zero => new FixRect(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
    }
    
    [System.Serializable]
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
    
    [System.Serializable]
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
