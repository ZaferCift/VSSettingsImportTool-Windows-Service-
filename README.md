This Windows service project is used to import Visual Studio settings from an external file when Visual Studio is closed. The service calls a PowerShell script to look for the .vssettings file in the AppData folder in the user folder and replaces it with the new .vssettings file you specify. If a setting is changed in Visual Studio, the service notices this and overwrites the .vssettings file when Visual Studio is closed.
