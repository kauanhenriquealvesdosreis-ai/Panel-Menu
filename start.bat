@echo off
setlocal enabledelayedexpansion

echo ============================================
echo   VessieFramework - Clean Build System
echo ============================================
echo.

REM Verificar se .NET SDK está instalado
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] .NET SDK nao encontrado!
    echo Instale o .NET 8 SDK em: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/6] Limpando pastas de build antigas...
if exist "bin" (
    rmdir /s /q "bin"
    echo   - Pasta 'bin' removida
)
if exist "obj" (
    rmdir /s /q "obj"
    echo   - Pasta 'obj' removida
)
if exist "build" (
    rmdir /s /q "build"
    echo   - Pasta 'build' removida
)

echo.
echo [2/6] Procurando arquivos .cs duplicados fora de src/...
set DUPLICATE_COUNT=0
for /r %%f in (*.cs) do (
    set "FILEPATH=%%f"
    set "FILEPATH=!FILEPATH:%CD%\=!"
    if not "!FILEPATH!"=="src" (
        if not "!FILEPATH:~0,4!"=="src\" (
            echo   [ALERTA] Arquivo .cs encontrado fora de src/: !FILEPATH!
            set /a DUPLICATE_COUNT+=1
        )
    )
)

if %DUPLICATE_COUNT% GTR 0 (
    echo.
    echo [3/6] Removendo arquivos .cs duplicados...
    
    REM Remover pastas antigas conhecidas que causam conflito
    if exist "Utils" (
        echo   - Removendo pasta Utils/
        rmdir /s /q "Utils"
    )
    if exist "Core" (
        if not exist "src\Core" (
            echo   - Movendo Core/ para src/Core/
            move "Core" "src\"
        ) else (
            echo   - Removendo Core/ duplicado
            rmdir /s /q "Core"
        )
    )
    if exist "Optimizer" (
        if not exist "src\Optimizer" (
            echo   - Movendo Optimizer/ para src/Optimizer/
            move "Optimizer" "src\"
        ) else (
            echo   - Removendo Optimizer/ duplicado
            rmdir /s /q "Optimizer"
        )
    )
    if exist "Performance" (
        if not exist "src\Performance" (
            echo   - Movendo Performance/ para src/Performance/
            move "Performance" "src\"
        ) else (
            echo   - Removendo Performance/ duplicado
            rmdir /s /q "Performance"
        )
    )
    if exist "Modules" (
        if not exist "src\Modules" (
            echo   - Movendo Modules/ para src/Modules/
            move "Modules" "src\"
        ) else (
            echo   - Removendo Modules/ duplicado
            rmdir /s /q "Modules"
        )
    )
    if exist "API" (
        if not exist "src\API" (
            echo   - Movendo API/ para src/API/
            move "API" "src\"
        ) else (
            echo   - Removendo API/ duplicado
            rmdir /s /q "API"
        )
    )
    if exist "Patches" (
        if not exist "src\Patches" (
            echo   - Movendo Patches/ para src/Patches/
            move "Patches" "src\"
        ) else (
            echo   - Removendo Patches/ duplicado
            rmdir /s /q "Patches"
        )
    )
) else (
    echo   - Nenhum arquivo duplicado encontrado
)

echo.
echo [4/6] Limpando cache do .NET...
dotnet clean --nologo --verbosity quiet

echo.
echo [5/6] Restaurando pacotes NuGet...
dotnet restore --nologo --verbosity quiet

echo.
echo [6/6] Compilando VessieFramework...
dotnet build --configuration Release --nologo --verbosity minimal

if %errorlevel% equ 0 (
    echo.
    echo ============================================
    echo   BUILD CONCLUÍDO COM SUCESSO!
    echo ============================================
    echo.
    echo Para executar:
    echo   dotnet run --configuration Release
    echo   ou
    echo   bin\Release\net8.0\VessieFramework.exe
    echo.
) else (
    echo.
    echo ============================================
    echo   ERRO NO BUILD!
    echo ============================================
    echo.
    echo Verifique os erros acima.
    echo Se persistir, execute manualmente:
    echo   rmdir /s /q bin obj build
    echo   dotnet clean
    echo   dotnet build
    echo.
)

pause
