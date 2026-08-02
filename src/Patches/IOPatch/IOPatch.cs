using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.IOPatch
{
    public class IOPatch : IPatch
    {
        public string Name => "I/O Scheduler Patch";
        public string Description => "Otimiza o scheduler de I/O para melhor performance";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply() => Compatibility.PlatformAbstraction.IsLinux || Compatibility.PlatformAbstraction.IsGoogleColab;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            try
            {
                var devices = GetBlockDevices();
                foreach (var device in devices)
                {
                    var currentScheduler = GetCurrentScheduler(device);
                    Snapshots.Add(new PatchSnapshot { Key = $"io_scheduler_{device}", Value = currentScheduler });
                    SetScheduler(device, "none"); // none ou mq-deadline para SSDs
                }
                IsActive = true;
                AppliedAt = DateTime.Now;
                result.Success = true;
                result.Message = $"I/O scheduler optimized for {devices.Count} devices";
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
                    if (snapshot.Key.StartsWith("io_scheduler_"))
                    {
                        var device = snapshot.Key.Replace("io_scheduler_", "");
                        SetScheduler(device, snapshot.Value?.ToString() ?? "mq-deadline");
                    }
                }
                IsActive = false;
                result.Message = "I/O scheduler restored";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollback failed: {ex.Message}";
            }
            return result;
        }

        private List<string> GetBlockDevices()
        {
            var devices = new List<string>();
            var blockPath = "/sys/block";
            if (System.IO.Directory.Exists(blockPath))
            {
                foreach (var dir in System.IO.Directory.GetDirectories(blockPath))
                {
                    var name = System.IO.Path.GetFileName(dir);
                    if (name.StartsWith("sd") || name.StartsWith("nvme") || name.StartsWith("vd"))
                        devices.Add(name);
                }
            }
            return devices;
        }

        private string GetCurrentScheduler(string device)
        {
            var path = $"/sys/block/{device}/queue/scheduler";
            if (System.IO.File.Exists(path))
            {
                var content = System.IO.File.ReadAllText(path);
                var parts = content.Trim().Split(' ');
                foreach (var part in parts)
                {
                    if (part.StartsWith("[") && part.EndsWith("]"))
                        return part.Trim('[', ']');
                }
            }
            return "unknown";
        }

        private void SetScheduler(string device, string scheduler)
        {
            var path = $"/sys/block/{device}/queue/scheduler";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, scheduler);
            }
        }
    }
}
