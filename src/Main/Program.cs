using VessieFramework.Main;
using System.Runtime.ExceptionServices;

namespace VessieFramework.Main;

class Program
{
    static async Task Main(string[] args)
    {
        // Captura exceções não tratadas em qualquer thread
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Logger.Log($"💥 Exceção crítica: {e.ExceptionObject}");
            try { RollbackManager.RestoreAllAndClear(); } catch { }
            Environment.Exit(1);
        };

        // Captura exceções em tasks assíncronas
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Logger.Log($"💥 Exceção não observada em Task: {ex}");
            e.SetObserved();
        };

        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║  Vessie Framework - Process Optimizer ║");
        Console.WriteLine("║  + Encoding/Crypto/Serialization      ║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine();

        // Modo de argumento
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "--diagnostics":
                    await RunDiagnosticsMode();
                    return;
                    
                case "--headless":
                    await RunHeadlessMode();
                    return;
                    
                case "--help":
                    PrintHelp();
                    return;
            }
        }

        // Modo interativo padrão
        await RunInteractiveMode();
    }

    static async Task RunInteractiveMode()
    {
        Console.WriteLine("🌐 Dashboard: http://localhost:5000");
        Console.WriteLine("📌 Pressione ESC para restaurar e sair.");
        Console.WriteLine();

        try
        {
            await VessieFrameworkApp.InitializeAsync();

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
                await Task.Delay(100);
            }

            await VessieFrameworkApp.ShutdownAsync();
        }
        catch (Exception ex)
        {
            Logger.Log($"Erro fatal: {ex}");
            RollbackManager.RestoreAllAndClear();
        }
        finally
        {
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }

    static async Task RunHeadlessMode()
    {
        Console.WriteLine("🖥️ Executando em modo headless...");
        Console.WriteLine("Pressione Ctrl+C para parar.");
        
        try
        {
            await VessieFrameworkApp.RunAsync();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n⏹️ Interrompido pelo usuário.");
        }
    }

    static async Task RunDiagnosticsMode()
    {
        Console.WriteLine("🔍 Executando diagnósticos do sistema...\n");
        
        VessieFrameworkApp.RunDiagnostics();
        
        Console.WriteLine("\n✅ Todos os testes completados!");
        await Task.Delay(1000);
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
VessieFramework - Usage:

  vessieframework [options]

Options:
  --diagnostics   Run system diagnostics tests
  --headless      Run in headless mode (no UI)
  --help          Show this help message

Examples:
  vessieframework                 # Interactive mode
  vessieframework --headless      # Headless server mode
  vessieframework --diagnostics   # Run diagnostics
");
    }
}
