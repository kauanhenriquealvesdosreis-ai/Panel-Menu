using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VessieFramework.Compatibility.Linux
{
    /// <summary>
    /// Implementação de operações nativas para Linux usando /proc e sysfs
    /// </summary>
    public static class LinuxNativeMethods
    {
        private const string ProcPath = "/proc";
        private const string SysfsPath = "/sys";

        public static Dictionary<int, ProcessInfoLinux> GetProcesses()
        {
            var processes = new Dictionary<int, ProcessInfoLinux>();
            
            try
            {
                if (!System.IO.Directory.Exists(ProcPath))
                    return processes;

                foreach (var dir in System.IO.Directory.GetDirectories(ProcPath))
                {
                    var dirName = System.IO.Path.GetFileName(dir);
                    if (int.TryParse(dirName, out int pid))
                    {
                        try
                        {
                            var proc = ReadProcessInfo(pid);
                            if (proc != null)
                                processes[pid] = proc;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return processes;
        }

        private static ProcessInfoLinux? ReadProcessInfo(int pid)
        {
            try
            {
                var statPath = $"{ProcPath}/{pid}/stat";
                var statusPath = $"{ProcPath}/{pid}/status";
                var cmdPath = $"{ProcPath}/{pid}/cmdline";

                if (!System.IO.File.Exists(statPath))
                    return null;

                var stat = System.IO.File.ReadAllText(statPath);
                var parts = stat.Split(' ');
                
                if (parts.Length < 24)
                    return null;

                var name = parts[1].Trim('(', ')');
                var state = parts[2];
                var ppid = int.Parse(parts[3]);
                var priority = long.Parse(parts[18]);
                var nice = long.Parse(parts[19]);
                
                var utime = ulong.Parse(parts[13]);
                var stime = ulong.Parse(parts[14]);
                var totalTime = utime + stime;

                string cmdline = "";
                if (System.IO.File.Exists(cmdPath))
                {
                    cmdline = System.IO.File.ReadAllText(cmdPath).Replace('\0', ' ').Trim();
                }

                long rss = 0;
                if (System.IO.File.Exists(statusPath))
                {
                    var statusLines = System.IO.File.ReadAllLines(statusPath);
                    foreach (var line in statusLines)
                    {
                        if (line.StartsWith("VmRSS:"))
                        {
                            var rssParts = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (rssParts.Length >= 2 && long.TryParse(rssParts[1], out long rssKb))
                                rss = rssKb * 1024; // Converter para bytes
                            break;
                        }
                    }
                }

                return new ProcessInfoLinux
                {
                    Pid = pid,
                    Name = name,
                    Cmdline = cmdline,
                    State = state,
                    Ppid = ppid,
                    Priority = priority,
                    Nice = nice,
                    TotalCpuTime = totalTime,
                    RssBytes = rss
                };
            }
            catch
            {
                return null;
            }
        }

        public static bool SetProcessPriority(int pid, int priority)
        {
            try
            {
                // Usar renice no Linux
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "renice",
                    Arguments = $"-n {priority} -p {pid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                
                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
                return proc?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetProcessAffinity(int pid, List<int> cpuCores)
        {
            try
            {
                var cpuList = string.Join(",", cpuCores);
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "taskset",
                    Arguments = $"-cp {cpuList} {pid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                
                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
                return proc?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public static long GetAvailableMemory()
        {
            try
            {
                var meminfoPath = $"{ProcPath}/meminfo";
                if (!System.IO.File.Exists(meminfoPath))
                    return 0;

                var lines = System.IO.File.ReadAllLines(meminfoPath);
                long memAvailable = 0;
                
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemAvailable:"))
                    {
                        var parts = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && long.TryParse(parts[1], out long kb))
                            return kb * 1024;
                    }
                }
                
                return memAvailable;
            }
            catch
            {
                return 0;
            }
        }

        public static long GetTotalMemory()
        {
            try
            {
                var meminfoPath = $"{ProcPath}/meminfo";
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
                
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetCpuCount()
        {
            try
            {
                if (System.IO.Directory.Exists($"{SysfsPath}/devices/system/cpu"))
                {
                    var cpuDirs = System.IO.Directory.GetDirectories($"{SysfsPath}/devices/system/cpu", "cpu[0-9]*");
                    return cpuDirs.Length;
                }
                
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
                
                return Environment.ProcessorCount;
            }
            catch
            {
                return Environment.ProcessorCount;
            }
        }

        public static double GetCpuUsage(int pid)
        {
            try
            {
                var statPath = $"{ProcPath}/{pid}/stat";
                if (!System.IO.File.Exists(statPath))
                    return 0;

                var stat = System.IO.File.ReadAllText(statPath);
                var parts = stat.Split(' ');
                
                if (parts.Length < 24)
                    return 0;

                var utime = ulong.Parse(parts[13]);
                var stime = ulong.Parse(parts[14]);
                var totalTime = utime + stime;

                // Calcular uptime do sistema
                var uptimePath = $"{ProcPath}/uptime";
                var uptime = double.Parse(System.IO.File.ReadAllText(uptimePath).Split(' ')[0]);
                
                // Calcular tempo do processo em segundos (assumindo 100 jiffies/segundo)
                var clkTck = 100.0; // Normalmente 100 no Linux
                var processTime = totalTime / clkTck;
                
                // Calcular uso de CPU
                var startTime = ulong.Parse(parts[21]) / clkTck;
                var elapsed = uptime - startTime;
                
                if (elapsed <= 0)
                    return 0;

                return (processTime / elapsed) * 100.0;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class ProcessInfoLinux
    {
        public int Pid { get; set; }
        public string Name { get; set; } = "";
        public string Cmdline { get; set; } = "";
        public string State { get; set; } = "";
        public int Ppid { get; set; }
        public long Priority { get; set; }
        public long Nice { get; set; }
        public ulong TotalCpuTime { get; set; }
        public long RssBytes { get; set; }
    }
}
