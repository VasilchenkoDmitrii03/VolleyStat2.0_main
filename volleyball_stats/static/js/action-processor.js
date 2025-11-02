/**
 * ActionProcessor - обработка действий и автоматическое заполнение
 * Портировано из C# PlayerActionTextRepresentation + AutomaticFillers
 */

class ActionProcessor {
    constructor(config) {
        this.config = config;
        this.inActionRules = config.inActionRules || [];
        this.sequenceRules = config.sequenceRules || [];
    }

    // Create empty action from action type
    createAction(authorType, actionType, player = null) {
        const action = {
            authorType: authorType,
            actionType: actionType,
            player: player,
            metrics: {},
            timeCode: 0,
            points: []
        };

        // Get metrics for this action type
        const actionMetrics = this.getMetricsForAction(actionType);
        
        // Initialize metrics
        actionMetrics.forEach(metricName => {
            const metricType = this.getMetricType(metricName);
            if (metricType) {
                action.metrics[metricName] = {
                    name: metricName,
                    value: null,
                    shortValue: null
                };
            }
        });

        return action;
    }

    // Get metrics for action type
    getMetricsForAction(actionType) {
        if (!this.config.actionMetrics) return [];
        return this.config.actionMetrics[actionType] || [];
    }

    // Get metric type definition
    getMetricType(metricName) {
        if (!this.config.metricTypes) return null;
        return this.config.metricTypes.find(m => m.name === metricName);
    }

    // Set metric value
    setMetric(action, metricName, value) {
        if (!action.metrics[metricName]) return false;

        const metricType = this.getMetricType(metricName);
        if (!metricType) return false;

        // Find the value definition
        const valueDef = metricType.values.find(v => 
            v.value === value || v.shortName === value
        );

        if (!valueDef) return false;

        action.metrics[metricName].value = valueDef.value;
        action.metrics[metricName].shortValue = valueDef.shortName;

        return true;
    }

    // Apply in-action automatic fillers
    applyInActionFillers(action, changedMetric) {
        let updated = false;

        // Find applicable rules for this action
        const rules = this.inActionRules.filter(rule => 
            rule.actionType === action.actionType &&
            rule.leftMetric === changedMetric
        );

        for (const rule of rules) {
            const leftValue = action.metrics[rule.leftMetric]?.value;
            
            // Check if left value matches rule conditions
            if (rule.leftValues.includes(leftValue)) {
                // Set right metric
                this.setMetric(action, rule.rightMetric, rule.rightValue);
                updated = true;
            }
        }

        return updated;
    }

    // Apply sequence automatic fillers (from previous action)
    applySequenceFillers(action, previousAction) {
        if (!previousAction) return false;

        let updated = false;

        // Find applicable rules
        const rules = this.sequenceRules.filter(rule =>
            rule.leftAction === previousAction.actionType &&
            rule.rightAction === action.actionType
        );

        for (const rule of rules) {
            if (rule.copyMode) {
                // Copy mode: copy value from left metric to right metric
                const leftValue = previousAction.metrics[rule.leftMetric]?.value;
                if (leftValue !== null && leftValue !== undefined) {
                    this.setMetric(action, rule.rightMetric, leftValue);
                    updated = true;
                }
            } else {
                // Conditional mode: if left metric has certain value, set right metric
                const leftValue = previousAction.metrics[rule.leftMetric]?.value;
                
                if (rule.leftValues && rule.leftValues.includes(leftValue)) {
                    this.setMetric(action, rule.rightMetric, rule.rightValue);
                    updated = true;
                }
            }
        }

        return updated;
    }

    // Fill automatic position metrics (FieldPosition, ArrangementPosition)
    fillPositionMetrics(action, gameState) {
        if (!action.player || !gameState.currentArrangement) return false;

        let updated = false;

        // Field Position (1-6 based on current zone)
        if (action.metrics['FieldPosition']) {
            const zone = gameState.getPlayerZone(action.player.number);
            if (zone !== -1) {
                this.setMetric(action, 'FieldPosition', zone + 1);
                updated = true;
            }
        }

        // Arrangement Position (based on starting arrangement)
        if (action.metrics['ArrangementPosition']) {
            // This would need position data container from settings
            // For now, skip or use simplified version
        }

        return updated;
    }

    // Check if action is complete (all required metrics filled)
    isActionComplete(action) {
        // Coach and Judge actions may not need metrics
        if (action.authorType === 'Coach' || action.authorType === 'Judge') {
            return true;
        }

        // Opponent actions need basic validation
        if (action.authorType === 'OpponentTeam') {
            return true;
        }

        // Player actions must have all metrics filled
        for (const metricName in action.metrics) {
            const metric = action.metrics[metricName];
            if (metric.value === null || metric.value === undefined) {
                return false;
            }
        }

        return true;
    }

    // Get next empty metric in action
    getNextEmptyMetric(action) {
        for (const metricName in action.metrics) {
            const metric = action.metrics[metricName];
            if (metric.value === null || metric.value === undefined) {
                return metricName;
            }
        }
        return null;
    }

    // Validate metric value
    isValidMetricValue(metricName, value) {
        const metricType = this.getMetricType(metricName);
        if (!metricType) return false;

        return metricType.values.some(v => 
            v.value === value || v.shortName === value
        );
    }

    // Get metric values (for populating dropdowns/buttons)
    getMetricValues(metricName) {
        const metricType = this.getMetricType(metricName);
        if (!metricType) return [];

        return metricType.values.map(v => ({
            value: v.value,
            shortName: v.shortName,
            display: v.shortName || v.value
        }));
    }

    // Process complete action (apply all fillers)
    processAction(action, gameState) {
        // Apply sequence fillers from previous action
        if (gameState.currentSegment.length > 0) {
            const previousAction = gameState.currentSegment[gameState.currentSegment.length - 1];
            this.applySequenceFillers(action, previousAction);
        }

        // Fill position metrics
        this.fillPositionMetrics(action, gameState);

        return action;
    }

    // Get metric definition with all details
    getMetricDefinition(metricName) {
        return this.getMetricType(metricName);
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ActionProcessor;
}
