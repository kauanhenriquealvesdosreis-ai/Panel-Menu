// Função para buscar os dados da API
async function fetchStats() {
    try {
        const response = await fetch('/api/stats');
        if (!response.ok) throw new Error('Erro ao buscar dados');
        const data = await response.json();
        updateUI(data);
    } catch (error) {
        console.error('Erro:', error);
        document.getElementById('last-update').textContent = '⚠️ Erro';
    }
}

// Função para atualizar a interface
function updateUI(data) {
    // Atualiza os cards
    document.getElementById('cpu-usage').textContent = `${Math.round(data.cpu)}%`;
    document.getElementById('memory-available').textContent = `${Math.round(data.memoryAvailable)} MB`;
    document.getElementById('tracked-count').textContent = data.trackedCount;
    document.getElementById('last-update').textContent = data.timestamp;

    // Atualiza a tabela
    const tbody = document.getElementById('process-table');
    if (data.processes.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="loading">Nenhum processo monitorado</td></tr>`;
        return;
    }

    let html = '';
    data.processes.forEach(p => {
        const statusClass = p.isTracked ? 'status-tracked' : 'status-untracked';
        const statusText = p.isTracked ? '✅ Otimizado' : '⏸️ Normal';
        html += `
            <tr>
                <td>${p.id}</td>
                <td><strong>${p.name}</strong></td>
                <td>${p.priority}</td>
                <td>${p.memoryUsage}</td>
                <td class="${statusClass}">${statusText}</td>
            </tr>
        `;
    });
    tbody.innerHTML = html;
}

// Busca a cada 2 segundos
setInterval(fetchStats, 2000);

// Busca imediatamente ao carregar
fetchStats();

// Log para saber que o dashboard está ativo
console.log('📊 Vessie Dashboard conectado!');