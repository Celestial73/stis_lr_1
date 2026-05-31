using LR_1.Clinic.Core;
using LR_1.Clinic.Demo;

ConsoleEncoding.Configure();

DemoRunner.Run();

if (!Console.IsInputRedirected)
{
    Console.WriteLine("\nНажмите любую клавишу для выхода...");
    Console.ReadKey();
}
