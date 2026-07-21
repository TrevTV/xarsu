Add-Type -AssemblyName System.Windows.Forms

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# --- 1. Select .so or .dll file ---
$dlg1 = New-Object System.Windows.Forms.OpenFileDialog
$dlg1.Title = "Select binary file (.so or .dll)"
$dlg1.Filter = "Binary files (*.so;*.dll)|*.so;*.dll|All files (*.*)|*.*"
$dlg1.InitialDirectory = $scriptDir
if ($dlg1.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    Write-Host "No binary file selected. Exiting."
    exit
}
$binaryFile = $dlg1.FileName

$gamePath = Split-Path -Parent $binaryFile

# --- 2. Select global-metadata.dat ---
$dlg2 = New-Object System.Windows.Forms.OpenFileDialog
$dlg2.Title = "Select global-metadata.dat"
$dlg2.Filter = "Metadata file (global-metadata.dat)|global-metadata.dat|All files (*.*)|*.*"
$dlg2.InitialDirectory = $gamePath
if ($dlg2.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    Write-Host "No metadata file selected. Exiting."
    exit
}
$metadataFile = $dlg2.FileName

# --- 3. Select folder ---
$dlg3 = New-Object System.Windows.Forms.FolderBrowserDialog
$dlg3.Description = "Select the game data folder"
$dlg3.SelectedPath = $gamePath
if ($dlg3.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    Write-Host "No folder selected. Exiting."
    exit
}
$targetFolder = $dlg3.SelectedPath

# --- Run the exe ---
$exeReleasePath = "$scriptDir\..\generator.cli\bin\Release\xarsu.Generator.CLI.exe"
$exeDebugPath = "$scriptDir\..\generator.cli\bin\Debug\xarsu.Generator.CLI.exe"

# find the newest exe file (Release or Debug)
if ((Test-Path $exeReleasePath) -and (Test-Path $exeDebugPath)) {
    $releaseTime = (Get-Item $exeReleasePath).LastWriteTime
    $debugTime = (Get-Item $exeDebugPath).LastWriteTime
    if ($releaseTime -gt $debugTime) {
        $exePath = $exeReleasePath
    } else {
        $exePath = $exeDebugPath
    }
} elseif (Test-Path $exeReleasePath) {
    $exePath = $exeReleasePath
} elseif (Test-Path $exeDebugPath) {
    $exePath = $exeDebugPath
} else {
    Write-Host "No executable found. Exiting."
    exit
}

$argsList = @(
    "-b", $binaryFile,
    "-m", $metadataFile,
    "-d", $targetFolder
)

Write-Host "Running: $exePath $($argsList -join ' ')"
Write-Host "Working directory: $scriptDir"

Start-Process -FilePath $exePath -ArgumentList $argsList -WorkingDirectory $scriptDir -NoNewWindow -Wait