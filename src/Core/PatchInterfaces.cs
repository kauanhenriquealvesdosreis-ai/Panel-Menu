using System;

namespace VessieFramework.Core
{
    public interface IPatch
    {
        string Name { get; }
        string Description { get; }
        bool IsActive { get; }
        DateTime AppliedAt { get; }
        
        bool CanApply();
        PatchResult Apply(PatchContext context);
        PatchResult Rollback();
    }

    public class PatchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public TimeSpan? Duration { get; set; }
    }

    public class PatchContext
    {
        public int ProcessId { get; set; }
        public string ProfileName { get; set; } = "";
        public bool DryRun { get; set; }
        public bool Verbose { get; set; }
    }

    public class PatchSnapshot
    {
        public string Key { get; set; } = "";
        public object? Value { get; set; }
        public DateTime SnapshotTime { get; set; } = DateTime.Now;
    }
}
