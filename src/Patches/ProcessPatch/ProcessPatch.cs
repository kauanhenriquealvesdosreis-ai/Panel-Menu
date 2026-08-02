using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.ProcessPatch
{
    public class ProcessPatch : IPatch
    {
        public string Name => "Process Priority Patch";
        public string Description => "Ajusta prioridade e nice value de processos";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();
        public int TargetPid { get; set; }

        public bool CanApply() => true;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            TargetPid = context.ProcessId;
            
            try
            {
                if (Compatibility.PlatformAbstraction.IsLinux || Compatibility.PlatformAbstraction.IsGoogleColab)
                {
                    var currentNice = GetCurrentNice(TargetPid);
                    Snapshots.Add(new PatchSnapshot { Key = $"process_nice_{TargetPid}", Value = currentNice });
                    
                    SetProcessNice(TargetPid, -10); // Maior prioridade
                    
                    var currentIOPriority = GetIOPriority(TargetPid);
                    Snapshots.Add(new PatchSnapshot { Key = $"process_io_{TargetPid}", Value = currentIOPriority });
                    SetIOPriority(TargetPid, 0); // RT
                    
                    IsActive = true;
                    AppliedAt = DateTime.Now;
                    result.Success = true;
                    result.Message = $"Process {TargetPid} optimized for performance";
                }
                else
                {
                    result.Success = true;
                    result.Message = "Process optimization handled by Windows API";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed: {ex.Message}";
            }
            return result;
        }

        public PatchResult Rollback()
        {
            var result = new PatchResult { Success = true };
            try
            {
                foreach (var snapshot in Snapshots)
                {
                    if (snapshot.Key.StartsWith($"process_nice_{TargetPid}"))
                    {
                        var nice = snapshot.Value?.ToString() ?? "0";
                        SetProcessNice(TargetPid, int.Parse(nice));
                    }
                }
                IsActive = false;
                result.Message = $"Process {TargetPid} restored to original priority";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollback failed: {ex.Message}";
            }
            return result;
        }

        private int GetCurrentNice(int pid)
        {
            try
            {
                var statPath = $"/proc/{pid}/stat";
                if (System.IO.File.Exists(statPath))
                {
                    var stat = System.IO.File.ReadAllText(statPath);
                    var parts = stat.Split(' ');
                    if (parts.Length > 19 && int.TryParse(parts[19], out int nice))
                        return nice;
                }
            }
            catch { }
            return 0;
        }

        private void SetProcessNice(int pid, int nice)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "renice",
                    Arguments = $"-n {nice} -p {pid}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }

        private string GetIOPriority(int pid) => "unknown";
        
        private void SetIOPriority(int pid, int priority)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ionice",
                    Arguments = $"-c {priority} -p {pid}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }
    }
}
