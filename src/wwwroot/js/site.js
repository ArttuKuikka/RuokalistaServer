// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Set a cookie
function setCookie(name, value) {
    document.cookie = `${name}=${value};path=/`; // No expiration date, session cookie
}

// Get a cookie
function getCookie(name) {
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i].trim();
        if (cookie.startsWith(`${name}=`)) {
            return cookie.substring(name.length + 1);
        }
    }
    return null;
}

// Apply the theme and update toggle text based on the cookie
function applyThemeFromCookie() {
    const theme = getCookie('theme');
    const themeToggle = document.getElementById('theme-toggle');

    document.body.classList.remove('dark-mode', 'light-mode'); // Clear existing classes
    if (theme === 'dark-mode') {
        document.body.classList.add('dark-mode');
        if (themeToggle) themeToggle.textContent = '🌗'; // Set toggle text for dark mode
    } else {
        document.body.classList.add('light-mode');
        if (themeToggle) themeToggle.textContent = '🌓'; // Set toggle text for light mode
    }
}

// Toggle theme and update the cookie and toggle text
function toggleTheme() {
    const themeToggle = document.getElementById('theme-toggle');
    if (document.body.classList.contains('dark-mode')) {
        document.body.classList.remove('dark-mode');
        document.body.classList.add('light-mode');
        setCookie('theme', 'light-mode');
        if (themeToggle) themeToggle.textContent = '🌓';
    } else {
        document.body.classList.remove('light-mode');
        document.body.classList.add('dark-mode');
        setCookie('theme', 'dark-mode');
        if (themeToggle) themeToggle.textContent = '🌗';
    }
}

// Initialize the theme on page load
document.addEventListener('DOMContentLoaded', () => {
    applyThemeFromCookie();

    const themeToggle = document.getElementById('theme-toggle');
    if (themeToggle) {
        themeToggle.addEventListener('click', (event) => {
            event.preventDefault();
            toggleTheme();
        });
    }
});


