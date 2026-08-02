using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VessieFramework.Core;

namespace VessieFramework.Systems
{
    /// <summary>
    /// Gerenciador central de todos os sistemas do VessieFramework
    /// </summary>
    public class SystemManager
    {
        private readonly List<IPatch> _patches = new();
        private readonly List<IModule> _modules = new();
        private readonly Dictionary<string, object> _systemState = new();

        public bool IsRunning { get; private set; }
        public DateTime StartedAt { get; private set; }
        public int ActivePatches => _patches.Count(p => p.IsActive);
        public int ActiveModules => _modules.Count(m => m.IsEnabled);

        public void RegisterPatch(IPatch patch) => _patches.Add(patch);
        public void RegisterModule(IModule module) => _modules.Add(module);

        public async Task<SystemStatus> StartAsync()
        {
            var status = new SystemStatus { Success = true };
            try
            {
                foreach (var module in _modules)
                {
                    if (module.CanLoad())
                    {
                        var result = module.Load();
                        if (!result.Success)
                            status.Warnings.Add($"Module {module.Name}: {result.Message}");
                    }
                }

                IsRunning = true;
                StartedAt = DateTime.Now;
                status.Message = $"System started with {_modules.Count} modules and {_patches.Count} patches";
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.Message = $"Failed to start: {ex.Message}";
            }
            return await Task.FromResult(status);
        }

        public async Task<SystemStatus> StopAsync()
        {
            var status = new SystemStatus { Success = true };
            try
            {
                foreach (var patch in _patches)
                {
                    if (patch.IsActive)
                        patch.Rollback();
                }

                foreach (var module in _modules)
                {
                    if (module.IsEnabled)
                        module.Unload();
                }

                IsRunning = false;
                status.Message = "System stopped successfully";
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.Message = $"Failed to stop: {ex.Message}";
            }
            return await Task.FromResult(status);
        }

        public async Task<PatchResult> ApplyPatch<T>() where T : IPatch, new()
        {
            var patch = new T();
            if (!patch.CanApply())
                return new PatchResult { Success = false, Message = "Patch not compatible with current platform" };

            var result = patch.Apply(new PatchContext());
            if (result.Success)
                _patches.Add(patch);

            return await Task.FromResult(result);
        }

        public async Task<Dictionary<string, object>> GetAllMetrics()
        {
            var metrics = new Dictionary<string, object>
            {
                ["isRunning"] = IsRunning,
                ["startedAt"] = StartedAt,
                ["activePatches"] = ActivePatches,
                ["activeModules"] = ActiveModules,
                ["platform"] = Compatibility.PlatformAbstraction.GetPlatformName(),
                ["environment"] = Compatibility.PlatformAbstraction.GetEnvironmentName()
            };

            foreach (var module in _modules)
            {
                if (module.IsEnabled)
                {
                    var moduleMetrics = await module.GetMetrics();
                    metrics[$"module_{module.Name}"] = moduleMetrics;
                }
            }

            return metrics;
        }
    }

    public class SystemStatus
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public List<string> Warnings { get; } = new();
    }
}
