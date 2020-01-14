using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AssemblyView
{
    class TypeNode
    {
        public string Name { get; private set; }
        public TypeEnum Type { get; private set; }
        public string TypeDisplay => TypeNode.GetDisplay(Type);
        public string Namespace { get; private set; }
        public List<MemberNode> Members { get; private set; }
        private List<MemberNode> GetMembers(Type type)
        {
            return type
                .GetMembers(GetMemberFlags(Type)) // no-inherit
                .Where(mi => !MemberNode.isCompilerGenerated(mi))
                .Select(mi => new MemberNode(mi))
                .ToList();
        }
        public TypeNode(Type type)
        {
            Name = type.Name;
            Namespace = type.Namespace;
            Type = TypeNode.GetType(type);
            Members = GetMembers(type);
        }
        public static TypeEnum GetType(Type type)
        {
            TypeEnum typeName = TypeEnum.Unkown;
            if (type.IsClass) typeName = TypeEnum.Class;
            if (type.IsSubclassOf(typeof(Delegate))) typeName = TypeEnum.Delegate; // the delegate is syntax sugar
            if (type.IsInterface) typeName = TypeEnum.Interface;
            if (type.IsValueType) typeName = TypeEnum.Struct; // valueType means Struct or Enum
            if (type.IsEnum) typeName = TypeEnum.Enumeration;
            return typeName;
        }
        public static string GetDisplay(TypeEnum type)
        {
            return type switch
            {
                TypeEnum.Class => "Class",
                TypeEnum.Interface => "Interface",
                TypeEnum.Struct => "Struct",
                TypeEnum.Enumeration => "Enumeration",
                TypeEnum.Delegate => "Delegate",
                _ => "Type"
            };
        }
        public static BindingFlags GetMemberFlags(TypeEnum type)
        {
            var flags = BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly; // no-inherit

            // https://stackoverflow.com/questions/10081668/what-is-value-defined-in-enum-in-c-sharp
            if (type == TypeEnum.Enumeration) flags = BindingFlags.Public | BindingFlags.Static;

            return flags;
        }
        public enum TypeEnum
        {
            Class,
            Interface,
            Struct,
            Enumeration,
            Delegate,
            Unkown,
        }
    }
    class MemberNode
    {
        public MemberEnum Type { get; private set; }
        public string Name { get; private set; }
        public String TypeDisplay => MemberNode.GetDisplay(Type);
        public List<String> Attributes { get; private set; }
        public MemberNode(MemberInfo mi)
        {
            Name = mi.Name;
            Type = MemberNode.GetType(mi.MemberType);
            Attributes = GetAttributes(mi);
        }
        private List<String> GetAttributes(MemberInfo info)
        {
            return info.GetCustomAttributes(false).Select(att => att.ToString()).ToList();
        }
        public static MemberEnum GetType(MemberTypes type)
        {
            return type switch
            {
                MemberTypes.NestedType => MemberEnum.NestedType,
                MemberTypes.Event => MemberEnum.Event,
                MemberTypes.Field => MemberEnum.Field,
                MemberTypes.Constructor => MemberEnum.Constructor,
                MemberTypes.Property => MemberEnum.Property,
                MemberTypes.Method => MemberEnum.Method,
                _ => MemberEnum.Unkown // TODO: Add rest members here
            };
            // switch (type)
            // {
            //     case MemberTypes.NestedType:
            //         return MemberEnum.NestedType;
            //     case MemberTypes.Event:
            //         return MemberEnum.Event;
            //     case MemberTypes.Field:
            //         return MemberEnum.Field;
            //     case MemberTypes.Constructor:
            //         return MemberEnum.Constructor;
            //     case MemberTypes.Property:
            //         return MemberEnum.Property;
            //     case MemberTypes.Method:
            //         return MemberEnum.Method;
            //     // TODO: Add rest members here
            //     default:
            //         return MemberEnum.Unkown;
            // }
        }
        public static string GetDisplay(MemberEnum type)
        {
            return type switch
            {
                MemberEnum.NestedType => "Nested Type",
                MemberEnum.Event => "Event",
                MemberEnum.Field => "Field",
                MemberEnum.Constructor => "Constructor",
                MemberEnum.Property => "Property",
                MemberEnum.Method => "Method",
                MemberEnum.Constant => "Constant",
                MemberEnum.Finalizer => "Finalizer",
                MemberEnum.Indexer => "Indexer",
                MemberEnum.Operator => "Operator",
                _ => "Member"
            };
        }
        public static bool isCompilerGenerated(MemberInfo mi)
        {
            return mi.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
        }
        public enum MemberEnum
        {
            Field,
            Constant,
            Property,
            Method,
            Constructor,
            Event,
            Finalizer,
            Indexer,
            Operator,
            NestedType,
            Unkown,
        }
    }

    class AssemblyInfo
    {
        public string FullName { get; private set; }
        public List<IEnumerable<TypeNode>> Types { get; private set; }
        public AssemblyInfo(Assembly assembly)
        {
            FullName = assembly.FullName;
            Types = assembly
                .GetTypes() // The returned array includes nested types.
                .Where(t => !(MemberNode.isCompilerGenerated(t as MemberInfo) || t.IsSpecialName))
                .Select(t => new TypeNode(t))
                .ToList()
                .GroupBy( // groupBy NameSpace
                    t => t.Namespace,
                    t => t,
                    (_, ts) => ts)
                .ToList();
        }
        public void DisplayInfo()
        {
            Display(0, "Assembly: {0}", FullName);
            Types.ForEach(t => DisplayNamespace( + 1, t));
        }
        private void Display (int indent, string format, params object[] args)
        {
            Console.Write(new string(' ', indent * 2));
            Console.WriteLine(format, args);
        }
        private void DisplayType (int indent, TypeNode node)
        {
            Display(indent, "{0}: {1}", node.TypeDisplay, node.Name);
            node.Members.ForEach(m => DisplayMember(indent + 1, m));
        }
        private void DisplayMember (int indent, MemberNode node)
        {
            Display(indent, "{0}: {1}", node.TypeDisplay, node.Name);
            if (node.Attributes.Count() > 0)
            {
                Display(indent + 1, "Attributes:");
                node.Attributes.ForEach(att => Display(indent + 2, "{0}", att));
            }
        }
        private void DisplayNamespace(int indent, IEnumerable<TypeNode> types)
        {
            var ns = types.First().Namespace;
            if (ns == null) return; // ignore local types

            Display(indent, "NameSpace: {0}", ns);
            types.ToList().ForEach(t => DisplayType(indent + 1, t));
        }
    }
}