# Nombre del proyecto
$projectName = "ConsoleTimer"

# Directorio base para la instalación en el perfil del usuario
$installDir = [System.IO.Path]::Combine($env:USERPROFILE, $projectName)

# Crear directorios necesarios
if (-Not (Test-Path -Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force
}

# Compilar el proyecto
dotnet publish "../ConsoleTimer/ConsoleTimer.csproj" --configuration Debug --output $installDir

# Copiar appsettings.json al directorio de instalación si existe
if (Test-Path -Path "../appsettings.json") {
    Copy-Item -Path "../appsettings.json" -Destination $installDir -Force
}

# Eliminar cualquier alias previo para timer
if (Get-Alias timer -ErrorAction SilentlyContinue) {
    Remove-Item Alias:timer
}

# Eliminar cualquier variable de entorno PATH previa que apunte al script anterior
$envPath = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
$newPath = ($envPath -split ';') -notmatch [Regex]::Escape("$env:USERPROFILE\scripts")
[System.Environment]::SetEnvironmentVariable("Path", ($newPath -join ';'), [System.EnvironmentVariableTarget]::User)

# Crear un script ejecutable en una ubicación accesible desde cualquier carpeta en el perfil del usuario
$scriptContent = 'dotnet "%USERPROFILE%\ConsoleTimer\ConsoleTimer.dll" %*'
$scriptPath = [System.IO.Path]::Combine($env:USERPROFILE, 'scripts\timer.cmd')
if (-Not (Test-Path -Path "$env:USERPROFILE\scripts")) {
    New-Item -ItemType Directory -Path "$env:USERPROFILE\scripts" -Force
}
$scriptContent | Out-File -FilePath $scriptPath -Encoding ASCII

# Agregar el directorio de scripts a la variable de entorno PATH para el usuario actual
$envPath = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
if ($envPath -notlike "*$env:USERPROFILE\scripts*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$env:USERPROFILE\scripts", [System.EnvironmentVariableTarget]::User)
}

Write-Host "Installation completed. You can now use the 'timer' command from any location. Please restart your terminal for the changes to take effect."
