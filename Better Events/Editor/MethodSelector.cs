    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class MethodSelector : OdinSelector<DelegateInfo>
    {
        private BindingFlags bindingFlags = Flags.InstancePublic | BindingFlags.FlattenHierarchy;
        private UnityEngine.Object currentObj = null;
        private HashSet<string> seenMethods = new HashSet<string>();

        public MethodSelector(UnityEngine.Object obj)
        {
            this.currentObj = obj;
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Config.DrawSearchToolbar = true;
            tree.Selection.SupportsMultiSelect = false;
            tree.Add("None", new DelegateInfo());
            tree.Config.UseCachedExpandedStates = false;

            var gameObject = this.currentObj as GameObject;
            var component = this.currentObj as Component;

            if (component)
            {
                gameObject = component.gameObject;
            }

            if (gameObject)
            {
                this.AddMethods(tree, gameObject);

                var components = gameObject.GetComponents(typeof(Component));
                foreach (var c in components)
                {
                    this.AddMethods(tree, c);
                }
            }
            else if (this.currentObj)
            {
                this.AddMethods(tree, this.currentObj);
            }
            else
            {
                this.AddMethods(tree, null);
            }

            // Add icons
            foreach (var item in tree.EnumerateTree())
            {
                if (item.Value is DelegateInfo) continue;
                if (item.ChildMenuItems.Count == 0) continue;
                var child = item.ChildMenuItems[0];
                if (child.Value is DelegateInfo)
                {
                    var del = (DelegateInfo)child.Value;
                    item.IconGetter = () => GUIHelper.GetAssetThumbnail(del.Target, del.Method.DeclaringType, true);
                }
            }
        }

        public override bool IsValidSelection(IEnumerable<DelegateInfo> collection)
        {
            var info = collection.FirstOrDefault();
            return info.Method != null;
        }

        private void AddMethods(OdinMenuTree tree, UnityEngine.Object obj)
        {
            var methods = obj.GetType().GetMethods(this.bindingFlags);
            foreach (var mi in methods)
            {
                if (mi.ReturnType != typeof(void)) continue;
                if (mi.IsGenericMethod) continue;

                var path = mi.DeclaringType.GetNiceName() + "/" + mi.GetNiceName();
                if (this.seenMethods.Add(path))
                {
                    var target = mi.IsStatic ? null : obj;
                    var info = new DelegateInfo() { Target = target, Method = mi };
                    tree.Add(path, info);
                }
            }
        }
    }
