using System;
using System.Reflection;

namespace MonoGame.Ruge.Glide {
    internal class GlideInfo {
        private static readonly BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                                     BindingFlags.Instance | BindingFlags.Static;

        private readonly FieldInfo field;
        private readonly PropertyInfo prop;
        private readonly object Target;

        public GlideInfo(object target, PropertyInfo info) {
            Target = target;
            prop = info;
            PropertyName = info.Name;
            PropertyType = prop.PropertyType;
        }

        public GlideInfo(object target, FieldInfo info) {
            Target = target;
            field = info;
            PropertyName = info.Name;
            PropertyType = info.FieldType;
        }

        public GlideInfo(object target, string property, bool writeRequired = true) {
            Target = target;
            PropertyName = property;

            var targetType = target as Type ?? target.GetType();

            if ((field = targetType.GetField(property, flags)) != null) {
                PropertyType = field.FieldType;
            }
            else if ((prop = targetType.GetProperty(property, flags)) != null) {
                PropertyType = prop.PropertyType;
            }
            else {
                //	Couldn't find either
                throw new Exception(string.Format("Field or {0} property '{1}' not found on object of type {2}.",
                    writeRequired ? "read/write" : "readable",
                    property, targetType.FullName));
            }
        }

        public string PropertyName { get; private set; }
        public Type PropertyType { get; private set; }

        public object Value {
            get {
                return field != null
                    ? field.GetValue(Target)
                    : prop.GetValue(Target, null);
            }

            set {
                if (field != null) field.SetValue(Target, value);
                else prop.SetValue(Target, value, null);
            }
        }
    }
}