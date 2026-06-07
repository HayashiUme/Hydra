# Hydra
<div align="center">
  <img src="https://github.com/MrDiamond64/Hydra/blob/main/img/main.png?raw=true" alt="Hydra 玩家界面截图"/>
</div>

Hydra 是一款基于 [BepInEx](https://github.com/BepInEx/BepInEx) 的 Among Us 模组，旨在提升 Among Us 的游戏体验。Hydra 提供了便捷功能、趣味恶搞功能，以及用于检测房间内作弊玩家的反作弊系统。

我们有一个 Discord 服务器，欢迎加入交流、寻求帮助或提出建议：https://discord.gg/N7azGPHm5F

# 功能特性
> [!NOTE]
> 本仓库是经过大幅精简的版本，移除了我认为过于强大、可能被滥用的功能，例如[无需房主身份即可从房间封禁玩家](https://streamable.com/wtb7jl)
>
> 无需房主即可封禁玩家是处理那些游戏一开始就乱开会的烦人玩家的好办法，也可以在我所在的房间封禁外挂。但这也会带来滥用的风险——比如有人可以封禁所有内鬼来瞬间获胜，或者封禁怀疑你是内鬼的玩家。

- 游戏内通知系统
- 显示幽灵的聊天消息（方便管理员判断玩家是否公平游戏）
- 聊天按钮始终可见
- 作为房主可使用翻转版 Skeld 地图
- 房主强制内鬼身份
- 玩家颜色随机化
- 传送功能
- 船员身份也可破坏和关门
- 设备和版本伪装
- 查看其他玩家的角色
- [不死之身](https://streamable.com/k1b0m0)
- [可配置的反作弊系统，检测常见外挂和漏洞](https://github.com/MrDiamond64/Hydra?tab=readme-ov-file#hydra-anticheat)
- 还有更多！

# Hydra 反作弊
Hydra 反作弊可以说是本模组的核心。它能够检测玩家何时试图作弊，例如没有相应角色却钻入管道，或试图在地图上瞬移。检测到作弊者后，Hydra 可以自动将其从房间封禁。你不需要是房主也能使用 Hydra 反作弊——非房主模式下只会向你发送通知而不会封禁玩家。Hydra 反作弊旨在扩展原版 Among Us 反作弊，增加其目前未检测到的作弊检查，不过它也可以用在反作弊较宽松的自定义服务器上，只要遵循基本的底线。

Hydra 反作弊有一个基本前提：后端服务器必须能够防止玩家身份伪造。如果作弊者能够以其他玩家的名义发送 RPC，那么 Hydra 反作弊将无法准确判断谁是真正的作弊者，可能会错误标记无辜玩家。原版 Among Us 服务器已经具备身份伪造检测，所以在这类服务器上通常不用担心此问题。

# 安装与使用
> [!WARNING]
> 在使用 Hydra 之前，请务必充分理解并同意[免责声明](#免责声明)部分中的警告。

## 安装 BepInEx
[BepInEx](https://github.com/bepinex/bepinex) 是一个允许为 Unity 游戏创建模组的模组框架。Among Us 使用 Unity 作为游戏引擎，因此 Hydra 使用 BepInEx 框架来修改游戏。在下载 Hydra 之前，你需要先安装 BepInEx。BepInEx 有多种变体，我们需要的是 il2cpp 版本，因为 Among Us 使用 il2cpp 编译器来实现跨平台兼容。你可以从 [Releases](https://github.com/MrDiamond64/Hydra/releases) 页面下载 BepInEx，也可以从 [BepInEx 官网](https://builds.bepinex.dev/projects/bepinex_be) 获取 BepInEx 二进制文件。请注意 BepInEx Il2Cpp 有两种架构：x86 和 x64，这点很重要，因为 Among Us 可能是 x86 或 x64 取决于你的下载来源。BepInEx 的架构必须与你的 Among Us 安装版本匹配。一般来说，Microsoft Store 和 Epic Games 提供的是 x64 版本，Steam 和 Itch.io 提供的是 x86 版本。如果还不确定，可以按 `ctrl` + `shift` + `esc` 打开任务管理器，在运行进程列表中找到 Among Us，看是否显示"Among Us.exe (32位)"，如果没有则说明你的 Among Us 是 x64 版本。

下载了适合你 Among Us 版本的 BepInEx 后，打开 Among Us 的安装目录（即 `Among Us.exe` 和 `GameAssembly.dll` 所在的位置），将 BepInEx 的内容解压到该目录中。在 `Among Us.exe` 旁边应该会出现新的文件和文件夹，如 `winhttp.dll`、`BepInEx` 和 `dotnet`。如果没有看到这些文件，可能是你的解压程序把它们解压到了单独的子文件夹中——打开那个新文件夹，把所有内容拖放到 `Among Us.exe` 所在的目录即可。如果一切正确，BepInEx 就安装好了，可以开始下载 Hydra。

## 安装 Hydra
下载 Hydra 非常简单：前往 [Releases](https://github.com/MrDiamond64/Hydra/releases) 页面，下载 `HydraMenu.dll` 文件，然后将其复制到 `./BepInEx/plugins/` 目录中。完成后就可以启动 Among Us 了。首次安装 BepInEx 后启动 Among Us 会与平常不同——启动可能需要更长时间，并且会看到一个终端窗口。这完全正常，启动延迟是 BepInEx 预检流程的一部分。之后再启动 Among Us 就不会有这个延迟了。稍等片刻后，Among Us 应该会打开，屏幕右上角会出现一个模组图标，此时 Hydra 已经准备就绪，可以开始享受了！

## 使用 Hydra
按键盘上的 `Insert` 键即可打开 Hydra 界面。根据你的键盘类型，可能需要开启 Num Lock 或同时按功能键才能弹出菜单。按下 Insert 后，你应该能看到 Hydra 界面。Hydra 界面由多个部分组成：左侧的导航标签栏和右侧的功能面板。导航栏包含 `欢迎`、`自身`、`主机`、`反作弊` 等标签页。点击任意标签页，功能面板中就会显示该部分的对应功能。功能面板中有滑块、按钮和复选框，可以用来配置 Hydra。

# 待办事项
- [ ] 增加更多反作弊检查（如船员破坏检测）
- [x] UI 区域添加滚动条
- [x] 玩家界面显示角色和颜色
- [ ] 探索似乎反作弊更宽松的 Modded 原版协议
- [ ] 可保存的配置文件

# 免责声明
> [!CAUTION]
> **Hydra 在任何情况下都不应用于破坏其他玩家的游戏体验。如果你使用某些恶搞功能，请确保你在私人大厅中，并且其他玩家知情同意。你可以带着 Hydra 进入公共房间，但前提是你使用它是为了改善你的 Among Us 游戏体验。能力越大，责任越大！**

我认识到像 Hydra 这样的工具模组可能会被恶意用户用来破坏房间。我已经尝试通过从公开版本中移除功能强大且容易被滥用的功能、添加防护措施来限制滥用的可能性。即使有这些保护措施，滥用和恶意行为仍然可能发生。我只能请求你——使用 Hydra 的人——请不要将 Hydra 用于恶意目的，遵守 [Innersloth 行为准则](https://www.innersloth.com/code-of-conduct/) 以及你所在房间的规则。仅在公共房间用于检测作弊者，或在其他玩家同意的情况下使用 Hydra 的高级功能。

如果你不遵守我的建议，请不要期望从我这里获得任何形式的支持或责任承担。你的账号可能会被 Innersloth 处罚，你将失去你的 Among Us 账号及其关联的所有数据，包括好友列表、已解锁的装扮、购买记录、豆豆和金币等。

本模组与 Among Us 或 Innersloth LLC 无关，其中包含的内容未经 Innersloth LLC 认可或赞助。本文所含部分材料为 Innersloth LLC 的财产。© Innersloth LLC。

---
汉化者：HayashiUme
