# 高考古诗文插件

随机展示高考必背60篇古诗文名句的 ClassIsland 插件，支持按教材册别分类筛选。

## 数据来源

- [gaokao-poetry](https://clover-yan.github.io/gaokao-poetry/) by Clover Yan（CC BY-SA 4.0）

## 功能特性

- 定时随机轮播高考必背古诗文名句
- 支持按教材册别分类筛选：必修上、必修下、选择性必修上、选择性必修中、选择性必修下、其他
- 支持「仅展示背诵篇目」：只轮播高考背诵篇目（21 篇，共 534 条名句，含《师说》第一段、《屈原列传》第三段、《离骚》第 3 段等指定段落）
- 支持显示/隐藏作者和篇名
- 支持调整轮播间隔和字体大小
- 支持离线模式（嵌入数据）

## 快速开始

### 编译

```bash
dotnet build -c Release
python package_cipx.py
```

生成 `cipx/GaokaoPoetryPlugin.cipx`（Windows 上 `python` 不可用时改用 `py package_cipx.py`）。

### 安装

把 `GaokaoPoetryPlugin.cipx` 拖入 ClassIsland 的插件页面，或解压到
`ClassIsland 数据目录/data/Plugins/gaokao-poetry/`。

## 使用说明

1. 在 ClassIsland 主界面右键 → 组件 → 找到「高考古诗文」
2. 拖动到主界面上
3. 右键组件 → 设置，可调整轮播间隔、字体大小、册别筛选等

## 分册说明

| 册别 | 篇数 | 代表篇目 |
|------|------|----------|
| 必修上册 | 16篇 | 短歌行、登高、赤壁赋、静女等 |
| 必修下册 | 8篇 | 阿房宫赋、六国论、谏太宗十思疏等 |
| 选择性必修上册 | 5篇 | 论语十二章、春江花月夜、将进酒等 |
| 选择性必修中册 | 7篇 | 屈原列传、过秦论、燕歌行等 |
| 选择性必修下册 | 14篇 | 离骚、蜀道难、陈情表、项脊轩志等 |
| 其他 | 10篇 | 报任安书、山居秋暝、青玉案·元夕等 |

## 许可证

本项目代码基于 MIT License 发布。古诗文数据遵循 CC BY-SA 4.0。

## 鸣谢与版权声明

### AI辅助生成说明
本项目在开发过程中使用了AI编程助手辅助代码生成，具体如下：

| AI工具 | 用途 | 生成代码许可 |
| :--- | :--- | :--- |
| **DeepSeek-V4** | 代码生成、逻辑优化 | MIT License |
| **Claude Code** | 代码生成、项目结构调整 | Anthropic 服务条款 |

### 声明
- 本项目生成的代码均经过人工审查，开发者已理解并验证其逻辑。
- 如果你使用了本项目代码，**必须在你的项目中保留本许可证及版权声明**。
- DeepSeek-V4 生成的部分适用 MIT License，完整许可文本见项目根目录的 `LICENSE` 文件。

### 致谢
感谢 [DeepSeek](https://deepseek.com) 和 [Anthropic](https://anthropic.com) 提供的优秀AI模型与开发工具。

---

© [2026] [Xin_Zn]. 本项目基于 [MIT License] 发布。
