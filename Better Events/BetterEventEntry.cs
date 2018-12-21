using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BetterEventEntry : ISerializationCallbackReceiver
{
    [NonSerialized, HideInInspector]
    public Delegate Delegate;

    [NonSerialized, HideInInspector]
    public object[] ParameterValues;

    public BetterEventEntry(Delegate del) 
    {
        if (del != null && del.Method != null)
        {
            this.Delegate = del;
            this.ParameterValues = new object[del.Method.GetParameters().Length];
        }
    }

    public void Invoke()
    {
        if (this.Delegate != null && this.ParameterValues != null)
        {
            this.Delegate.DynamicInvoke(this.ParameterValues);
        }
    }

    #region OdinSerialization
    [SerializeField, HideInInspector]
    private List<UnityEngine.Object> unityReferences;

    [SerializeField, HideInInspector]
    private byte[] bytes;

    public void OnAfterDeserialize()
    {
        var val = SerializationUtility.DeserializeValue<OdinSerializedData>(this.bytes, DataFormat.Binary, this.unityReferences);
        this.Delegate = val.Delegate;
        this.ParameterValues = val.ParameterValues;
    }

    public void OnBeforeSerialize()
    {
        var val = new OdinSerializedData() { Delegate = this.Delegate, ParameterValues = this.ParameterValues };
        this.bytes = SerializationUtility.SerializeValue(val, DataFormat.Binary, out this.unityReferences);
    }

    private struct OdinSerializedData
    {
        public Delegate Delegate;
        public object[] ParameterValues;
    }
    #endregion
}
