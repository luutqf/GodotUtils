namespace GodotUtils;

using Godot;
using System;
using System.Collections.Generic;

public static class ExtensionsLineEdit
{
    static readonly Dictionary<ulong, string> prevTexts = new();
    
    static readonly Dictionary<ulong, int> prevNums = new();

    /// <summary>
    /// Filter 方法通过一个委托 filter 接收一个条件，用来确定当前 LineEdit 控件中的文本是否满足某种规则。
    /// 如果文本不满足该规则，则该方法使 LineEdit 控件中的文本恢复到之前满足规则的内容，并返回该文本。如果满足规则，文本则保持不变。
    /// 这个方法还处理了空文本的情况，并在内部维护了一个 prevTexts 字典来跟踪每个 LineEdit 的先前符合条件的文本状态。
    /// </summary>
    /// <param name="lineEdit"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        string text = lineEdit.Text;
        ulong id = lineEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return prevTexts.GetValueOrDefault(id);

        if (!filter(text))
        {
            if (!prevTexts.TryGetValue(id, out var prevText))
            {
                lineEdit.ChangeLineEditText("");
                return null;
            }

            lineEdit.ChangeLineEditText(prevText);
            return prevTexts[id];
        }

        prevTexts[id] = text;
        return text;
    }

    /// <summary>
    /// FilterRange 方法用来限制在 LineEdit 中输入的数字或长度必须在一定的范围内。maxRange 参数指定了数字的最大可能值。如
    /// 果用户输入的文本不能转换为一个数字或者数字大于 maxRange，LineEdit 控件的文本将被更改为上一次有效的数字，或者空字符串（如果之前没有有效数字）。
    /// 此外，还会特别处理超出范围长度的文本，以及文本中插入非数字字符的情况。该方法同样利用了一个 prevNums 字典来跟踪每个 LineEdit 控件的先前有效的数字。
    /// </summary>
    /// <param name="lineEdit"></param>
    /// <param name="maxRange"></param>
    /// <returns></returns>
    public static int FilterRange(this LineEdit lineEdit, int maxRange)
    {
        string text = lineEdit.Text;
        ulong id = lineEdit.GetInstanceId();

        // Ignore blank spaces
        if (string.IsNullOrWhiteSpace(text))
        {
            lineEdit.ChangeLineEditText("");
            return 0;
        }

        // Text is not a number
        if (!int.TryParse(text.Trim(), out int num))
        {
            // No keys are in the dictionary for the first case, so handle this by returning 0
            if (!prevNums.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return 0;
            }

            // Scenario #1: Text is 'a'  -> returns ""
            // Scenario #2: Text is '1a' -> returns ""
            if (text.Length == 1 || text.Length == 2)
            {
                if (!int.TryParse(text, out int number))
                {
                    lineEdit.ChangeLineEditText("");
                    return 0;
                }
            }

            // Text is '123', user types a letter -> returns "123"
            lineEdit.ChangeLineEditText($"{prevNums[id]}");
            return prevNums[id];
        }

        // Not sure why this is here but I'm sure it's here for a good reason
        if (text.Length > maxRange.ToString().Length && num <= maxRange)
        {
            string spliced = text.Remove(text.Length - 1);
            prevNums[id] = int.Parse(spliced);

            lineEdit.Text = spliced;
            lineEdit.CaretColumn = spliced.Length;
            return prevNums[id];
        }

        // Text is at max range, return max range text if greater than max range
        if (num > maxRange)
        {
            num = maxRange;
            lineEdit.ChangeLineEditText($"{maxRange}");
        }

        // Keep track of the previous number
        prevNums[id] = num;
        return num;
    }

    /// <summary>
    /// ChangeLineEditText 是一个私有辅助方法，用于更改 LineEdit 控件的文本并将插入（光标）位置置于文本末尾。
    /// 这个方法在上面的 Filter 和 FilterRange 方法中被调用。
    /// </summary>
    /// <param name="lineEdit"></param>
    /// <param name="text"></param>
    static void ChangeLineEditText(this LineEdit lineEdit, string text)
    {
        lineEdit.Text = text;
        lineEdit.CaretColumn = text.Length;
    }
}
