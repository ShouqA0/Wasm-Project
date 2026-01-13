// وسم - نظام توثيق الملكية
(function ($) {
    $(function () {
        // تعريف العناصر
        const chatWindow = $('#chat-window');
        const chatToggleBtn = $('#chat-toggle-btn');
        const greetingBubble = $('#greeting-bubble');
        const chatMessages = $('#chat-messages');
        const chatInput = $('#chat-input');
        const sendBtn = $('#btn-send-chat');

        let isProcessing = false;

        // دالة الفتح والإغلاق
        function toggleChat(e) {
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }

            if (isProcessing) return;
            isProcessing = true;

            if (chatWindow.is(':visible')) {
                chatWindow.fadeOut(300, function () {
                    isProcessing = false;
                });
            } else {
                greetingBubble.hide();
                chatWindow.fadeIn(300, function () {
                    isProcessing = false;
                    chatInput.focus(); // تركيز المؤشر عند الفتح
                });
            }
        }

        // ربط الأحداث
        $(document).off('click', '#chat-toggle-btn').on('click', '#chat-toggle-btn', toggleChat);

        $(document).off('click', '#minimize-chat').on('click', '#minimize-chat', function (e) {
            e.preventDefault();
            chatWindow.fadeOut(300);
        });

        // إظهار الفقاعة الترحيبية عند التحميل
        setTimeout(() => {
            if (chatWindow.is(":hidden")) {
                greetingBubble.fadeIn(500).delay(5000).fadeOut(500);
            }
        }, 2000);

        // دالة إرسال الرسالة إلى Controller
        function sendMessage() {
            const text = chatInput.val().trim();
            if (text === "") return;

            // 1. عرض رسالة المستخدم
            const userMsgHtml = `<div class="user-msg animate__animated animate__fadeInUp text-end mb-2">
                                    <span class="p-2 d-inline-block rounded bg-success text-white">${text}</span>
                                 </div>`;
            chatMessages.append(userMsgHtml);
            chatInput.val('');
            scrollToBottom();

            // 2. إظهار تأثير "جاري الكتابة"
            const typingId = "typing-" + Date.now();
            const typingHtml = `<div id="${typingId}" class="bot-msg animate__animated animate__fadeIn mb-2">
                                    <span class="p-2 d-inline-block rounded bg-light text-muted">وسّام يكتب الآن...</span>
                                </div>`;
            chatMessages.append(typingHtml);
            scrollToBottom();

            // 3. استدعاء الكنترلور عبر AJAX
            $.ajax({
                url: '/Chatbot/GetResponse',
                type: 'POST',
                data: { userMessage: text },
                success: function (data) {
                    $(`#${typingId}`).remove(); // إزالة تأثير الكتابة

                    const botMsgHtml = `<div class="bot-msg animate__animated animate__fadeInUp mb-2">
                                            <div class="p-2 d-inline-block rounded shadow-sm bg-white border-start border-success border-4" style="max-width: 85%;">
                                                ${data.response}
                                            </div>
                                        </div>`;
                    chatMessages.append(botMsgHtml);
                    scrollToBottom();
                },
                error: function () {
                    $(`#${typingId}`).remove();
                    chatMessages.append(`<div class="text-danger small text-center">عذراً، حدث خطأ في الاتصال.</div>`);
                }
            });
        }

        // تنفيذ الإرسال عند النقر أو الضغط على Enter
        $(document).off('click', '#btn-send-chat').on('click', '#btn-send-chat', sendMessage);

        chatInput.on('keypress', function (e) {
            if (e.which === 13) { // زر Enter
                sendMessage();
            }
        });

        function scrollToBottom() {
            chatMessages.animate({ scrollTop: chatMessages[0].scrollHeight }, 500);
        }
    });
})(jQuery);