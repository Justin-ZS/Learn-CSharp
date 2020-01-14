// 写一个程序，把一个 `Assembly` 中的所有类型用一个 `TreeView` 展现出来，并且按照 `Namespace` 分层。(基本的结果和 `Reflector` 功能类似)

// * 具体要求
//   1. 使用代码规范。
//   2. Assembly 为任意指定的一个.NET Assembly
//   3. 使用 Namespace 分层
//   4. 区分 Method, Property, Event 和构造函数（构造函数节点名为.ctor）

// * 注意事项
//   1. 函数参数检查，并合理使用 Exception
//   3. 不显示所有从父类派生得到的 Method, Property 和 Event
//   4. 如果 Member 有 Attribute，请显示出来

// * 考查目的：
//   1. C#基本语法。
//   2. 常用的类库。
//   3. WinForm / WPF
//   4. Reflection
//   5. 代码规范。
//   6. OOP方面: 正确抽象 ,数据和显示分离

using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// The key organizational concepts in C# are programs, namespaces, types, members, and assemblies.
// 5 Types: -> DisplayType
//  Classes
//  Interfaces
//  Structs
//  Enumerations: Fields member only
//  Delegates
// 10 Members: -> DisplayMember
//  Fields -> DisplayField
//  Constants
//  Properties -> DisplayProperty
//  Methods -> DisplayMethod
//  Constructors -> DisplayConstructor
//  Events -> DisplayEvent
//  Finalizers
//  Indexers
//  Operators
//  Nested Types -> Recursive
namespace AssemblyView
{
    class Program
    {
        public static string GetModifier(MethodInfo info)
        {
            StringBuilder modifier = new StringBuilder();
            if (info.IsStatic) modifier.Append("Static ");
            if (info.IsPublic) modifier.Append("Public ");
            if (info.IsPrivate) modifier.Append("Private ");
            if (info.IsFamily) modifier.Append("Protected ");
            if (info.IsAssembly) modifier.Append("Internal ");
            return modifier.ToString().TrimEnd();
        }
        public static string GetTypeName(Type type)
        {
            var typeName = "Type";
            if (type.IsClass) typeName = "Class";
            if (type.IsSubclassOf(typeof(Delegate))) typeName = "Delegate"; // the delegate is syntax sugar
            if (type.IsInterface) typeName = "Interface";
            if (type.IsValueType) typeName = "Struct"; // valueType means Struct or Enum
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
        public static void Display(int indent, string format, params object[] args)
        {
            Console.Write(new string(' ', indent * 2));
            Console.WriteLine(format, args);
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
            // these cannot be generally called by the user
            if (method.IsSpecialName) return;
            // Display(indent, "Method: {0} :: ({1}) -> {2}", method.Name, String.Join(", ", method.GetParameters().Select(info => info.ParameterType.Name)), method.ReturnType.Name);
            Display(indent, "Method: {0}", method.Name);
            // Display(indent, "{0} Method: {1}", GetModifier(method), method.Name);
            DisplayMethodSignature(indent + 1, method);
        }
        public static void DisplayProperty(int indent, PropertyInfo property)
        {
            Display(indent, "Property: {0}", property.Name);

            var accInfos = property.GetAccessors().ToList();
            accInfos.ForEach((info) =>
            {
                var type = info.ReturnType == typeof(void) ? "Setter" : "Getter";
                Display(indent + 1, "{0} {1}", GetModifier(info), type);
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
                    // has been included in Assembly.GetTypes
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
            memberInfos.ToList().ForEach(info => DisplayMember(indent + 1, info));
        }
        public static void DisplayNamespace(int indent, IEnumerable<Type> types)
        {
            var ns = types.First().Namespace;
            if (ns == null) return; // ignore local types

            Display(indent, "NameSpace: {0}", ns);
            types.ToList().ForEach(t => DisplayType(indent + 1, t));
        }
        public static Assembly LoadAssemblyFrom(string filePath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(filePath);
                return assembly;
            }
            catch
            {
                return null;
            }
        }
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please specify a valid assembly path!");
                return 1;
            }
            
            // Remember to build an assembly before run below code.
            // sample path: "../../../Class2/homework/MyList/List/bin/Debug/netstandard2.0/List.dll";
            var assembly = LoadAssemblyFrom(args[0]);
            if (assembly == null)
            {
                Console.WriteLine("Can't load specified assembly!");
                return 1;
            }

            Display(0, "Assembly: {0}", assembly.FullName);
            assembly
                .GetTypes() // The returned array includes nested types.
                .ToList()
                .GroupBy( // groupBy NameSpace
                    t => t.Namespace,
                    t => t,
                    (_, ts) => ts)
                .ToList()
                .ForEach(ts => DisplayNamespace(1, ts));

            return 0;
        }
    }
}
