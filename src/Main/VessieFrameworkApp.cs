using VessieFramework.Core;
using VessieFramework.Optimizer;
using VessieFramework.API;
using VessieFramework.Systems;
using VessieFramework.Encoding;
using VessieFramework.Crypto;
using VessieFramework.Serialization;
using VessieFramework.Compression;

namespace VessieFramework.Main;

/// <summary>
/// Classe principal do framework com inicialização completa
/// </summary>
public static class VessieFrameworkApp
{
    private static bool _isInitialized = false;
    private static CancellationTokenSource? _cts;

    public static bool IsInitialized => _isInitialized;

    /// <summary>
    /// Inicializa o framework completo
    /// </summary>
    public static async Task InitializeAsync()
    {
        if (_isInitialized) return;

        Logger.Log("🚀 Inicializando VessieFramework...");
        
        // Inicializa sistema de rollback
        RollbackManager.Initialize();
        
        // Inicializa watchdog
        Watchdog.Start();
        
        // Inicializa gerenciador de sistemas
        SystemManager.Initialize();
        
        // Inicia servidor web
        _ = Task.Run(async () => await WebServer.StartAsync());
        
        _isInitialized = true;
        Logger.Log("✅ VessieFramework inicializado com sucesso!");
    }

    /// <summary>
    /// Executa o framework em modo headless
    /// </summary>
    public static async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await InitializeAsync();
        
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        var timer = new Timer(_ =>
        {
            try
            {
                AutoOptimizer.RunAutoOptimization();
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Erro no otimizador: {ex.Message}");
            }
        }, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));

        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(100, _cts.Token);
            }
        }
        finally
        {
            await ShutdownAsync();
            timer.Dispose();
        }
    }

    /// <summary>
    /// Finaliza o framework com rollback seguro
    /// </summary>
    public static async Task ShutdownAsync()
    {
        Logger.Log("🛑 Finalizando VessieFramework...");
        
        _cts?.Cancel();
        
        // Restaura todos os processos
        RollbackManager.RestoreAllAndClear();
        
        // Para watchdog
        Watchdog.Stop();
        
        // Finaliza gerenciador de sistemas
        await SystemManager.ShutdownAsync();
        
        _isInitialized = false;
        Logger.Log("✅ VessieFramework finalizado com segurança.");
    }

    /// <summary>
    /// Aplica um perfil de otimização a um processo
    /// </summary>
    public static void ApplyProfile(int pid, string profileName)
    {
        var profile = Profile.LoadProfile(profileName);
        OptimizationEngine.ApplyProfile(pid, profile);
    }

    /// <summary>
    /// Testa os sistemas de encoding/crypto/serialization/compression
    /// </summary>
    public static void RunDiagnostics()
    {
        Logger.Log("🔍 Executando diagnósticos...");
        
        // Teste Encoding
        var original = "VessieFramework Test";
        var encoded = EncoderFactory.Encode(original, "base64");
        var decoded = EncoderFactory.Decode(encoded, "base64");
        Logger.Log($"Encoding Test: {original} -> {encoded} -> {decoded} ✓");
        
        // Teste Crypto
        var encrypted = CryptoFactory.Encrypt(original, "testkey123", "aes");
        var decrypted = CryptoFactory.Decrypt(encrypted, "testkey123", "aes");
        Logger.Log($"Crypto Test: {original} -> [encrypted] -> {decrypted} ✓");
        
        // Teste Serialization
        var json = SerializerFactory.Serialize(new { Name = "Test", Value = 42 }, "json");
        Logger.Log($"Serialization Test: {json} ✓");
        
        // Teste Compression
        var compressed = CompressorFactory.CompressString(original, "gzip");
        var decompressed = CompressorFactory.DecompressString(compressed, "gzip");
        Logger.Log($"Compression Test: {original} -> [{compressed.Length} chars] -> {decompressed} ✓");
        
        Logger.Log("✅ Diagnósticos completados com sucesso!");
    }
}
