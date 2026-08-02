# Script RÁPIDO para corrigir erros de duplicação
# Execute como Administrador no PowerShell

$ErrorActionPreference = "SilentlyContinue"

Write-Host "=== CORREÇÃO RÁPIDA - VessieFramework ===" -ForegroundColor Cyan
Write-Host ""

cd "H:\Meu Drive\Process\Panel-Menu"

# Matar dotnet
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

# Limpar build
Write-Host "[1/4] Limpando pastas de build..." -ForegroundColor Yellow
Remove-Item -Recurse -Force obj, bin, build

# Listar pastas suspeitas
Write-Host "[2/4] Verificando pastas fora de src/..." -ForegroundColor Yellow
$pastasProblema = @()
Get-ChildItem -Directory | ForEach-Object {
    $nome = $_.Name
    if ($nome -notin @("src", "wwwroot", "Profiles", "Database", "Logs", "Config", "Benchmarks", "Docs")) {
        $temCs = Get-ChildItem -Recurse -Filter "*.cs" -Path $_.FullName -ErrorAction SilentlyContinue | Measure-Object | Select-Object -ExpandProperty Count
        if ($temCs -gt 0) {
            $pastasProblema += $nome
            Write-Host "  ⚠️  $nome (contém $temCs arquivos .cs)" -ForegroundColor Red
        }
    }
}

if ($pastasProblema.Count -gt 0) {
    Write-Host ""
    Write-Host "⚠️  ATENÇÃO: Existem pastas com arquivos .cs fora de src/" -ForegroundColor Red
    Write-Host "Estas pastas estão causando os erros de duplicação:" -ForegroundColor Red
    $pastasProblema | ForEach-Object { Write-Host "   - $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Para corrigir, execute:" -ForegroundColor Green
    Write-Host "  Remove-Item -Recurse -Force " + ($pastasProblema -join ", ") -ForegroundColor Green
    Write-Host ""
    
    # Perguntar se quer remover
    $resposta = Read-Host "Deseja remover estas pastas agora? (S/N)"
    if ($resposta -eq "S" -or $resposta -eq "s") {
        foreach ($pasta in $pastasProblema) {
            Write-Host "  Removendo $pasta..." -ForegroundColor Yellow
            Remove-Item -Recurse -Force $pasta
        }
    } else {
        Write-Host "Remoção cancelada. Execute manualmente o comando acima." -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "  ✓ Nenhuma pasta problemática encontrada" -ForegroundColor Green
}

# Limpar NuGet
Write-Host "[3/4] Limpando cache NuGet..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# Build
Write-Host "[4/4] Compilando..." -ForegroundColor Cyan
dotnet clean
dotnet restore
dotnet build --configuration Release

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ BUILD CONCLUÍDO COM SUCESSO!" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ Build falhou. Verifique se ainda há arquivos .cs fora de src/" -ForegroundColor Red
}
