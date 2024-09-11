window.audioPlayer = {
    player: null,

    initialize: function (elementId) {
        this.player = document.getElementById(elementId);
    },

    play: function () {
        if (this.player) {
            this.player.play();
        }
    },

    pause: function () {
        if (this.player) {
            this.player.pause();
        }
    },

    stop: function () {
        if (this.player) {
            this.player.pause();
            this.player.currentTime = 0;
        }
    },

    next: function (nextUrl) {
        if (this.player) {
            this.player.src = nextUrl;
            this.player.play();
        }
    },

    previous: function (previousUrl) {
        if (this.player) {
            this.player.src = previousUrl;
            this.player.play();
        }
    },

    setVolume: function (volume) {
        if (this.player) {
            this.player.volume = volume / 100;
        }
    }
};
