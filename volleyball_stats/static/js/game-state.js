/**
 * GameState Manager - управление состоянием игры
 * Портировано из C# TextStatisticsModule
 */

class GameState {
    constructor() {
        this.reset();
    }

    reset() {
        // Game data
        this.team = null;
        this.config = null;
        this.opponent = null;
        this.gameInfo = {};
        
        // Current game state
        this.currentSet = null;
        this.currentRally = null;
        this.currentSegment = [];
        this.sets = [];
        this.actions = [];
        
        // Score
        this.score = { left: 0, right: 0 };
        this.setLength = 25;
        
        // Phase tracking
        this.phase = 'Break'; // Break, Recep_1, Recep
        this.isRallyEnded = true;
        this.isSetStart = false;
        
        // Rotation
        this.currentArrangement = null; // Array of 6 players
        this.currentArrangementNumber = -1; // 0-5
        
        // Available actions
        this.availableActions = {
            authors: ['Coach'], // Coach, Player, Judge, OpponentTeam
            playerActions: [] // Available for Player
        };
    }

    // Initialize new game
    initializeGame(gameSetup) {
        this.reset();
        this.team = gameSetup.team;
        this.config = gameSetup.config;
        this.opponent = gameSetup.opponent;
        this.gameInfo = {
            date: gameSetup.date,
            time: gameSetup.time,
            location: gameSetup.location,
            videoUrl: gameSetup.videoUrl,
            notes: gameSetup.notes,
            maxSets: gameSetup.maxSets
        };
        this.setLength = gameSetup.setLength;
        
        // Begin first set
        this.beginNewSet();
    }

    beginNewSet() {
        this.currentSet = {
            number: this.sets.length + 1,
            score: { left: 0, right: 0 },
            rallies: [],
            result: 'NotFinished', // Won, Lost, NotFinished
            length: this.setLength
        };
        
        this.currentRally = {
            segments: [],
            result: 'Undefined' // Won, Lost, Disputable, Undefined
        };
        
        this.currentSegment = [];
        this.score = { left: 0, right: 0 };
        this.isRallyEnded = true;
        this.isSetStart = false;
        
        // At game beginning, only Coach can set arrangement
        this.availableActions.authors = ['Coach'];
        this.availableActions.playerActions = [];
    }

    // Set starting arrangement (6 players in positions P1-P6)
    setStartingArrangement(players) {
        if (!players || players.length !== 6) {
            throw new Error('Starting arrangement must have exactly 6 players');
        }
        
        this.currentArrangement = [...players];
        this.currentArrangementNumber = 0; // Will be set by SetStartParams action
        this.isSetStart = true;
        
        // After arrangement set, allow normal game actions
        this.updateAvailableActionsForGameStart();
    }

    // Set starting parameters (who serves first - by player zone)
    setStartingParams(playerZone) {
        this.currentArrangementNumber = playerZone;
        this.phase = 'Recep_1';
        this.updateAvailableActionsBetweenRallies();
    }

    // Rotate players clockwise
    rotate() {
        if (!this.currentArrangement) return;
        
        // Rotate: P1→P6, P6→P5, P5→P4, P4→P3, P3→P2, P2→P1
        const last = this.currentArrangement[5];
        for (let i = 5; i > 0; i--) {
            this.currentArrangement[i] = this.currentArrangement[i - 1];
        }
        this.currentArrangement[0] = last;
        
        // Update arrangement number
        this.currentArrangementNumber = (this.currentArrangementNumber - 1 + 6) % 6;
    }

    // Get player at specific zone (0-5)
    getPlayerAtZone(zone) {
        if (!this.currentArrangement) return null;
        return this.currentArrangement[zone];
    }

    // Get player zone by player number
    getPlayerZone(playerNumber) {
        if (!this.currentArrangement) return -1;
        return this.currentArrangement.findIndex(p => p && p.number === playerNumber);
    }

    // Add action to current segment
    addAction(action) {
        this.actions.push(action);
        this.currentSegment.push(action);
        this.isRallyEnded = false;
        
        // Check if segment should be added to rally and new one started
        if (this.isNewSegment(action)) {
            this.currentRally.segments.push([...this.currentSegment]);
            this.currentSegment = [action];
        }
        
        // Check if rally is finished
        const rallyResult = this.checkRallyFinished();
        if (rallyResult !== 'NotEnded') {
            this.finishRally(rallyResult);
            return true;
        }
        
        // Update available actions for next input
        this.updateAvailableActionsInRally();
        return false;
    }

