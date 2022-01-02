using System;

namespace Code.Common
{
    [Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;
        public float ConstantValue;
        public FloatValue Variable;

        public float Value => UseConstant ? ConstantValue : Variable.Value;
    }
}