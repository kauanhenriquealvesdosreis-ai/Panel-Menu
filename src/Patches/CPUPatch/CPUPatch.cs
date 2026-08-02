using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.CPUPatch
{
    /// <summary>
    /// Patch temporário para otimização de CPU - Ajuste de frequência e governor
    /// </summary>
    public class CPUPatch : IPatch
    {
        public string Name => "CPU Performance Patch";
        public string Description => "Otimiza temporariamente a frequência da CPU e o governor";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply()
        {
            return Compatibility.PlatformAbstraction.IsLinux || 
                   Compatibility.PlatformAbstraction.IsGoogleColab;
        }

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            
            try
            {
                if (Compatibility.PlatformAbstraction.IsLinux)
                {
                    // Salvar estado atual do governor
                    var governors = GetCpuGovernors();
                    foreach (var gov in governors)
                    {
                        Snapshots.Add(new PatchSnapshot
                        {
                            Key = $"cpu_governor_{gov.Key}",
                            Value = gov.Value
                        });
                    }

                    // Aplicar performance governor
                    SetCpuGovernor("performance");
                    
                    IsActive = true;
                    AppliedAt = DateTime.Now;
                    result.Success = true;
                    result.Message = "CPU governor set to performance mode";
                }
                else if (Compatibility.PlatformAbstraction.IsWindows)
                {
                    result.Success = true;
                    result.Message = "CPU optimization handled by Windows power plan";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to apply CPU patch: {ex.Message}";
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
                    if (snapshot.Key.StartsWith("cpu_governor_"))
                    {
                        var cpuId = snapshot.Key.Replace("cpu_governor_", "");
                        SetCpuGovernor(snapshot.Value?.ToString() ?? "ondemand", int.Parse(cpuId));
                    }
                }

                IsActive = false;
                result.Message = "CPU governor restored to previous state";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to rollback CPU patch: {ex.Message}";
            }

            return result;
        }

        private Dictionary<int, string> GetCpuGovernors()
        {
            var governors = new Dictionary<int, string>();
            var sysfsPath = "/sys/devices/system/cpu";

            if (!System.IO.Directory.Exists(sysfsPath))
                return governors;

            foreach (var dir in System.IO.Directory.GetDirectories(sysfsPath, "cpu[0-9]*"))
            {
                var cpuIdStr = System.IO.Path.GetFileName(dir).Replace("cpu", "");
                if (int.TryParse(cpuIdStr, out int cpuId))
                {
                    var governorPath = $"{dir}/cpufreq/scaling_governor";
                    if (System.IO.File.Exists(governorPath))
                    {
                        var governor = System.IO.File.ReadAllText(governorPath).Trim();
                        governors[cpuId] = governor;
                    }
                }
            }

            return governors;
        }

        private void SetCpuGovernor(string governor, int cpuId = -1)
        {
            if (cpuId >= 0)
            {
                var governorPath = $"/sys/devices/system/cpu/cpu{cpuId}/cpufreq/scaling_governor";
                if (System.IO.File.Exists(governorPath))
                {
                    System.IO.File.WriteAllText(governorPath, governor);
                }
            }
            else
            {
                var sysfsPath = "/sys/devices/system/cpu";
                foreach (var dir in System.IO.Directory.GetDirectories(sysfsPath, "cpu[0-9]*"))
                {
                    var governorPath = $"{dir}/cpufreq/scaling_governor";
                    if (System.IO.File.Exists(governorPath))
                    {
                        System.IO.File.WriteAllText(governorPath, governor);
                    }
                }
            }
        }
    }
}
