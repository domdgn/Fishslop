using System;

namespace DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NumberStepAttribute : Attribute
    {
        public float Step { get; }

        public NumberStepAttribute(int step)
        {
            Step = step;
        }
        
        public NumberStepAttribute(float step)
        {
            Step = step;
        }
    }
}