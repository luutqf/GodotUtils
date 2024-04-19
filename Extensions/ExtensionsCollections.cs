namespace GodotUtils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class CollectionExtensions
{
    /// <summary>
    /// A convience method for a foreach loop at the the sacrafice of debugging support
    /// 以牺牲调试支持为代价，为foreach循环提供了一个方便的方法
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (T element in value)
            action(element);
    }

    /// <summary>
    /// Returns true if a dictionary has a duplicate key and warns the coder
    /// 如果字典有重复键则返回true并警告编码器
    /// </summary>
    public static bool Duplicate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (!dict.ContainsKey(key))
            return false;

        //Logger.LogWarning($"'{caller}' tried to add duplicate key '{key}' to dictionary\n" +
        //                    $"   at {path} line:{lineNumber}");

        return true;
    }

    /// <summary>
    /// Returns true if a dictionary has a non-existent key and warns the coder
    /// 如果字典的键不存在，则返回true并警告编码器
    /// </summary>
    public static bool DoesNotHave<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (dict.ContainsKey(key))
            return false;

        //Logger.LogWarning($"'{caller}' tried to access non-existent key '{key}' from dictionary\n" +
        //                    $"   at {path} line:{lineNumber}");

        return true;
    }
}
