using System.Runtime.InteropServices;
using System.Text;

namespace LR_1.Clinic.Core;

public static class ConsoleEncoding
{
    private const uint Utf8CodePage = 65001;
    private const uint RussianOemCodePage = 866;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleOutputCP(uint codePage);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCP(uint codePage);

    public static void Configure()
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            return;
        }

        if (TryUseUtf8())
            return;

        TryUseRussianOem();
    }

    private static bool TryUseUtf8()
    {
        try
        {
            SetConsoleOutputCP(Utf8CodePage);
            SetConsoleCP(Utf8CodePage);
            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            Console.InputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void TryUseRussianOem()
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            SetConsoleOutputCP(RussianOemCodePage);
            SetConsoleCP(RussianOemCodePage);
            var encoding = Encoding.GetEncoding((int)RussianOemCodePage);
            Console.OutputEncoding = encoding;
            Console.InputEncoding = encoding;
        }
        catch
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }
    }
}
