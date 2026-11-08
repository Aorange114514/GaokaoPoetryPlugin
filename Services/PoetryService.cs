using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using GaokaoPoetryPlugin.Models;

namespace GaokaoPoetryPlugin.Services;

/// <summary>
/// 古诗文数据服务：负责加载数据（嵌入资源 / 网络回退）和随机选取。
/// </summary>
public class PoetryService
{
    private readonly List<PoetryItem> _items = new();
    private readonly List<PoetryItem> _recitationItems = new();
    private readonly Random _random = new();
    private readonly HttpClient _http = new();
    private bool _loaded = false;

    /// <summary>是否已成功加载数据</summary>
    public bool IsLoaded => _loaded;

    /// <summary>已加载的条目数量</summary>
    public int Count => _items.Count;

    /// <summary>已加载的背诵篇目条目数量</summary>
    public int RecitationCount => _recitationItems.Count;

    /// <summary>最近一次加载的错误信息（null 表示无错误）</summary>
    public string? LastError { get; private set; }

    /// <summary>
    /// 从嵌入资源加载 divided.json（离线模式）。
    /// </summary>
    /// <returns>是否加载成功</returns>
    public bool LoadFromEmbeddedResource()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("divided.json"));

            if (resourceName == null)
            {
                LastError = "嵌入资源中未找到 divided.json";
                return false;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                LastError = "无法读取嵌入资源流";
                return false;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<PoetryItem>>(json);
            if (items != null && items.Count > 0)
            {
                _items.Clear();
                _items.AddRange(items);
                _loaded = true;
                LastError = null;
                return true;
            }

            LastError = "嵌入资源数据为空或格式错误";
            return false;
        }
        catch (Exception ex)
        {
            LastError = $"嵌入资源加载失败: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 从嵌入资源加载背诵篇目数据 mustResite.json。
    /// 该数据不属于常规 60 篇数据集，仅在勾选「仅展示背诵篇目」时参与轮播；
    /// 数据中不含册别字段，加载时按篇名补齐。
    /// </summary>
    /// <returns>是否加载到数据（资源缺失或为空返回 false，不影响主数据）</returns>
    public bool LoadRecitationFromEmbeddedResource()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("mustResite.json"));

            if (resourceName == null)
                return false;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return false;

            using var reader = new StreamReader(stream);
            var items = JsonSerializer.Deserialize<List<PoetryItem>>(reader.ReadToEnd());
            if (items == null)
                return false;

            foreach (var item in items)
                item.book = RecitationCatalog.GetBook(item.title);

            _recitationItems.Clear();
            _recitationItems.AddRange(items);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 从网络加载数据（在线回退模式）。
    /// </summary>
    /// <returns>是否加载成功</returns>
    public async Task<bool> LoadFromNetworkAsync()
    {
        if (_loaded) return true;

        try
        {
            const string url = "https://clover-yan.github.io/gaokao-poetry/generated/divided.json";
            var json = await _http.GetStringAsync(url);
            var items = JsonSerializer.Deserialize<List<PoetryItem>>(json);
            if (items != null && items.Count > 0)
            {
                _items.Clear();
                _items.AddRange(items);
                _loaded = true;
                LastError = null;
                return true;
            }

            LastError = "网络数据为空或格式错误";
            return false;
        }
        catch (HttpRequestException ex)
        {
            LastError = $"网络请求失败: {ex.Message}";
            return false;
        }
        catch (TaskCanceledException)
        {
            LastError = "网络请求超时";
            return false;
        }
        catch (Exception ex)
        {
            LastError = $"网络加载失败: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 从允许的册别列表中随机选取一条。
    /// </summary>
    /// <param name="allowedBooks">允许的册别</param>
    /// <param name="onlyRecitation">仅从背诵篇目数据（mustResite.json）中选取</param>
    public PoetryItem? GetRandom(List<string> allowedBooks, bool onlyRecitation = false)
    {
        IEnumerable<PoetryItem> pool = onlyRecitation ? _recitationItems : _items;

        var filtered = pool.Where(i => allowedBooks.Contains(i.book)).ToList();
        if (filtered.Count == 0) return null;
        return filtered[_random.Next(filtered.Count)];
    }

    /// <summary>
    /// 获取数据中所有不重复的册别。
    /// </summary>
    public List<string> GetAllBooks()
    {
        return _items.Select(i => i.book).Distinct().OrderBy(b => b).ToList();
    }
}
