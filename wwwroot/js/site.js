// Logic cho Chibi Animation
document.addEventListener('DOMContentLoaded', function () {
    const characters = document.querySelectorAll('.chibi-character');
    const dialogueBox = document.getElementById('dialogue-box');

    characters.forEach(char => {
        char.addEventListener('click', function () {
            const dialogue = this.getAttribute('data-dialogue');
            dialogueBox.textContent = dialogue;
            dialogueBox.classList.remove('dialogue-hidden');

            // Tự động ẩn sau 1 giây
            setTimeout(() => {
                dialogueBox.classList.add('dialogue-hidden');
            }, 1000);
        });
    });
});

// --- LOGIC ĐIỀU KHIỂN NHẠC NỀN ---
document.addEventListener('DOMContentLoaded', function () {
    const musicControl = document.getElementById('music-control');
    const bgMusic = document.getElementById('bg-music');

    if (musicControl && bgMusic) {
        // Mặc định nhạc bị tắt
        bgMusic.muted = true;
        musicControl.classList.add('muted');
        musicControl.innerHTML = '🔇'; // Biểu tượng loa tắt

        musicControl.addEventListener('click', function () {
            if (bgMusic.paused) {
                // Nếu đang dừng, thì phát
                bgMusic.play();
            }

            // Chuyển đổi trạng thái tắt/bật tiếng
            if (bgMusic.muted) {
                bgMusic.muted = false;
                musicControl.classList.remove('muted');
                musicControl.innerHTML = '🎵'; // Biểu tượng nốt nhạc
            } else {
                bgMusic.muted = true;
                musicControl.classList.add('muted');
                musicControl.innerHTML = '🔇'; // Biểu tượng loa tắt
            }
        });
    }
});