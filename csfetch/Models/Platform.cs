using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csfetch.Models
{
    internal class Platform
    {
        public string ComputerName { get; private set; } = "";
        public string Kernel { get; private set; } = "";
        public string Motherboard { get; private set; } = "";
        public string OSArchitecture { get; private set; } = "";
        public string OSCaption { get; private set; } = "";
        public string OsPlatform { get; private set; } = "";
        public string Uptime { get; private set; } = "";
        public string UserName { get; private set; } = "";

        public Platform()
        {
            SetPlatform();

            GetUserName();
            GetComputerName();
            GetComputerOS();
            GetMotherboard();
            GetUptime();
        }

        private void SetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OsPlatform = "windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OsPlatform = "macos";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OsPlatform = "linux";
            }
        }

        private void GetUserName()
        {
            UserName = Environment.UserName;
        }

        private void GetMotherboard()
        {
            string output;

            if (OsPlatform == "windows")
            {
                output = RunProcess("powershell", "wmic baseboard get product,manufacturer");
                ParseMotherboard(output);
            }
        }

        private string RunProcess(string filename, string arguments)
        {
            string output = "";

            if (OsPlatform == "windows")
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = filename,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                p.Start();
                output = p.StandardOutput.ReadToEnd() ?? "";
                p.WaitForExit();
                p.Dispose();
            }

            return output;
        }

        private void GetComputerName()
        {
            ComputerName = Environment.MachineName;
        }

        private void GetComputerOS()
        {
            string osOutput;

            if (OsPlatform == "windows")
            {
                osOutput = RunProcess("powershell", "gcim Win32_OperatingSystem | select Caption,OSArchitecture,Version | Format-List");
                ParseOsWindows(osOutput);
            }
        }

        private void GetUptime()
        {
            string output;

            if (OsPlatform == "windows")
            {
                output = RunProcess("powershell", "wmic path Win32_OperatingSystem get LastBootUpTime");
                ParseWindowsUptime(output);
            }
        }

        private void ParseMotherboard(string data)
        {
            var parts = data.Split(new char[] { '\n' });
            if (parts.Length > 2)
            {
                Motherboard = parts[1];
            }
        }

        private void ParseWindowsUptime(string data)
        {
            var parts = data.Split(new char[] { '\n' });
            if (parts.Length > 2)
            {
                var lastBoot = parts[1].Trim();

                // 20230214212526.500000-300
                DateTime lb = ManagementDateTimeConverter.ToDateTime(lastBoot);
                var span = DateTime.Now.Subtract(lb);
                Uptime = ParseTimeSpan(span);
             }
            else
            {
                Uptime = "";
            }
        }

        private string ParseTimeSpan(TimeSpan span)
        {
            var sb = new StringBuilder();

            if (span.Hours > 0)
            {
                sb.Append($"{span.Hours} hours ");
            }

            if (span.Minutes > 1)
            {
                sb.Append($"{span.Minutes} minutes");
            }
            else if (span.Minutes == 1)
            {
                sb.Append($"{span.Minutes} minute");
            }

            return sb.ToString();
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

        public string ToHostString()
        {
            return $"Host: {Motherboard}";
        }

        public string ToKernelString()
        {
            return $"Kernel: {Kernel}";
        }

        public string ToMotherboardString()
        {
            return $"Motherboard: {Motherboard}";
        }

        public string ToOsString()
        {
            return $"OS: {OSCaption} [{OSArchitecture}]";
        }

        public string ToUptimeString()
        {
            return $"Uptime: {Uptime}";
        }

        public string TitleDashes()
        {
            return new string('-', ToString().Length);
        }
    }
}
