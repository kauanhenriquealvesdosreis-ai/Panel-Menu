# Script PowerShell para limpar e reconstruir o VessieFramework
Write-Host "=== Limpando projeto VessieFramework ===" -ForegroundColor Cyan

# Navegar para o diretório do projeto
$projectDir = "H:\Meu Drive\Process\Panel-Menu"
Set-Location $projectDir

# 1. Matar qualquer processo dotnet em execução
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

# 2. Remover pastas de build antigas
Write-Host "Removendo pastas de build..." -ForegroundColor Yellow
Remove-Item -Recurse -Force "obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "build" -ErrorAction SilentlyContinue

# 3. Verificar e remover pastas antigas fora de src/
Write-Host "Verificando pastas antigas..." -ForegroundColor Yellow

$pastasAntigas = @("Utils", "Optimizer", "Performance", "Core", "Modules", "Patches", "API", "Main")
foreach ($pasta in $pastasAntigas) {
    if (Test-Path $pasta -PathType Container) {
        Write-Host "  Encontrada pasta antiga: $pasta" -ForegroundColor Red
        # NÃO removemos automaticamente para segurança, apenas alertamos
    }
}

# 4. Verificar se existe pasta src/
if (-not (Test-Path "src" -PathType Container)) {
    Write-Host "ERRO: Pasta src/ não encontrada!" -ForegroundColor Red
    Write-Host "Você precisa copiar a estrutura src/ correta para este diretório." -ForegroundColor Red
    exit 1
}

# 5. Limpar cache do .NET
Write-Host "Limpando cache do .NET..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# 6. Executar clean
Write-Host "Executando dotnet clean..." -ForegroundColor Cyan
dotnet clean

# 7. Restaurar pacotes
Write-Host "Restaurando pacotes..." -ForegroundColor Cyan
dotnet restore

# 8. Build final
Write-Host "Compilando projeto..." -ForegroundColor Green
dotnet build --configuration Release

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== BUILD CONCLUÍDO COM SUCESSO ===" -ForegroundColor Green
    Write-Host "Executável em: bin\Release\net8.0\VessieFramework.exe" -ForegroundColor Green
} else {
    Write-Host "`n=== ERRO NO BUILD ===" -ForegroundColor Red
    Write-Host "Verifique se todos os arquivos antigos foram removidos das pastas fora de src/" -ForegroundColor Red
}
