// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Поточна дата в topbar
document.addEventListener('DOMContentLoaded', function () {
    const el = document.getElementById('topbar-date');
    if (el) {
        const now = new Date();
        el.textContent = now.toLocaleDateString('uk-UA', {
            weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
        });
    }

    // Модальні вікна
    document.querySelectorAll('[data-modal-open]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const id = btn.dataset.modalOpen;
            const modal = document.getElementById(id);
            if (modal) modal.classList.add('is-open');
        });
    });
    document.querySelectorAll('[data-modal-close]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            btn.closest('.modal-overlay').classList.remove('is-open');
        });
    });
    document.querySelectorAll('.modal-overlay').forEach(function (overlay) {
        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) overlay.classList.remove('is-open');
        });
    });

    // Автозакриття flash
    document.querySelectorAll('.flash').forEach(function (flash) {
        setTimeout(function () {
            flash.style.transition = 'opacity .4s';
            flash.style.opacity = '0';
            setTimeout(function () { flash.remove(); }, 400);
        }, 4000);
    });
});