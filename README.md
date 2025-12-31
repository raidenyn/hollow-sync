# Silksong Save Sync (S3)

**Silksong Save Sync** is a desktop utility built with **.NET 10 (Preview)** and **.NET MAUI** designed to synchronize "Hollow Knight: Silksong" save files between the Steam version and the Xbox (PC Game Pass) version on Windows.

It ensures that your latest progress is available on both platforms by intelligently comparing file timestamps and syncing the newer save, while automatically creating backups before any overwrite.

## Features

- **Smart Sync**: Automatically detects the newer save file and updates the older one.
- **Safety First**: Creates a timestamped backup of any file before it is overwritten (`AppData/Backups/`).
- **User Control**: Manually select Steam and Xbox save folders.
- **Visual Feedback**: Clear status indicators (Green/Yellow/Red) and a detailed activity log.
- **Modern UI**: Built with WinUI 3 / .NET MAUI.

## Tech Stack

- **Framework**: .NET 10 (Preview)
- **UI**: .NET MAUI (Targeting Windows / WinUI 3)
- **Pattern**: MVVM (CommunityToolkit.Mvvm)

## Prerequisites

- **OS**: Windows 10 (1809) or higher / Windows 11.
- **SDK**: [.NET 10 Preview SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (or latest supported .NET 9+ SDK if retargeting).
- **Workload**: `.NET MAUI` workload installed.

## How to Build & Run

1.  **Clone the repository**.
2.  **Open in Visual Studio 2022 (Preview)** or VS Code.
3.  **Restore dependencies**:
    ```powershell
    dotnet restore
    ```
4.  **Run the application** (Windows only):
    ```powershell
    dotnet run -f net10.0-windows10.0.19041.0
    ```

## Usage

1.  Launch the application.
2.  Click **Browse** to select your **Steam Save Folder**.
3.  Click **Browse** to select your **Xbox Save Folder**.
    *   *Tip: Xbox saves are usually deep inside `%LOCALAPPDATA%\Packages\TeamCherry...\SystemAppData\wgs`*
4.  Click **Sync Now**.
5.  Check the log output for details on which files were updated.

## Development

This project is configured to allow restoration on non-Windows platforms (like macOS) for code editing, but the application **must be run on Windows** due to its WinUI 3 dependency.

- **Project File**: `SilksongSaveSync/SilksongSaveSync.csproj`
- **Context/Rules**: See `SilksongSaveSync/.cursor/rules/context.mdc` for architectural rules.

