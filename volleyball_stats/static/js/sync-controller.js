/**
 * SyncController - синхронизация текстового интерфейса и кнопок
 * Портировано из C# синхронизации между ActionTextRepresentationControl и ButtonControllerControl
 */

class SyncController {
    constructor(gameState, actionProcessor) {
        this.gameState = gameState;
        this.actionProcessor = actionProcessor;
        
        // Current UI state
        this.currentAction = null;
        this.focusedField = null; // 'author', 'actionType', or metric name
        this.comboBoxes = []; // List of all metric comboboxes
        
        // UI elements (will be set by init)
        this.elements = {};
    }

    // Initialize UI elements
    init(elements) {
        this.elements = elements;
        this.setupEventListeners();
    }

    // Setup event listeners for syncing
    setupEventListeners() {
        // Author ComboBox
        if (this.elements.authorCombo) {
            this.elements.authorCombo.addEventListener('focus', () => {
                this.onFieldFocus('author');
            });
            
            this.elements.authorCombo.addEventListener('change', () => {
                this.onAuthorChanged();
            });
        }

        // Action Type ComboBox
        if (this.elements.actionTypeCombo) {
            this.elements.actionTypeCombo.addEventListener('focus', () => {
                this.onFieldFocus('actionType');
            });
            
            this.elements.actionTypeCombo.addEventListener('change', () => {
                this.onActionTypeChanged();
            });
        }
    }

    // Field got focus - update buttons
    onFieldFocus(fieldName) {
        this.focusedField = fieldName;
        
        if (fieldName === 'author') {
            this.updateButtonsForAuthors();
        } else if (fieldName === 'actionType') {
            this.updateButtonsForActionTypes();
        } else {
            // Metric field
            this.updateButtonsForMetric(fieldName);
        }
    }

    // Update buttons for author selection
    updateButtonsForAuthors() {
        const authors = this.gameState.availableActions.authors;
        
        // Add player numbers if Player is available
        const items = [];
        authors.forEach(author => {
            if (author === 'Player' && this.gameState.currentArrangement) {
                this.gameState.currentArrangement.forEach(player => {
                    if (player) {
                        items.push({
                            type: 'player',
                            value: `#${player.number}`,
                            data: player
                        });
                    }
                });
            } else {
                items.push({
                    type: 'author',
                    value: author,
                    data: author
                });
            }
        });
        
        this.updateButtons(items, 'Select action author');
    }

    // Update buttons for action type selection
    updateButtonsForActionTypes() {
        const author = this.elements.authorCombo?.value;
        if (!author) return;
        
        let actionTypes = [];
        
        if (author.startsWith('#')) {
            // Player selected
            actionTypes = this.gameState.availableActions.playerActions;
        } else {
            // Other author types
            actionTypes = this.getActionTypesForAuthor(author);
        }
        
        const items = actionTypes.map(actionType => ({
            type: 'actionType',
            value: actionType,
            data: actionType
        }));
        
        this.updateButtons(items, 'Select action type');
    }

    // Update buttons for metric selection
    updateButtonsForMetric(metricName) {
        if (!this.currentAction) return;
        
        const values = this.actionProcessor.getMetricValues(metricName);
        const items = values.map(v => ({
            type: 'metricValue',
            value: v.display,
            data: v
        }));
        
        this.updateButtons(items, `Select ${metricName}`);
    }

    // Update button panel
    updateButtons(items, label) {
        if (this.elements.buttonContainer) {
            this.elements.buttonContainer.innerHTML = '';
            
            // Update label
            if (this.elements.buttonLabel) {
                this.elements.buttonLabel.textContent = label;
            }
            
            // Create buttons
            items.forEach((item, index) => {
                const button = document.createElement('button');
                button.className = 'action-button';
                button.textContent = item.value;
                button.onclick = () => this.onButtonClick(index, item);
                this.elements.buttonContainer.appendChild(button);
            });
        }
    }

