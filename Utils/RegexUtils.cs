using System.Text.RegularExpressions;

namespace GodotUtils.RegEx;

public static partial class RegexUtils
{
    [GeneratedRegex(@"(?<=type=""Script""[^\n]*path="")[^""]+(?="")", RegexOptions.Multiline)]
    public static partial Regex ScriptPath();

    [GeneratedRegex(@"[^\s""']+|""([^""]*)""|'([^']*)'")]
    public static partial Regex CommandParams();

    [GeneratedRegex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")]
    public static partial Regex IpAddress();

    [GeneratedRegex(@"^[a-zA-Z0-9\s,]*$")]
    public static partial Regex AlphaNumericAndSpaces();
}
