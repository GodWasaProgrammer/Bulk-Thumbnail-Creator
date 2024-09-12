window.audioPlayer = {
    player: null,

    initialize: function (elementId) {
        this.player = document.getElementById(elementId);
        if (this.player) {
            console.log("Player initialized with ID:", elementId);
            console.log("Current player source:", this.player.src);
        } else {
            console.error("Player initialization failed: Element not found");
        }
    },

    play: function () {
        if (this.player) {
            console.log("Attempting to play");
            this.player.play().then(() => {
                console.log("Started Playing");
            }).catch(error => {
                console.error("Error playing video:", error);
            });
        } else {
            console.error("Player not initialized");
        }
    },

    pause: function () {
        if (this.player) this.player.pause();
    },

    stop: function () {
        if (this.player) {
            this.player.pause();
            this.player.currentTime = 0;
        }
    },

    next: function (url) {
        if (this.player) {
            console.log("Setting video source to:", url);
            this.player.src = url;
            this.player.load(); // Ensure the video metadata is loaded
            this.player.play().then(() => {
                console.log("Playing new video");
            }).catch(error => {
                console.error("Error playing new video:", error);
            });
        }
    },

    previous: function (url) {
        if (this.player) {
            console.log("Setting video source to:", url);
            this.player.src = url;
            this.player.load(); // Ensure the video metadata is loaded
            this.player.play().then(() => {
                console.log("Playing previous video");
            }).catch(error => {
                console.error("Error playing previous video:", error);
            });
        }
    },

    setVolume: function (volume) {
        if (this.player) this.player.volume = volume / 100;
    }
};

