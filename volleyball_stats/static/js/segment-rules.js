/**
 * VolleyActionSegmentRules - правила последовательности волейбольных действий
 * Портировано из C# StatisticsCreatorModule/Logic/VolleyActionSegmetRules.cs
 * 
 * Определяет:
 * 1. Какие действия могут следовать после действия с определённым качеством
 * 2. Результат сегмента (Won/Lost/NotEnded) в зависимости от действия и качества
 */

export class VolleyActionSegmentRules {
    constructor() {
        // Правила последовательности: actionType -> [quality 1..6] -> allowed next actions
        this.sequenceLogic = {};
        
        // Правила результата: actionType -> [quality 1..6] -> segment result
        this.winLossLogic = {};
        
        this.setDefaultRules();
    }

    /**
     * Установить правило для действия и качества
     */
    setRuleForActionAndQuality(actionType, qualities, ...allowedNextActions) {
        if (!this.sequenceLogic[actionType]) {
            this.sequenceLogic[actionType] = Array(6).fill(null);
        }
        
        const qualityArray = Array.isArray(qualities) ? qualities : [qualities];
        
        qualityArray.forEach(quality => {
            this.sequenceLogic[actionType][quality - 1] = [...allowedNextActions];
        });
    }

    /**
     * Установить правило результата для действия
     */
    setWinLoseRule(actionType, results) {
        this.winLossLogic[actionType] = results;
    }

    /**
     * Установить правила по умолчанию из C# кода
     */
    setDefaultRules() {
        // SERVE - подача
        this.setRuleForActionAndQuality('Serve', 6, 'Serve'); // Ace - следующая подача
        this.setRuleForActionAndQuality('Serve', [5, 4, 3, 2], 'Block', 'Defence', 'FreeBall', 'Set', 'Attack');
        this.setRuleForActionAndQuality('Serve', 1, 'Reception'); // Ошибка - приём соперника
        
        // RECEPTION - приём
        this.setRuleForActionAndQuality('Reception', [6, 5, 4, 3], 'Set', 'Attack', 'Block', 'Defence', 'FreeBall');
        this.setRuleForActionAndQuality('Reception', 2, 'Block', 'Defence', 'FreeBall', 'Set', 'Attack');
        this.setRuleForActionAndQuality('Reception', 1, 'Reception'); // Ошибка - приём соперника
        
        // SET - передача
        this.setRuleForActionAndQuality('Set', [6, 5, 4, 3, 2], 'Attack', 'Transfer', 'Block', 'Defence', 'FreeBall');
        this.setRuleForActionAndQuality('Set', 1, 'Reception'); // Ошибка
        
        // ATTACK - атака
        this.setRuleForActionAndQuality('Attack', 6, 'Serve'); // Выигрыш - наша подача
        this.setRuleForActionAndQuality('Attack', [5, 4, 3], 'Block', 'Defence', 'FreeBall', 'Set', 'Attack');
        this.setRuleForActionAndQuality('Attack', [2, 1], 'Reception'); // Ошибка или в аут
        
        // BLOCK - блок
        this.setRuleForActionAndQuality('Block', 6, 'Serve'); // Выигрыш
        this.setRuleForActionAndQuality('Block', [5, 4, 3, 2], 'Block', 'Defence', 'FreeBall', 'Set', 'Attack');
        this.setRuleForActionAndQuality('Block', 1, 'Reception'); // Ошибка
        
        // DEFENCE - защита
        this.setRuleForActionAndQuality('Defence', [6, 5, 4, 3, 2], 'Block', 'Defence', 'Set', 'Attack', 'Transfer');
        this.setRuleForActionAndQuality('Defence', 1, 'Reception'); // Ошибка
        
        // FREEBALL - свободный мяч
        this.setRuleForActionAndQuality('FreeBall', [6, 5, 4, 3, 2], 'Block', 'Defence', 'Set', 'Attack', 'Transfer');
        this.setRuleForActionAndQuality('FreeBall', 1, 'Reception'); // Ошибка
        
        // TRANSFER - обводка
        this.setRuleForActionAndQuality('Transfer', [6, 5, 4, 3, 2], 'Block', 'Defence', 'FreeBall', 'Set', 'Attack');
        this.setRuleForActionAndQuality('Transfer', 1, 'Reception'); // Ошибка
        
        // OPPONENT ACTIONS - действия соперника
        this.setRuleForActionAndQuality('OpponentPoint', [1, 2, 3, 4, 5, 6], 'Reception'); // Очко соперника - наш приём
        this.setRuleForActionAndQuality('OpponentError', [1, 2, 3, 4, 5, 6], 'Serve'); // Ошибка соперника - наша подача
        
        // Win/Loss Rules - правила результата сегмента
        this.setDefaultWinLoseLogic();
    }

