using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GaokaoPoetryPlugin.Components;

/// <summary>
/// 插件设置：轮播间隔、显示选项、册别筛选。
/// </summary>
public class PoetryComponentSettings : INotifyPropertyChanged
{
    // ---- 轮播 & 显示设置 ----
    private int _intervalSeconds = 30;
    private bool _showAuthor = true;
    private bool _showTitle = true;
    private double _fontSize = 14;
    private bool _showOnlyRecitation = false;

    public int IntervalSeconds
    {
        get => _intervalSeconds;
        set { _intervalSeconds = value; OnPropertyChanged(); }
    }

    public bool ShowAuthor
    {
        get => _showAuthor;
        set { _showAuthor = value; OnPropertyChanged(); }
    }

    public bool ShowTitle
    {
        get => _showTitle;
        set { _showTitle = value; OnPropertyChanged(); }
    }

    public double FontSize
    {
        get => _fontSize;
        set { _fontSize = value; OnPropertyChanged(); }
    }

    /// <summary>仅展示高考背诵篇目（含指定段落）。</summary>
    public bool ShowOnlyRecitation
    {
        get => _showOnlyRecitation;
        set { _showOnlyRecitation = value; OnPropertyChanged(); }
    }

    // ---- 册别筛选 ----
    // 使用字典集中管理，减少样板代码；保留独立属性以兼容 XAML 绑定

    private readonly Dictionary<string, bool> _bookFilters = new()
    {
        ["必修上"] = true,
        ["必修下"] = true,
        ["选择性必修上"] = true,
        ["选择性必修中"] = true,
        ["选择性必修下"] = true,
        ["其他"] = true,
    };

    public bool BookRequiredUp
    {
        get => _bookFilters["必修上"];
        set { _bookFilters["必修上"] = value; OnPropertyChanged(); }
    }

    public bool BookRequiredDown
    {
        get => _bookFilters["必修下"];
        set { _bookFilters["必修下"] = value; OnPropertyChanged(); }
    }

    public bool BookSelectiveUp
    {
        get => _bookFilters["选择性必修上"];
        set { _bookFilters["选择性必修上"] = value; OnPropertyChanged(); }
    }

    public bool BookSelectiveMiddle
    {
        get => _bookFilters["选择性必修中"];
        set { _bookFilters["选择性必修中"] = value; OnPropertyChanged(); }
    }

    public bool BookSelectiveDown
    {
        get => _bookFilters["选择性必修下"];
        set { _bookFilters["选择性必修下"] = value; OnPropertyChanged(); }
    }

    public bool BookOther
    {
        get => _bookFilters["其他"];
        set { _bookFilters["其他"] = value; OnPropertyChanged(); }
    }

    /// <summary>获取当前勾选的册别列表。</summary>
    public List<string> GetAllowedBooks()
    {
        var list = new List<string>();
        foreach (var (book, enabled) in _bookFilters)
            if (enabled) list.Add(book);
        return list;
    }

    /// <summary>检查是否所有册别都被选中。</summary>
    public bool AllBooksSelected =>
        _bookFilters.Values.All(v => v);

    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
