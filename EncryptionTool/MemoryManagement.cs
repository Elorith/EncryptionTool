using System;
using System.Runtime.InteropServices;

public static class MemoryManagement
{
    [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory")]
    private static extern bool ZeroMemory(IntPtr destination, int length);

    public static void HandleSensitiveResource(object resource, int resourceLength, bool releaseResource)
    {
        if (!releaseResource)
        {
            return;
        }
        
        GCHandle handle = MemoryManagement.AllocatePinnedGarbageCollectionHandle(resource);
        MemoryManagement.SecurelyReleasePinnedGarbageCollectionHandle(handle, resourceLength);
    }

    private static GCHandle AllocatePinnedGarbageCollectionHandle(object value)
    {
        return GCHandle.Alloc(value, GCHandleType.Pinned);
    }

    private static void SecurelyReleasePinnedGarbageCollectionHandle(GCHandle handle, int length)
    {
        MemoryManagement.ZeroMemory(handle.AddrOfPinnedObject(), length);
        handle.Free();
    } 
}