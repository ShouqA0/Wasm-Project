document.addEventListener("DOMContentLoaded", function () {
    let firstPattern = "";
    let secondPattern = "";
    let step = 1;

    // إعداد لوحة النمط الأولى والثانية
    // ملاحظة: افترضنا وجود مكتبة PatternLock أو منطق الرسم الذي صممناه سابقاً
    // هذا الكود يركز على منطق التبديل (Switching Logic)

    const step1Container = document.getElementById("step-1-pattern");
    const step2Container = document.getElementById("step-2-pattern");
    const mainPatternInput = document.getElementById("PatternData"); // الحقل المخفي

    // دالة تُستدعى عند اكتمال رسم النمط (Callback)
    window.onPatternComplete = function (pattern) {
        if (step === 1) {
            firstPattern = pattern;

            // تأثير انتقال ناعم
            step1Container.classList.add("animate__animated", "animate__fadeOutLeft");

            setTimeout(() => {
                step1Container.style.display = "none";
                step2Container.style.display = "block";
                step2Container.classList.add("animate__animated", "animate__fadeInRight");
                step = 2;
            }, 500);

        } else if (step === 2) {
            secondPattern = pattern;

            // التحقق من تطابق النمطين
            if (firstPattern === secondPattern) {
                mainPatternInput.value = secondPattern; // حفظ النمط النهائي
                Swal.fire({
                    icon: 'success',
                    title: 'تم تأكيد النمط بنجاح',
                    showConfirmButton: false,
                    timer: 1500,
                    background: '#0A192F',
                    color: '#fff'
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'الأنماط غير متطابقة',
                    text: 'يرجى إعادة المحاولة',
                    background: '#0A192F',
                    color: '#fff',
                    confirmButtonColor: '#D4AF37'
                });
                resetToStepOne(); // إعادة البدء من جديد
            }
        }
    };

    // دالة إعادة التعيين
    window.resetToStepOne = function () {
        step = 1;
        firstPattern = "";
        secondPattern = "";

        step2Container.style.display = "none";
        step1Container.style.display = "block";

        // مسح الرسوم من الـ SVG (تعتمد على كود الرسم الخاص بك)
        document.getElementById("patternSvg1").innerHTML = "";
        document.getElementById("patternSvg2").innerHTML = "";
        document.querySelectorAll(".dot-node").forEach(dot => dot.classList.remove("active"));
    };
});