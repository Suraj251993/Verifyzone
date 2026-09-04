(function () {
    var STORAGE_KEY = 'vzTheme';

    function applyTheme(theme) {
        if (theme === 'dark') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
    }

    // Default is always light unless the user has explicitly toggled dark before.
    var saved = localStorage.getItem(STORAGE_KEY);
    applyTheme(saved === 'dark' ? 'dark' : 'light');

    window.toggleVzTheme = function () {
        var isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        var next = isDark ? 'light' : 'dark';
        applyTheme(next);
        localStorage.setItem(STORAGE_KEY, next);
        if (window.onVzThemeChange) window.onVzThemeChange(next);
    };

    window.isVzThemeDark = function () {
        return document.documentElement.getAttribute('data-theme') === 'dark';
    };
})();
