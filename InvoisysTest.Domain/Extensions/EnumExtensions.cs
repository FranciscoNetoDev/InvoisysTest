using System.ComponentModel;
using System.Reflection;

namespace InvoisysTest.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        return fieldInfo?
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? value.ToString();
    }
}
