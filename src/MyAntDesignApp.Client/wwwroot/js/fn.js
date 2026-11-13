window.fn = {
    openDialog(element) { if (!element.open) { element._returnFocus = document.activeElement; element.showModal(); } },
    closeDialog(element) { element.close(); element._returnFocus?.focus(); },
    closeActiveDialog() { const el = document.querySelector('dialog[open]'); if (el) window.fn.closeDialog(el); },
    focusComment() { document.getElementById('comment-input')?.focus(); },
    clock() {
        const now = new Date();
        return { hour: now.getHours(), date: `${now.getFullYear()}-${String(now.getMonth()+1).padStart(2,'0')}-${String(now.getDate()).padStart(2,'0')}` };
    },
    clockWatchers: new Map(),
    nextClockId: 0,
    observeClock(receiver) {
        const id = ++window.fn.nextClockId;
        const update = () => {
            const value = window.fn.clock();
            const night = value.hour >= 17 || value.hour < 5;
            const icon = document.querySelector('link[rel="icon"]');
            if (icon) icon.setAttribute('href', night ? '/img/logo-night.png' : '/img/logo-morning.png');
            receiver.invokeMethodAsync('UpdateClock', value).catch(() => window.fn.stopClock(id));
        };
        const visibility = () => { if (!document.hidden) update(); };
        const timer = setInterval(update, 30000);
        document.addEventListener('visibilitychange', visibility);
        window.fn.clockWatchers.set(id, {timer, visibility});
        update();
        return id;
    },
    stopClock(id) {
        const watch = window.fn.clockWatchers.get(id);
        if (!watch) return;
        clearInterval(watch.timer);
        document.removeEventListener('visibilitychange', watch.visibility);
        window.fn.clockWatchers.delete(id);
    }
};
