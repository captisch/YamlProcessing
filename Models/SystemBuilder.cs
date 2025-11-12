using System;
using System.Diagnostics;

namespace YamlProcessing.Models;

public class SystemBuilder
{
    public int call()
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "wsl.exe",
            Arguments = "cd ~/liteX\n" +
                        "source venv/bin/activate\n" +
                        "python3 SystemBuilder/LiteX-related/Python/litex_generator.py\n" +
                        "\nread -p \"Press enter to continue...\" x",
            UseShellExecute = true,
            CreateNoWindow = false
        };

        try
        {
            using var process = Process.Start(psi);
            if (process is null) return -1;
            process.WaitForExit();
            return process.ExitCode;
        }
        catch ( Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        return 0;
    }
}