    // Button clicked - update ComboBox and move to next
    onButtonClick(index, item) {
        if (this.focusedField === 'author') {
            this.elements.authorCombo.value = item.value;
            this.onAuthorChanged();
            
            // Move to action type
            if (this.elements.actionTypeCombo) {
                this.elements.actionTypeCombo.focus();
            }
            
        } else if (this.focusedField === 'actionType') {
            this.elements.actionTypeCombo.value = item.value;
            this.onActionTypeChanged();
            
            // Move to first metric or complete action
            this.focusNextField();
            
        } else {
            // Metric field
            const comboBox = this.elements.metricCombos?.[this.focusedField];
            if (comboBox) {
                comboBox.value = item.value;
                this.onMetricChanged(this.focusedField, item.data);
            }
        }
    }

    // Author changed
    onAuthorChanged() {
        const author = this.elements.authorCombo?.value;
        if (!author) return;
        
        // Update available action types
        this.updateActionTypeComboBox(author);
        
        // Clear action type and metrics
        if (this.elements.actionTypeCombo) {
            this.elements.actionTypeCombo.value = '';
        }
        this.clearMetricCombos();
        this.currentAction = null;
    }

    // Action type changed
    onActionTypeChanged() {
        const author = this.elements.authorCombo?.value;
        const actionType = this.elements.actionTypeCombo?.value;
        
        if (!author || !actionType) return;
        
        // Determine author type
        let authorType = author;
        let player = null;
        
        if (author.startsWith('#')) {
            authorType = 'Player';
            const playerNumber = parseInt(author.substring(1));
            player = this.gameState.currentArrangement.find(p => p && p.number === playerNumber);
        }
        
        // Create new action
        this.currentAction = this.actionProcessor.createAction(authorType, actionType, player);
        
        // Apply sequence fillers from previous action
        this.actionProcessor.processAction(this.currentAction, this.gameState);
        
        // Update metric ComboBoxes
        this.updateMetricCombos();
        
        // Focus first empty metric
        this.focusNextField();
    }

    // Metric changed
    onMetricChanged(metricName, valueData) {
        if (!this.currentAction) return;
        
        // Set metric value
        this.actionProcessor.setMetric(this.currentAction, metricName, valueData.value);
        
        // Apply in-action automatic fillers
        const updated = this.actionProcessor.applyInActionFillers(this.currentAction, metricName);
        
        if (updated) {
            // Update UI to show auto-filled values
            this.updateMetricCombosFromAction();
        }
        
        // Check if action is complete
        if (this.actionProcessor.isActionComplete(this.currentAction)) {
            this.onActionComplete();
        } else {
            // Focus next empty field
            this.focusNextField();
        }
    }

    // Action is complete - add to game
    onActionComplete() {
        if (!this.currentAction) return;
        
        // Add time code if available
        if (this.elements.getCurrentTimeCode) {
            this.currentAction.timeCode = this.elements.getCurrentTimeCode();
        }
        
        // Add points if available
        if (this.elements.getCurrentPoints) {
            this.currentAction.points = this.elements.getCurrentPoints();
        }
        
        // Add action to game state
        const setFinished = this.gameState.addAction(this.currentAction);
        
        // Trigger action added event
        if (this.elements.onActionAdded) {
            this.elements.onActionAdded(this.currentAction, setFinished);
        }
        
        // Reset for next action
        this.resetForNextAction();
    }

    // Reset UI for next action
    resetForNextAction() {
        this.currentAction = null;
        
        // Clear action type
        if (this.elements.actionTypeCombo) {
            this.elements.actionTypeCombo.value = '';
        }
        
        // Clear metrics
        this.clearMetricCombos();
        
        // Update author combo (available authors may have changed)
        this.updateAuthorComboBox();
        
        // Focus author
        if (this.elements.authorCombo) {
            this.elements.authorCombo.focus();
        }
    }

    // Update author ComboBox
    updateAuthorComboBox() {
        if (!this.elements.authorCombo) return;
        
        const authors = this.gameState.availableActions.authors;
        this.elements.authorCombo.innerHTML = '<option value="">Select author...</option>';
        
        authors.forEach(author => {
            if (author === 'Player' && this.gameState.currentArrangement) {
                this.gameState.currentArrangement.forEach(player => {
                    if (player) {
                        const option = document.createElement('option');
                        option.value = `#${player.number}`;
                        option.textContent = `#${player.number} - ${player.name}`;
                        this.elements.authorCombo.appendChild(option);
                    }
                });
            } else {
                const option = document.createElement('option');
                option.value = author;
                option.textContent = author;
                this.elements.authorCombo.appendChild(option);
            }
        });
    }

