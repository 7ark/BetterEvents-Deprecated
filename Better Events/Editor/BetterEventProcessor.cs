using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;

// This turns the BetterEventEntry.ParameterValues object array into properties in the inspector.

public class BetterEventProcessor : OdinPropertyProcessor<BetterEventEntry>
{
    public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
    {
        var val = (BetterEventEntry)this.Property.ValueEntry.WeakSmartValue;
        if (val.Delegate == null) return;
        if (val.Delegate.Method == null) return;

        var ps = val.Delegate.Method.GetParameters();
        for (int i = 0; i < ps.Length; i++)
        {
            var p = ps[i];
            var getterSetterType = typeof(ArrayIndexGetterSetter<>).MakeGenericType(p.ParameterType);
            var getterSetter = Activator.CreateInstance(getterSetterType, new object[] { this.Property, i }) as IValueGetterSetter;
            var info = InspectorPropertyInfo.CreateValue(p.Name, i, SerializationBackend.Odin, getterSetter);
            propertyInfos.Add(info);
        }
    }

    private class ArrayIndexGetterSetter<T> : IValueGetterSetter<object, T>
    {
        private readonly InspectorProperty property;
        private readonly int index;

        public bool IsReadonly { get { return false; } }
        public Type OwnerType { get { return typeof(object); } }
        public Type ValueType { get { return typeof(T); } }

        public object[] parameterValues
        {
            get
            {
                var val = (BetterEventEntry)this.property.ValueEntry.WeakSmartValue;
                return val.ParameterValues;
            }
        }

        public ArrayIndexGetterSetter(InspectorProperty property, int index)
        {
            this.property = property;
            this.index = index;
        }

        public T GetValue(ref object owner)
        {
            var parms = this.parameterValues;
            if (parms == null || this.index >= parms.Length) { return default(T); }

            if (parms[this.index] == null)
            {
                return default(T);
            }
            try
            {
                return (T)parms[this.index];
            }
            catch
            {
                return default(T);
            }
        }

        public object GetValue(object owner)
        {
            var parms = this.parameterValues;
            if (parms == null || this.index >= parms.Length) { return default(T); }

            return this.parameterValues[this.index];
        }

        public void SetValue(ref object owner, T value)
        {
            var parms = this.parameterValues;
            if (parms == null || this.index >= parms.Length) { return ; }

            parms[this.index] = value;
        }

        public void SetValue(object owner, object value)
        {
            var parms = this.parameterValues;
            if (parms == null || this.index >= parms.Length) { return; }

            parms[this.index] = value;
        }
    }
}
