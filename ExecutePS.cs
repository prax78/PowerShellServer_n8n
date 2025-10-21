using Microsoft.OpenApi.Writers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace PowerShellServer
{
    public class ExecutePS
    {
        public string Result { get; set; } =string.Empty;
        public int ErrorCode { get; set; } = 0;
        private  ILogger _logger;

        public ExecutePS(string code)
        {
              List<string> msg = new List<string>();
           _logger= LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("PowerShellServer");
            var guid = Guid.NewGuid().ToString();
            var writeStatus = HandleFileOperations($"{guid}.ps1", code, "Write");
            if(writeStatus == 0)
            {
                Process p = new Process();
                ProcessStartInfo si = new ProcessStartInfo(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",new string[] { 
                "-File", 
                $"{guid}.ps1",
                "-OutputFormat XML"
                });
 
                
                p.StartInfo = si;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                try {
                    p.Start();
                    _logger.LogInformation("PowerShell script started.");
                    Result = p.StandardOutput.ReadToEnd();
                    p.Close();
                }
                catch(Exception ex)
                {
                    _logger.LogCritical("PowerShell script Error."+ ex.Message);
                    Result = $"Error executing PowerShell script: {ex.Message}";
                }
              
                
              
                HandleFileOperations($"{guid}.ps1", "", "Delete");
            }
            else
            {
               
                _logger.LogCritical($"Error in file operations. Error Code: {writeStatus}");

            }


                Result = JsonSerializer.Serialize(Result,new JsonSerializerOptions { Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping,ReferenceHandler=ReferenceHandler.Preserve});
        }

        public static int HandleFileOperations(string filename,string content,string operation)
        {
            if (operation == "Write")
            {
                try
                {
                    File.WriteAllText(filename, content);
            
                }
                catch (Exception ex)
                {
                    var ErrorCode = 1;
                    return ErrorCode;
                }
            }
            else
            {
                try
                {
                    File.Delete(filename);
                }
                catch(Exception ex)
                {
                    var ErrorCode = 2;
                   
                    return ErrorCode;
                }
            }
            return 0;
        }
    }
}
