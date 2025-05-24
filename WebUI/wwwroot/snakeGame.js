// Game state
let canvas, ctx;
let snake = [];
let food = {};
let gridSize;
let direction = { x: 0, y: -1 };
let nextDirection = { x: 0, y: -1 };
let score = 0;
let gameInterval;
let dotNetRef;

// Colors
const colors = {
    background: '#121212',
    snake: '#4CAF50',
    snakeHead: '#2E7D32',
    food: '#FF5252',
    text: '#FFFFFF'
};

export function initializeGame(canvasElement, dotNetReference, width, height, size) {
    // Initialize game elements
    canvas = canvasElement;
    ctx = canvas.getContext('2d');
    gridSize = size;
    dotNetRef = dotNetReference;

    // Set canvas size
    canvas.width = width;
    canvas.height = height;

    // Initialize game state
    resetGame();

    // Set up keyboard controls
    setupControls();

    // Start game loop
    gameInterval = setInterval(gameLoop, 100);
}

function setupControls() {
    window.addEventListener('keydown', e => {
        switch (e.key) {
            case 'ArrowUp':
                if (direction.y === 0) nextDirection = { x: 0, y: -1 };
                break;
            case 'ArrowDown':
                if (direction.y === 0) nextDirection = { x: 0, y: 1 };
                break;
            case 'ArrowLeft':
                if (direction.x === 0) nextDirection = { x: -1, y: 0 };
                break;
            case 'ArrowRight':
                if (direction.x === 0) nextDirection = { x: 1, y: 0 };
                break;
        }
        e.preventDefault();
    });
}

function resetGame() {
    // Initial snake position
    snake = [
        { x: 10, y: 10 },
        { x: 10, y: 11 },
        { x: 10, y: 12 }
    ];

    // Initial direction
    direction = { x: 0, y: -1 };
    nextDirection = { x: 0, y: -1 };

    // Initial score
    score = 0;
    dotNetRef.invokeMethodAsync('UpdateScore', score);

    // Place first food
    placeFood();
}

function placeFood() {
    const maxX = Math.floor(canvas.width / gridSize);
    const maxY = Math.floor(canvas.height / gridSize);

    let potentialFood;
    let validPosition = false;

    while (!validPosition) {
        potentialFood = {
            x: Math.floor(Math.random() * maxX),
            y: Math.floor(Math.random() * maxY)
        };

        validPosition = !snake.some(segment =>
            segment.x === potentialFood.x &&
            segment.y === potentialFood.y
        );
    }

    food = potentialFood;
}

function gameLoop() {
    // Update direction
    direction = { ...nextDirection };

    // Move snake
    const head = {
        x: snake[0].x + direction.x,
        y: snake[0].y + direction.y
    };

    // Check collisions
    if (
        head.x < 0 || head.y < 0 ||
        head.x >= canvas.width / gridSize ||
        head.y >= canvas.height / gridSize ||
        snake.some(segment => segment.x === head.x && segment.y === head.y)
    ) {
        clearInterval(gameInterval);
        dotNetRef.invokeMethodAsync('GameEnded');
        return;
    }

    // Add new head
    snake.unshift(head);

    // Check food collision
    if (head.x === food.x && head.y === food.y) {
        score++;
        dotNetRef.invokeMethodAsync('UpdateScore', score);
        placeFood();
    } else {
        // Remove tail if no food eaten
        snake.pop();
    }

    // Render game
    render();
}

function render() {
    // Clear canvas
    ctx.fillStyle = colors.background;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Draw snake
    snake.forEach((segment, index) => {
        ctx.fillStyle = index === 0 ? colors.snakeHead : colors.snake;
        ctx.fillRect(
            segment.x * gridSize,
            segment.y * gridSize,
            gridSize,
            gridSize
        );

        // Add border for better visibility
        ctx.strokeStyle = colors.background;
        ctx.lineWidth = 1;
        ctx.strokeRect(
            segment.x * gridSize,
            segment.y * gridSize,
            gridSize,
            gridSize
        );
    });

    // Draw food
    ctx.fillStyle = colors.food;
    ctx.beginPath();
    ctx.arc(
        food.x * gridSize + gridSize / 2,
        food.y * gridSize + gridSize / 2,
        gridSize / 2 - 2,
        0,
        Math.PI * 2
    );
    ctx.fill();
}

export function restartGame() {
    clearInterval(gameInterval);
    resetGame();
    gameInterval = setInterval(gameLoop, 100);
}