    // Check if new segment should be started
    isNewSegment(action) {
        if (this.currentSegment.length === 0) return false;
        if (action.authorType === 'Judge') return false;
        
        const lastAction = this.currentSegment[this.currentSegment.length - 2]; // -2 because action already added
        if (!lastAction) return false;
        
        // New segment if:
        // 1. Same action type appears again (except Defence)
        if (this.currentSegment.filter(a => a.actionType === action.actionType).length > 1 && 
            action.actionType !== 'Defence') {
            return true;
        }
        
        // 2. Block with quality 5 (точный блок)
        if (action.actionType === 'Block' && action.metrics?.Quality?.value === 5) {
            return true;
        }
        
        // 3. After Attack
        if (lastAction.actionType === 'Attack') {
            return true;
        }
        
        // 4. Segment too long (>4 actions)
        if (this.currentSegment.length > 4) {
            return true;
        }
        
        // 5. Different author
        if (lastAction.authorType !== action.authorType) {
            return true;
        }
        
        // 6. Block or Defence after any action
        if (action.actionType === 'Block' || action.actionType === 'Defence') {
            return true;
        }
        
        return false;
    }

    // Check if rally is finished and return result
    checkRallyFinished() {
        if (this.currentSegment.length === 0) return 'NotEnded';
        
        const lastAction = this.currentSegment[this.currentSegment.length - 1];
        
        if (lastAction.authorType === 'Player') {
            return this.getActionResult(lastAction.actionType, lastAction.metrics?.Quality?.value);
        } else if (lastAction.authorType === 'OpponentTeam') {
            return this.getActionResultForOpponent(lastAction.actionType);
        } else if (lastAction.authorType === 'Judge') {
            return this.getActionResultForJudge(lastAction.actionType);
        }
        
        return 'NotEnded';
    }

    // Get action result based on action type and quality
    getActionResult(actionType, quality) {
        // This uses SegmentRules logic
        const rulesMap = {
            'Serve': { 1: 'Lost', 5: 'Won', 6: 'Won' },
            'Reception': {},
            'Set': {},
            'Attack': { 1: 'Lost', 5: 'Won', 6: 'Won' },
            'Block': { 1: 'Lost', 5: 'Won' },
            'Defence': {},
            'FreeBall': { 1: 'Lost' }
        };
        
        const actionRules = rulesMap[actionType];
        if (actionRules && actionRules[quality]) {
            return actionRules[quality];
        }
        
        return 'NotEnded';
    }

    getActionResultForOpponent(actionType) {
        if (actionType === 'OpponentError') return 'Won';
        if (actionType === 'OpponentPoint') return 'Lost';
        return 'NotEnded';
    }

    getActionResultForJudge(actionType) {
        if (actionType === 'CardGift') return 'Won';
        if (actionType === 'CardPenalty') return 'Lost';
        return 'NotEnded';
    }

    // Finish current rally
    finishRally(result) {
        // Add current segment to rally
        this.currentRally.segments.push([...this.currentSegment]);
        this.currentRally.result = result;
        
        // Update score
        if (result === 'Won') {
            this.score.left++;
            this.currentSet.score.left++;
        } else if (result === 'Lost') {
            this.score.right++;
            this.currentSet.score.right++;
        }
        
        // Add rally to set
        this.currentSet.rallies.push({ ...this.currentRally });
        
        // Check if rotation needed
        if (this.isRotationNeeded(result)) {
            this.rotate();
        }
        
        // Update phase
        this.phase = result === 'Won' ? 'Break' : 'Recep_1';
        
        // Check if set finished
        const setResult = this.checkSetFinished();
        if (setResult !== 'NotFinished') {
            this.finishSet(setResult);
            return true;
        }
        
        // Reset for next rally
        this.currentRally = {
            segments: [],
            result: 'Undefined'
        };
        this.currentSegment = [];
        this.isRallyEnded = true;
        
        // Update available actions
        this.updateAvailableActionsBetweenRallies();
        
        return false;
    }

    // Check if rotation is needed
    isRotationNeeded(rallyResult) {
        // Rotate if won point while receiving
        return this.phase === 'Recep_1' && rallyResult === 'Won';
    }

    // Check if set is finished
    checkSetFinished() {
        const left = this.score.left;
        const right = this.score.right;
        const target = this.setLength;
        
        // Standard win: reach target with 2+ point lead
        if (left >= target && left - right >= 2) return 'Won';
        if (right >= target && right - left >= 2) return 'Lost';
        
        return 'NotFinished';
    }

