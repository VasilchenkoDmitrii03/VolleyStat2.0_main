/**
 * UIUpdater - обновление UI элементов
 * Портировано из C# ScoreDisplayer и других UI модулей
 */

class UIUpdater {
    constructor(gameState) {
        this.gameState = gameState;
        this.elements = {};
    }

    // Initialize UI elements
    init(elements) {
        this.elements = elements;
    }

    // Update score display
    updateScore() {
        const state = this.gameState.getState();
        
        if (this.elements.scoreLeft) {
            this.elements.scoreLeft.textContent = state.score.left;
        }
        
        if (this.elements.scoreRight) {
            this.elements.scoreRight.textContent = state.score.right;
        }
        
        if (this.elements.setNumber) {
            this.elements.setNumber.textContent = `Set ${state.set}`;
        }
    }

    // Update phase indicator
    updatePhase() {
        const state = this.gameState.getState();
        
        if (this.elements.phaseIndicator) {
            let phaseText = '';
            if (state.phase === 'Break') {
                phaseText = '🏐 Подача';
            } else if (state.phase === 'Recep_1') {
                phaseText = '🛡️ Приём';
            } else if (state.phase === 'Recep') {
                phaseText = '🛡️ Приём (в розыгрыше)';
            }
            
            this.elements.phaseIndicator.textContent = phaseText;
        }
    }

    // Update arrangement display
    updateArrangement() {
        if (!this.elements.arrangementDisplay || !this.gameState.currentArrangement) {
            return;
        }
        
        const arrangement = this.gameState.currentArrangement;
        const arrNum = this.gameState.currentArrangementNumber;
        
        // Display as 6 positions in volleyball formation
        // P4  P3  P2
        // P5  P6  P1
        
        const positions = [
            arrangement[0], // P1 - Front Right
            arrangement[1], // P2 - Front Center
            arrangement[2], // P3 - Front Left
            arrangement[3], // P4 - Back Left
            arrangement[4], // P5 - Back Center
            arrangement[5]  // P6 - Back Right
        ];
        
        let html = '<div class="court-arrangement">';
        html += '<div class="court-row front-row">';
        html += this.createPlayerCell(positions[3], 4); // P4
        html += this.createPlayerCell(positions[2], 3); // P3
        html += this.createPlayerCell(positions[1], 2); // P2
        html += '</div>';
        html += '<div class="court-row back-row">';
        html += this.createPlayerCell(positions[4], 5); // P5
        html += this.createPlayerCell(positions[5], 6); // P6
        html += this.createPlayerCell(positions[0], 1); // P1
        html += '</div>';
        html += '</div>';
        
        this.elements.arrangementDisplay.innerHTML = html;
    }

    // Create player cell HTML
    createPlayerCell(player, position) {
        if (!player) {
            return `<div class="player-cell empty">P${position}</div>`;
        }
        
        const ampluaClass = this.getAmpluaClass(player.amplua);
        const isLibero = player.amplua === 'Libero';
        
        return `
            <div class="player-cell ${ampluaClass} ${isLibero ? 'libero' : ''}">
                <div class="player-number">#${player.number}</div>
                <div class="player-name">${player.name}</div>
                <div class="position-label">P${position}</div>
            </div>
        `;
    }

    // Get CSS class for amplua
    getAmpluaClass(amplua) {
        const ampluaMap = {
            'Setter': 'setter',
            'Libero': 'libero',
            'OutsideHitter': 'outside',
            'MiddleBlocker': 'middle',
            'Opposite': 'opposite'
        };
        return ampluaMap[amplua] || '';
    }

    // Add action to history
    addActionToHistory(action) {
        if (!this.elements.actionHistory) return;
        
        const actionDiv = document.createElement('div');
        actionDiv.className = 'action-item';
        
        // Create action text
        let actionText = '';
        
        if (action.authorType === 'Player' && action.player) {
            actionText = `#${action.player.number} - ${action.actionType}`;
            
            // Add metrics
            const metricTexts = [];
            for (const metricName in action.metrics) {
                const metric = action.metrics[metricName];
                if (metric.shortValue) {
                    metricTexts.push(`${metricName}: ${metric.shortValue}`);
                }
            }
            
            if (metricTexts.length > 0) {
                actionText += ` (${metricTexts.join(', ')})`;
            }
        } else {
            actionText = `${action.authorType} - ${action.actionType}`;
        }
        
        // Add timestamp
        if (action.timeCode) {
            actionText += ` [${this.formatTimeCode(action.timeCode)}]`;
        }
        
        actionDiv.textContent = actionText;
        
        // Add to top of history
        if (this.elements.actionHistory.firstChild) {
            this.elements.actionHistory.insertBefore(actionDiv, this.elements.actionHistory.firstChild);
        } else {
            this.elements.actionHistory.appendChild(actionDiv);
        }
        
        // Limit history length
        while (this.elements.actionHistory.children.length > 50) {
            this.elements.actionHistory.removeChild(this.elements.actionHistory.lastChild);
        }
    }

