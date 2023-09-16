//Install - Package Microsoft.CodeAnalysis.CSharp - Version 4.0.0
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

namespace ConsoleCompiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string sourceCode =
            @"using System;

            public class TestCodeAnalysisCSharp
            {
                public static void Main(string[] args)
                {
                    Console.WriteLine(""log start CSharp "");
                    var str = Console.ReadLine();
                    Console.WriteLine(""log end CSharp "" + str);
                    Console.ReadLine();
                }
            }";

            string sourceCode2 =
           @" 
                string csharpVersion = System.Environment.Version.ToString();
                System.Console.WriteLine(""csharpVersion: "" + csharpVersion);
            ";

            Console.WriteLine("log start Program.Main");
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            string assemblyName = Path.GetRandomFileName();
            var references =
                AppDomain.CurrentDomain.GetAssemblies().
                Where(a => !a.IsDynamic).
                Select(a => MetadataReference.CreateFromFile(a.Location));

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation failed!");
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    MethodInfo entryPoint = assembly.EntryPoint;
                    if (entryPoint != null)
                    {
                        object[] methodArgs = new object[] { args };
                        entryPoint.Invoke(null, methodArgs);
                    }
                    else
                    {
                        Console.WriteLine("Entry point not found.");
                    }
                }
            }
            Console.WriteLine("log end Program.Main");
            Console.ReadLine();
        }
    }
}
