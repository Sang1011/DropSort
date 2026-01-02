using DropSort.Core.Enums;

namespace Application.Services.Logging;

public static class LogMessageResolver
{
    public static string ResolveMessage(LogEvent evt) =>
        evt switch
        {
            LogEvent.FileDetected =>
                "File detected",

            LogEvent.ProcessingStarted =>
                "Processing started",

            LogEvent.FileMoved =>
                "File moved",

            LogEvent.ProcessingCompleted =>
                "Processing completed",

            LogEvent.RemainingTasks =>
                "Remaining tasks",

            LogEvent.AllTasksCompleted =>
                "All files have been processed",

            LogEvent.FileMoveFailed =>
                "File move failed",

            LogEvent.DiskFull =>
                "Target disk is full",

            LogEvent.WaitingForIdle =>
                "Waiting for system idle",

            LogEvent.Failed =>
                "Operation failed",
            
            LogEvent.SourceFileMissing => 
                "Source file no longer exists, task skipped",

            _ => evt.ToString()
        };
}
