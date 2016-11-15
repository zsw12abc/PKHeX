using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Reflection
{
    public class BatchEditorStringInstruction
    {
        public string PropertyName;
        public string PropertyValue;
        public bool Evaluator;
        public void setScreenedValue(string[] arr)
        {
            int index = Array.IndexOf(arr, PropertyValue);
            PropertyValue = index > -1 ? index.ToString() : PropertyValue;
        }
    }
}
