function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
}

// Initialize theme on page load
window.initializeTheme = function () {
    const savedTheme = localStorage.getItem('theme') || 'dark';
    setTheme(savedTheme);
};
