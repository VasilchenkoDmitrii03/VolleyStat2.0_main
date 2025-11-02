// automatic-fillers.js
// Автоматические связи между действиями и метриками (порт из C# AutomaticFillersRulesHolder.cs)

/**
 * InActionAutomaticFiller - автозаполнение метрики внутри одного действия
 * Если leftMetric имеет определённое значение, то автоматически заполняет rightMetric
 * 
 * Пример: Serve.Quality = "-", "/", "!", "+", "#" => Serve.ErrorType = "NoError"
 */
export class InActionAutomaticFiller {
    constructor(actionType, leftMetric, rightMetric, leftValues, rightValue) {
        this.actionType = actionType;        // VolleyActionType (Serve, Reception, etc.)
        this.leftMetric = leftMetric;        // Название левой метрики (например, "Quality")
        this.rightMetric = rightMetric;      // Название правой метрики (например, "ErrorType")
        this.leftValues = leftValues;        // Массив значений левой метрики ["=", "-", "/"]
        this.rightValue = rightValue;        // Значение для правой метрики ("NoError")
    }

    /**
     * Применить правило к действию
     * @param {Object} action - объект действия с metrics: { metricName: value, ... }
     * @returns {boolean} - true если правило применено
     */
    use(action) {
        // Проверяем тип действия
        if (action.actionType !== this.actionType) {
            return false;
        }

        // Проверяем что правая метрика ещё не заполнена
        if (action.metrics && action.metrics[this.rightMetric] !== undefined && action.metrics[this.rightMetric] !== null) {
            return false;
        }

        // Проверяем что левая метрика заполнена
        if (!action.metrics || action.metrics[this.leftMetric] === undefined || action.metrics[this.leftMetric] === null) {
            return false;
        }

        // Получаем значение левой метрики
        const leftValue = action.metrics[this.leftMetric];
        
        // Проверяем что значение левой метрики входит в список разрешённых
        if (this.leftValues.includes(String(leftValue))) {
            // Устанавливаем значение правой метрики
            if (!action.metrics) {
                action.metrics = {};
            }
            action.metrics[this.rightMetric] = this.rightValue;
            return true;
        }

        return false;
    }

    toString() {
        return `${this.actionType}: ${this.leftMetric} (${this.leftValues.join(', ')}) ==> ${this.rightMetric}(${this.rightValue})`;
    }
}

/**
 * SequenceAutomaticFiller - автозаполнение метрики на основе предыдущего действия в сегменте
 * Копирует или устанавливает значение метрики текущего действия на основе предыдущего
 * 
 * Пример: Reception.Quality => Set.ReceptionQuality (копирование)
 * Пример: Set.Combination => Attack.Combination (копирование)
 */
export class SequenceAutomaticFiller {
    constructor(leftActionType, rightActionType, leftMetric, rightMetric, leftValues = null, rightValue = null, isCopying = false) {
        this.leftActionType = leftActionType;    // Тип предыдущего действия (Reception)
        this.rightActionType = rightActionType;  // Тип текущего действия (Set)
        this.leftMetric = leftMetric;            // Метрика предыдущего действия (Quality)
        this.rightMetric = rightMetric;          // Метрика текущего действия (ReceptionQuality)
        this.leftValues = leftValues;            // Массив значений (или null для копирования)
        this.rightValue = rightValue;            // Значение для установки (или null для копирования)
        this.isCopying = isCopying;              // true = копировать значение, false = установить конкретное
    }

    /**
     * Применить правило к действию на основе сегмента
     * @param {Object} currentAction - текущее действие
     * @param {Array} segment - массив предыдущих действий в сегменте
     * @returns {boolean} - true если правило применено
     */
    use(currentAction, segment) {
        // Проверяем что текущее действие соответствует правилу
        if (currentAction.actionType !== this.rightActionType) {
            return false;
        }

        // Ищем в сегменте предыдущее действие нужного типа
        const previousAction = segment.find(action => action.actionType === this.leftActionType);
        
        if (!previousAction) {
            return false;
        }

        // Проверяем что у предыдущего действия есть нужная метрика
        if (!previousAction.metrics || previousAction.metrics[this.leftMetric] === undefined || previousAction.metrics[this.leftMetric] === null) {
            return false;
        }

        const leftValue = previousAction.metrics[this.leftMetric];

        // Режим копирования
        if (this.isCopying) {
            if (!currentAction.metrics) {
                currentAction.metrics = {};
            }
            currentAction.metrics[this.rightMetric] = leftValue;
            return true;
        }

        // Режим условной установки значения
        if (this.leftValues && this.leftValues.includes(String(leftValue))) {
            if (!currentAction.metrics) {
                currentAction.metrics = {};
            }
            currentAction.metrics[this.rightMetric] = this.rightValue;
            return true;
        }

        return false;
    }

