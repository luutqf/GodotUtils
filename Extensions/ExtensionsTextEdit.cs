namespace GodotUtils;

using Godot;
using System;
using System.Collections.Generic;

public static class ExtensionsTextEdit
{
    static readonly Dictionary<ulong, string> prevTexts = new();

    /// <summary>
    /// 这是一个扩展方法，它允许对 TextEdit 组件内的文本进行过滤。这个方法接收一个 filter 函数，这个函数使用字符串作为参数，并返回一个布尔值，用以指示文本是否通过过滤。
    /// 如果 TextEdit 的文本是空白或不存在，该方法返回之前的文本（如果有的话）。
    /// 如果文本不通过 filter 函数的检查，那么 TextEdit 会被重新设置为之前的有效文本（如果之前的文本存在的话），否则会清空文本。
    /// 如果文本通过了 filter 函数的检查，这个新的文本会被保存为当前的文本，并返回。
    /// 该方法利用 prevTexts 字典保存了之前有效的文本状态，以便在用户输入了无效的文本时可以恢复。这个字典以 TextEdit 实例的 id 作为键，文本作为值。
    /// </summary>
    /// <param name="textEdit"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        string text = textEdit.Text;
        ulong id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return prevTexts.ContainsKey(id) ? prevTexts[id] : null;

        if (!filter(text))
        {
            if (!prevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(prevTexts[id]);
            return prevTexts[id];
        }

        prevTexts[id] = text;
        return text;
    }
    static void ChangeTextEditText(this TextEdit textEdit, string text)
    {
        textEdit.Text = text;
        //textEdit.CaretColumn = text.Length;
    }
}
