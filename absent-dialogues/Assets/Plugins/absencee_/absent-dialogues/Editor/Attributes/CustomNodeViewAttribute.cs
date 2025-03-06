using com.absence.dialoguesystem.internals;
using System;

namespace com.absence.dialoguesystem.editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomNodeViewAttribute : Attribute
    {
        public Type type;
        public bool useForChildren;

        public CustomNodeViewAttribute(Type type, bool useForChildren = true)
        {
            Type baseType = type;
            while (baseType != null)
            {
                if (baseType.Equals(typeof(Node)))
                    break;

                baseType = baseType.BaseType;
            }

            if (baseType == null)
                throw new ArgumentException("Argument must derived from the type: 'Node'.", "type");

            this.type = type;
            this.useForChildren = useForChildren;
        }
    }
}