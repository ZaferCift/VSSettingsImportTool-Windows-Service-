#  This script replaces the .vssettings file you specify 
# with the parameter when running the script with the Visual Studio .vssettings file.
# Sample usage: .\Import-VSSettings.ps1 -vsSettingsPath "C:\Users\wndws\desktop\Exported-2024-07-24.vssettings"

param (
    [string]$vsSettingsPath
)

# User profile path
$userProfile = [System.Environment]::GetFolderPath("UserProfile")

# Visual Studio version (örneğin, 17.0)
$vsVersion = "17.0"

#The root directory where the Visual Studio .vssettings files are located
$vsSettingsRootPath = "$userProfile\AppData\Local\Microsoft\VisualStudio"

# Directories for the specified Visual Studio version
$vsVersionDirectories = Get-ChildItem -Directory -Path $vsSettingsRootPath | Where-Object { $_.Name -like "$vsVersion*" }

# If there is more than one folder for the same version, it takes the first one.
$vsSettingsTargetDirectory = $vsVersionDirectories | Select-Object -First 1

# Checks if the directory is valid
if ($vsSettingsTargetDirectory) {
    
    # Creates a full path by appending the settings directory to the version directory.
    $vsSettingsTargetPath = "$($vsSettingsTargetDirectory.FullName)\Settings\CurrentSettings.vssettings"

    # Checks if the .vssettings file is valid
    if (Test-Path -Path $vsSettingsPath) {
        # Copies the .vssettings file to the destination location with overwriting
        Copy-Item -Path $vsSettingsPath -Destination $vsSettingsTargetPath -Force
        Write-Output "True"
    } else {
        Write-Output "Error: The specified .vssettings file could not be found."
    }
} else {
    Write-Output "Error: No appropriate directory could be found for the specified Visual Studio version."
}
