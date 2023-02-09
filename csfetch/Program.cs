using System.Diagnostics;

namespace CliStatus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(UserHostName());
            Console.WriteLine(UserHostSpacer());

            var swVers = new SwVers();
            Console.WriteLine(swVers.ToString());
            Console.WriteLine(Uname());

            // Environment.GetEnvironmentVariables
        }

        private static string UserHostName()
        {
            return $"{Environment.UserName}@{Environment.MachineName}";
        }

        private static string UserHostSpacer()
        {
            // TODO: Should be some default or width of UserHostName, whichever
            // is shorter
            return "------------------------------";
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