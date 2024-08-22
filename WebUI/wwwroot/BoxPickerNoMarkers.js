let rect = { x: 0, y: 0, width: 0, height: 0 };

function initCanvasNoMarker()
{
    const container = document.querySelector("[id='BoxPickerNoMarker']");
    const canvas = container.querySelector('#canvas');
    const ctx = canvas.getContext('2d');
    let isDragging = false;
    let startX, startY, currentX, currentY;

    canvas.addEventListener('mousedown', function (e)
    {
        if (!isDragging) {
            const rectBounds = canvas.getBoundingClientRect();
            isDragging = true;
            startX = e.clientX - rectBounds.left;
            startY = e.clientY - rectBounds.top;
        }
    });

    canvas.addEventListener('mousemove', function (e)
    {
        if (isDragging) {
            const rectBounds = canvas.getBoundingClientRect();
            currentX = e.clientX - rectBounds.left;
            currentY = e.clientY - rectBounds.top;

            // Calculate rectangle dimensions
            rect.x = Math.min(startX, currentX);
            rect.y = Math.min(startY, currentY);
            rect.width = Math.abs(currentX - startX);
            rect.height = Math.abs(currentY - startY);

            // Prevent dragging outside the canvas
            rect.x = Math.max(0, Math.min(rect.x, canvas.width - rect.width));
            rect.y = Math.max(0, Math.min(rect.y, canvas.height - rect.height));
            rect.width = Math.max(0, Math.min(rect.width, canvas.width - rect.x));
            rect.height = Math.max(0, Math.min(rect.height, canvas.height - rect.y));

            draw();
        }
    });

    canvas.addEventListener('mouseup', function ()
    {
        if (isDragging) {
            isDragging = false;
            draw();
        }
    });

    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 2;
        ctx.strokeRect(rect.x, rect.y, rect.width, rect.height);
    }

    function getRectangleData()
    {
        const image = container.querySelector('img');

        // Bildens faktiska storlek
        const originalWidth = image.naturalWidth;
        const originalHeight = image.naturalHeight;

        // Bildens visade storlek
        const displayedWidth = image.clientWidth;
        const displayedHeight = image.clientHeight;

        // Skalningsfaktorer
        const scaleX = originalWidth / displayedWidth;
        const scaleY = originalHeight / displayedHeight;

        // Justera rektangeldatan
        return {
            x: rect.x * scaleX,
            y: rect.y * scaleY,
            width: rect.width * scaleX,
            height: rect.height * scaleY
        };
    }

    // Make methods available to Blazor
    window.getRectangleData = getRectangleData;
    window.removeMarkers = function ()
    {
        const markers = document.querySelectorAll('.corner-marker');
        markers.forEach(marker => marker.remove());
    };

    createMarkers();
}