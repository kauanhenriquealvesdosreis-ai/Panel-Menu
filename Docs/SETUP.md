# 🚀 VessieFramework - Setup Guide

## Visão Geral do Projeto

O VessieFramework é um framework avançado de otimização dinâmica para Windows, focado exclusivamente em otimização temporária de processos.

### Características Principais:
- ✅ 100% Reversível - Todas as otimizações são temporárias
- ✅ Sem alterações permanentes - Não modifica Registry, Drivers ou Kernel
- ✅ Sistema baseado em snapshots com rollback automático
- ✅ Dashboard web em tempo real
- ✅ API REST + WebSocket
- ✅ Proteção de processos críticos do sistema

---

## Estrutura de Diretórios

```
VessieFramework/
│
├── src/                          # Código fonte principal
│   ├── Core/                     # Núcleo do framework
│   │   ├── Framework.cs          # Entry point do core
│   │   ├── Bootstrap.cs          # Inicialização
│   │   ├── Kernel.cs             # Núcleo de operações
│   │   ├── Runtime.cs            # Gerenciamento de runtime
│   │   ├── Scheduler.cs          # Agendador de tarefas
│   │   ├── EventBus.cs           # Sistema de eventos
│   │   ├── DependencyContainer.cs # DI Container
│   │   ├── ServiceLocator.cs     # Service Locator
│   │   ├── Configuration.cs      # Configurações
│   │   ├── Logger.cs             # Logging
│   │   ├── ModuleLoader.cs       # Carregador de módulos
│   │   ├── VersionManager.cs     # Versionamento
│   │   ├── Diagnostics.cs        # Diagnósticos
│   │   ├── Metrics.cs            # Métricas
│   │   ├── Watchdog.cs           # Monitor de processos
│   │   └── CrashRecovery.cs      # Recuperação de crashes
│   │
│   ├── Optimizer/                # Motor de otimização
│   │   ├── OptimizationEngine.cs
│   │   ├── OptimizationPipeline.cs
│   │   ├── OptimizationProfile.cs
│   │   ├── SmartOptimizer.cs
│   │   ├── AutoOptimizer.cs
│   │   ├── BottleneckDetector.cs
│   │   ├── PerformanceAnalyzer.cs
│   │   ├── ResourceBalancer.cs
│   │   ├── PriorityEngine.cs
│   │   ├── ThreadOptimizer.cs
│   │   ├── MemoryOptimizer.cs
│   │   └── RollbackManager.cs
│   │
│   ├── ProcessEngine/            # Gerenciamento de processos
│   │   ├── ProcessManager.cs
│   │   ├── ProcessScanner.cs
│   │   ├── ProcessMonitor.cs
│   │   ├── ThreadManager.cs
│   │   ├── CPUAffinityManager.cs
│   │   ├── PriorityController.cs
│   │   ├── WorkingSetManager.cs
│   │   ├── ProcessSnapshot.cs
│   │   ├── ProcessRestore.cs
│   │   └── ProcessRules.cs
│   │
│   ├── Memory/                   # Otimização de memória
│   │   ├── MemoryAnalyzer.cs
│   │   ├── WorkingSetOptimizer.cs
│   │   ├── CacheAnalyzer.cs
│   │   ├── GarbageCollector.cs
│   │   ├── MemorySnapshot.cs
│   │   └── MemoryBalancer.cs
│   │
│   ├── Performance/              # Monitores de performance
│   │   ├── CPUAnalyzer.cs
│   │   ├── GPUAnalyzer.cs
│   │   ├── RAMAnalyzer.cs
│   │   ├── DiskAnalyzer.cs
│   │   ├── NetworkAnalyzer.cs
│   │   ├── FPSMonitor.cs
│   │   └── LatencyMonitor.cs
│   │
│   ├── AI/                       # Inteligência Artificial
│   │   ├── AIEngine.cs
│   │   ├── PredictionEngine.cs
│   │   ├── UsageAnalyzer.cs
│   │   ├── OptimizationAdvisor.cs
│   │   ├── PatternLearning.cs
│   │   └── SmartProfiles.cs
│   │
│   ├── Modules/                  # Módulos especializados
│   │   ├── CPU/
│   │   ├── GPU/
│   │   ├── RAM/
│   │   ├── Processes/
│   │   ├── Gaming/
│   │   ├── Benchmark/
│   │   └── Diagnostics/
│   │
│   ├── Plugins/                  # Sistema de plugins
│   │   ├── PluginLoader.cs
│   │   ├── PluginInterface.cs
│   │   └── PluginManager.cs
│   │
│   └── API/                      # ASP.NET Core API
│       ├── Controllers/
│       ├── Services/
│       ├── WebSocket/
│       └── Models/
│
├── Dashboard/                    # Frontend React
│   ├── Components/
│   ├── Pages/
│   ├── Charts/
│   ├── Themes/
│   └── Assets/
│
├── Database/                     # SQLite databases
│   ├── Performance.db
│   ├── Profiles.db
│   └── Logs.db
│
├── Profiles/                     # Perfis de otimização
│   ├── Gaming.json
│   ├── Balanced.json
│   ├── Performance.json
│   └── Background.json
│
├── Config/                       # Configurações
├── Logs/                         # Logs da aplicação
├── Benchmarks/                   # Resultados de benchmarks
├── Docs/                         # Documentação
├── wwwroot/                      # Static files (dashboard build)
│
├── Core/                         # Legacy (será migrado para src/)
├── Models/                       # Legacy (será migrado para src/API/Models/)
├── Monitors/                     # Legacy (será migrado para src/Performance/)
├── Native/                       # Legacy (será migrado para src/Core/)
├── Optimizer/                    # Legacy (será migrado para src/Optimizer/)
├── Utils/                        # Legacy (será migrado para src/Core/)
│
├── Program.cs                    # Entry point da aplicação
├── WebServer.cs                  # Servidor web ASP.NET Core
├── VessieFramework.csproj        # Projeto .NET
├── VessieFramework.sln           # Solution file
├── README.md                     # Documentação principal
├── LICENSE                       # Licença MIT
└── build.bat                     # Script de build Windows
```

