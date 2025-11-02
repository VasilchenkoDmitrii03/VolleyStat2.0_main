/**
 * AvailableActionTypes - управление доступными действиями в зависимости от фазы игры
 * Портировано из C# StatisticsCreatorModule/Logic/AvaibleActionTypes.cs
 * 
 * Фазы игры:
 * - GameBeginning: начало игры - только StartArrangement
 * - ArrangementSet: расстановка установлена - только SetStartParams
 * - BetweenRallies: между розыгрышами - можно делать замены, таймауты
 * - InRally: во время розыгрыша - нельзя делать замены, таймауты
 */

// Маппинг типов действий по авторам (из Action.cs)
const ACTION_TYPES_BY_AUTHOR = {
    'Player': ['Serve', 'Reception', 'Set', 'Attack', 'Block', 'Defence', 'FreeBall'],
    'Coach': ['TimeOut', 'Change', 'StartArrangement', 'SetStartParams'],
    'Judge': ['JudgeMistakeWon', 'JudgeMistakeLost', 'DisputableBall'],
    'OpponentTeam': ['OpponentError', 'OpponentPoint']
};

// Количество игроков для действий тренера (из ActionLoader.LoadCoac)
export const COACH_ACTION_PLAYER_COUNTS = {
    'Change': 2,          // Замена - 2 игрока (кто выходит, кто заходит)
    'StartArrangement': 6, // Начальная расстановка - 6 игроков
    'SetStartParams': 1,   // Установить параметры сета - 1 игрок (либеро)
    'TimeOut': 0          // Тайм-аут - 0 игроков
};

export class AvailableActionTypes {
    constructor() {
        // Хранилище доступных действий: authorType -> [actionTypes]
        this.data = {};
    }

    /**
     * Установить все действия по умолчанию
     */
    setDefault() {
        this.data = {};
        this.data['Player'] = [...ACTION_TYPES_BY_AUTHOR['Player']];
        this.data['OpponentTeam'] = [...ACTION_TYPES_BY_AUTHOR['OpponentTeam']];
        this.data['Coach'] = [...ACTION_TYPES_BY_AUTHOR['Coach']];
        this.data['Judge'] = [...ACTION_TYPES_BY_AUTHOR['Judge']];
    }

    /**
     * ФАЗА: Начало игры
     * Доступно только: StartArrangement (тренер)
     */
    gameBeginning() {
        this.data = {};
        this.data['Coach'] = ['StartArrangement'];
    }

    /**
     * ФАЗА: Расстановка установлена
     * Доступно только: SetStartParams (тренер) - установить либеро
     */
    arrangementSet() {
        this.data = {};
        this.data['Coach'] = ['SetStartParams'];
    }

    /**
     * ФАЗА: Между розыгрышами
     * Доступно:
     * - Игроки: определённые действия (Serve/Reception в зависимости от фазы)
     * - Соперник: OpponentError, OpponentPoint
     * - Тренер: Change, TimeOut
     * - Судья: все действия судьи
     * 
     * @param {Array<string>} playersActions - доступные действия игроков
     */
    betweenRallies(playersActions) {
        this.data = {};
        this.data['Player'] = [...playersActions];
        this.data['OpponentTeam'] = [...ACTION_TYPES_BY_AUTHOR['OpponentTeam']];
        this.data['Coach'] = ['Change', 'TimeOut'];
        this.data['Judge'] = [...ACTION_TYPES_BY_AUTHOR['Judge']];
    }

    /**
     * ФАЗА: Во время розыгрыша
     * Доступно:
     * - Игроки: определённые действия (в зависимости от предыдущего действия)
     * - Соперник: OpponentError, OpponentPoint
     * - Судья: все действия судьи
     * НЕ доступно: замены и таймауты тренера
     * 
     * @param {Array<string>} playersActions - доступные действия игроков
     */
    inRally(playersActions) {
        this.data = {};
        this.data['Player'] = [...playersActions];
        this.data['OpponentTeam'] = [...ACTION_TYPES_BY_AUTHOR['OpponentTeam']];
        this.data['Judge'] = [...ACTION_TYPES_BY_AUTHOR['Judge']];
        // Тренера нет - нельзя делать замены/таймауты во время розыгрыша
    }

    /**
     * Очистить все доступные действия
     */
    clear() {
        this.data = {};
    }

    /**
     * Получить доступные действия для автора
     * @param {string} authorType - тип автора
     * @returns {Array<string>} массив доступных действий
     */
    getActionsForAuthor(authorType) {
        return this.data[authorType] || [];
    }

    /**
     * Получить всех доступных авторов
     * @returns {Array<string>} массив типов авторов
     */
    getAuthors() {
        return Object.keys(this.data);
    }

    /**
     * Проверить, доступен ли автор
     * @param {string} authorType - тип автора
     * @returns {boolean}
     */
    isAuthorAvailable(authorType) {
        return authorType in this.data;
    }

    /**
     * Проверить, доступно ли действие для автора
     * @param {string} authorType - тип автора
     * @param {string} actionType - тип действия
     * @returns {boolean}
     */
    isActionAvailable(authorType, actionType) {
        const actions = this.data[authorType];
        return actions ? actions.includes(actionType) : false;
    }

    /**
     * Получить количество игроков для действия тренера
     * @param {string} actionType - тип действия тренера
     * @returns {number} количество игроков
     */
    static getCoachActionPlayerCount(actionType) {
        return COACH_ACTION_PLAYER_COUNTS[actionType] || 0;
    }

    /**
     * Проверить, требует ли действие тренера выбор игроков
     * @param {string} actionType - тип действия тренера
     * @returns {boolean}
     */
    static requiresPlayers(actionType) {
        const count = COACH_ACTION_PLAYER_COUNTS[actionType];
        return count !== undefined && count > 0;
    }

    /**
     * Получить типы действий по автору (статический метод)
     * @param {string} authorType - тип автора
     * @returns {Array<string>} массив действий
     */
    static getActionTypesByAuthor(authorType) {
        return ACTION_TYPES_BY_AUTHOR[authorType] || [];
    }
}

/**
 * Русские переводы для всех действий
 */
export const ACTION_TRANSLATIONS = {
    // Player actions
    'Serve': 'Подача',
    'Reception': 'Приём',
    'Set': 'Передача',
    'Attack': 'Атака',
    'Block': 'Блок',
    'Defence': 'Защита',
    'FreeBall': 'Свободный мяч',
    
    // Coach actions
    'StartArrangement': 'Начальная расстановка',
    'SetStartParams': 'Установить параметры сета',
    'Change': 'Замена',
    'TimeOut': 'Тайм-аут',
    
    // Judge actions
    'JudgeMistakeWon': 'Ошибка судьи в нашу пользу',
    'JudgeMistakeLost': 'Ошибка судьи в пользу соперника',
    'DisputableBall': 'Спорный мяч',
    
    // Opponent actions
    'OpponentError': 'Ошибка соперника',
    'OpponentPoint': 'Очко соперника'
};

/**
 * Русские переводы авторов
 */
export const AUTHOR_TRANSLATIONS = {
    'Player': 'Игрок',
    'Coach': 'Тренер',
    'Judge': 'Судья',
    'OpponentTeam': 'Команда соперника'
};
