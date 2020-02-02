using System.Reflection;

namespace SamSeifert.Utilities {
    public static class Hacking {
        public static T GetPrivateField<T, X>(X instance, string fieldName) {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo fieldInfo = instance.GetType().GetField(fieldName, bindFlags);
            if (fieldInfo == null)
                return default;
            var obj = fieldInfo.GetValue(instance);
            if (!typeof(T).IsInstanceOfType(obj))
                return default;
            return (T) obj;
        }

        /// <summary>
        /// Throws exception or array length 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="instances"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T[] GetPrivateFields<T, X>(X[] instances, string fieldName) {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo fieldInfo = instances[0].GetType().GetField(fieldName, bindFlags);
            if (fieldInfo == null)
                return null;
            var ts = new T[instances.Length];
            var obj = fieldInfo.GetValue(instances[0]);
            if (!typeof(T).IsInstanceOfType(obj))
                return default;
            ts[0] = (T) obj;
            for (int i = 1; i < instances.Length; i++) {
                ts[i] = (T) fieldInfo.GetValue(instances[i]);
            }
            return ts;
        }

    }
}