    /**
     * Установить правила результата по умолчанию
     */
    setDefaultWinLoseLogic() {
        // Результаты: NotEnded = 0, Won = 1, Lost = 2, Disputable = 3
        // Индексы массива: quality 1-6
        
        this.setWinLoseRule('Serve', [2, 0, 0, 0, 0, 1]); // Q1=Lost, Q2-5=NotEnded, Q6=Won
        this.setWinLoseRule('Reception', [2, 0, 0, 0, 0, 0]); // Q1=Lost, остальные NotEnded
        this.setWinLoseRule('Set', [2, 0, 0, 0, 0, 0]); // Q1=Lost, остальные NotEnded
        this.setWinLoseRule('Attack', [2, 2, 0, 0, 0, 1]); // Q1-2=Lost, Q3-5=NotEnded, Q6=Won
        this.setWinLoseRule('Block', [2, 0, 0, 0, 0, 1]); // Q1=Lost, Q2-5=NotEnded, Q6=Won
        this.setWinLoseRule('Defence', [2, 0, 0, 0, 0, 0]); // Q1=Lost, остальные NotEnded
        this.setWinLoseRule('FreeBall', [2, 0, 0, 0, 0, 0]); // Q1=Lost, остальные NotEnded
        this.setWinLoseRule('Transfer', [2, 0, 0, 0, 0, 0]); // Q1=Lost, остальные NotEnded
        this.setWinLoseRule('OpponentError', [1, 1, 1, 1, 1, 1]); // Всегда Won
        this.setWinLoseRule('OpponentPoint', [2, 2, 2, 2, 2, 2]); // Всегда Lost
    }

    /**
     * Получить возможные следующие действия
     * @param {string} actionType - тип действия
     * @param {number} quality - качество (1-6)
     * @returns {Array<string>} массив возможных действий
     */
    getPossibleActions(actionType, quality) {
        if (!this.sequenceLogic[actionType]) {
            return [];
        }
        return this.sequenceLogic[actionType][quality - 1] || [];
    }

    /**
     * Получить возможные действия для соперника
     * @param {string} actionType - тип действия соперника
     * @returns {Array<string>} массив возможных действий
     */
    getPossibleActionsForOpponent(actionType) {
        // Для соперника используем quality = 2 (из C# кода - index 1)
        if (!this.sequenceLogic[actionType]) {
            return [];
        }
        return this.sequenceLogic[actionType][1] || [];
    }

    /**
     * Получить результат действия
     * @param {string} actionType - тип действия
     * @param {number} quality - качество (1-6)
     * @returns {number} результат (0=NotEnded, 1=Won, 2=Lost, 3=Disputable)
     */
    getActionResult(actionType, quality) {
        if (!this.winLossLogic[actionType]) {
            return 0; // NotEnded
        }
        return this.winLossLogic[actionType][quality - 1];
    }

    /**
     * Получить результат для действия соперника
     * @param {string} actionType - тип действия соперника
     * @returns {number} результат (1=Won, 2=Lost)
     */
    getActionResultForOpponent(actionType) {
        if (actionType === 'OpponentError') {
            return 1; // Won
        }
        return 2; // Lost
    }

    /**
     * Получить результат для действия судьи
     * @param {string} actionType - тип действия судьи
     * @returns {number} результат
     */
    getActionResultForJudge(actionType) {
        if (actionType === 'DisputableBall') {
            return 3; // Disputable
        }
        if (actionType === 'JudgeMistakeLost') {
            return 2; // Lost
        }
        return 1; // Won (JudgeMistakeWon)
    }

    /**
     * Получить начальные действия игроков для старта сета
     * @returns {Array<string>} массив действий
     */
    getSetStartPlayersActions() {
        return ['Serve', 'Reception'];
    }

    /**
     * Получить действия приёма
     * @returns {Array<string>} массив действий
     */
    getReceptionPlayersActions() {
        return ['Reception'];
    }
}