    toString() {
        if (this.isCopying) {
            return `${this.leftActionType}: ${this.leftMetric} ==> ${this.rightActionType}: ${this.rightMetric}`;
        }
        return `${this.leftActionType}: ${this.leftMetric} (${this.leftValues.join(', ')}) ==> ${this.rightActionType}: ${this.rightMetric}(${this.rightValue})`;
    }
}

/**
 * AutomaticFillersRulesHolder - хранилище всех правил автозаполнения
 */
export class AutomaticFillersRulesHolder {
    constructor() {
        this.inActionFillers = [];
        this.sequenceFillers = [];
    }

    /**
     * Добавить правило InAction
     */
    addInActionFiller(filler) {
        this.inActionFillers.push(filler);
    }

    /**
     * Добавить правило Sequence
     */
    addSequenceFiller(filler) {
        this.sequenceFillers.push(filler);
    }

    /**
     * Получить все InAction правила для конкретного действия и метрики
     */
    getInActionFillers(actionType, metricType) {
        return this.inActionFillers.filter(
            filler => filler.actionType === actionType && filler.leftMetric === metricType
        );
    }

    /**
     * Получить все Sequence правила для конкретного действия
     */
    getSequenceFillers(rightActionType) {
        return this.sequenceFillers.filter(
            filler => filler.rightActionType === rightActionType
        );
    }

    /**
     * Применить все InAction правила к действию
     */
    applyInActionFillers(action) {
        let applied = false;
        for (const filler of this.inActionFillers) {
            if (filler.use(action)) {
                applied = true;
                console.log(`✅ InAction rule applied: ${filler.toString()}`);
            }
        }
        return applied;
    }

    /**
     * Применить все Sequence правила к действию на основе сегмента
     */
    applySequenceFillers(action, segment) {
        let applied = false;
        for (const filler of this.sequenceFillers) {
            if (filler.use(action, segment)) {
                applied = true;
                console.log(`✅ Sequence rule applied: ${filler.toString()}`);
            }
        }
        return applied;
    }
}

/**
 * Создать и вернуть экземпляр с правилами Kvadratiki
 */
export function createKvadratikiFillers() {
    const holder = new AutomaticFillersRulesHolder();

    // ===== 7 InAction правил из конфига Kvadratiki =====
    
    // 0. Serve: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Serve',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 1. Reception: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Reception',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 2. Set: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Set',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 3. Attack: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Attack',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 4. Block: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Block',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 5. Defence: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'Defence',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // 6. FreeBall: Quality (-, /, !, +, #) => ErrorType (NoError)
    holder.addInActionFiller(new InActionAutomaticFiller(
        'FreeBall',
        'Quality',
        'ErrorType',
        ['-', '/', '!', '+', '#'],
        'NoError'
    ));

    // ===== 9 Sequence правил из конфига Kvadratiki =====

    // 0. Reception.Quality => Set.ReceptionQuality (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Reception',      // leftActionType
        'Set',            // rightActionType
        'Quality',        // leftMetric
        'ReceptionQuality', // rightMetric
        null,             // leftValues
        null,             // rightValue
        true              // isCopying = true
    ));

    // 1. Set.Quality => Attack.SetQuality (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Set',
        'Attack',
        'Quality',
        'SetQuality',
        null,
        null,
        true
    ));

    // 2. Set.Direction => Attack.position (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Set',
        'Attack',
        'Direction',
        'position',
        null,
        null,
        true
    ));

    // 3. Set.Combination => Attack.Combination (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Set',
        'Attack',
        'Combination',
        'Combination',
        null,
        null,
        true
    ));

    // 4. Set.BlockersCount => Attack.BlockersCount (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Set',
        'Attack',
        'BlockersCount',
        'BlockersCount',
        null,
        null,
        true
    ));

    // 5. Attack.Combination => Block.Combination (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Attack',
        'Block',
        'Combination',
        'Combination',
        null,
        null,
        true
    ));

    // 6. Attack.AttackType => Block.AttackType (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Attack',
        'Block',
        'AttackType',
        'AttackType',
        null,
        null,
        true
    ));

    // 7. Attack.BlockersCount => Block.BlockersCount (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Attack',
        'Block',
        'BlockersCount',
        'BlockersCount',
        null,
        null,
        true
    ));

    // 8. Attack.position => Defence.Direction (копирование)
    holder.addSequenceFiller(new SequenceAutomaticFiller(
        'Attack',
        'Defence',
        'position',
        'Direction',
        null,
        null,
        true
    ));

    console.log(`✅ Kvadratiki automatic fillers loaded: ${holder.inActionFillers.length} InAction, ${holder.sequenceFillers.length} Sequence`);
    
    return holder;
}
