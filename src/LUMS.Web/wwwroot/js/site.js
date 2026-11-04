(function () {
    'use strict';

    function toast(message, kind) {
        var region = document.getElementById('toast-region');
        if (!region) { return; }
        var el = document.createElement('div');
        el.className = 'toast' + (kind ? ' toast--' + kind : '');
        el.textContent = message;
        region.appendChild(el);
        window.setTimeout(function () { el.remove(); }, 4500);
    }

    document.addEventListener('click', function (e) {
        var target = e.target.closest('[data-toast]');
        if (!target) { return; }
        e.preventDefault();
        toast(target.getAttribute('data-toast'));
    });

    function csrfToken() {
        var match = document.cookie.match(/(?:^|;\s*)XSRF-TOKEN=([^;]+)/);
        return match ? decodeURIComponent(match[1]) : '';
    }

    function formatDate(iso) {
        var p = String(iso).split('-');
        return p.length === 3 ? p[2] + '/' + p[1] + '/' + p[0] : iso;
    }

    document.addEventListener('click', function (e) {
        var button = e.target.closest('[data-renew]');
        if (!button) { return; }

        var loanId = button.getAttribute('data-renew');
        var row = button.closest('[data-loan-row]');
        button.classList.add('is-loading');

        fetch('/api/loans/' + encodeURIComponent(loanId) + '/renew', {
            method: 'POST',
            headers: { 'X-CSRF-TOKEN': csrfToken(), 'Accept': 'application/json' },
            credentials: 'same-origin'
        })
            .then(function (res) {
                return res.json().catch(function () { return {}; }).then(function (body) {
                    return { ok: res.ok, status: res.status, body: body };
                });
            })
            .then(function (r) {
                if (r.ok) {
                    var due = row && row.querySelector('[data-due]');
                    if (due) { due.textContent = formatDate(r.body.newDueDate); due.classList.remove('text-danger'); }
                    toast('Gia hạn thành công. Hạn trả mới: ' + formatDate(r.body.newDueDate), 'ok');
                    window.setTimeout(function () { window.location.reload(); }, 1200);
                    return;
                }

                button.classList.remove('is-loading');
                if (r.status === 401) {
                    window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                    return;
                }
                toast(r.body.detail || r.body.title || 'Không thể gia hạn. Vui lòng thử lại.', 'error');
            })
            .catch(function () {
                button.classList.remove('is-loading');
                toast('Không kết nối được máy chủ. Vui lòng thử lại.', 'error');
            });
    });

    document.querySelectorAll('[data-tabs]').forEach(function (root) {
        var tabs = Array.prototype.slice.call(root.querySelectorAll('[role="tab"]'));

        function select(tab) {
            tabs.forEach(function (t) {
                var active = t === tab;
                t.classList.toggle('is-active', active);
                t.setAttribute('aria-selected', active ? 'true' : 'false');
                t.tabIndex = active ? 0 : -1;
                var panel = document.getElementById(t.getAttribute('aria-controls'));
                if (panel) { panel.hidden = !active; }
            });
        }

        tabs.forEach(function (tab, i) {
            tab.addEventListener('click', function () { select(tab); });
            tab.addEventListener('keydown', function (e) {
                var next = e.key === 'ArrowRight' ? tabs[(i + 1) % tabs.length]
                    : e.key === 'ArrowLeft' ? tabs[(i - 1 + tabs.length) % tabs.length] : null;
                if (next) { e.preventDefault(); select(next); next.focus(); }
            });
        });
    });

    document.querySelectorAll('[data-toggle-password]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = btn.closest('.field__password').querySelector('input');
            var show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            btn.textContent = show ? 'Ẩn' : 'Hiện';
        });
    });

    document.querySelectorAll('select[data-autosubmit]').forEach(function (select) {
        select.addEventListener('change', function () { select.form.submit(); });
    });

    document.querySelectorAll('[data-open-dialog]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var dialog = document.getElementById(btn.getAttribute('data-open-dialog'));
            if (dialog && typeof dialog.showModal === 'function') { dialog.showModal(); }
        });
    });
    document.querySelectorAll('[data-close-dialog]').forEach(function (btn) {
        btn.addEventListener('click', function () { btn.closest('dialog').close(); });
    });
    document.querySelectorAll('form[data-toast-on-submit]').forEach(function (form) {
        form.addEventListener('submit', function () { toast(form.getAttribute('data-toast-on-submit')); });
    });
})();
