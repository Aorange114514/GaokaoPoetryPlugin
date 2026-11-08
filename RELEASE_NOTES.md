# v1.2.1.0 更新说明

> **发布日期**: 2026-11-08  
> **下载**: [GaokaoPoetryPlugin.cipx](https://github.com/xiaoxinxin-Zn/GaokaoPoetryPlugin/releases/download/v1.2.1.0/GaokaoPoetryPlugin.cipx)

---

## 一、新增功能：仅展示背诵篇目

### 1.1 内容筛选

设置面板新增「内容筛选」区域，勾选「仅展示背诵篇目」后，轮播池只包含课本要求背诵的篇目，并与册别筛选自动取交集：

- **勾选后**：只轮播背诵篇目；若当前册别筛选下没有匹配条目，组件会提示「暂无匹配的背诵篇目条目，请检查册别筛选」
- **取消勾选**：立即恢复 60 篇全量轮播
- **数据未加载时**：明确提示「背诵篇目数据未加载」，不再静默空白

### 1.2 背诵篇目数据

新增 `Assets/mustResite.json`：**21 篇、533 条名句**，按高考要求收录全文或指定段落：

- 《师说》仅收录第一段（「古之学者必有师……道之所存，师之所存也」）
- 《屈原列传》收录第三段（「屈平疾王听之不聪也……虽与日月争光可也」）
- 《离骚》收录节选第 3 段
- 其余篇目按教材要求全文或指定段落收录

收录篇目：短歌行、沁园春·长沙、梦游天姥吟留别、登高、念奴娇·赤壁怀古、劝学、师说、赤壁赋、子路、曾皙、冉有、公西华侍坐、阿房宫赋、六国论、《论语》十二章、大学之道、《老子》四章、屈原列传、过秦论、离骚（节选）、蜀道难、蜀相、陈情表、兰亭集序。

数据在构建时嵌入 DLL，离线可用，不依赖网络。

### 1.3 全新插件图标

插件图标更换为宣纸底色与墨色背景的新设计：`icon.png`（512×512），并附 `icon.svg` 矢量源文件，方便后续调整。

---

## 二、数据校验增强

`validate_data.py` 新增背诵篇目校验，接入构建流程（`dotnet build` 时自动执行，失败会输出 MSBuild Warning）：

- **结构完整性**：`title` / `author` / `content` 均不能为空
- **重复条目检查**：同一篇目下不得出现重复名句
- **与原文一致性**：每条名句必须是 `poetry.json` 对应篇目原文的精确子串

> **校验盲区说明**（透明起见）：`poetry.json` 未收录大学之道、《老子》四章、兰亭集序、沁园春·长沙 四篇，对应 80 条名句无法比对原文；《离骚（节选）》因 `poetry.json` 只收录开头，按设计跳过比对（14 条）。以上条目依赖 `mustResite.json` 编写时的核对。

## 三、构建与发布

- 新增 `package_cipx.py`：一条命令生成 ClassIsland 插件包 `cipx/GaokaoPoetryPlugin.cipx`
- 发布包附带 PDB（便于排查问题）、CHANGELOG 与 LICENSE

## 四、修复

- 修正文档笔误：背诵篇目条数「534」→「533」（README、CHANGELOG）

---

## 五、安装 & 升级

### 新安装
1. 下载 `GaokaoPoetryPlugin.cipx`
2. 拖入 ClassIsland 插件页面（或解压到数据目录 `data/Plugins/gaokao-poetry/`）

### 从 v1.2.0.0 升级
直接覆盖安装即可，原有设置（册别筛选、轮播间隔、字体大小等）会自动保留。

---

## 六、数据来源 & 许可

- 古诗文数据来自 [gaokao-poetry](https://clover-yan.github.io/gaokao-poetry/) by Clover Yan（CC BY-SA 4.0）
- 插件代码基于 MIT License

---

## 七、贡献者致谢

本次更新感谢以下贡献者：

- [@Aorange114514](https://github.com/Aorange114514)：贡献「仅展示背诵篇目」完整功能（[PR #2](https://github.com/xiaoxinxin-Zn/GaokaoPoetryPlugin/pull/2)）——内容筛选选项、`mustResite.json` 背诵篇目数据（21 篇 533 条）、`validate_data.py` 校验增强与 `package_cipx.py` 打包脚本

---

## 完整文件清单

```
GaokaoPoetryPlugin/
├── Assets/
│   ├── divided.json              # 251 条名句（嵌入资源）
│   ├── mustResite.json           # 533 条背诵篇目名句（嵌入资源）★新增
│   └── typo_prone_chars.json     # 23 组易错字（嵌入资源）
├── Components/
│   ├── PoetryComponent.cs        # 主组件（async 加载 + 定时轮播）
│   ├── PoetryComponent.xaml      # UI 布局
│   ├── PoetryComponentSettings.cs # 设置类（含「仅展示背诵篇目」）
│   ├── PoetryComponentSettingsControl.xaml
│   └── PoetryComponentSettingsControl.xaml.cs
├── Models/
│   └── PoetryItem.cs             # 数据模型
├── Services/
│   ├── PoetryService.cs          # 数据服务（嵌入加载 + 网络回退）
│   └── RecitationCatalog.cs      # 背诵篇目 → 册别映射 ★新增
├── Plugin.cs                     # 插件入口（DI 注册）
├── manifest.yml                  # 插件清单 v1.2.1.0
├── GaokaoPoetry.csproj           # 项目文件（BeforeBuild 自动校验）
├── validate_data.py              # 数据校验脚本（含背诵篇目校验）
├── download_data.py              # 数据下载脚本
├── package_cipx.py               # CIPX 打包脚本 ★新增
├── poetry.json                   # 60 篇古诗文全文（开发参考）
├── README.md
├── CHANGELOG.md
├── RELEASE_NOTES.md
├── icon.png                      # 插件图标（512×512）★更新
└── icon.svg                      # 图标矢量源文件 ★新增
```

<!-- CLASSISLAND_PKG_MD5 {"GaokaoPoetryPlugin.cipx": "f78b635a4d972808a040ad84f7cc0d5e"} -->
