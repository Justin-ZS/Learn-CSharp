
## Reading Resource
1. [Get started with the .NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/get-started/index#Introducing)
1. [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/index)
1. [Reflection in .NET](https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection)
1. [Assembly Class](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly?view=netframework-4.8)

## Notes
初步了解`.Net`开发平台， 熟悉常用的`.Net`类库
思考：
1. 什么是`Assembly`,`AppDomain`,`Type`?
2. 什么是`Metadata`
3. 如何使用 `.Net Exception`, `I/O` 和其他常见的类库

## Homework
### 程序集类型查看器 (AssemblyView)
* 简介  
写一个程序，把一个 `Assembly` 中的所有类型用一个 `TreeView` 展现出来，并且按照 `Namespace` 分层。(基本的结果和 `Reflector` 功能类似)

* 具体要求
  1. 使用代码规范。
  2. `Assembly` 为任意指定的一个.NET Assembly
  3. 使用 `Namespace` 分层
  4. 区分 `Method.Property.Event` 和构造函数（构造函数节点名为`.ctor`）

* 注意事项
  1. 函数参数检查，并合理使用 `Exception`
  3. 不显示所有从父类派生得到的 `Method.Property` 和 `Event`
  4. 如果 `Member` 有 `Attribute0`，请显示出来

* 考查目的：
  1. C#基本语法。
  2. 常用的类库。
  3. `WinForm / WPF`
  4. `Reflection`
  5. 代码规范。
  6. OOP方面: 正确抽象 ,数据和显示分离