    // Format time code (seconds to MM:SS)
    formatTimeCode(seconds) {
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${mins}:${secs.toString().padStart(2, '0')}`;
    }

    // Update rally counter
    updateRallyInfo() {
        if (!this.elements.rallyInfo) return;
        
        const currentSet = this.gameState.currentSet;
        if (!currentSet) return;
        
        const rallyCount = currentSet.rallies.length + (this.gameState.isRallyEnded ? 0 : 1);
        this.elements.rallyInfo.textContent = `Rally #${rallyCount}`;
    }

    // Update statistics summary
    updateStatsSummary() {
        if (!this.elements.statsSummary) return;
        
        const stats = this.calculateBasicStats();
        
        let html = '<div class="stats-grid">';
        html += `<div class="stat-item"><span>Total Actions:</span><strong>${stats.totalActions}</strong></div>`;
        html += `<div class="stat-item"><span>Serves:</span><strong>${stats.serves}</strong></div>`;
        html += `<div class="stat-item"><span>Attacks:</span><strong>${stats.attacks}</strong></div>`;
        html += `<div class="stat-item"><span>Blocks:</span><strong>${stats.blocks}</strong></div>`;
        html += '</div>';
        
        this.elements.statsSummary.innerHTML = html;
    }

    // Calculate basic statistics
    calculateBasicStats() {
        const actions = this.gameState.actions;
        
        return {
            totalActions: actions.length,
            serves: actions.filter(a => a.actionType === 'Serve').length,
            attacks: actions.filter(a => a.actionType === 'Attack').length,
            blocks: actions.filter(a => a.actionType === 'Block').length,
            receptions: actions.filter(a => a.actionType === 'Reception').length,
            sets: actions.filter(a => a.actionType === 'Set').length
        };
    }

    // Show notification/message
    showMessage(message, type = 'info') {
        if (!this.elements.messageContainer) return;
        
        const messageDiv = document.createElement('div');
        messageDiv.className = `message message-${type}`;
        messageDiv.textContent = message;
        
        this.elements.messageContainer.appendChild(messageDiv);
        
        // Auto-remove after 3 seconds
        setTimeout(() => {
            messageDiv.remove();
        }, 3000);
    }

    // Show set finished dialog
    showSetFinished(setResult) {
        const message = setResult === 'Won' ? 
            '🎉 Set Won!' : 
            '😔 Set Lost';
        
        this.showMessage(message, setResult === 'Won' ? 'success' : 'error');
        
        if (this.elements.setFinishedCallback) {
            this.elements.setFinishedCallback(setResult);
        }
    }

    // Show game finished dialog
    showGameFinished(wonSets, lostSets) {
        const gameWon = wonSets > lostSets;
        const message = gameWon ? 
            `🏆 Game Won! (${wonSets}:${lostSets})` : 
            `Game Lost (${wonSets}:${lostSets})`;
        
        this.showMessage(message, gameWon ? 'success' : 'error');
        
        if (this.elements.gameFinishedCallback) {
            this.elements.gameFinishedCallback(gameWon);
        }
    }

    // Update all UI elements
    updateAll() {
        this.updateScore();
        this.updatePhase();
        this.updateArrangement();
        this.updateRallyInfo();
        this.updateStatsSummary();
    }

    // Highlight rally end
    highlightRallyEnd(rallyResult) {
        if (!this.elements.scoreDisplay) return;
        
        const color = rallyResult === 'Won' ? '#4caf50' : '#f44336';
        this.elements.scoreDisplay.style.backgroundColor = color;
        this.elements.scoreDisplay.style.transition = 'background-color 0.3s';
        
        setTimeout(() => {
            this.elements.scoreDisplay.style.backgroundColor = '';
        }, 1000);
    }

    // Show error
    showError(message) {
        this.showMessage(message, 'error');
    }

    // Show success
    showSuccess(message) {
        this.showMessage(message, 'success');
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = UIUpdater;
}
