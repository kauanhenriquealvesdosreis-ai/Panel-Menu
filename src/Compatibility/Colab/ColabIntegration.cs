using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VessieFramework.Compatibility.Colab
{
    /// <summary>
    /// Integração com Google Colab para execução em ambiente cloud
    /// </summary>
    public static class ColabIntegration
    {
        private static bool? _isColab = null;
        
        public static bool IsRunningInColab()
        {
            if (_isColab.HasValue)
                return _isColab.Value;

            _isColab = Environment.GetEnvironmentVariable("COLAB_JUPYTER_IP") != null ||
                       Environment.GetEnvironmentVariable("CLOUD_SHELL") == "true" ||
                       System.IO.File.Exists("/content/.colab-abort.txt") ||
                       System.IO.Directory.Exists("/content");

            return _isColab.Value;
        }

        public static string GetContentPath()
        {
            return "/content";
        }

        public static bool HasGPU()
        {
            try
            {
                var nvidiaSmi = System.IO.File.Exists("/usr/bin/nvidia-smi");
                var gpuPath = System.IO.Directory.Exists("/proc/driver/nvidia/gpus");
                return nvidiaSmi || gpuPath;
            }
            catch
            {
                return false;
            }
        }

        public static string GetGPUInfo()
        {
            try
            {
                if (!HasGPU())
                    return "No GPU available";

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = "--query-gpu=name,memory.total,driver_version --format=csv,noheader",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(startInfo);
                if (proc != null)
                {
                    proc.WaitForExit();
                    return proc.StandardOutput.ReadToEnd().Trim();
                }
            }
            catch { }

            return "GPU info unavailable";
        }

        public static long GetAvailableRAM()
        {
            try
            {
                var meminfoPath = "/proc/meminfo";
                if (!System.IO.File.Exists(meminfoPath))
                    return 0;

                var lines = System.IO.File.ReadAllLines(meminfoPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemAvailable:"))
                    {
                        var parts = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && long.TryParse(parts[1], out long kb))
                            return kb * 1024;
                    }
                }
            }
            catch { }

            return 0;
        }

        public static long GetTotalRAM()
        {
            try
            {
                var meminfoPath = "/proc/meminfo";
                if (!System.IO.File.Exists(meminfoPath))
                    return 0;

                var lines = System.IO.File.ReadAllLines(meminfoPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemTotal:"))
                    {
                        var parts = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && long.TryParse(parts[1], out long kb))
                            return kb * 1024;
                    }
                }
            }
            catch { }

            return 0;
        }

        public static int GetCPUCount()
        {
            try
            {
                var nprocStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "nproc",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(nprocStartInfo);
                if (proc != null)
                {
                    proc.WaitForExit();
                    var output = proc.StandardOutput.ReadToEnd().Trim();
                    if (int.TryParse(output, out int count))
                        return count;
                }
            }
            catch { }

            return Environment.ProcessorCount;
        }

        public static async Task MountGoogleDrive(string mountPoint = "/content/drive")
        {
            if (!IsRunningInColab())
                throw new InvalidOperationException("Not running in Google Colab");

            try
            {
                var script = $@"
from google.colab import drive
drive.mount('{mountPoint}')
";
                // Em um cenário real, isso seria executado via Python.NET
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to mount Google Drive: {ex.Message}");
            }
        }

        public static void InstallPackage(string packageName)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "pip",
                    Arguments = $"install -q {packageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }

        public static void InstallAptPackage(string packageName)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "apt-get",
                    Arguments = $"install -y -qq {packageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }

        public static Dictionary<string, object> GetColabSpecs()
        {
            var specs = new Dictionary<string, object>
            {
                ["IsColab"] = IsRunningInColab(),
                ["HasGPU"] = HasGPU(),
                ["GPUInfo"] = GetGPUInfo(),
                ["TotalRAM"] = GetTotalRAM(),
                ["AvailableRAM"] = GetAvailableRAM(),
                ["CPUCount"] = GetCPUCount(),
                ["ContentPath"] = GetContentPath()
            };

            return specs;
        }

        public static async Task RunPythonScript(string script)
        {
            if (!IsRunningInColab())
                throw new InvalidOperationException("Not running in Google Colab");

            try
            {
                var tempFile = System.IO.Path.GetTempFileName() + ".py";
                await System.IO.File.WriteAllTextAsync(tempFile, script);

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = tempFile,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(startInfo);
                if (proc != null)
                {
                    await Task.Run(() => proc.WaitForExit());
                    var output = await proc.StandardOutput.ReadToEndAsync();
                    var error = await proc.StandardError.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(error))
                        throw new Exception($"Python error: {error}");
                }

                System.IO.File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to run Python script: {ex.Message}");
            }
        }
    }
}
