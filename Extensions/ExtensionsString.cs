using Godot;

namespace GodotUtils;

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class ExtensionsString
{
    public static bool IsAddress(this string v) =>
        v != null && (Regex.IsMatch(v, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}") || v.Contains("localhost"));

    /// <summary>
    /// 在每一个大写字母前添加空格，用于将驼峰命名风格（CamelCase）的字符串转换为有空格的短语。
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string AddSpaceBeforeEachCapital(this string v) =>
        string.Concat(v.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

    /// <summary>
    /// 把字符串转换为标题大小写（Title Case），即每个单词的首字母大写。
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string v) =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(v.ToLower());

    /// <summary>
    /// 使用正则表达式检查字符串是否与给定的模式匹配。
    /// </summary>
    /// <param name="v"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static bool IsMatch(this string v, string expression) =>
        Regex.IsMatch(v, expression);

    
    public static bool IsNum(this string v) =>
        int.TryParse(v, out _);

    /// <summary>
    /// 将字符串中长度小于或等于 maxLength 的单词全部转换为大写，可以提供一个可选的筛选函数 filter 来决定哪些单词应该被转换。
    /// </summary>
    /// <param name="v"></param>
    /// <param name="maxLength"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static string SmallWordsToUpper(this string v, int maxLength = 2, Func<string, bool> filter = null)
    {
        string[] words = v.Split(' ');

        for (int i = 0; i < words.Length; i++)
            if (words[i].Length <= maxLength && (filter == null || filter(words[i])))
                words[i] = words[i].ToUpper();

        return string.Join(" ", words);
    }

    /// <summary>
    /// 检查一个字符串是否只包含数字字符。
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static bool IsDigitsOnly(this string v)
    {
        foreach (char c in v)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Strips all BBCode from a given string. Note that this means you can no
    /// longer use square brackets '[]' for stylish purposes.
    /// 移除字符串中的所有 BBCode 格式代码，使用正则表达式忽略方括号中的所有内容。
    /// 
    /// </summary>
    /// <param name="source">The string to strip the BBCode from</param>
    /// <returns>The string without the BBCode</returns>
    public static string StripBBCode(this string source)
    {
        RegEx regex = new RegEx();
        regex.Compile("\\[.+?\\]");
        return regex.Sub(source, replacement: "", all: true);
    }
}
