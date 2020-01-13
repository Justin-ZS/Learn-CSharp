// 写一个程序，把一个 `Assembly` 中的所有类型用一个 `TreeView` 展现出来，并且按照 `Namespace` 分层。(基本的结果和 `Reflector` 功能类似)

// * 具体要求
//   1. 使用代码规范。
//   2. `Assembly` 为任意指定的一个.NET Assembly
//   3. 使用 `Namespace` 分层
//   4. 区分 `Method.Property.Event` 和构造函数（构造函数节点名为`.ctor`）

// * 注意事项
//   1. 函数参数检查，并合理使用 `Exception`
//   3. 不显示所有从父类派生得到的 `Method.Property` 和 `Event`
//   4. 如果 `Member` 有 `Attribute0`，请显示出来

// * 考查目的：
//   1. C#基本语法。
//   2. 常用的类库。
//   3. `WinForm / WPF`
//   4. `Reflection`
//   5. 代码规范。
//   6. OOP方面: 正确抽象 ,数据和显示分离

using System;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

// The key organizational concepts in C# are programs, namespaces, types, members, and assemblies.
// Types: -> DisplayType
//  Classes
//  Interfaces
//  Structs
//  Enumerations
//  Delegates
// Members: belong to classes or instances -> DisplayMember
//  Constants
//  Fields
//  Methods
//  Properties -> DisplayProperty
//  Indexers
//  Events
//  Operators
//  Constructors
//  Finalizers
//  Types
namespace AssemblyView
{
    class Program
    {
        public static string GetVisibility(MethodInfo m)
        {
            string visibility = "";
            if (m.IsPublic) return "Public";
            else if (m.IsPrivate) return "Private";
            else
               if (m.IsFamily) visibility = "Protected ";
            else if (m.IsAssembly) visibility += "Assembly";
            return visibility;
        }
        public static string GetTypeName(Type type)
        {
            var typeName = "Type";
            if (type.IsClass) typeName = "Class";
            if (type.IsInterface) typeName = "Interface";
            if (type.IsEnum) typeName = "Enum";
            return typeName;
        }
        public static BindingFlags GetMemberFlags(Type type)
        {
            var flags = BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly; // no-inherit
            // https://stackoverflow.com/questions/10081668/what-is-value-defined-in-enum-in-c-sharp
            if (type.IsEnum) flags = BindingFlags.Public | BindingFlags.Static;
            return flags;
        }
        public static bool isCompilerGenerated(MemberInfo info)
        {
            return info.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
        }
        public static void DisplayAttributes(int indent, MemberInfo info)
        {
            object[] attrs = info.GetCustomAttributes(false);
            if (attrs.Length == 0) return;

            Display(indent, "Attributes:");
            attrs.ToList().ForEach(att => Display(indent + 1, "{0}", att.ToString()));
        }
        public static void DisplayMethodSignature(int indent, MethodInfo method)
        {
            var argTypes = method.GetParameters().Select(info => info.ParameterType.Name);
            Display(indent, "Signature: ({0}) -> {1}", String.Join(", ", argTypes), method.ReturnType.Name);
        }
        public static void DisplayMethod(int indent, MethodInfo method)
        {
            if (method.IsSpecialName) return;
            Display(indent, "Method: {0}", method.Name);
            DisplayMethodSignature(indent + 1, method);
        }
        public static void DisplayProperty(int indent, PropertyInfo property)
        {
            Display(indent, "Property: {0}", property.Name);

            var accInfos = property.GetAccessors().ToList();
            accInfos.ForEach((info) => {
                var type = info.ReturnType == typeof(void) ? "Setter" : "Getter";
                Display(indent + 1, "{0} {1}", GetVisibility(info), type);
            });
        }
        public static void DisplayConstructor(int indent, ConstructorInfo ctor)
        {
            Display(indent, "Constructor: {0}", ctor.ToString());
        }
        public static void DisplayField(int indent, FieldInfo field)
        {
            Display(indent, "Field: {0}", field.Name);
        }
        public static void DisplayEvent(int indent, EventInfo ev)
        {
            Display(indent, "Event: {0}", ev.Name);
        }
        public static void DisplayMember(int indent, MemberInfo info)
        {
            if (isCompilerGenerated(info)) return;

            switch (info.MemberType)
            {
                case MemberTypes.NestedType:
                    DisplayType(indent, info as Type);
                    break;
                case MemberTypes.Event:
                    DisplayEvent(indent, info as EventInfo);
                    break;
                case MemberTypes.Field:
                    DisplayField(indent, info as FieldInfo);
                    break;
                case MemberTypes.Constructor:
                    DisplayConstructor(indent, info as ConstructorInfo);
                    break;
                case MemberTypes.Property:
                    DisplayProperty(indent, info as PropertyInfo);
                    break;
                case MemberTypes.Method:
                    DisplayMethod(indent, info as MethodInfo);
                    break;
                default:
                    Display(indent, "Member: {0}", info.Name);
                    break;
            }
            DisplayAttributes(indent + 1, info);
        }
        public static void DisplayType(int indent, Type type)
        {
            if (isCompilerGenerated(type as MemberInfo) || type.IsSpecialName) return;

            Console.WriteLine();
            Display(indent, "{0}: {1}", GetTypeName(type), type.Name);

            var memberInfos = type.GetMembers(GetMemberFlags(type));
            memberInfos.ToList().ForEach(mInfo => DisplayMember(indent + 1, mInfo));
        }
        public static void Display(int indent, string format, params object[] args)

        {
            Console.Write(new string(' ', indent * 2));
            Console.WriteLine(format, args);
        }
        static void Main(string[] args)
        {
            // Remember to build an assembly before run below code.
            const string filePath = @"../../../Class2/homework/MyList/List/bin/Debug/netstandard2.0/List.dll";
            var listAssembly = Assembly.LoadFrom(filePath);
            Display(0, "Assembly: {0}", listAssembly.FullName);
            listAssembly.GetTypes().ToList().ForEach(t => DisplayType(1, t));
        }
    }
}
