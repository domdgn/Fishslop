using System;

namespace DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NumberRangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }

        public NumberRangeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
        
        public NumberRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}