# 🚀 Otimizador de Processos em C#

[![.NET](https://img.shields.io/badge/.NET-6.0%2B-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## 📋 Descrição

O **Otimizador de Processos** é uma biblioteca e aplicação console desenvolvida em C# para resolver problemas de otimização em processos industriais, logísticos ou de negócios. Utilizando algoritmos meta-heurísticos (como **Algoritmo Genético**, **Simulated Annealing** e **Busca Tabu**), a ferramenta permite encontrar soluções aproximadas para problemas complexos de alocação de recursos, sequenciamento de tarefas e minimização de custos/tempo.

Ideal para ambientes onde a otimização manual é inviável e métodos exatos são computacionalmente caros.

---

## ✨ Funcionalidades

- ✅ **Múltiplos algoritmos de otimização**:
  - Algoritmo Genético (GA)
  - Simulated Annealing (SA)
  - Busca Tabu (Tabu Search)
- ✅ **Definição flexível de problemas** através de interfaces (função objetivo, restrições, representação de soluções).
- ✅ **Paralelismo** para avaliação de soluções em multi‑core.
- ✅ **Exportação de resultados** em JSON, CSV e gráficos (via `ScottPlot`).
- ✅ **Configuração via arquivo JSON** (parâmetros do algoritmo, critérios de parada, etc.).
- ✅ **Logging** detalhado do processo de otimização.
- ✅ **Extensível**: adicione seus próprios operadores de crossover, mutação ou vizinhança.

---

## 🛠 Tecnologias

- **.NET 6.0+** (compatível com .NET Core e .NET 5+)
- **C# 10+**
- **Newtonsoft.Json** – para configuração e exportação
- **ScottPlot** – para visualização opcional de convergência
- **xUnit** – testes unitários

---

## 📦 Pré-requisitos

- [.NET SDK 6.0 ou superior](https://dotnet.microsoft.com/download)
- Sistema operacional: Windows, Linux ou macOS

---

## 🔧 Instalação

Clone o repositório:

```bash
git clone https://github.com/seu-usuario/otimizador-processos.git
cd otimizador-processos
