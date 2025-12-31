namespace SilksongSaveSync.Services;

public interface ISyncService
{
    Task SyncFoldersAsync(string steamPath, string xboxPath, Action<string> logAction);
}

