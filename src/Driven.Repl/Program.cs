using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Driven.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () => await Run());
            Task.WaitAll(task);
        }

        private static async Task Run()
        {
            var app = new App();
            var methods = typeof(App).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            do
            {
                Console.WriteLine("Enter a command:");

                var response = Console.ReadLine();

                if (response == "q" || response == "Q")
                {
                    break;
                }

                var method = methods.SingleOrDefault(x => x.Name == response);

                if (method == null)
                {
                    Console.WriteLine("Unknown method " + response + ". Allowed methods are:");
                    foreach (var m in methods)
                    {
                        Console.WriteLine(m.Name);
                    }
                    continue;
                }

                var task = (Task)method.Invoke(app, new object[0]);

                await Task.WhenAll(task);

            } while (true);

        }
    }
}
