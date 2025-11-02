/**
 * GameController - главный контроллер приложения
 * Объединяет все модули и управляет игровым процессом
 */

class GameController {
    constructor() {
        this.gameState = new GameState();
        this.actionProcessor = null;
        this.syncController = null;
        this.uiUpdater = new UIUpdater(this.gameState);
        
        this.initialized = false;
    }

    // Initialize from game setup
    async initialize() {
        try {
            // Load game setup from sessionStorage
            const gameSetupStr = sessionStorage.getItem('currentGameSetup');
            if (!gameSetupStr) {
                throw new Error('No game setup found. Please go back and set up the game.');
            }
            
            const gameSetup = JSON.parse(gameSetupStr);
            
            // Initialize action processor with config
            this.actionProcessor = new ActionProcessor(gameSetup.config);
            
            // Initialize game state
            this.gameState.initializeGame(gameSetup);
            
            // Initialize sync controller
            this.syncController = new SyncController(this.gameState, this.actionProcessor);
            
            // Initialize UI elements
            this.initializeUI();
            
            // Initialize video if URL provided
            if (gameSetup.videoUrl) {
                this.initializeVideo(gameSetup.videoUrl);
            }
            
            this.initialized = true;
            
            return true;
        } catch (error) {
            console.error('Initialization error:', error);
            alert('Error initializing game: ' + error.message);
            return false;
        }
    }

    // Initialize UI elements
    initializeUI() {
        // Get UI elements
        const elements = {
            // Text interface
            authorCombo: document.getElementById('authorCombo'),
            actionTypeCombo: document.getElementById('actionTypeCombo'),
            metricContainer: document.getElementById('metricContainer'),
            metricCombos: {},
            
            // Button controller
            buttonContainer: document.getElementById('buttonContainer'),
            buttonLabel: document.getElementById('buttonLabel'),
            
            // Score display
            scoreLeft: document.getElementById('scoreLeft'),
            scoreRight: document.getElementById('scoreRight'),
            setNumber: document.getElementById('setNumber'),
            scoreDisplay: document.getElementById('scoreDisplay'),
            
            // Phase and arrangement
            phaseIndicator: document.getElementById('phaseIndicator'),
            arrangementDisplay: document.getElementById('arrangementDisplay'),
            rallyInfo: document.getElementById('rallyInfo'),
            
            // Action history
            actionHistory: document.getElementById('actionHistory'),
            
            // Statistics
            statsSummary: document.getElementById('statsSummary'),
            
            // Messages
            messageContainer: document.getElementById('messageContainer'),
            
            // Callbacks
            getCurrentTimeCode: () => this.getCurrentVideoTime(),
            getCurrentPoints: () => this.getCurrentCourtPoints(),
            onActionAdded: (action, setFinished) => this.onActionAdded(action, setFinished),
            setFinishedCallback: (result) => this.onSetFinished(result),
            gameFinishedCallback: (won) => this.onGameFinished(won)
        };
        
        // Initialize sync controller
        this.syncController.init(elements);
        
        // Initialize UI updater
        this.uiUpdater.init(elements);
        
        // Update UI
        this.uiUpdater.updateAll();
        
        // Setup additional event listeners
        this.setupEventListeners();
    }

    // Setup event listeners
    setupEventListeners() {
        // Save game button
        const saveBtn = document.getElementById('saveGameBtn');
        if (saveBtn) {
            saveBtn.addEventListener('click', () => this.saveGame());
        }
        
        // Export button
        const exportBtn = document.getElementById('exportGameBtn');
        if (exportBtn) {
            exportBtn.addEventListener('click', () => this.exportGame());
        }
        
        // Undo button
        const undoBtn = document.getElementById('undoBtn');
        if (undoBtn) {
            undoBtn.addEventListener('click', () => this.undoLastAction());
        }
        
        // Manual add button
        const addBtn = document.getElementById('addActionBtn');
        if (addBtn) {
            addBtn.addEventListener('click', () => this.syncController.onActionComplete());
        }
        
        // Court clicks for point markers
        this.setupCourtClicks();
    }

    // Action added callback
    onActionAdded(action, setFinished) {
        // Update UI
        this.uiUpdater.updateAll();
        this.uiUpdater.addActionToHistory(action);
        
        // Highlight if rally ended
        if (this.gameState.isRallyEnded) {
            const lastRally = this.gameState.currentSet.rallies[this.gameState.currentSet.rallies.length - 1];
            if (lastRally) {
                this.uiUpdater.highlightRallyEnd(lastRally.result);
            }
        }
        
        // Check if set finished
        if (setFinished) {
            const setResult = this.gameState.currentSet.result;
            this.uiUpdater.showSetFinished(setResult);
            
            // Check if game finished
            const wonSets = this.gameState.sets.filter(s => s.result === 'Won').length;
            const lostSets = this.gameState.sets.filter(s => s.result === 'Lost').length;
            const maxSets = this.gameState.gameInfo.maxSets || 5;
            const setsToWin = Math.ceil(maxSets / 2);
            
            if (wonSets >= setsToWin || lostSets >= setsToWin) {
                this.uiUpdater.showGameFinished(wonSets, lostSets);
            }
        }
        
        // Auto-save
        this.autoSave();
    }

