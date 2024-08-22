let rect = { x: 0, y: 0, width: 0, height: 0 };

function initCanvas()
{
    const container = document.querySelector("[id='BoxPicker']");
    const canvas = container.querySelector('#canvas');
    const ctx = canvas.getContext('2d');
    const image = container.querySelector('img');
    let isDragging = false;
    let startX, startY, currentX, currentY;
    let markerSize = 8; // Size of the corner markers

    function createMarkers()
    {
        // Ensure the parent container is positioned relatively
        container.style.position = 'relative';

        // Define marker positions relative to the canvas
        const markers = [
            { left: 0, top: 0 }, // Top-left
            { left: canvas.width - markerSize, top: 0 }, // Top-right
            { left: 0, top: canvas.height - markerSize }, // Bottom-left
            { left: canvas.width - markerSize, top: canvas.height - markerSize } // Bottom-right
        ];

        markers.forEach(marker => {
            const mark = document.createElement('div');
            mark.style.position = 'absolute';
            mark.style.width = `${markerSize}px`;
            mark.style.height = `${markerSize}px`;
            mark.style.backgroundColor = 'red';
            mark.style.borderRadius = '50%';
            mark.style.cursor = 'pointer';
            mark.style.left = `${marker.left}px`;
            mark.style.top = `${marker.top}px`;
            mark.className = 'corner-marker';
            mark.dataset.x = marker.x;
            mark.dataset.y = marker.y;
            mark.addEventListener('mousedown', (e) => startDragging(e, marker.x, marker.y));
            document.body.appendChild(mark);
            container.appendChild(mark);
        });
    }

    function startDragging(e, x, y)
    {
        isDragging = true;
        startX = x - imageRect.left; // Adjust for image offset
        startY = y - imageRect.top; // Adjust for image offset
        e.preventDefault();
    }

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

    image.addEventListener("load", function ()
    {
        console.log("image has loaded");
    })

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

    function getRectangleData() {
        const image = document.querySelector('img');

        if (!image) {
            console.error("Image element not found");
            return null;
        }

        if (image.complete) {
            return getImageData(image);
        }

        return new Promise((resolve) => {
            image.onload = () => {
                resolve(getImageData(image));
            };
            image.onerror = () => {
                console.error("Image failed to load");
                resolve(null);
            };
        });
    }

    function getImageData(image) {
        const displayedWidth = image.clientWidth;
        const displayedHeight = image.clientHeight;

        if (displayedWidth === 0 || displayedHeight === 0) {
            console.error("Image dimensions are zero");
            return null;
        }

        // Bildens faktiska storlek
        const originalWidth = image.naturalWidth;
        const originalHeight = image.naturalHeight;

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