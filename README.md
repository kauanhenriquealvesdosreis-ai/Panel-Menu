# 🚀 VessieFramework

## Framework Avançado de Otimização Dinâmica para Windows

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 📋 Descrição

O **VessieFramework** é uma plataforma avançada de gerenciamento de performance para Windows, focada exclusivamente em **otimização temporária de processos**. 

### Princípios Fundamentais:

- ✅ **100% Reversível** - Todas as otimizações são temporárias e podem ser desfeitas
- ✅ **Sem Alterações Permanentes** - Não modifica Registry, Drivers ou Kernel
- ✅ **Baseado em Snapshots** - Sistema completo de rollback automático
- ✅ **Monitoramento em Tempo Real** - Dashboard web com atualizações via WebSocket
- ✅ **Seguro por Design** - Proteção de processos críticos do sistema

---

## ✨ Funcionalidades Principais

### Core Engine
- Monitoramento em tempo real de CPU, RAM, Threads e Handles
- Análise inteligente de gargalos de performance
- Otimização temporária de processos com rollback automático
- Balanceamento automático de recursos
- Perfis de desempenho pré-configurados
- Sistema de plugins extensível

### Dashboard Web
- Interface moderna com React + Vite + TypeScript
- Gráficos em tempo real com Charts.js
- WebSocket para atualizações instantâneas
- Gerenciamento visual de processos
- Histórico de otimizações e logs

### Sistema de Otimização
- **CPU**: Ajuste de prioridade, controle de afinidade, balanceamento de threads
- **RAM**: Working Set Trim, Memory Pressure Analysis, Cache Optimization
- **Processos**: Boost Mode, Gaming Mode, Low Latency Mode, Safe Mode

### Inteligência Artificial (Recomendações)
- Análise preditiva de uso de recursos
- Sugestões de otimização baseadas em padrões
- Perfis inteligentes que aprendem com o uso

---

## 🏗 Arquitetura

```
VessieFramework/
├── src/
│   ├── Core/              # Framework core, bootstrap, kernel
│   ├── Optimizer/         # Motor de otimização e perfis
│   ├── ProcessEngine/     # Gerenciamento de processos e threads
│   ├── Memory/            # Análise e otimização de memória
│   ├── Performance/       # Monitores de CPU, GPU, RAM, Disk, Network
│   ├── AI/                # Engine de IA para recomendações
│   ├── Modules/           # Módulos especializados
│   ├── Plugins/           # Sistema de plugins
│   └── API/               # ASP.NET Core REST API + SignalR
├── Dashboard/             # Frontend React + Vite
├── Database/              # SQLite para histórico e perfis
├── Profiles/              # Perfis de otimização em JSON
├── Logs/                  # Logs detalhados
└── Config/                # Configurações do framework
```

---

## 🛠 Tecnologias

### Backend (C#/.NET 8)
- **.NET 8** - Runtime principal
- **ASP.NET Core** - Servidor web e API REST
- **SignalR** - Comunicação WebSocket em tempo real
- **Entity Framework Core** - ORM para banco de dados
- **SQLite** - Banco de dados leve
- **System.Diagnostics** - Monitoramento de processos
- **LibreHardwareMonitorLib** - Leitura de hardware
- **Serilog** - Logging estruturado

### Frontend (Dashboard)
- **React 18** - Biblioteca UI
- **Vite** - Build tool ultra-rápido
- **TypeScript** - Type safety
- **Tailwind CSS** - Estilização utilitária
- **Charts.js** - Gráficos em tempo real
- **SignalR Client** - WebSocket client

---

## 🔧 Instalação

### Pré-requisitos
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) (para o dashboard)
- Windows 10/11 (para execução das otimizações)
- Permissões administrativas (opcional, para mais recursos)

### Build do Projeto

```bash
# Build da aplicação .NET
dotnet build -c Release

# Build do dashboard (opcional)
cd Dashboard
npm install
npm run build
```

### Execução

```bash
# Iniciar o framework com dashboard
dotnet run

# Dashboard disponível em: http://localhost:5000
```

---

## 📖 Uso

### API REST

#### Listar Processos
```http
GET /api/process/list
```

#### Otimizar Processo
```http
POST /api/process/optimize
Content-Type: application/json

{
  "pid": 1234,
  "profile": "Gaming"
}
```

#### Restaurar Processo
```http
POST /api/process/restore
Content-Type: application/json

{
  "pid": 1234
}
```

#### Estatísticas em Tempo Real
```http
GET /api/stats
```

### Perfis Disponíveis

| Perfil | Prioridade | Uso Recomendado |
|--------|-----------|-----------------|
| `Gaming` | Above Normal | Jogos, aplicações fullscreen |
| `Balanced` | Normal | Uso geral, workstation |
| `Background` | Below Normal | Processos em segundo plano |
| `HighPerformance` | High | Aplicações críticas |

---

## 🔒 Segurança

### Processos Protegidos (Bloqueados)
- System
- Registry
- Kernel
- CSRSS
- Wininit
- Services

### Recursos de Segurança
- Apenas processos permitidos podem ser otimizados
- Modo simulação para testes
- Logs completos de todas as operações
- Rollback automático em caso de erro
- Captura de exceções não tratadas

---

## 📊 Dashboard Features

### Página Home
- Uso de CPU em tempo real
- Memória disponível
- GPU status
- Score de performance
- Processos ativos otimizados

### Process Manager
- Lista completa de processos
- Ordenação por CPU, RAM, Nome
- Busca rápida
- Aplicar otimização
- Restaurar configurações originais

### Analyzer
- Detecção de gargalos
- Recomendações de otimização
- Histórico de performance

### Profiles
- Criar perfil personalizado
- Salvar configurações
- Aplicar perfil a processos

### Logs
- Histórico completo de operações
- Filtros por data, processo, tipo
- Exportação de logs

---

## 🎯 Exemplo de Otimização

### Antes da Otimização
```
chrome.exe
CPU Priority: Normal
Affinity: All Cores
Working Set: 2.4GB
```

### Depois da Otimização (Gaming Profile)
```
chrome.exe
CPU Priority: Above Normal
Affinity: Performance Cores
Working Set: Optimized
Status: ACTIVE ✅
```

### Rollback Automático
Ao fechar o processo ou o framework:
```
Restore() → Retorna todas as configurações originais
```

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 🤝 Contribuição

Contribuições são bem-vindas! Por favor:

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📞 Suporte

- Documentação completa em `/Docs`
- Issues no GitHub para bugs e features
- Dashboard embutido para monitoramento

---

**VessieFramework** - Performance sob controle, 100% reversível.