
//   This service replaces the specified .vssettings file with Visual Studio's .vssettings file when
// Visual Studio is closed. When this replacement is made, it creates a file named VSSettinsImportLog.txt
// in your user folder and logs the replacement. When you add the service to Windows Service, it requires admin rights to run properly.

using System.Diagnostics;
using System.Runtime.Versioning;


namespace VSSettingsImportTool
{
    [SupportedOSPlatform("windows")]
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        bool _settingsImported = false;
        
        // Import-VSSettings.ps1 script file full path
        string _scriptPath = @"Your Import-VSSettings.ps1 file directory with file name";

        // Full path to .vssettings file to import
        string _vsSettingsPath = @"Your .vssettings file directory with file name ";        
        
        string _command = "";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _command = $"-ExecutionPolicy Bypass -File \"{_scriptPath}\" -vsSettingsPath \"{_vsSettingsPath}\"";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                   
                    var vsRunning = CheckVisualStudioRunning();
                    Console.WriteLine("VS Running: " + vsRunning.ToString() + " " + "IsImported: " + _settingsImported.ToString());
                    
                    
                    if (!vsRunning && !_settingsImported)
                    {
                        Console.WriteLine("Visual Studio closed, settings Importing...");
                        LogToFile("Visual Studio closed, settings Importing...");
                        var settingsImported = RunPowerShellScriptProcess();
                        if (settingsImported)
                        {
                            Console.WriteLine("Successfully imported");
                            LogToFile("Successfully imported");
                            _settingsImported = true;
                        }
                    }


                }
                await Task.Delay(1000, stoppingToken);
            }
        }


        //    Searches for visual studio process. If found, sets the _settingsImported
        // variable to false to import the .vssettings file when closing visual studio.
        // If cannot find , returns false
        private bool CheckVisualStudioRunning()
        {
            var processes = Process.GetProcessesByName("devenv");
            if (processes.Length > 0)
            {

                _settingsImported = false;

                return true;
            }

            return false;
        }

        // Runs the PowerShell script specified in the scriptpath variable.
        private bool RunPowerShellScriptProcess()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = _command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    string error = process.StandardError.ReadToEnd().Trim();

                    process.WaitForExit();

                    if (output=="True")
                    {
                        return true;
                    }

                    Console.WriteLine(error);
                    
                    return false;

                }


            }
            catch (Exception exeption)
            {
                _logger.LogError(exeption, "An error occurred while running PowerShell script.");
                return false;
            }
        }

        private void LogToFile(string logMessage)
        {
            File.AppendAllText(@"Your .txt file directory with file name",DateTime.Now.ToString()+"  "+ logMessage + Environment.NewLine);
        }



    }
}
