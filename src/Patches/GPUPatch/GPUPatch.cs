using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.GPUPatch
{
    public class GPUPatch : IPatch
    {
        public string Name => "GPU Performance Patch";
        public string Description => "Otimiza frequência e power limit da GPU";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply() => Compatibility.PlatformAbstraction.IsLinux || Compatibility.PlatformAbstraction.IsGoogleColab;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            try
            {
                if (HasNvidiaGPU())
                {
                    var currentPowerLimit = GetGPUPowerLimit();
                    Snapshots.Add(new PatchSnapshot { Key = "gpu_power_limit", Value = currentPowerLimit });
                    
                    SetGPUPowerMode("performance");
                    IsActive = true;
                    AppliedAt = DateTime.Now;
                    result.Success = true;
                    result.Message = "GPU optimized for performance mode";
                }
                else
                {
                    result.Success = true;
                    result.Message = "No NVIDIA GPU detected, patch skipped";
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
                    if (snapshot.Key == "gpu_power_limit")
                        RestoreGPUPowerLimit();
                }
                IsActive = false;
                result.Message = "GPU settings restored";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollback failed: {ex.Message}";
            }
            return result;
        }

        private bool HasNvidiaGPU()
        {
            return System.IO.File.Exists("/usr/bin/nvidia-smi") || 
                   System.IO.Directory.Exists("/proc/driver/nvidia/gpus");
        }

        private string GetGPUPowerLimit()
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = "--query-gpu=power.limit --format=csv,noheader",
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
            return "";
        }

        private void SetGPUPowerMode(string mode)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = $"-pm {mode} -pl 250",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }

        private void RestoreGPUPowerLimit()
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = "-pm 0",
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