    // Update action type ComboBox
    updateActionTypeComboBox(author) {
        if (!this.elements.actionTypeCombo) return;
        
        let actionTypes = [];
        
        if (author.startsWith('#')) {
            actionTypes = this.gameState.availableActions.playerActions;
        } else {
            actionTypes = this.getActionTypesForAuthor(author);
        }
        
        this.elements.actionTypeCombo.innerHTML = '<option value="">Select action...</option>';
        actionTypes.forEach(actionType => {
            const option = document.createElement('option');
            option.value = actionType;
            option.textContent = actionType;
            this.elements.actionTypeCombo.appendChild(option);
        });
    }

    // Update metric ComboBoxes
    updateMetricCombos() {
        if (!this.currentAction || !this.elements.metricContainer) return;
        
        // Clear existing
        this.elements.metricContainer.innerHTML = '';
        this.elements.metricCombos = {};
        
        // Create ComboBox for each metric
        const metricsOrder = this.actionProcessor.getMetricsForAction(this.currentAction.actionType);
        
        metricsOrder.forEach(metricName => {
            const metric = this.currentAction.metrics[metricName];
            if (!metric) return;
            
            const metricDef = this.actionProcessor.getMetricDefinition(metricName);
            if (!metricDef) return;
            
            // Create container
            const container = document.createElement('div');
            container.className = 'metric-field';
            
            // Label
            const label = document.createElement('label');
            label.textContent = metricDef.name;
            container.appendChild(label);
            
            // ComboBox
            const combo = document.createElement('select');
            combo.className = 'metric-combo';
            
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = `Select ${metricDef.shortName || metricDef.name}...`;
            combo.appendChild(emptyOption);
            
            // Add values
            metricDef.values.forEach(v => {
                const option = document.createElement('option');
                option.value = v.shortName || v.value;
                option.textContent = v.shortName || v.value;
                combo.appendChild(option);
            });
            
            // Set current value if exists
            if (metric.shortValue) {
                combo.value = metric.shortValue;
            }
            
            // Event listeners
            combo.addEventListener('focus', () => {
                this.onFieldFocus(metricName);
            });
            
            combo.addEventListener('change', () => {
                const selectedOption = combo.options[combo.selectedIndex];
                if (selectedOption && selectedOption.value) {
                    this.onMetricChanged(metricName, {
                        value: metricDef.values[combo.selectedIndex - 1].value,
                        shortName: selectedOption.value
                    });
                }
            });
            
            container.appendChild(combo);
            this.elements.metricContainer.appendChild(container);
            
            // Store reference
            this.elements.metricCombos[metricName] = combo;
        });
    }

    // Update metric ComboBoxes from current action (after auto-fill)
    updateMetricCombosFromAction() {
        if (!this.currentAction || !this.elements.metricCombos) return;
        
        for (const metricName in this.currentAction.metrics) {
            const metric = this.currentAction.metrics[metricName];
            const combo = this.elements.metricCombos[metricName];
            
            if (combo && metric.shortValue) {
                combo.value = metric.shortValue;
            }
        }
    }

    // Clear metric ComboBoxes
    clearMetricCombos() {
        if (this.elements.metricContainer) {
            this.elements.metricContainer.innerHTML = '';
        }
        if (this.elements.metricCombos) {
            this.elements.metricCombos = {};
        }
    }

    // Focus next empty field
    focusNextField() {
        if (!this.currentAction) return;
        
        // Check for next empty metric
        const nextMetric = this.actionProcessor.getNextEmptyMetric(this.currentAction);
        
        if (nextMetric && this.elements.metricCombos?.[nextMetric]) {
            this.elements.metricCombos[nextMetric].focus();
        } else if (this.actionProcessor.isActionComplete(this.currentAction)) {
            // Action complete - trigger completion
            this.onActionComplete();
        }
    }

    // Get action types for non-player authors
    getActionTypesForAuthor(author) {
        const authorActionTypes = {
            'Coach': ['StartArrangement', 'SetStartParams', 'Change', 'TimeOut'],
            'Judge': ['CardGift', 'CardPenalty', 'DisputableBall'],
            'OpponentTeam': ['OpponentError', 'OpponentPoint']
        };
        
        return authorActionTypes[author] || [];
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = SyncController;
}
