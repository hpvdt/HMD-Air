namespace HMDCommons.Scripts
{
    using System;
    using System.Reflection;
    using Unity.VisualScripting;
    using UnityEngine;
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    // [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)] TODO: enable this
    public class RequiredAttribute : PropertyAttribute
    {
    }

    public static class NullSafeGameObjectChecker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckNullSafeGameObjects()
        {
            var objects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var obj in objects)
            {
                var type = obj.GetType();
                var fields = type.GetFields(BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                // var properties = type.GetProperties(BindingFlags.Instance |
                //                                     BindingFlags.Public |
                //                                     BindingFlags.NonPublic);

                var enabledOnClass = Attribute.GetCustomAttribute(type, typeof(RequiredAttribute)) is
                    RequiredAttribute;

                foreach (var field in fields)
                    if (enabledOnClass || Attribute.GetCustomAttribute(field, typeof(RequiredAttribute)) is
                            RequiredAttribute)
                    {
                        var value = field.GetValue(obj);
                        Report(value, type, field);
                    }

                // foreach (var property in properties)
                //     if (enabledOnClass || Attribute.GetCustomAttribute(property, typeof(NullSafeAttribute)) is
                //             NullSafeAttribute)
                //         if (property.IsAccessor())
                //         {
                //             var value = property.GetValue(obj);
                //             Report(value, type, property);
                //         }
            }

            void Report(object value, Type type, MemberInfo field)
            {
                if (value == null || value.IsUnityNull())
                    Debug.LogException(new NullReferenceException(
                        $"NullSafe violation: {type.Name}.{field.Name} is null")
                    );
            }
        }
    }
}
