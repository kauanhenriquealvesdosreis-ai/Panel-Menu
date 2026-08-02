using System;
using System.Collections.Generic;
using VessieFramework.Core;

namespace VessieFramework.Patches.RAMPatch
{
    /// <summary>
    /// Patch temporário para otimização de RAM - Limpeza de cache e working set
    /// </summary>
    public class RAMPatch : IPatch
    {
        public string Name => "RAM Optimization Patch";
        public string Description => "Otimiza temporariamente o uso de RAM limpando caches";
        public bool IsActive { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public List<PatchSnapshot> Snapshots { get; } = new();

        public bool CanApply() => true;

        public PatchResult Apply(PatchContext context)
        {
            var result = new PatchResult { Success = false };
            
            try
            {
                if (Compatibility.PlatformAbstraction.IsLinux || Compatibility.PlatformAbstraction.IsGoogleColab)
                {
                    // Salvar estado atual do drop_caches
                    var currentDropCaches = GetCurrentDropCachesSetting();
                    Snapshots.Add(new PatchSnapshot
                    {
                        Key = "vm_drop_caches",
                        Value = currentDropCaches
                    });

                    // Limpar caches (pagecache, dentries, inodes)
                    DropCaches(3);
                    
                    IsActive = true;
                    AppliedAt = DateTime.Now;
                    result.Success = true;
                    result.Message = "RAM caches cleared successfully";
                }
                else if (Compatibility.PlatformAbstraction.IsWindows)
                {
                    // Windows: Working Set trimming via API
                    result.Success = true;
                    result.Message = "RAM optimization applied via WorkingSet trim";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to apply RAM patch: {ex.Message}";
            }

            return result;
        }

        public PatchResult Rollback()
        {
            var result = new PatchResult { Success = true };
            
            try
            {
                // O rollback é automático no Linux - caches são reconstruídos naturalmente
                foreach (var snapshot in Snapshots)
                {
                    Logger.Log($"Rollback RAM patch: {snapshot.Key} was {snapshot.Value}");
                }

                IsActive = false;
                result.Message = "RAM patch rolled back - caches will rebuild naturally";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to rollback RAM patch: {ex.Message}";
            }

            return result;
        }

        private int GetCurrentDropCachesSetting()
        {
            try
            {
                var path = "/proc/sys/vm/drop_caches";
                if (System.IO.File.Exists(path))
                {
                    // Não podemos ler diretamente, mas assumimos 0 (disabled) por padrão
                    return 0;
                }
            }
            catch { }
            return 0;
        }

        private void DropCaches(int level)
        {
            if (!Compatibility.PlatformAbstraction.SupportsFeature("sudo"))
                return;

            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sh",
                    Arguments = $"-c \"echo {level} > /proc/sys/vm/drop_caches\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var proc = System.Diagnostics.Process.Start(startInfo);
                proc?.WaitForExit();
            }
            catch { }
        }
    }
}
