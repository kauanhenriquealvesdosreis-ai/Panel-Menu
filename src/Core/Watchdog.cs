using System.Diagnostics;
using VessieFramework.Models;
using VessieFramework.Utils;

namespace VessieFramework.Core;

/// <summary>
/// Interface para o sistema Watchdog de monitoramento
/// </summary>
public interface IWatchdog
{
    bool IsRunning { get; }
    void Start(TimeSpan? checkInterval = null);
    void Stop();
    Task<int> GetMonitoredProcessesCountAsync();
    event EventHandler<WatchdogEventArgs>? ProcessRestored;
}

public class WatchdogEventArgs : EventArgs
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public DateTime RestoredAt { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Sistema avançado de monitoramento e rollback automático de processos
/// </summary>
public static class Watchdog
{
    private static Timer? _timer;
    private static readonly object _lock = new();
    private static bool _isRunning;
    
    /// <summary>
    /// Evento disparado quando um processo é restaurado
    /// </summary>
    public static event EventHandler<WatchdogEventArgs>? ProcessRestored;
    
    /// <summary>
    /// Indica se o watchdog está em execução
    /// </summary>
    public static bool IsRunning => _isRunning;

    /// <summary>
    /// Inicia o watchdog com intervalo padrão de 5 segundos
    /// </summary>
    public static void Start(TimeSpan? checkInterval = null)
    {
        lock (_lock)
        {
            if (_isRunning) return;
            
            var interval = checkInterval ?? TimeSpan.FromSeconds(5);
            _timer = new Timer(Check, null, TimeSpan.Zero, interval);
            _isRunning = true;
            Logger.Log($"Watchdog iniciado com intervalo de {interval.TotalSeconds}s");
        }
    }

    /// <summary>
    /// Para o watchdog
    /// </summary>
    public static void Stop()
    {
        lock (_lock)
        {
            if (!_isRunning) return;
            
            _timer?.Dispose();
            _timer = null;
            _isRunning = false;
            Logger.Log("Watchdog parado");
        }
    }

    /// <summary>
    /// Verifica processos monitorados e restaura se necessário
    /// </summary>
    private static void Check(object? state)
    {
        try
        {
            var snapshots = RollbackManager.GetAllSnapshots();
            foreach (var snap in snapshots.ToList())
            {
                try
                {
                    using var p = Process.GetProcessById(snap.ProcessId);
                    if (p.HasExited)
                    {
                        RestoreProcess(snap, "Processo encerrado");
                    }
                }
                catch (ArgumentException)
                {
                    // Processo não existe mais
                    RestoreProcess(snap, "Processo não encontrado");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Erro no Watchdog ao verificar {snap.ProcessName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Erro grave no Watchdog: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Restaura um processo e dispara evento
    /// </summary>
    private static void RestoreProcess(ProcessSnapshot snapshot, string reason)
    {
        if (RollbackManager.RestoreSnapshot(snapshot.ProcessId))
        {
            RollbackManager.RemoveSnapshot(snapshot.ProcessId);
            Logger.Log($"Watchdog restaurou {reason}: {snapshot.ProcessName} (PID {snapshot.ProcessId})");
            
            ProcessRestored?.Invoke(null, new WatchdogEventArgs
            {
                ProcessId = snapshot.ProcessId,
                ProcessName = snapshot.ProcessName,
                RestoredAt = DateTime.Now,
                Reason = reason
            });
        }
    }
    
    /// <summary>
    /// Obtém quantidade de processos sendo monitorados
    /// </summary>
    public static async Task<int> GetMonitoredProcessesCountAsync()
    {
        return await Task.Run(() => RollbackManager.GetAllSnapshots().Count);
    }
}