# VK Script Compiler Unity Plugin

一个用于编译.vk脚本文件为DLL程序集的Unity插件。

## 功能特性

- 自动监视.vk文件变化
- 实时编译为Unity可用的DLL
- 集成到Unity的Asset导入系统
- 支持自定义脚本语法

## 安装方法

### 通过Git引用（推荐）

在Unity项目的`Packages/manifest.json`文件中添加：

```json
{
  "dependencies": {
    "com.yourcompany.vk-compiler": "git+https://github.com/yourusername/vk-compiler.git"
  }
}
```

### 通过本地路径引用

```json
{
  "dependencies": {
    "com.yourcompany.vk-compiler": "file:../Packages/vk-compiler"
  }
}
```

## 使用方法

1. 在Assets文件夹中创建.vk文件
2. 使用以下语法编写脚本：

```vk
class MyClassName log "Hello from VK Script!"
```

3. 保存文件后，插件会自动编译并生成对应的DLL文件
4. 生成的DLL位于`Assets/Plugins/VkGenerated/`文件夹

## 文件结构

```
Packages/vk-compiler/
├── package.json          # 插件配置
├── README.md             # 说明文档
└── Editor/               # 编辑器脚本
    ├── VkCompiler.cs     # 编译器主类
    ├── VkParser.cs       # 语法解析器
    └── VkImporter.cs     # 资源导入器
```

## 技术说明

- 使用.NET的Reflection.Emit动态生成IL代码
- 集成Unity的ScriptedImporter系统
- 支持实时编译和热重载

## 许可证

MIT License