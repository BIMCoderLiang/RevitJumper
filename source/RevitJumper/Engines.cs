
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RevitJumper
{
    public enum Engines
    {
        [Description("Revitapidocs")]
        Revitapidocs,

        [Description("Revit API Forum")]
        RevitAPIForum,
    }

    public class DescriptionAttributeUtility
    {
        public static string GetDescriptionFromEnumValue(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null)
                return String.Empty;

            object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (!customAttributes.Any<object>())
                return String.Empty;

            DescriptionAttribute attribute = customAttributes.SingleOrDefault() as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DescriptionAttribute)a.Att)
                                .Description == description);
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}
