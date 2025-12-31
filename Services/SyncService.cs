using System.IO;
using Microsoft.Maui.Storage;

namespace SilksongSaveSync.Services;

public class SyncService : ISyncService
{
    private readonly string _backupBaseDir;

    public SyncService()
    {
        // Using AppDataDirectory for write permissions assurance
        _backupBaseDir = Path.Combine(FileSystem.Current.AppDataDirectory, "Backups");
    }

    public async Task SyncFoldersAsync(string steamPath, string xboxPath, Action<string> logAction)
    {
        await Task.Run(() =>
        {
            if (!Directory.Exists(steamPath))
            {
                logAction($"[Error] Steam path not found: {steamPath}");
                return;
            }
            if (!Directory.Exists(xboxPath))
            {
                logAction($"[Error] Xbox path not found: {xboxPath}");
                return;
            }

            logAction($"[Info] Starting sync check between:\nSteam: {steamPath}\nXbox: {xboxPath}");

            // Gather all files from both directories
            HashSet<string> steamFiles = [];
            HashSet<string> xboxFiles = [];

            try 
            {
                 steamFiles = Directory.GetFiles(steamPath).Select(Path.GetFileName).Where(x => x != null).ToHashSet()!;
                 xboxFiles = Directory.GetFiles(xboxPath).Select(Path.GetFileName).Where(x => x != null).ToHashSet()!;
            }
            catch (Exception ex)
            {
                logAction($"[Error] Failed to list files: {ex.Message}");
                return;
            }

            var allFiles = steamFiles.Union(xboxFiles);

            bool anyAction = false;

            foreach (var filename in allFiles)
            {
                string steamFileFull = Path.Combine(steamPath, filename);
                string xboxFileFull = Path.Combine(xboxPath, filename);

                bool inSteam = File.Exists(steamFileFull);
                bool inXbox = File.Exists(xboxFileFull);

                try
                {
                    if (inSteam && inXbox)
                    {
                        var steamInfo = new FileInfo(steamFileFull);
                        var xboxInfo = new FileInfo(xboxFileFull);

                        // Compare timestamps (Utc to be safe)
                        // Using a small tolerance if file systems differ in precision, but here we assume strict >
                        if (steamInfo.LastWriteTimeUtc > xboxInfo.LastWriteTimeUtc)
                        {
                            PerformSync(steamFileFull, xboxFileFull, "SteamToXbox", logAction);
                            anyAction = true;
                        }
                        else if (xboxInfo.LastWriteTimeUtc > steamInfo.LastWriteTimeUtc)
                        {
                            PerformSync(xboxFileFull, steamFileFull, "XboxToSteam", logAction);
                            anyAction = true;
                        }
                        else
                        {
                            // Equal, do nothing
                        }
                    }
                    else if (inSteam)
                    {
                        logAction($"[New] Found {filename} in Steam only. Copying to Xbox...");
                        File.Copy(steamFileFull, xboxFileFull);
                        logAction($"[Success] Copied {filename} to Xbox.");
                        anyAction = true;
                    }
                    else if (inXbox)
                    {
                        logAction($"[New] Found {filename} in Xbox only. Copying to Steam...");
                        File.Copy(xboxFileFull, steamFileFull);
                        logAction($"[Success] Copied {filename} to Steam.");
                        anyAction = true;
                    }
                }
                catch (IOException ex)
                {
                    logAction($"[Error] IO Exception syncing {filename}: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    logAction($"[Error] Access Denied syncing {filename}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    logAction($"[Error] Unexpected error syncing {filename}: {ex.Message}");
                }
            }

            if (!anyAction)
            {
                logAction("[Info] All files are up to date.");
            }
        });
    }

    private void PerformSync(string sourceFile, string targetFile, string direction, Action<string> logAction)
    {
        string filename = Path.GetFileName(sourceFile);
        
        // 1. Backup Target
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string backupFolder = Path.Combine(_backupBaseDir, $"{timestamp}_{direction}");
        
        try 
        {
            Directory.CreateDirectory(backupFolder);
            string backupPath = Path.Combine(backupFolder, filename);
            
            logAction($"[Backup] Backing up {filename} to {backupPath}...");
            File.Copy(targetFile, backupPath, overwrite: true);
        }
        catch (Exception ex)
        {
            logAction($"[Error] Backup failed for {filename}: {ex.Message}. Aborting sync for this file.");
            return; // Abort if backup fails
        }

        // 2. Overwrite Target
        logAction($"[Sync] Updating {filename} ({direction})...");
        try
        {
            File.Copy(sourceFile, targetFile, overwrite: true);
            logAction($"[Success] {filename} updated successfully.");
        }
        catch (Exception ex)
        {
            logAction($"[Error] Copy failed for {filename}: {ex.Message}");
        }
    }
}

