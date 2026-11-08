using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GaokaoPoetryPlugin.Services;

/// <summary>
/// 背诵篇目数据（mustResite.json）不含册别字段，这里按统编版教材指定篇目所属册别。
/// </summary>
public static class RecitationCatalog
{
    private const string RequiredUp = "必修上";
    private const string RequiredDown = "必修下";
    private const string SelectiveUp = "选择性必修上";
    private const string SelectiveMiddle = "选择性必修中";
    private const string SelectiveDown = "选择性必修下";
    private const string Other = "其他";

    /// <summary>篇名 -> 教材册别。</summary>
    private static readonly Dictionary<string, string> TitleToBook = new()
    {
        // 必修上
        ["短歌行"] = RequiredUp,
        ["沁园春·长沙"] = RequiredUp,
        ["梦游天姥吟留别"] = RequiredUp,
        ["登高"] = RequiredUp,
        ["念奴娇·赤壁怀古"] = RequiredUp,
        ["劝学"] = RequiredUp,
        ["师说"] = RequiredUp,
        ["赤壁赋"] = RequiredUp,

        // 必修下
        ["子路、曾皙、冉有、公西华侍坐"] = RequiredDown,
        ["阿房宫赋"] = RequiredDown,
        ["六国论"] = RequiredDown,

        // 选择性必修上
        ["《论语》十二章"] = SelectiveUp,
        ["大学之道"] = SelectiveUp,
        ["《老子》四章"] = SelectiveUp,

        // 选择性必修中
        ["屈原列传"] = SelectiveMiddle,
        ["过秦论"] = SelectiveMiddle,

        // 选择性必修下
        ["离骚"] = SelectiveDown,
        ["蜀道难"] = SelectiveDown,
        ["蜀相"] = SelectiveDown,
        ["陈情表"] = SelectiveDown,
        ["兰亭集序"] = SelectiveDown,
    };

    /// <summary>归一化篇名 -> 册别，避免书名号、间隔号、括号注记等写法差异。</summary>
    private static readonly Dictionary<string, string> NormalizedTitleToBook = TitleToBook
        .ToDictionary(pair => Normalize(pair.Key), pair => pair.Value);

    /// <summary>获取背诵篇目所属册别；未收录的篇目归入「其他」。</summary>
    public static string GetBook(string title)
    {
        return NormalizedTitleToBook.TryGetValue(Normalize(title), out var book) ? book : Other;
    }

    /// <summary>去掉括号注记后只保留文字与数字，忽略标点、空白和异体字差异。</summary>
    private static string Normalize(string title)
    {
        var core = Regex.Replace(title, @"[（(][^）)]*[）)]", "");
        var builder = new StringBuilder(core.Length);
        foreach (var ch in core)
        {
            if (!char.IsLetterOrDigit(ch))
                continue;

            builder.Append(ch == '皙' ? '晳' : ch);
        }

        return builder.ToString();
    }
}