    // Finish current set
    finishSet(result) {
        this.currentSet.result = result;
        this.sets.push({ ...this.currentSet });
        
        // Check if game finished
        const wonSets = this.sets.filter(s => s.result === 'Won').length;
        const lostSets = this.sets.filter(s => s.result === 'Lost').length;
        const maxSets = this.gameInfo.maxSets || 5;
        const setsToWin = Math.ceil(maxSets / 2);
        
        if (wonSets >= setsToWin || lostSets >= setsToWin) {
            // Game finished!
            return true;
        }
        
        // Start next set
        this.beginNewSet();
        return false;
    }

    // Update available actions based on game state
    updateAvailableActionsForGameStart() {
        this.availableActions.authors = ['Coach'];
        this.availableActions.playerActions = [];
    }

    updateAvailableActionsBetweenRallies() {
        this.availableActions.authors = ['Player', 'Coach', 'Judge', 'OpponentTeam'];
        
        if (this.phase === 'Break') {
            // We serve
            this.availableActions.playerActions = ['Serve'];
        } else {
            // We receive
            this.availableActions.playerActions = ['Reception'];
        }
    }

    updateAvailableActionsInRally() {
        this.availableActions.authors = ['Player', 'Judge', 'OpponentTeam'];
        
        if (this.currentSegment.length === 0) {
            this.updateAvailableActionsBetweenRallies();
            return;
        }
        
        const lastAction = this.currentSegment[this.currentSegment.length - 1];
        
        if (lastAction.authorType === 'Player') {
            const quality = lastAction.metrics?.Quality?.value;
            this.availableActions.playerActions = this.getPossibleActionsAfter(
                lastAction.actionType, 
                quality
            );
        }
    }

    // Get possible actions based on previous action and quality
    getPossibleActionsAfter(actionType, quality) {
        // Simplified rules - full rules would come from SegmentRules
        const rulesMap = {
            'Serve': {
                1: [], 5: [], 6: [], // Rally ends
                2: ['Reception', 'Block', 'Defence'],
                3: ['Reception', 'Block', 'Defence'],
                4: ['Reception', 'Block', 'Defence']
            },
            'Reception': {
                1: [], // Error - rally ends
                2: ['Set', 'Attack'],
                3: ['Set', 'Attack'],
                4: ['Set', 'Attack'],
                5: ['Set', 'Attack'],
                6: ['Set', 'Attack']
            },
            'Set': {
                1: [], // Error - rally ends
                2: ['Attack'],
                3: ['Attack'],
                4: ['Attack'],
                5: ['Attack'],
                6: ['Attack']
            },
            'Attack': {
                1: [], 5: [], 6: [], // Rally ends
                2: ['Block', 'Defence'],
                3: ['Block', 'Defence'],
                4: ['Block', 'Defence']
            },
            'Block': {
                1: [], 5: [], // Rally ends
                2: ['Defence', 'Set', 'Attack'],
                3: ['Defence', 'Set', 'Attack'],
                4: ['Defence', 'Set', 'Attack']
            },
            'Defence': {
                1: [], // Error - rally ends
                2: ['Set', 'Attack'],
                3: ['Set', 'Attack'],
                4: ['Set', 'Attack'],
                5: ['Set', 'Attack'],
                6: ['Set', 'Attack']
            },
            'FreeBall': {
                1: [], // Error - rally ends
                2: ['Set', 'Attack', 'Reception'],
                3: ['Set', 'Attack', 'Reception'],
                4: ['Set', 'Attack', 'Reception']
            }
        };
        
        const actionRules = rulesMap[actionType];
        if (actionRules && actionRules[quality]) {
            return actionRules[quality];
        }
        
        return [];
    }

    // Get current state summary
    getState() {
        return {
            set: this.currentSet?.number || 0,
            score: { ...this.score },
            phase: this.phase,
            arrangement: this.currentArrangementNumber,
            availableActions: { ...this.availableActions },
            rallyActive: !this.isRallyEnded,
            actionsCount: this.actions.length
        };
    }

    // Export game data
    exportGame() {
        return {
            team: this.team,
            opponent: this.opponent,
            gameInfo: this.gameInfo,
            sets: this.sets,
            actions: this.actions,
            currentState: this.getState()
        };
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = GameState;
}
