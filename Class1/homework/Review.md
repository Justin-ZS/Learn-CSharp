1. [Capitalization Conventions](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions)
1. New an Exception instance will consume much resource, so don't throw an exception unless indeed necessary.
1. Prefer [method overloading](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/member-overloading) rather than [optional parameter](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments)
1. Prefer Int.Parse, which has more powerful API, rather than Convert.ToInt32