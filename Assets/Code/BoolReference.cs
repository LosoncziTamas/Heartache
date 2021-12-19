using System;

namespace Code
{
    [Serializable]
    public class BoolReference
    {
        public bool UseConstant = true;
        public bool ConstantValue;
        public BoolValue Variable;

        public bool Value => UseConstant ? ConstantValue : Variable.Value;
    }
}