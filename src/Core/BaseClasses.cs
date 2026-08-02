namespace VessieFramework.Core;

/// <summary>
/// Interface base para todos os módulos do sistema
/// </summary>
public interface IModule
{
    string Name { get; }
    string Description { get; }
    bool IsEnabled { get; }
    
    Task InitializeAsync();
    Task ExecuteAsync();
    Task ShutdownAsync();
    Task ValidateAsync();
}

/// <summary>
/// Classe abstrata base para implementação de módulos
/// </summary>
public abstract class ModuleBase : IModule
{
    protected readonly ILogger _logger;
    protected readonly IProcessManager _processManager;
    protected readonly IRollbackManager _rollbackManager;
    
    public string Name => GetType().Name.Replace("Module", "");
    public abstract string Description { get; }
    public bool IsEnabled { get; protected set; }
    
    protected ModuleBase(ILogger logger, IProcessManager processManager, IRollbackManager rollbackManager)
    {
        _logger = logger;
        _processManager = processManager;
        _rollbackManager = rollbackManager;
        IsEnabled = false;
    }
    
    public virtual Task InitializeAsync()
    {
        IsEnabled = true;
        _logger.Log($"{Name} initialized");
        return Task.CompletedTask;
    }
    
    public abstract Task ExecuteAsync();
    
    public virtual Task ShutdownAsync()
    {
        IsEnabled = false;
        _logger.Log($"{Name} shutdown");
        return Task.CompletedTask;
    }
    
    public virtual Task ValidateAsync()
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// Interface para sistemas de patch
/// </summary>
public interface IPatch
{
    string PatchName { get; }
    string TargetPlatform { get; }
    bool IsApplied { get; }
    DateTime? AppliedAt { get; }
    
    Task<bool> CanApplyAsync();
    Task<PatchResult> ApplyAsync();
    Task<PatchResult> RevertAsync();
    Task<PatchStatus> GetStatusAsync();
}

/// <summary>
/// Resultado da aplicação de um patch
/// </summary>
public class PatchResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Error { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public object? PreviousState { get; set; }
    public object? NewState { get; set; }
}

/// <summary>
/// Status de um patch
/// </summary>
public enum PatchStatus
{
    NotApplied,
    Applied,
    Failed,
    Reverted,
    Pending
}

/// <summary>
/// Classe abstrata base para patches
/// </summary>
public abstract class PatchBase : IPatch
{
    protected readonly ILogger _logger;
    protected readonly IProcessManager _processManager;
    protected readonly IRollbackManager _rollbackManager;
    
    public abstract string PatchName { get; }
    public virtual string TargetPlatform => "Windows";
    public bool IsApplied { get; protected set; }
    public DateTime? AppliedAt { get; protected set; }
    
    protected PatchBase(ILogger logger, IProcessManager processManager, IRollbackManager rollbackManager)
    {
        _logger = logger;
        _processManager = processManager;
        _rollbackManager = rollbackManager;
        IsApplied = false;
    }
    
    public abstract Task<bool> CanApplyAsync();
    public abstract Task<PatchResult> ApplyAsync();
    public abstract Task<PatchResult> RevertAsync();
    public abstract Task<PatchStatus> GetStatusAsync();
    
    protected async Task<T?> TakeSnapshotAsync<T>(int pid, string snapshotType)
    {
        var snapshot = await _rollbackManager.TakeSnapshot(pid);
        return snapshot != null ? (T?)Convert.ChangeType(snapshot, typeof(T)) : default;
    }
}

/// <summary>
/// Interface para analisadores de performance
/// </summary>
public interface IAnalyzer
{
    string AnalyzerName { get; }
    Task<AnalysisResult> AnalyzeAsync();
    Task<IEnumerable<Recommendation>> GetRecommendationsAsync();
}

/// <summary>
/// Resultado de análise
/// </summary>
public class AnalysisResult
{
    public string Component { get; set; } = string.Empty;
    public double UsagePercent { get; set; }
    public double Temperature { get; set; }
    public long AvailableResources { get; set; }
    public long TotalResources { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Dictionary<string, object> Metrics { get; set; } = new();
}

/// <summary>
/// Recomendação de otimização
/// </summary>
public class Recommendation
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int EstimatedImpact { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsSafe { get; set; } = true;
}

/// <summary>
/// Interface para otimizadores
/// </summary>
public interface IOptimizer
{
    string OptimizerName { get; }
    Task<OptimizationResult> OptimizeAsync(OptimizationContext context);
    Task<bool> RevertAsync(int optimizationId);
}

/// <summary>
/// Contexto de otimização
/// </summary>
public class OptimizationContext
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public Profile Profile { get; set; } = new();
    public bool DryRun { get; set; }
    public CancellationToken CancellationToken { get; set; }
}

/// <summary>
/// Resultado de otimização
/// </summary>
public class OptimizationResult
{
    public int OptimizationId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> ChangesApplied { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public RollbackInfo? RollbackInfo { get; set; }
}

/// <summary>
/// Informações de rollback
/// </summary>
public class RollbackInfo
{
    public int SnapshotId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool AutoRollback { get; set; }
    public string RollbackTrigger { get; set; } = string.Empty;
}
