namespace FixMath.NET
{
    public partial struct Fix64 
    {
        public static Fix64 Min(Fix64 a, Fix64 b)
        {
            return a < b? a: b;
        }
        
        public static Fix64 Max(Fix64 a, Fix64 b)
        {
            return a > b? a: b;
        }
    }
}