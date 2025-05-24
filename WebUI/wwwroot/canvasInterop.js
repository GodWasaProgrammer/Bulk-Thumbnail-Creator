export function getContext(canvas) {
    return canvas.getContext('2d');
}

export function clearRect(ctx, x, y, width, height) {
    ctx.clearRect(x, y, width, height);
}

export function fillRect(ctx, color, x, y, width, height) {
    ctx.fillStyle = color;
    ctx.fillRect(x, y, width, height);
}

export function fillText(ctx, text, color, font, x, y) {
    ctx.fillStyle = color;
    ctx.font = font;
    ctx.fillText(text, x, y);
}