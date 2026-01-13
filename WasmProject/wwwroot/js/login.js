document.addEventListener("DOMContentLoaded", function () {

    // --- 1. تأثير الطباعة الآلية (Typing Effect) للجملة الترحيبية ---
    const welcomeText = "أهلاً بك في وسم، حيث يلتقي التراث بالتقنية..";
    const typingElement = document.getElementById("typing-text");
    let index = 0;

    function typeWriter() {
        if (typingElement && index < welcomeText.length) {
            typingElement.innerHTML += welcomeText.charAt(index);
            index++;
            setTimeout(typeWriter, 70); // سرعة الطباعة الموصى بها
        }
    }

    // --- 2. تأثير ظهور نصوص العلا بالتدريج (Scroll Reveal) ---
    const revealElements = document.querySelectorAll(".reveal-text");
    const revealOnScroll = () => {
        const triggerBottom = (window.innerHeight / 5) * 4;
        revealElements.forEach((el) => {
            const elTop = el.getBoundingClientRect().top;
            if (elTop < triggerBottom) {
                el.classList.add("show");
            }
        });
    };

    // --- 3. منطق رسم النمط البصري (Visual Pattern Logic) ---
    const container = document.getElementById('patternContainer');
    const svg = document.getElementById('patternSvg');
    const dots = document.querySelectorAll('.dot-node');
    const patternInput = document.getElementById('PatternData');

    let isDrawing = false;
    let selectedDots = [];

    const startDrawing = (e) => {
        isDrawing = true;
        resetPattern();
        handleInteraction(e);
    };

    const draw = (e) => {
        if (!isDrawing) return;
        handleInteraction(e);
    };

    const stopDrawing = () => {
        if (!isDrawing) return;
        isDrawing = false;
        // تخزين تسلسل النمط (مثل: 125) لإرساله للسيرفر للتحقق
        if (patternInput) {
            patternInput.value = selectedDots.join('');
        }
    };

    const handleInteraction = (e) => {
        e.preventDefault();
        const x = e.clientX || (e.touches ? e.touches[0].clientX : 0);
        const y = e.clientY || (e.touches ? e.touches[0].clientY : 0);

        dots.forEach(dot => {
            const rect = dot.getBoundingClientRect();
            const dotX = rect.left + rect.width / 2;
            const dotY = rect.top + rect.height / 2;
            const distance = Math.hypot(x - dotX, y - dotY);

            if (distance < 25) { // حساسية لمس النقاط
                const dotIndex = dot.getAttribute('data-index');
                if (!selectedDots.includes(dotIndex)) {
                    selectedDots.push(dotIndex);
                    dot.classList.add('active'); // تفعيل التوهج الأبيض
                    updateLines();
                }
            }
        });
    };

    const updateLines = () => {
        if (!svg) return;
        svg.innerHTML = '';
        const containerRect = container.getBoundingClientRect();

        for (let i = 0; i < selectedDots.length - 1; i++) {
            const dot1 = document.querySelector(`.dot-node[data-index="${selectedDots[i]}"]`);
            const dot2 = document.querySelector(`.dot-node[data-index="${selectedDots[i + 1]}"]`);
            const r1 = dot1.getBoundingClientRect();
            const r2 = dot2.getBoundingClientRect();

            const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
            line.setAttribute('x1', r1.left + r1.width / 2 - containerRect.left);
            line.setAttribute('y1', r1.top + r1.height / 2 - containerRect.top);
            line.setAttribute('x2', r2.left + r2.width / 2 - containerRect.left);
            line.setAttribute('y2', r2.top + r2.height / 2 - containerRect.top);
            line.setAttribute('class', 'pattern-line'); // الخط الأبيض المنسق في CSS
            svg.appendChild(line);
        }
    };

    const resetPattern = () => {
        selectedDots = [];
        dots.forEach(dot => dot.classList.remove('active'));
        if (svg) svg.innerHTML = '';
        if (patternInput) patternInput.value = '';
    };

    // --- 4. التمرير السلس (Smooth Scroll) للانتقال لقسم تسجيل الدخول ---
    const setupSmoothScroll = () => {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const targetId = this.getAttribute('href');
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    e.preventDefault();
                    targetElement.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });
    };

    // --- 5. منطق المساعد الرقمي الاستثنائي (Advanced Assistant Logic) ---
    const setupAssistant = () => {
        const chatToggle = $("#chat-toggle-btn");
        const chatWindow = $("#chat-window");
        const greetingBubble = $("#greeting-bubble");
        const minimizeBtn = $("#minimize-chat");

        // ظهور فقاعة الترحيب بحركة ناعمة (Up) بعد 3 ثوانٍ من التحميل
        setTimeout(() => {
            if (chatWindow.is(":hidden")) {
                greetingBubble.show().addClass("animate__fadeInUp");

                // تختفي الفقاعة تلقائياً بعد 8 ثوانٍ لعدم إزعاج المستخدم
                setTimeout(() => {
                    greetingBubble.removeClass("animate__fadeInUp").addClass("animate__fadeOut");
                    setTimeout(() => greetingBubble.hide(), 1000);
                }, 8000);
            }
        }, 3000);

        // فتح وإغلاق النافذة بحركات انسيابية (Fade & Slide)
        chatToggle.on('click', function () {
            greetingBubble.hide();
            $(".notification-badge").fadeOut(); // إخفاء إشعار التنبيه عند الفتح

            if (chatWindow.is(":hidden")) {
                chatWindow.show()
                    .removeClass("animate__fadeOutDown")
                    .addClass("animate__fadeInUp");
            } else {
                chatWindow.removeClass("animate__fadeInUp")
                    .addClass("animate__fadeOutDown");
                setTimeout(() => chatWindow.hide(), 500);
            }
        });

        // تصغير النافذة
        minimizeBtn.on('click', function () {
            chatWindow.removeClass("animate__fadeInUp")
                .addClass("animate__fadeOutDown");
            setTimeout(() => chatWindow.hide(), 500);
        });
    };

    // ربط أحداث النمط بالحاوية لضمان عمل اللمس والماوس
    if (container) {
        container.addEventListener('mousedown', startDrawing);
        container.addEventListener('touchstart', startDrawing, { passive: false });
        window.addEventListener('mousemove', draw);
        window.addEventListener('touchmove', draw, { passive: false });
        window.addEventListener('mouseup', stopDrawing);
        window.addEventListener('touchend', stopDrawing);
    }

    // تشغيل كافة الوظائف البرمجية عند جاهزية الصفحة
    typeWriter();
    setupSmoothScroll();
    setupAssistant();
    window.addEventListener("scroll", revealOnScroll);
    revealOnScroll();
});