#!/usr/bin/env python3
"""把 Release 构建输出打包为 ClassIsland 插件包（.cipx）。

用法：
    dotnet build -c Release
    python package_cipx.py

.cipx 本质是一个 zip 包：manifest.yml 与入口 DLL 必须位于压缩包根目录。
"""

import re
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).parent


def manifest_field(text: str, name: str) -> str:
    match = re.search(rf"^{name}:\s*(.+?)\s*$", text, re.MULTILINE)
    if not match:
        raise SystemExit(f"manifest.yml 缺少字段: {name}")
    return match.group(1).strip().strip('"').strip("'")


def main() -> int:
    config = sys.argv[1] if len(sys.argv) > 1 else "Release"
    manifest_text = (ROOT / "manifest.yml").read_text(encoding="utf-8")
    entrance = manifest_field(manifest_text, "entranceAssembly")
    icon = manifest_field(manifest_text, "icon")
    readme = manifest_field(manifest_text, "readme")
    stem = Path(entrance).stem

    config_dir = ROOT / "bin" / config
    candidates = sorted(config_dir.glob("*/" + entrance)) if config_dir.exists() else []
    if not candidates:
        print(f"未找到 {config_dir}/*/{entrance}，请先执行: dotnet build -c {config}")
        return 1
    build_dir = candidates[-1].parent

    required = {
        "manifest.yml": ROOT / "manifest.yml",
        entrance: build_dir / entrance,
        f"{stem}.deps.json": build_dir / f"{stem}.deps.json",
        icon: build_dir / icon,
        readme: build_dir / readme,
    }
    optional = {
        f"{stem}.pdb": build_dir / f"{stem}.pdb",
        "CHANGELOG.md": build_dir / "CHANGELOG.md",
        "LICENSE": ROOT / "LICENSE",
    }

    missing = [str(path) for path in required.values() if not path.exists()]
    if missing:
        print("缺少必需文件：")
        for path in missing:
            print(f"  {path}")
        return 1

    out_dir = ROOT / "cipx"
    out_dir.mkdir(exist_ok=True)
    target = out_dir / f"{stem}.cipx"
    with zipfile.ZipFile(target, "w", zipfile.ZIP_DEFLATED) as archive:
        for name, path in required.items():
            archive.write(path, name)
        for name, path in optional.items():
            if path.exists():
                archive.write(path, name)

    print(f"已生成: {target}  ({target.stat().st_size / 1024:.1f} KiB)")
    with zipfile.ZipFile(target) as archive:
        for name in archive.namelist():
            print(f"  {name}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
