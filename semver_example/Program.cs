using System;
using System.Reflection;

namespace netcore_versioning
{


    class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(Program).Assembly;
            foreach (var attribute in assembly.GetCustomAttributesData())
            {
                if (!attribute.TryParse(out string value)) value = string.Empty;
                Console.WriteLine($"{attribute.AttributeType.Name} - {value}");
            }
        }

    }
}
