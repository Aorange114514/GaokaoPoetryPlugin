using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using GaokaoPoetryPlugin.Services;

namespace GaokaoPoetryPlugin.Components;

[ComponentInfo("12345678-1234-1234-1234-123456789012", "高考古诗文", "随机展示高考必背古诗文名句")]
public partial class PoetryComponent : ComponentBase<PoetryComponentSettings>
{
    private readonly PoetryService _poetryService;
    private System.Timers.Timer? _timer;
    private bool _dataReady = false;

    /// <summary>
    /// 构造函数：通过 DI 注入 PoetryService。
    /// 如需自行创建实例（如设计时），可传入 null 使用默认 new。
    /// </summary>
    public PoetryComponent() : this(new PoetryService()) { }

    public PoetryComponent(PoetryService poetryService)
    {
        _poetryService = poetryService;
        InitializeComponent();
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloaded;
    }

    private async void OnLoadedAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadDataAsync();
        _dataReady = true;
        RefreshPoetry();
        StartTimer();

        if (Settings != null)
            Settings.PropertyChanged += OnSettingsPropertyChanged;
    }

    /// <summary>
    /// 加载数据：优先嵌入资源，失败后尝试网络回退。
    /// </summary>
    private async Task LoadDataAsync()
    {
        // 0. 背诵篇目数据（与主数据相互独立，失败不影响主数据）
        _poetryService.LoadRecitationFromEmbeddedResource();

        // 1. 尝试嵌入资源（离线模式）
        var ok = _poetryService.LoadFromEmbeddedResource();
        if (ok) return;

        // 2. 网络回退
        ok = await _poetryService.LoadFromNetworkAsync();
        if (ok) return;

        // 3. 全部失败 — 在下次 Refresh 时显示错误
    }

    private void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        if (Settings != null)
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
    }

    private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PoetryComponentSettings.IntervalSeconds):
                Dispatcher.UIThread.Invoke(() =>
                {
                    _timer?.Stop();
                    _timer?.Dispose();
                    StartTimer();
                });
                break;
            case nameof(PoetryComponentSettings.BookRequiredUp):
            case nameof(PoetryComponentSettings.BookRequiredDown):
            case nameof(PoetryComponentSettings.BookSelectiveUp):
            case nameof(PoetryComponentSettings.BookSelectiveMiddle):
            case nameof(PoetryComponentSettings.BookSelectiveDown):
            case nameof(PoetryComponentSettings.BookOther):
            case nameof(PoetryComponentSettings.ShowOnlyRecitation):
                Dispatcher.UIThread.Invoke(() => RefreshPoetry());
                break;
            default:
                Dispatcher.UIThread.Invoke(() => RefreshPoetry());
                break;
        }
    }

    private void StartTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = new System.Timers.Timer((Settings?.IntervalSeconds ?? 30) * 1000);
        _timer.Elapsed += (_, _) => Dispatcher.UIThread.Invoke(() => RefreshPoetry());
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void RefreshPoetry()
    {
        if (Settings == null)
            return;

        // 显示错误状态
        if (!_dataReady || !_poetryService.IsLoaded)
        {
            var error = _poetryService.LastError;
            if (!string.IsNullOrEmpty(error))
                PoetryTextBlock.Text = $"❌ 数据加载失败\n{error}";
            else
                PoetryTextBlock.Text = "⏳ 正在加载数据...";
            return;
        }

        var allowed = Settings.GetAllowedBooks();
        if (allowed.Count == 0)
        {
            PoetryTextBlock.Text = "请至少选择一个册别";
            return;
        }

        var item = _poetryService.GetRandom(allowed, Settings.ShowOnlyRecitation);
        if (item == null)
        {
            if (Settings.ShowOnlyRecitation)
                PoetryTextBlock.Text = _poetryService.RecitationCount == 0
                    ? "背诵篇目数据未加载"
                    : "暂无匹配的背诵篇目条目\n请检查册别筛选";
            else
                PoetryTextBlock.Text = "暂无匹配的古诗文条目";
            return;
        }

        var sentence = item.content.Length > 0 ? item.content[0] : "";

        // 构建信息行
        var parts = new List<string>();
        if (Settings.ShowAuthor && !string.IsNullOrEmpty(item.author))
            parts.Add(item.author);
        if (Settings.ShowTitle && !string.IsNullOrEmpty(item.title))
            parts.Add("《" + item.title + "》");
        parts.Add("[" + item.book + "]");
        var info = string.Join(" · ", parts);

        PoetryTextBlock.Text = string.IsNullOrEmpty(sentence)
            ? info
            : sentence + Environment.NewLine + info;
        PoetryTextBlock.FontSize = Settings.FontSize;
    }
}
