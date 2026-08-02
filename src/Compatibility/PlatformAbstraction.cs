using System;
using System.Runtime.InteropServices;

namespace VessieFramework.Compatibility
{
    /// <summary>
    /// Abstração de plataforma para suporte multiplataforma (Windows/Linux)
    /// </summary>
    public enum PlatformType
    {
        Windows,
        Linux,
        MacOS,
        Unknown
    }

    public enum EnvironmentType
    {
        Native,
        GoogleColab,
        Docker,
        VM,
        WSL
    }

    public static class PlatformAbstraction
    {
        public static PlatformType CurrentPlatform { get; private set; }
        public static EnvironmentType CurrentEnvironment { get; private set; }
        public static bool IsWindows => CurrentPlatform == PlatformType.Windows;
        public static bool IsLinux => CurrentPlatform == PlatformType.Linux;
        public static bool IsMacOS => CurrentPlatform == PlatformType.MacOS;
        public static bool IsGoogleColab => CurrentEnvironment == EnvironmentType.GoogleColab;
        public static bool IsDocker => CurrentEnvironment == EnvironmentType.Docker;
        public static bool IsWSL => CurrentEnvironment == EnvironmentType.WSL;

        static PlatformAbstraction()
        {
            DetectPlatform();
            DetectEnvironment();
        }

        private static void DetectPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CurrentPlatform = PlatformType.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CurrentPlatform = PlatformType.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                CurrentPlatform = PlatformType.MacOS;
            }
            else
            {
                CurrentPlatform = PlatformType.Unknown;
            }
        }

        private static void DetectEnvironment()
        {
            // Detectar Google Colab
            if (Environment.GetEnvironmentVariable("COLAB_JUPYTER_IP") != null ||
                Environment.GetEnvironmentVariable("CLOUD_SHELL") == "true" ||
                System.IO.File.Exists("/content/.colab-abort.txt"))
            {
                CurrentEnvironment = EnvironmentType.GoogleColab;
                return;
            }

            // Detectar Docker
            if (System.IO.File.Exists("/.dockerenv") ||
                (System.IO.File.Exists("/proc/1/cgroup") && 
                 System.IO.File.ReadAllText("/proc/1/cgroup").Contains("docker")))
            {
                CurrentEnvironment = EnvironmentType.Docker;
                return;
            }

            // Detectar WSL
            if (IsLinux && 
                (System.IO.File.Exists("/proc/version") && 
                 System.IO.File.ReadAllText("/proc/version").ToLower().Contains("microsoft")))
            {
                CurrentEnvironment = EnvironmentType.WSL;
                return;
            }

            // Detectar VM (simplificado)
            CurrentEnvironment = EnvironmentType.Native;
        }

        public static string GetPlatformName()
        {
            return CurrentPlatform switch
            {
                PlatformType.Windows => "Windows",
                PlatformType.Linux => "Linux",
                PlatformType.MacOS => "macOS",
                _ => "Unknown"
            };
        }

        public static string GetEnvironmentName()
        {
            return CurrentEnvironment switch
            {
                EnvironmentType.Native => "Native",
                EnvironmentType.GoogleColab => "Google Colab",
                EnvironmentType.Docker => "Docker",
                EnvironmentType.VM => "Virtual Machine",
                EnvironmentType.WSL => "WSL",
                _ => "Unknown"
            };
        }

        public static bool SupportsFeature(string feature)
        {
            return feature.ToLower() switch
            {
                "registry" => IsWindows,
                "systemd" => IsLinux && !IsGoogleColab,
                "sudo" => IsLinux || IsMacOS,
                "winapi" => IsWindows,
                "procfs" => IsLinux || IsMacOS,
                "colab_gpu" => IsGoogleColab,
                _ => true
            };
        }
    }
}
