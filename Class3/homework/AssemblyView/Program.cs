using System;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

// The key organizational concepts in C# are programs, namespaces, types, members, and assemblies.
// Types:
//  Classes -> DisplayType
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
        static string GetVisibility(MethodInfo m)
        {
            string visibility = "";
            if (m.IsPublic) return "Public";
            else if (m.IsPrivate) return "Private";
            else
               if (m.IsFamily) visibility = "Protected ";
            else if (m.IsAssembly) visibility += "Assembly";
            return visibility;
        }
        public static void DisplayAttributes(Int32 indent, MemberInfo mi)
        {
            // Get the set of custom attributes; if none exist, just return.
            object[] attrs = mi.GetCustomAttributes(false);
            if (attrs.Length == 0) { return; }

            // Display the custom attributes applied to this member.
            Display(indent + 1, "Attributes:");
            foreach (object o in attrs)
            {
                Display(indent + 2, "{0}", o.ToString());
            }
        }
        
        public static void DisplayMethodSignature(int indent, MethodInfo method)
        {
            var argTypes = method.GetParameters().Select(info => info.ParameterType.Name);
            Display(indent, "Signature: ({0}) -> {1}", String.Join(", ", argTypes), method.ReturnType.Name);
        }
        public static void DisplayMethod(int indent, MethodInfo method)
        {
            if (method.IsSpecialName)
            {
                return;
            }
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
        public static void DisplayMember(int indent, MemberInfo obj)
        {
            var isGenerated = obj.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            if (isGenerated)
            {
                return;
            }

            switch (obj.MemberType)
            {
                case MemberTypes.NestedType:
                    DisplayType(indent, obj as Type);
                    break;
                case MemberTypes.Event:
                    DisplayEvent(indent, obj as EventInfo);
                    break;
                case MemberTypes.Field:
                    DisplayField(indent, obj as FieldInfo);
                    break;
                case MemberTypes.Constructor:
                    DisplayConstructor(indent, obj as ConstructorInfo);
                    break;
                case MemberTypes.Property:
                    DisplayProperty(indent, obj as PropertyInfo);
                    break;
                case MemberTypes.Method:
                    DisplayMethod(indent, obj as MethodInfo);
                    break;
                default:
                    Display(indent, "Member: {0}", obj.Name);
                    break;
            }
        }
        public static void DisplayType(int indent, Type obj)
        {
            Display(indent, "\nType: {0}", obj.Name);

            var allTypes =  BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly; // no-inherit
            var memberInfos = obj.GetMembers(allTypes);
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
            // Console.WriteLine("\n Assembly Full Name:");
            // Console.WriteLine(listAssembly.FullName);
            // Console.WriteLine("\n Assembly CodeBase:");
            // Console.WriteLine(listAssembly.CodeBase);
            // Console.WriteLine("\n Assembly Types:");
            // Console.Write($"{listAssembly.GetTypes().Count()}: ");
            // Console.WriteLine(String.Join("  ", listAssembly.GetTypes().Select(t => t.Name).ToArray()));
            // Console.WriteLine("\n NameSpace: ");
            // Console.WriteLine(listAssembly.GetTypes()[0].Namespace);
            // Console.WriteLine("\n ElementType: ");
            // Console.WriteLine(listAssembly.GetTypes()[0].GetElementType());
            // Console.WriteLine("\n List GetMembers: ");
            // Console.WriteLine(String.Join("  ", listAssembly.GetTypes()[0].GetMembers().Select(c => c.Name)));
            // Console.WriteLine("\n List GetConstructors: ");
            // Console.WriteLine(String.Join("  ", listAssembly.GetTypes()[0].GetConstructors().Select(c => c.ToString())));
            // Console.WriteLine("\n GetFields: ");
            // Console.WriteLine(String.Join("  ", listAssembly.GetTypes()[0].GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Select(c => c.Name)));
            // Console.WriteLine("\n List FullName: ");
            // Console.WriteLine(listAssembly.GetTypes()[0].FullName);

            Console.WriteLine("\nClass: ");
            DisplayType(0, listAssembly.GetTypes()[0]);
        }
    }
}
