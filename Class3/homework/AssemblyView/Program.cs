using System;
using System.Reflection;
using System.Linq;

namespace AssemblyView
{
    class Program
    {
        static void Main(string[] args)
        {
            // Remember to build an assembly before run below code.
            const string filePath = @"../../../Class2/homework/MyList/List/bin/Debug/netstandard2.0/List.dll";
            var listAssembly = Assembly.LoadFrom(filePath);
            Console.WriteLine("\n Assembly Full Name:");
            Console.WriteLine(listAssembly.FullName);
            Console.WriteLine("\n Assembly CodeBase:");
            Console.WriteLine(listAssembly.CodeBase);
            Console.WriteLine("\n Assembly Types:");
            Console.WriteLine(String.Join("  ", listAssembly.GetTypes().Select(t => t.Name).ToArray()));
            Console.WriteLine("\n NameSpace: ");
            Console.WriteLine(listAssembly.GetTypes()[0].Namespace);
            // Console.WriteLine("\n ElementType: ");
            // Console.WriteLine(listAssembly.GetTypes()[0].GetElementType());
            Console.WriteLine("\n List GetConstructors: ");
            Console.WriteLine(String.Join("  ", listAssembly.GetTypes()[0].GetConstructors().Select(c => c.ToString())));
            Console.WriteLine("\n GetFields: ");
            Console.WriteLine(String.Join("  ", listAssembly.GetTypes()[0].GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Select(c => c.Name)));
            Console.WriteLine("\n List FullName: ");
            Console.WriteLine(listAssembly.GetTypes()[0].FullName);
        }
    }
}
