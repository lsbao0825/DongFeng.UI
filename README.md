# DongFeng.UI

DongFeng.UI 是一个现代化、轻量级且功能丰富的 WPF UI 控件库。

![GitHub](https://img.shields.io/github/license/YourUsername/DongFeng.UI)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)

## ✨ 特性 (Features)

- 🎨 **现代化设计**：提供了一套清新、现代的 UI 风格，即插即用。
- 🧩 **丰富的控件**：包含从基础按钮到高级图表、日期选择等多种常用控件。
- 🌙 **主题支持**：内置亮色 (Light) 和暗色 (Dark) 主题支持，可轻松切换。
- 🛠️ **易于使用**：自动应用基础控件样式，减少重复代码。
- 🔧 **高可定制**：基于标准 WPF 控件开发，易于集成和定制。

## 📦 包含的控件 (Controls)

DongFeng.UI 提供了大量增强型控件：

### 基础与布局
- **DFWindow**: 现代化的无边框窗口，支持自定义标题栏。
- **Layout**: DFUniformGrid, Divider, Spacer
- **Navigation**: BreadcrumbBar, Pagination, StepBar, TabControl (Styled), Drawer (抽屉)

### 数据输入 (Data Entry)
- **Input**: TagInput, NumericUpDown, ColorPicker, PasswordBox (Styled)
- **Selection**: ToggleSwitch, CheckBox, RadioButton, ComboBox, Slider, RangeSlider, Rating
- **Date & Time**: DateTimePicker, DatePicker, Calendar

### 数据展示 (Data Display)
- **Visuals**: Avatar, Badge, Carousel (轮播图), CircleProgressBar
- **Information**: DescriptionList, Statistic, Timeline, TreeView, DataGrid (Styled)
- **State**: Empty (空状态), Skeleton (骨架屏), Loading

### 反馈与提示 (Feedback)
- **Messages**: DFMessageBox, Message (全局消息), Popover, ToolTip

## 🚀 快速开始 (Quick Start)

### 1. 环境要求

- .NET 10.0-windows 或更高版本

### 2. 安装

目前你可以通过引用项目或 DLL 的方式使用。
*(将来发布到 NuGet 后，这里可以使用 `Install-Package DongFeng.UI`)*

### 3. 引入资源

在你的 `App.xaml` 中引入资源字典：

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- 引入 DongFeng.UI 资源 -->
            <ResourceDictionary Source="pack://application:,,,/DongFeng.UI;component/Themes/Generic.xaml"/>
            
            <!-- 可选：明确指定主题 (默认跟随系统或 Generic 定义) -->
            <!-- <ResourceDictionary Source="pack://application:,,,/DongFeng.UI;component/Themes/Theme.Light.xaml"/> -->
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 4. 使用控件

由于 `Generic.xaml` 会自动应用样式到标准控件（如 `Button`, `TextBox` 等），你可以直接使用标准控件获得新样式：

```xml
<Button Content="Click Me" Width="100" Height="30"/>
```

或者使用 DongFeng.UI 的特有控件：

```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:df="clr-namespace:DongFeng.UI.Controls;assembly=DongFeng.UI"
        Title="My App" Height="450" Width="800">
    
    <StackPanel Margin="20" Spacing="10">
        <!-- 使用特有控件 -->
        <df:ToggleSwitch IsChecked="True" />
        <df:Rating Value="4" />
        <df:Avatar ImageSource="/Assets/avatar.png" Name="User" />
    </StackPanel>
</Window>
```

## 🤝 贡献 (Contribution)

欢迎提交 Issue 或 Pull Request！

## 📄 许可证 (License)

MIT License

