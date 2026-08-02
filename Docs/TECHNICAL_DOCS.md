# 📘 VessieFramework - Documentação Técnica

## Visão Geral

O VessieFramework é um sistema de otimização de processos para Windows que opera exclusivamente em modo usuário, sem modificações permanentes no sistema.

---

## Princípios de Design

### 1. Reversibilidade Total
Todas as otimizações aplicadas devem ser completamente reversíveis. O sistema utiliza snapshots para capturar o estado original antes de qualquer modificação.

### 2. Segurança Primeiro
- Processos críticos do sistema são protegidos contra otimização
- Validações rigorosas antes de aplicar qualquer mudança
- Rollback automático em caso de erro

### 3. Transparência
- Logs detalhados de todas as operações
- Dashboard em tempo real mostrando mudanças
- Histórico completo de otimizações

---

## Arquitetura do Sistema

### Camada Core (`src/Core/`)

#### Framework.cs
Ponto de entrada principal do framework, responsável pela inicialização e coordenação de todos os módulos.

#### Bootstrap.cs
Gerencia o processo de bootstrap, carregamento de configurações e inicialização de serviços.

#### Kernel.cs
Núcleo do sistema de otimização, coordenando as operações de baixo nível.

#### Runtime.cs
Gerencia o ciclo de vida da aplicação e estados de execução.

#### Scheduler.cs
Agenda tarefas periódicas de monitoramento e otimização.

#### EventBus.cs
Sistema de eventos para comunicação entre módulos.

#### DependencyContainer.cs
Injeção de dependência para gerenciar serviços.

#### Logger.cs
Logging estruturado com suporte a múltiplos sinks.

#### Watchdog.cs
Monitora processos otimizados e restaura configurações se o processo fechar.

#### CrashRecovery.cs
Recuperação automática em caso de falhas.

---

### Camada Optimizer (`src/Optimizer/`)

#### OptimizationEngine.cs
Motor principal de otimização que aplica perfis a processos.

#### OptimizationPipeline.cs
Pipeline de processamento para otimizações em cadeia.

#### SmartOptimizer.cs
Otimizador inteligente com detecção automática de gargalos.

#### BottleneckDetector.cs
Identifica gargalos de performance em tempo real.

#### PerformanceAnalyzer.cs
Analisa métricas de performance e gera recomendações.

#### ResourceBalancer.cs
Balanceia recursos entre processos concorrentes.

#### PriorityEngine.cs
Gerencia prioridades de processos dinamicamente.

#### ThreadOptimizer.cs
Otimiza distribuição de threads entre núcleos.

#### MemoryOptimizer.cs
Gerencia working set e memória de processos.

#### RollbackManager.cs
**CRÍTICO**: Gerencia snapshots e rollback de todas as otimizações.

---

### Camada ProcessEngine (`src/ProcessEngine/`)

#### ProcessManager.cs
Gerencia operações CRUD em processos.

#### ProcessScanner.cs
Escaneia processos ativos e coleta informações.

#### ProcessMonitor.cs
Monitora métricas de processos em tempo real.

#### ThreadManager.cs
Gerencia threads de processos.

#### CPUAffinityManager.cs
Controla afinidade de CPU para processos.

#### PriorityController.cs
Controla classes de prioridade.

#### WorkingSetManager.cs
Gerencia working set de memória.

#### ProcessSnapshot.cs
Modelo para snapshots de processos.

#### ProcessRestore.cs
Lógica de restauração de processos.

#### ProcessRules.cs
Regras de negócio para validação de processos.

---

### Camada Memory (`src/Memory/`)

#### MemoryAnalyzer.cs
Analisa uso de memória do sistema.

#### WorkingSetOptimizer.cs
Otimiza working set de processos.

#### CacheAnalyzer.cs
Analisa caches de memória.

#### MemorySnapshot.cs
Snapshots de estado de memória.

#### MemoryBalancer.cs
Balanceia memória entre processos.

---

### Camada Performance (`src/Performance/`)

#### CPUAnalyzer.cs
Monitora e analisa uso de CPU.

#### GPUAnalyzer.cs
Monitora uso de GPU (requer bibliotecas externas).

#### RAMAnalyzer.cs
Analisa uso de RAM.

#### DiskAnalyzer.cs
Monitora I/O de disco.

#### NetworkAnalyzer.cs
Analisa tráfego de rede.

#### FPSMonitor.cs
Monitora FPS em jogos (para módulos gaming).

#### LatencyMonitor.cs
Mede latência do sistema.

---

### Camada AI (`src/AI/`)

#### AIEngine.cs
Motor de IA para recomendações.

#### PredictionEngine.cs
Prevê padrões de uso.

#### UsageAnalyzer.cs
Analisa histórico de uso.

#### OptimizationAdvisor.cs
Gera conselhos de otimização.

#### PatternLearning.cs
Aprende padrões de uso do usuário.

#### SmartProfiles.cs
Cria perfis inteligentes baseados em uso.

**Nota**: A IA apenas recomenda, nunca aplica mudanças automaticamente sem confirmação.

---

## API REST

### Endpoints

#### `GET /api/process/list`
Retorna lista de processos ativos.

