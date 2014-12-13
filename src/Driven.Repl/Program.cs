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
            var methods =
                typeof (App).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .ToList().Select((m, i) => new {i, m})
                            .ToDictionary(x => x.i, x => x.m);

            do
            {
                Console.WriteLine("Enter a command:");

                var response = Console.ReadLine();

                if (response == "q" || response == "Q")
                {
                    break;
                }

                var method = default(MethodInfo);

                int index;
                if (int.TryParse(response, out index))
                {
                    if (methods.ContainsKey(index))
                    {
                        method = methods[index];
                    }
                }
                else
                {
                    method = methods.Values.SingleOrDefault(x => x.Name == response);
                }

                if (method == null)
                {
                    Console.WriteLine("Unknown method " + response + ". Allowed methods are:");
                    foreach (var m in methods)
                    {
                        Console.WriteLine("{0} - {1}", m.Key, m.Value.Name);
                    }
                    Console.WriteLine();
                    Console.WriteLine("'Q' or 'q' to quit");
                    Console.WriteLine();
                    continue;
                }

                var timer = new Timer();

                Console.WriteLine("Running " + method.Name);

                var task = (Task)method.Invoke(app, new object[0]);

                await Task.WhenAll(task);

                Console.WriteLine("Completed " + method.Name + ": " + timer.Interval);

            } while (true);

        }
    }
}
