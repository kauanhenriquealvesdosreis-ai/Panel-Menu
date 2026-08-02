# VessieFramework - Script de Correção Automática

## 🔧 Problema Identificado

Os erros de compilação ocorrem porque existem **arquivos .cs duplicados** fora da pasta `src/` no seu diretório local. O .NET SDK está compilando:
- Arquivos em `src/Core/ProcessManager.cs` ✅
- **E TAMBÉM** arquivos em `Core/ProcessManager.cs` (fora de src) ❌

Isso causa 47 erros de definição duplicada.

---

## ✅ Solução Automática (Windows)

### Opção 1: Usar o script `start.bat` (Recomendado)

```batch
start.bat
```

Este script automaticamente:
1. Limpa pastas `bin/`, `obj/`, `build/`
2. Detecta arquivos `.cs` fora de `src/`
3. Remove pastas duplicadas (`Utils/`, `Core/`, `Optimizer/`, etc.)
4. Executa `dotnet clean`
5. Executa `dotnet restore`
6. Executa `dotnet build`

### Opção 2: Script PowerShell Manual

Execute no PowerShell como **Administrador**:

```powershell
# Navegue até a pasta do projeto
cd "H:\Meu Drive\Process\Panel-Menu"

# 1. Matar qualquer processo dotnet em execução
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# 2. Remover pastas de build
Remove-Item -Recurse -Force bin, obj, build -ErrorAction SilentlyContinue

# 3. Listar arquivos .cs fora de src/
Write-Host "=== Arquivos .cs fora de src/ ==="
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\src\\' } | Select-Object FullName

# 4. Remover pastas duplicadas conhecidas
$foldersToRemove = @('Utils', 'Core', 'Optimizer', 'Performance', 'Modules', 'API', 'Patches', 'Encoding', 'Crypto', 'Serialization', 'Compression')
foreach ($folder in $foldersToRemove) {
    if (Test-Path $folder -PathType Container) {
        Write-Host "Removendo pasta duplicada: $folder"
        Remove-Item -Recurse -Force $folder -ErrorAction SilentlyContinue
    }
}

# 5. Limpar cache do .NET
dotnet clean --verbosity quiet

# 6. Restaurar pacotes
dotnet restore --verbosity quiet

# 7. Compilar
dotnet build --configuration Release

Write-Host "=== Build concluído! ==="
```

### Opção 3: Limpeza Manual Completa

```powershell
cd "H:\Meu Drive\Process\Panel-Menu"

# Parar processos
taskkill /F /IM dotnet.exe 2>$null

# Remover TUDO que não seja src/, Dashboard/, Docs/, Profiles/, Config/, Logs/, Database/, Benchmarks/, wwwroot/
Get-ChildItem -Directory | Where-Object { 
    $_.Name -notin @('src', 'Dashboard', 'Docs', 'Profiles', 'Config', 'Logs', 'Database', 'Benchmarks', 'wwwroot', '.git') 
} | Remove-Item -Recurse -Force

# Remover arquivos .cs soltos na raiz
Get-ChildItem -Filter *.cs | Remove-Item -Force

# Limpar build
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# Build
dotnet clean
dotnet restore
dotnet build --configuration Release
```

---

## 📁 Estrutura Correta do Projeto

Após a limpeza, sua pasta deve ter **APENAS**:

```
Panel-Menu/
├── src/                 ✅ ÚNICA pasta com código .cs
│   ├── Core/
│   ├── API/
│   ├── Modules/
│   ├── Patches/
│   ├── Performance/
│   ├── Optimizer/
│   ├── Encoding/
│   ├── Crypto/
│   ├── Serialization/
│   └── Compression/
├── Dashboard/           ✅ Frontend (se houver)
├── wwwroot/             ✅ Dashboard estático
├── Profiles/            ✅ JSON profiles
├── Config/              ✅ Configurações
├── Logs/                ✅ Logs
├── Database/            ✅ SQLite DBs
├── Benchmarks/          ✅ Benchmarks
├── Docs/                ✅ Documentação
├── VessieFramework.csproj ✅ Project file
├── start.bat            ✅ Script de build
└── README.md            ✅ Documentação
```

**NÃO DEVE TER:**
- ❌ `Utils/` na raiz
- ❌ `Core/` na raiz
- ❌ `Optimizer/` na raiz
- ❌ `Performance/` na raiz
- ❌ Qualquer `.cs` fora de `src/`

---

## 🔍 Verificação Pós-Correção

Após executar o script, verifique:

```powershell
# Deve mostrar APENAS arquivos dentro de src/
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\src\\' }

# Se retornar algo, delete manualmente!
```

---

## 🚀 Como Executar Após Correção

```bash
# Modo normal (com dashboard web)
dotnet run --configuration Release

# Modo headless (sem UI)
dotnet run --configuration Release -- --headless

# Modo diagnóstico
dotnet run --configuration Release -- --diagnostics
```

O dashboard estará disponível em: `http://localhost:5000`

---

## ⚠️ Erros Comuns e Soluções

### Erro: "DllNotFoundException: Unable to load DLL 'kernel32.dll'"
**Solução:** Isso é normal no Linux. O código detecta automaticamente e usa métodos alternativos.

### Erro: "Access denied" ao otimizar processos
**Solução:** Execute como Administrador no Windows.

### Erro: "dotnet: command not found"
**Solução:** Instale o .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

---

## 📞 Suporte

Se os erros persistirem:
1. Delete manualmente as pastas `bin/`, `obj/`, `build/`
2. Delete QUALQUER pasta na raiz que NÃO esteja na lista acima
3. Execute `dotnet clean && dotnet build`
4. Verifique se há arquivos `.cs` soltos na raiz