**Response:**
```json
[
  {
    "id": 1234,
    "name": "chrome.exe",
    "cpu": 5.2,
    "memory": 256,
    "priority": "Normal",
    "isTracked": false
  }
]
```

#### `POST /api/process/optimize`
Aplica otimização a um processo.

**Request:**
```json
{
  "pid": 1234,
  "profile": "Gaming"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Processo otimizado com sucesso",
  "snapshotId": "abc123"
}
```

#### `POST /api/process/restore`
Restaura configurações originais de um processo.

**Request:**
```json
{
  "pid": 1234
}
```

#### `GET /api/stats`
Retorna estatísticas em tempo real do sistema.

**Response:**
```json
{
  "cpu": 45.2,
  "memoryAvailable": 8192,
  "trackedCount": 5,
  "timestamp": "14:30:25"
}
```

#### `GET /api/profiles`
Lista perfis disponíveis.

#### `POST /api/profiles`
Cria novo perfil personalizado.

---

## WebSocket (SignalR)

### Hub: `/ws/realtime`

#### Eventos do Servidor
- `OnStatsUpdate`: Atualização de estatísticas
- `OnProcessOptimized`: Processo foi otimizado
- `OnProcessRestored`: Processo foi restaurado
- `OnWarning`: Alerta do sistema
- `OnError`: Erro ocorreu

#### Eventos do Cliente
- `Subscribe`: Inscreve em atualizações
- `Unsubscribe`: Cancela inscrição
- `OptimizeProcess`: Solicita otimização
- `RestoreProcess`: Solicita restauração

---

## Sistema de Perfis

### Estrutura JSON
```json
{
  "name": "ProfileName",
  "priority": "Normal|AboveNormal|High|BelowNormal|Idle",
  "cpuAffinity": "All|PerformanceCores|EfficiencyCores|Custom",
  "customAffinityMask": 255,
  "memoryMode": "Balanced|Aggressive|Conservative",
  "workingSetMin": 0,
  "workingSetMax": 0,
  "latencyMode": false,
  "description": "Descrição do perfil"
}
```

### Perfis Built-in
1. **Gaming**: Prioridade elevada, foco em baixa latência
2. **Balanced**: Configuração padrão para uso geral
3. **Performance**: Máxima performance para aplicações críticas
4. **Background**: Prioridade reduzida para processos em segundo plano

---

## Segurança

### Lista de Processos Protegidos
Estes processos NUNCA podem ser otimizados:

- `System` (PID 4)
- `smss.exe`
- `csrss.exe`
- `wininit.exe`
- `services.exe`
- `lsass.exe`
- `svchost.exe` (críticos)
- Qualquer processo com nome contendo "registry"

### Validações
1. Verificar se PID é válido
2. Verificar se processo não está na lista protegida
3. Verificar permissões administrativas quando necessário
4. Validar faixa de valores para affinity e working set

---

## Logs

### Formato
```
[HH:mm:ss] [LEVEL] [Module] Message
```

### Exemplo
```
[14:30:25] [INFO] [RollbackManager] Snapshot criado para chrome.exe (PID 1234)
[14:30:26] [INFO] [OptimizationEngine] Perfil Gaming aplicado a chrome.exe
[14:35:00] [INFO] [Watchdog] Processo 1234 encerrou, restaurando configurações
```

### Sinks Suportados
- Console
- Arquivo (rolling)
- SQLite (para histórico)

---

## Banco de Dados

### Tabelas SQLite

#### `PerformanceHistory`
- `Id` (INTEGER PRIMARY KEY)
- `Timestamp` (DATETIME)
- `CpuUsage` (REAL)
- `MemoryUsage` (REAL)
- `ActiveProcesses` (INTEGER)

#### `OptimizationLogs`
- `Id` (INTEGER PRIMARY KEY)
- `Timestamp` (DATETIME)
- `ProcessName` (TEXT)
- `ProcessId` (INTEGER)
- `ProfileApplied` (TEXT)
- `Success` (BOOLEAN)

#### `SavedProfiles`
- `Id` (INTEGER PRIMARY KEY)
- `Name` (TEXT)
- `Configuration` (TEXT JSON)
- `CreatedAt` (DATETIME)

---

## Desenvolvimento de Plugins

### Interface IPlugin
```csharp
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void Initialize();
    void Execute(ProcessInfo process);
    void Shutdown();
}
```

### Registro de Plugin
Plugins devem ser colocados em `/Plugins/` e seguir convenção de nomenclatura.

---

## Build e Deploy

### Requisitos
- .NET SDK 8.0+
- Node.js 18+ (dashboard)
- Windows 10/11 (runtime)

### Comandos
```bash
# Build
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish

# Run
dotnet run
```

---

## Troubleshooting

### Problemas Comuns

#### "Acesso negado"
- Executar como administrador
- Verificar UAC settings

#### "Porta já em uso"
- Alterar porta no appsettings.json
- Verificar se outra instância está rodando

#### Rollback não funciona
- Verificar se processo ainda existe
- Checar logs para detalhes

---

## Contribuição

Ver README.md para diretrizes de contribuição.
