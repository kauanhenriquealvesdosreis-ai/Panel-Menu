using VessieFramework;
using VessieFramework.Core;
using VessieFramework.Optimizer;
using VessieFramework.Utils;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace VessieFramework;

class Program
{
    static async Task Main(string[] args)
    {
        // Captura exceções não tratadas em qualquer thread
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Logger.Log($"💥 Exceção crítica: {e.ExceptionObject}");
            // Tenta restaurar tudo antes de morrer
            try { RollbackManager.RestoreAllAndClear(); } catch { }
            Environment.Exit(1);
        };

        // Captura exceções em tasks assíncronas
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Logger.Log($"💥 Exceção não observada em Task: {e.Exception}");
            e.SetObserved();
        };

        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║  Vessie Framework - Process Optimizer ║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine("🌐 Dashboard: http://localhost:5000");
        Console.WriteLine("📌 Pressione ESC para restaurar e sair.");
        Console.WriteLine();

        try
        {
            Watchdog.Start();
            var webTask = WebServer.StartAsync();

            var timer = new Timer(_ =>
            {
                try
                {
                    AutoOptimizer.RunAutoOptimization();
                }
                catch (Exception ex)
                {
                    Logger.Log($"Erro no otimizador: {ex.Message}");
                }
            }, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));

            // Loop principal com captura de tecla
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
                await Task.Delay(100);
            }

            Logger.Log("Restaurando todos os processos...");
            RollbackManager.RestoreAllAndClear();
            Watchdog.Stop();
            timer.Dispose();
            Logger.Log("Framework finalizado com sucesso.");
        }
        catch (Exception ex)
        {
            Logger.Log($"Erro fatal no Main: {ex}");
            RollbackManager.RestoreAllAndClear();
        }
        finally
        {
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}