using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using VessieFramework.Core;
using VessieFramework.Performance;
using VessieFramework.Models;
using VessieFramework.Native;
using VessieFramework.Utils;

namespace VessieFramework.API;

public static class WebServer
{
    public static async Task StartAsync()
    {
        try
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration["urls"] = "http://localhost:5000";

            var app = builder.Build();
            app.UseStaticFiles();

            // Endpoint da API - usando um método separado para garantir retorno
            app.MapGet("/api/stats", HandleApiRequest);

            Console.WriteLine("🌐 Dashboard disponível em: http://localhost:5000");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erro no servidor web: {ex.Message}");
            if (ex.Message.Contains("address already in use") || ex.Message.Contains("port"))
            {
                Console.WriteLine("Tentando porta 5001...");
                try
                {
                    var builder2 = WebApplication.CreateBuilder();
                    builder2.Configuration["urls"] = "http://localhost:5001";
                    var app2 = builder2.Build();
                    app2.UseStaticFiles();
                    app2.MapGet("/api/stats", HandleApiRequest);
                    Console.WriteLine("🌐 Dashboard em http://localhost:5001");
                    await app2.RunAsync();
                }
                catch { /* ignora */ }
            }
        }
    }

    // Método separado que retorna um objeto explícito
    private static object HandleApiRequest()
    {
        try
        {
            var processes = ProcessManager.GetRunningProcesses();
            var tracked = RollbackManager.GetAllSnapshots();

            var processData = processes
                .Where(p => p.Id > 0 && p.Id != 0)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    Priority = GetPriorityName(p.PriorityClass),
                    p.MemoryUsage,
                    IsTracked = RollbackManager.IsTracked(p.Id),
                    p.AffinityMask
                })
                .Take(30)
                .ToList();

            return new
            {
                Cpu = CpuMonitor.GetTotalUsage(),
                MemoryAvailable = MemoryMonitor.GetAvailableMemoryMB(),
                TrackedCount = tracked.Count,
                Processes = processData,
                Timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
        }
        catch (Exception ex)
        {
            Logger.Log($"Erro na API /stats: {ex.Message}");
            return new { Error = ex.Message };
        }
    }

    private static string GetPriorityName(uint priority)
    {
        return priority switch
        {
            NativeMethods.IDLE_PRIORITY_CLASS => "Idle",
            NativeMethods.BELOW_NORMAL_PRIORITY_CLASS => "BelowNormal",
            NativeMethods.NORMAL_PRIORITY_CLASS => "Normal",
            NativeMethods.ABOVE_NORMAL_PRIORITY_CLASS => "AboveNormal",
            NativeMethods.HIGH_PRIORITY_CLASS => "High",
            NativeMethods.REALTIME_PRIORITY_CLASS => "Realtime",
            _ => "Unknown"
        };
    }
}