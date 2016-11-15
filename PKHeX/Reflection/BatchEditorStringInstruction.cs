using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Reflection
{
    public class BatchEditorStringInstruction
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Target value of the property
        /// </summary>
        public string PropertyValue { get; set; }

        /// <summary>
        /// The evaluation mode.  True for equality, false for inequality.  Ignored for instructions.
        /// </summary>
        public bool Evaluator { get; set; }

        public void setScreenedValue(string[] arr)
        {
            int index = Array.IndexOf(arr, PropertyValue);
            PropertyValue = index > -1 ? index.ToString() : PropertyValue;
        }
    }
}
