using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public static class ExtensionMethods
    {
        public static void SetPortColor(this Port port, Color connectedColor, Color notConnectedColor)
        {
            VisualElement connector = port.Q("connector");
            VisualElement cap = connector.Q("cap");

            Color color = port.connected ? connectedColor : notConnectedColor;

            cap.style.backgroundColor = color;
            connector.style.borderTopColor = connectedColor;
            connector.style.borderRightColor = connectedColor;
            connector.style.borderBottomColor = connectedColor;
            connector.style.borderLeftColor = connectedColor;
        }
        public static void SetPortLabelColor(this Port port, Color color)
        {
            VisualElement label = port.Q("type");

            label.style.color = color;
        }
    }
}
