using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
using System.Collections.ObjectModel;
using SilksongSaveSync.Services;

namespace SilksongSaveSync.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISyncService _syncService;

    public MainViewModel(ISyncService syncService)
    {
        _syncService = syncService;
        Logs = [];
        StatusColor = Colors.Gray;
    }

    [ObservableProperty]
    private string _steamPath = string.Empty;

    [ObservableProperty]
    private string _xboxPath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _logs;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private Color _statusColor;

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task BrowseSteam()
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful && result.Folder != null)
            {
                SteamPath = result.Folder.Path;
                Log($"Selected Steam Path: {SteamPath}");
            }
        }
        catch (Exception ex)
        {
            Log($"Error picking folder: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task BrowseXbox()
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful && result.Folder != null)
            {
                XboxPath = result.Folder.Path;
                Log($"Selected Xbox Path: {XboxPath}");
            }
        }
        catch (Exception ex)
        {
            Log($"Error picking folder: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Sync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(SteamPath) || string.IsNullOrWhiteSpace(XboxPath))
        {
            Log("Error: Please select both Steam and Xbox save folders.");
            StatusMessage = "Invalid Paths";
            StatusColor = Colors.Red;
            return;
        }

        IsBusy = true;
        StatusMessage = "Processing...";
        StatusColor = Colors.Yellow; // Yellow for processing

        try
        {
            await _syncService.SyncFoldersAsync(SteamPath, XboxPath, Log);
            
            // Check logs or just set to Synced? 
            // The service logs errors, so we can assume if no exception bubbled up it's "Done". 
            // But if specific errors occurred they are in logs.
            StatusMessage = "Synced";
            StatusColor = Colors.Green; 
        }
        catch (Exception ex)
        {
            Log($"Critical Error: {ex.Message}");
            StatusMessage = "Error";
            StatusColor = Colors.Red;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Log(string message)
    {
        if (MainThread.IsMainThread)
        {
            Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            });
        }
    }
}

