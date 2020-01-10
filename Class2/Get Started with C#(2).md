## Reading Resource
1. [C# reference](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/)
1. [.Net Core Source Browser](https://source.dot.net/)
1. [yield (C# Reference)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield)

## Notes
1. 掌握常用的类型和成员
  Object, Int32, Double, String, DateTime, TimeSpan, Enum, Math, Convert, Array, List<T>, Dictionary<TKey, TValue>, Nullable, Type
1. 如何为结构体重写`Equals`和`GetHashCode`
1. 如何定义`Interface`，如何实现`Interface`
1. 如何实现自定义的`Event`

## Homeworks
### 实现链表（LinkedList）

* 简介
写一个链表的数据结构，要求实现IList<T>接口。

* 具体要求
  1. 使用代码规范。
  2. 至少对 `IList` 中的 `Add`，`Remove`，`Insert`，`Indexer`，`IEnumerator`进行单元测试。
  3. 对上述每个单元测试方法至少书写4种不同的单元测试。
  4. 要求从`Object`派生，实现`System.Collections.Generic.IList<T>`。
  5. 内部存储不能使用.NET内置链表。

* 注意事项
  1. 单元测试采用Visual Studio Team Edition内置UnitTest
  2. 对方法的参数要进行检查，并抛出合理的`Exception`。

* 考查目的
  1. C#语法。
  2. 常用的类库。
  3. 代码规范。
  4. 单元测试。