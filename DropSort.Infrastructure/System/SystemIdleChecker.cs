using DropSort.Core.Interfaces;

namespace Infrastructure.System;

public class SystemIdleChecker : ISystemIdleChecker
{
    public bool IsIdle()
    {
        // TODO bước sau: CPU/Disk thật
        return true;
    }
}