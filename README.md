<div align="center">

<img src="https://github.com/PCL-Community/FlowLauncher/raw/refs/heads/main/FlowLauncher/Assets/logo.ico" alt="Logo" width="100">

# Flow Launcher

[下载启动器](https://github.com/PCL-Community/FlowLauncher/releases/latest) |
[PCL](https://github.com/Meloong-Git/PCL) |
[PCL CE](https://github.com/PCL-Community/PCL-CE) |
[社区主页](https://github.com/PCL-Community)

</div>

基于 .NET & [CmlLib](https://github.com/CmlLib) 的跨平台 Minecraft 启动器，使用 Avalonia UI 构建用户界面，提供接近 PCL / PCL CE 的 UI 体验与更加统一易用的设计。

*本项目仍处于初步开发阶段，随时可能有很大的功能和使用逻辑变动，并可能伴随各种意想不到的问题，仅适合尝鲜，请勿日常使用或加入整合包。*

## 使用方式

本项目使用单文件打包方式发行，直接下载 Releases 中对应平台的可执行文件运行即可。

项目有三种构建形式，可通过发行文件名的后缀看出：

1. 框架依赖型可执行文件 [normal] - 需要安装 .NET Runtime 来运行
2. 独立可执行文件 [trimmed] - 直接运行即可
3. AOT 可执行文件 [native] - 同样直接运行即可，由于 AOT 构建的特殊性，可能会有奇奇怪怪的问题，若无特殊性能需求请勿使用

注意：Flow Launcher 支持插件，但由于后两种构建形式使用 .NET 的 TRIM 功能减小发行文件体积，会导致插件引用不存在的库时触发异常。因此，**若要使用插件功能，请勿使用后两种构建方式的发行文件**。

项目发布第一个稳定版时将同步发布适用于 macOS 的 app 包——别急，不会像 HMCL 一样让你手撸 jar 的。

## 注意事项

本项目 UI 控件的设计风格极大程度借鉴了 PCL 及其衍生品，因此，出于尊重原作者及其许可证的原则，本项目暂不提供 Windows 平台的可执行文件，若有需求请自行构建。
