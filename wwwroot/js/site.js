// Logic cho Chibi Animation
document.addEventListener('DOMContentLoaded', function () {
    const stage = document.getElementById('chibi-stage');
    if (stage) {
        const dialogueBox = document.getElementById('dialogue-box');
        const characters = document.querySelectorAll('.chibi-character');

        characters.forEach(char => {
            char.addEventListener('click', function (event) {
                const dialogue = this.dataset.dialogue;

                dialogueBox.textContent = dialogue;

                // Lấy vị trí click tương đối so với sân khấu
                const stageRect = stage.getBoundingClientRect();
                const clickX = event.clientX - stageRect.left;
                const clickY = event.clientY - stageRect.top;

                // Đặt vị trí cho hộp thoại
                dialogueBox.style.left = `${clickX}px`;
                dialogueBox.style.top = `${clickY - 10}px`; // Hơi nhích lên trên một chút

                dialogueBox.classList.remove('dialogue-hidden');

                // Tự động ẩn hộp thoại sau 3 giây
                setTimeout(() => {
                    dialogueBox.classList.add('dialogue-hidden');
                }, 3000);
            });
        });
    }
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