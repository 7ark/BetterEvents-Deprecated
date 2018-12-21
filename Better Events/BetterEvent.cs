using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
[HideReferenceObjectPicker]
public class BetterEvent
{
    [HideReferenceObjectPicker, ListDrawerSettings(CustomAddFunction = "GetDefaultBetterEvent", OnTitleBarGUI = "DrawInvokeButton")]
    public List<BetterEventEntry> Events;

    public void Invoke()
    {
        if (this.Events == null) return;
        for (int i = 0; i < this.Events.Count; i++)
        {
            this.Events[i].Invoke();
        }
    }

#if UNITY_EDITOR

    private BetterEventEntry GetDefaultBetterEvent()
    {
        return new BetterEventEntry(null);
    }

    private void DrawInvokeButton()
    {
        if (Sirenix.Utilities.Editor.SirenixEditorGUI.ToolbarButton("Invoke"))
        {
            this.Invoke();
        }
    }

#endif
}
