using System;
using System.Reflection;
using Verse;

namespace Keepercraft.RimKeeperTakeHemopacks.Extensions
{
    public static class GenericExtension
    {
        public static void SetPrivateField(this object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
            else
            {
                Log.Error("[RimKeeperTakeHemopacks] SetPrivateField:" + fieldName);
            }
        }

        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                return (T)fieldInfo.GetValue(obj);
            }
            else
            {
                Log.Error("[RimKeeperTakeHemopacks] SetPrivateField:" + fieldName);
                return default;
            }
        }

        public static T GetPrivateMethod<T>(this object obj, string methodName, params object[] methodParams)
        {
            Type type = obj.GetType();

            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
            {
                return (T)methodInfo.Invoke(obj, methodParams);
            }
            else
            {
                Log.Error("[RimKeeperTakeHemopacks] GetPrivateMethod:" + methodName);
                return default;
            }
        }
    }
}
