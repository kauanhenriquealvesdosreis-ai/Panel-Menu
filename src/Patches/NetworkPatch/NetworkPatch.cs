using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.NetworkPatch
{
    public class NetworkPatch : IPatch
    {
        public string Name => "Network Stack Patch";
        public string Description => "Otimiza parâmetros de rede para baixa latência";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply() => Compatibility.PlatformAbstraction.IsLinux || Compatibility.PlatformAbstraction.IsGoogleColab;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            try
            {
                var settings = new Dictionary<string, string>
                {
                    ["net.core.rmem_max"] = "16777216",
                    ["net.core.wmem_max"] = "16777216",
                    ["net.ipv4.tcp_rmem"] = "4096 87380 16777216",
                    ["net.ipv4.tcp_wmem"] = "4096 65536 16777216",
                    ["net.ipv4.tcp_congestion_control"] = "bbr"
                };

                foreach (var setting in settings)
                {
                    var currentValue = GetSysctlValue(setting.Key);
                    Snapshots.Add(new PatchSnapshot { Key = setting.Key, Value = currentValue });
                    SetSysctlValue(setting.Key, setting.Value);
                }

                IsActive = true;
                AppliedAt = DateTime.Now;
                result.Success = true;
                result.Message = "Network stack optimized for low latency";
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
                    SetSysctlValue(snapshot.Key, snapshot.Value?.ToString() ?? "");
                }
                IsActive = false;
                result.Message = "Network stack restored";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollback failed: {ex.Message}";
            }
            return result;
        }

        private string GetSysctlValue(string key)
        {
            try
            {
                var path = $"/proc/sys/{key.Replace('.', '/')}";
                if (System.IO.File.Exists(path))
                    return System.IO.File.ReadAllText(path).Trim();
            }
            catch { }
            return "";
        }

        private void SetSysctlValue(string key, string value)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sysctl",
                    Arguments = $"-w {key}={value}",
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
