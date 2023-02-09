using System;
using System.Diagnostics;

namespace CliStatus
{
	public class SwVers
	{
		public string? ProductName { get; private set; } = "";
		public string? ProductVersion { get; private set; } = "";
		public string? BuildVersion { get; private set; } = "";

        private string swVersOutput = "";

		public SwVers()
		{
			FetchSwVers();
            ParseSwVers();
		}

		private void FetchSwVers()
		{
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "sw_vers",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            p.Start();
            swVersOutput = p.StandardOutput.ReadToEnd() ?? "";
            p.WaitForExit();
            p.Dispose();
        }

        private void ParseSwVers()
        {
            var parts = swVersOutput.Split("\n");
            if (parts.Length == 4)
            {
                string[] lineParts;
                foreach (string line in parts)
                {
                    lineParts = line.Split(":");
                    if (lineParts.Length == 2)
                    {
                        switch (lineParts[0].ToLower())
                        {
                            case "productname":
                                ProductName = lineParts[1].Trim();
                                break;
                            case "productversion":
                                ProductVersion = lineParts[1].Trim();
                                break;
                            case "buildversion":
                                BuildVersion = lineParts[1].Trim(); ;
                                break;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"OS: {ProductName} {ProductVersion} {BuildVersion}";
        }
	}
}