    // Set finished callback
    onSetFinished(result) {
        // Could show dialog to start next set
        console.log('Set finished:', result);
    }

    // Game finished callback
    onGameFinished(won) {
        // Could show save/export dialog
        console.log('Game finished, won:', won);
    }

    // Get current video time
    getCurrentVideoTime() {
        // This would integrate with YouTube player
        if (window.player && window.player.getCurrentTime) {
            return window.player.getCurrentTime();
        }
        return 0;
    }

    // Get current court points
    getCurrentCourtPoints() {
        // This would get points from court visualization
        if (window.courtPoints) {
            return window.courtPoints;
        }
        return [];
    }

    // Setup court clicks for point markers
    setupCourtClicks() {
        const courtCanvas = document.getElementById('courtCanvas');
        if (!courtCanvas) return;
        
        courtCanvas.addEventListener('click', (e) => {
            const rect = courtCanvas.getBoundingClientRect();
            const x = (e.clientX - rect.left) / rect.width;
            const y = (e.clientY - rect.top) / rect.height;
            
            // Add point marker
            this.addCourtPoint(x, y);
        });
    }

    // Add point marker to court
    addCourtPoint(x, y) {
        if (!window.courtPoints) {
            window.courtPoints = [];
        }
        
        window.courtPoints.push({ x, y });
        
        // Draw point marker
        this.drawCourtPoints();
    }

    // Draw court points
    drawCourtPoints() {
        const courtCanvas = document.getElementById('courtCanvas');
        if (!courtCanvas) return;
        
        const ctx = courtCanvas.getContext('2d');
        
        // Redraw court (would be from existing court drawing function)
        // For now, just draw points
        
        if (window.courtPoints) {
            window.courtPoints.forEach(point => {
                const x = point.x * courtCanvas.width;
                const y = point.y * courtCanvas.height;
                
                ctx.fillStyle = '#ff0000';
                ctx.beginPath();
                ctx.arc(x, y, 5, 0, 2 * Math.PI);
                ctx.fill();
            });
        }
    }

    // Clear court points
    clearCourtPoints() {
        window.courtPoints = [];
        this.drawCourtPoints();
    }

    // Initialize video player
    initializeVideo(url) {
        // Extract YouTube video ID
        const videoId = this.extractYouTubeId(url);
        if (!videoId) {
            console.warn('Invalid YouTube URL');
            return;
        }
        
        // Initialize YouTube player (if YouTube API loaded)
        if (window.YT && window.YT.Player) {
            window.player = new YT.Player('youtubePlayer', {
                videoId: videoId,
                playerVars: {
                    'playsinline': 1
                }
            });
        }
    }

    // Extract YouTube video ID from URL
    extractYouTubeId(url) {
        const regex = /(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&]+)/;
        const match = url.match(regex);
        return match ? match[1] : null;
    }

    // Save game
    async saveGame() {
        try {
            const gameData = this.gameState.exportGame();
            
            // Save to backend API
            const response = await fetch('/api/games', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(gameData)
            });
            
            if (!response.ok) {
                throw new Error('Failed to save game');
            }
            
            const result = await response.json();
            
            this.uiUpdater.showSuccess('Game saved successfully!');
            
            return result;
        } catch (error) {
            console.error('Save error:', error);
            this.uiUpdater.showError('Failed to save game: ' + error.message);
        }
    }

    // Auto-save
    autoSave() {
        // Save to localStorage as backup
        try {
            const gameData = this.gameState.exportGame();
            localStorage.setItem('currentGame', JSON.stringify(gameData));
        } catch (error) {
            console.error('Auto-save error:', error);
        }
    }

    // Export game to file
    exportGame() {
        try {
            const gameData = this.gameState.exportGame();
            const json = JSON.stringify(gameData, null, 2);
            
            const blob = new Blob([json], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            
            const a = document.createElement('a');
            a.href = url;
            a.download = `volleyball_game_${Date.now()}.json`;
            a.click();
            
            URL.revokeObjectURL(url);
            
            this.uiUpdater.showSuccess('Game exported!');
        } catch (error) {
            console.error('Export error:', error);
            this.uiUpdater.showError('Failed to export game: ' + error.message);
        }
    }

    // Undo last action
    undoLastAction() {
        if (this.gameState.actions.length === 0) {
            this.uiUpdater.showError('No actions to undo');
            return;
        }
        
        // Remove last action
        this.gameState.actions.pop();
        
        // Rebuild game state from actions
        // This would require re-processing all actions
        // For now, show warning
        this.uiUpdater.showError('Undo not fully implemented yet');
    }

    // Load game from file
    async loadGame(file) {
        try {
            const text = await file.text();
            const gameData = JSON.parse(text);
            
            // Restore game state
            // This would require full restore logic
            this.uiUpdater.showSuccess('Game loaded!');
        } catch (error) {
            console.error('Load error:', error);
            this.uiUpdater.showError('Failed to load game: ' + error.message);
        }
    }
}

// Initialize when DOM ready
document.addEventListener('DOMContentLoaded', async () => {
    window.gameController = new GameController();
    const success = await window.gameController.initialize();
    
    if (!success) {
        // Show error and redirect option
        const goBack = confirm('Failed to initialize game. Go back to setup?');
        if (goBack) {
            window.location.href = '/game-setup';
        }
    }
});
