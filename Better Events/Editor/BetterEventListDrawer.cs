using Sirenix.OdinInspector.Editor;
using UnityEngine;

public class BetterEventListDrawer : OdinValueDrawer<BetterEvent>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        this.Property.Children["Events"].Draw(label);
    }
}