---

## Migração para Nova Estrutura

A estrutura atual está em processo de migração. Arquivos nas pastas raiz (`Core/`, `Models/`, `Monitors/`, `Native/`, `Optimizer/`, `Utils/`) serão gradualmente movidos para `src/`.

### Status da Migração:
- ✅ `src/Core/` - Arquivos básicos copiados
- ✅ `src/Optimizer/` - Arquivos copiados
- ✅ `src/Performance/` - Monitores copiados
- ✅ `src/API/Models/` - Models copiados
- ⏳ Demais módulos - Em progresso

---

## Tecnologias Utilizadas

### Backend (.NET 8)
| Pacote | Versão | Finalidade |
|--------|--------|------------|
| Microsoft.NET.Sdk | 8.0 | SDK principal |
| Microsoft.AspNetCore.App | 8.0 | Framework web |
| System.Diagnostics.PerformanceCounter | 10.0.10 | Monitoramento de performance |
| LibreHardwareMonitorLib | (pending) | Leitura de hardware |
| Serilog | (pending) | Logging estruturado |
| Microsoft.EntityFrameworkCore.Sqlite | (pending) | Banco de dados |
| Microsoft.AspNetCore.SignalR | (pending) | WebSocket |

### Frontend (Dashboard)
| Tecnologia | Finalidade |
|------------|------------|
| React 18 | UI Library |
| Vite | Build tool |
| TypeScript | Type safety |
| Tailwind CSS | Estilização |
| Charts.js | Gráficos |
| SignalR Client | WebSocket |

---

## Como Contribuir

### 1. Clone o Repositório
```bash
git clone https://github.com/seu-usuario/vessie-framework.git
cd vessie-framework
```

### 2. Instale Dependências
```bash
# Backend
dotnet restore

# Frontend (opcional)
cd Dashboard
npm install
```

### 3. Build
```bash
# Backend
dotnet build -c Release

# Frontend
cd Dashboard
npm run build
```

### 4. Run
```bash
dotnet run
```

Acesse o dashboard em: `http://localhost:5000`

---

## Próximos Passos

### Fase 1 - Estrutura Básica ✅
- [x] Definir arquitetura
- [x] Criar estrutura de diretórios
- [x] Implementar ProcessManager
- [x] Implementar RollbackManager
- [x] Implementar Watchdog
- [x] Dashboard básico funcional

### Fase 2 - Otimização Avançada
- [ ] Implementar BottleneckDetector
- [ ] Implementar SmartOptimizer
- [ ] Adicionar suporte a perfis personalizados
- [ ] Melhorar UI do dashboard

### Fase 3 - IA e Aprendizado
- [ ] Implementar AIEngine
- [ ] PatternLearning
- [ ] OptimizationAdvisor
- [ ] SmartProfiles

### Fase 4 - Plugins e Extensibilidade
- [ ] Sistema de plugins completo
- [ ] API para desenvolvedores
- [ ] Marketplace de plugins

### Fase 5 - Produção
- [ ] Tests unitários
- [ ] Integration tests
- [ ] CI/CD pipeline
- [ ] Documentação completa
- [ ] Installer Windows

---

## Contato e Suporte

- 📧 Email: support@vessieframework.dev
- 💬 Discord: (link pending)
- 📖 Docs: `/Docs` directory
- 🐛 Issues: GitHub Issues

---

**VessieFramework** - Performance sob controle, 100% reversível.
