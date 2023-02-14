using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csfetch.Models
{
    internal class Platform
    {
        public string ComputerName { get; private set; } = "";
        public string Kernel { get; private set; } = "";
        public string OSArchitecture { get; private set; } = "";
        public string OSCaption { get; private set; } = "";
        public string UserName { get; private set; } = "";

        public Platform()
        {
            GetUserName();
            GetComputerName();
            GetComputerOS();
        }

        private void GetUserName()
        {
            UserName = Environment.UserName;
        }

        private void GetComputerName()
        {
            ComputerName = Environment.MachineName;
        }

        private void GetComputerOS()
        {
            string osOutput;

            var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = "powershell",
                        Arguments = "gcim Win32_OperatingSystem | select Caption,OSArchitecture,Version | Format-List",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                p.Start();
                osOutput = p.StandardOutput.ReadToEnd() ?? "";
                ParseOsWindows(osOutput);
                p.WaitForExit();
                p.Dispose();
            }
        }

        private void ParseOsWindows(string os)
        {
            string caption = "";
            string kernel = "";
            string osArchitecture = "";

            var parts = os.Split(new char[] { '\n' });
            if (parts.Length > 0)
            {
                foreach(string line in parts)
                {
                    if(line.ToLower().StartsWith("caption"))
                    {
                        var halves = line.Split(':');
                        if (halves.Length == 2)
                        {
                            caption = halves[1].Trim();
                        }
                    }
                    else if (line.ToLower().StartsWith("osarchitecture"))
                    {
                        var halves = line.Split(':');
                        if (halves.Length == 2)
                        {
                            osArchitecture = halves[1].Trim();
                        }
                    }
                    else if (line.ToLower().StartsWith("version"))
                    {
                        var halves = line.Split(':');
                        if (halves.Length == 2)
                        {
                            kernel = halves[1].Trim();
                        }
                    }
                }
            }

            OSCaption = caption;
            Kernel= kernel;
            OSArchitecture = osArchitecture;
        }

        public override string ToString()
        {
            return $"{UserName}@{ComputerName}";
        }

        public string ToKernelString()
        {
            return $"Kernel: {Kernel}";
        }

        public string ToOsString()
        {
            return $"OS: {OSCaption} [{OSArchitecture}]";
        }

        public string TitleDashes()
        {
            return new string('-', ToString().Length);
        }
    }
}
