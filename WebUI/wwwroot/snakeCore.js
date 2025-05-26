// Game state
let canvas, ctx;
let snake = [];
let food = {};
let gameLoopId;
let settings = {
    speed: 150,
    difficulty: 'medium'
};

export function initGame(canvasElement, dotNetRef, width, height, size, difficulty, speed) {
    // Initiera spel
    canvas = canvasElement;
    ctx = canvas.getContext('2d');

    // Sätt upp spelplanen
    canvas.width = width;
    canvas.height = height;

    // Spara inställningar
    settings.difficulty = difficulty;
    settings.speed = speed;

    // Starta spelet
    resetGame();
    setupControls();
    startGame();
}

function resetGame() {
    // Återställ spelstatus
    snake = [{ x: 10, y: 10 }];
    placeFood();
}

function setupControls() {
    // Enkel tangentbordshantering
    document.addEventListener('keydown', e => {
        // Lägg till kontroller här senare
    });
}

function startGame() {
    gameLoopId = setInterval(gameLoop, settings.speed);
}

function gameLoop() {
    // Grundläggande spelmekanik
    moveSnake();
    checkCollisions();
    drawGame();
}

// ... resten av spelfunktionerna ...

export function updateSettings(difficulty, speed) {
    settings.difficulty = difficulty;
    settings.speed = speed;

    clearInterval(gameLoopId);
    startGame();
}

export function pauseGame() {
    clearInterval(gameLoopId);
}

export function resumeGame() {
    startGame();
}

export function cleanup() {
    clearInterval(gameLoopId);
}