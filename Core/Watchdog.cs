using System.Diagnostics;
using VessieFramework.Models;
using VessieFramework.Utils;   // <-- ADICIONADO

namespace VessieFramework.Core;

public static class Watchdog
{
    private static Timer? _timer;

    public static void Start()
    {
        _timer = new Timer(Check, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    public static void Stop()
    {
        _timer?.Dispose();
    }

    private static void Check(object? state)
    {
        try
        {
            var snapshots = RollbackManager.GetAllSnapshots();
            foreach (var snap in snapshots)
            {
                try
                {
                    var p = Process.GetProcessById(snap.ProcessId);
                    if (p.HasExited)
                    {
                        RollbackManager.RestoreSnapshot(snap.ProcessId);
                        RollbackManager.RemoveSnapshot(snap.ProcessId);
                        Logger.Log($"Watchdog restaurou processo morto: {snap.ProcessName} (PID {snap.ProcessId})");
                    }
                }
                catch (ArgumentException)
                {
                    RollbackManager.RestoreSnapshot(snap.ProcessId);
                    RollbackManager.RemoveSnapshot(snap.ProcessId);
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
}