using csfetch.Models;
using System.Diagnostics;

namespace CliStatus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ascii = new AsciiArt();
            var platform = new Platform();

            Console.Write(ascii.WindowsArt[0]);
            Console.Write("  ");
            Console.WriteLine(platform.ToString());
            Console.Write(ascii.WindowsArt[1]);
            Console.Write("  ");
            Console.WriteLine(platform.TitleDashes());

            Console.Write(ascii.WindowsArt[2]);
            Console.Write("  ");
            Console.WriteLine(platform.ToOsString());

            Console.Write(ascii.WindowsArt[3]);
            Console.Write("  ");
            Console.WriteLine(platform.ToHostString());

            Console.Write(ascii.WindowsArt[4]);
            Console.Write("  ");
            Console.WriteLine(platform.ToKernelString());

            Console.Write(ascii.WindowsArt[5]);
            Console.Write("  ");
            Console.WriteLine(platform.ToMotherboardString());

            Console.Write(ascii.WindowsArt[5]);
            Console.Write("  ");
            Console.WriteLine(platform.ToUptimeString());

            //var swVers = new SwVers();
        }

        private static string Uname()
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "uname",
                    Arguments = "-mrs",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            p.Start();
            string? inputText = p.StandardOutput.ReadToEnd() ?? "";
            p.WaitForExit();
            p.Dispose();

            return inputText;
        }
    }
}