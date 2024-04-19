using System.Linq;

namespace GodotUtils;

using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class ExtensionsPrint
{
    /// <summary>
    /// Prints a collection in a readable format
    /// 以可读格式打印集合
    /// </summary>
    public static string Print<T>(this IEnumerable<T> value, bool newLine = true) =>
        value != null ? string.Join(newLine ? "\n" : ", ", value) : null;

    /// <summary>
    /// Prints the entire object in a readable format (supports Godot properties)
    /// 以可读格式打印整个对象(支持Godot属性)
    /// If you should ever run into a problem, see the IgnorePropsResolver class to ignore more
    /// properties.
    /// 如果遇到问题，请参阅IgnorePropsResolver类来忽略更多属性。
    /// </summary>
    public static string PrintFull(this object v) =>
        JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver() // ignore all Godot props
        });

    /// <summary>
    /// Used when doing JsonConvert.SerializeObject to ignore Godot properties
    /// as these are massive.
    /// 在做json转换时使用。SerializeObject来忽略Godot属性，因为这些属性非常大。
    /// </summary>
    class IgnorePropsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop =
                base.CreateProperty(member, memberSerialization);

            // Ignored properties (prevents crashes)
            // 忽略属性(防止崩溃)
            var ignoredProps = new Type[]
            {
                typeof(GodotObject),
                typeof(Node),
                typeof(NodePath),
                typeof(ENet.Packet)
            };

            foreach (Type ignoredProp in ignoredProps)
            {
                if (ignoredProp.GetProperties().Contains(member))
                    prop.Ignored = true;

                if (prop.PropertyType != null && (prop.PropertyType == ignoredProp || prop.PropertyType.IsSubclassOf(ignoredProp)))
                    prop.Ignored = true;
            }

            return prop;
        }
    }
}
