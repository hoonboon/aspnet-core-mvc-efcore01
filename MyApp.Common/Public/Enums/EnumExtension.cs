using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyApp.Common.Public.Enums
{
    public static class EnumExtension
    {
        public static EnumItemDisplayValues GetEnumItemDisplayValues(this Enum value)
        {
            EnumItemDisplayValues results = new EnumItemDisplayValues();

            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                results.Name = attributes[0].GetName();
                results.Description = attributes[0].GetDescription();
                results.GroupName = attributes[0].GetGroupName();
            }
            else
            {
                results.Name = value.ToString();
                results.Description = value.ToString();
                results.GroupName = value.ToString();
            }

            ObsoleteAttribute[] attribObsolete = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (attribObsolete != null && attribObsolete.Length > 0)
            {
                results.IsObsolete = true;
            }

            return results;
        }
    }
}

namespace MyApp.Common.Public.Enums
{
    public class EnumItemDisplayValues
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool IsObsolete { get; set; } = false;
    }
}
