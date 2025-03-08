using com.absence.dialoguesystem.builtin;
using System;

namespace com.absence.dialoguesystem.internals
{
    public static class CustomData
    {
        public static Type DefaultDataType = typeof(NodeCustomData);

        public static Type NodeDataType = DefaultDataType;
        public static Type OptionDataType = DefaultDataType;
        public static Type GenericOptionDataType = DefaultDataType;
    }
}
