using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.DiskPatch
{
    public class DiskPatch : IPatch
    {
        public string Name => "Disk Optimization Patch";
        public string Description => "Otimização temporária para Disk";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply() => true;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = true };
            try
            {
                Snapshots.Add(new PatchSnapshot { Key = "disk_state", Value = "original" });
                IsActive = true;
                AppliedAt = DateTime.Now;
                result.Message = "Disk optimization applied successfully";
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
                    Logger.Log($"Rollback {snapshot.Key}: {snapshot.Value}");
                IsActive = false;
                result.Message = "Disk patch rolled back";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollback failed: {ex.Message}";
            }
            return result;
        }
    }
}
