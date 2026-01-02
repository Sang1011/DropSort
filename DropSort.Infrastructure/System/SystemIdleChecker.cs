using System.Diagnostics;
using DropSort.Core.Interfaces;

namespace Infrastructure.System;

public class SystemIdleChecker : ISystemIdleChecker, IDisposable
{
    private readonly PerformanceCounter _cpuCounter;
    private readonly PerformanceCounter _diskCounter;
    private readonly ISettingRepository _settings;

    private const float CpuIdleThreshold = 15f;
    private const float DiskIdleThreshold = 10f;
    private const int DefaultStableSeconds = 7;

    private DateTime? _idleSince;

    public SystemIdleChecker(ISettingRepository settings)
    {
        _settings = settings;

        _cpuCounter = new PerformanceCounter(
            "Processor",
            "% Processor Time",
            "_Total"
        );

        _diskCounter = new PerformanceCounter(
            "PhysicalDisk",
            "% Disk Time",
            "_Total"
        );

        // warm up
        _cpuCounter.NextValue();
        _diskCounter.NextValue();
    }

    public bool IsIdle()
    {
        Thread.Sleep(200);

        var cpu = _cpuCounter.NextValue();
        var disk = _diskCounter.NextValue();

        var now = DateTime.UtcNow;

        if (cpu > CpuIdleThreshold || disk > DiskIdleThreshold)
        {
            _idleSince = null;
            return false;
        }

        if (_idleSince == null)
        {
            _idleSince = now;
            return false;
        }

        var stableSeconds = GetStableSeconds();
        return now - _idleSince >= TimeSpan.FromSeconds(stableSeconds);
    }

    private int GetStableSeconds()
    {
        var raw = _settings.Get("idle_stable_seconds");

        if (int.TryParse(raw, out var seconds) && seconds > 0)
            return seconds;

        return DefaultStableSeconds;
    }

    public void Dispose()
    {
        _cpuCounter.Dispose();
        _diskCounter.Dispose();
    }
}