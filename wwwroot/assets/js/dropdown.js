document.getElementById('openDropdown').classList.add('open');

document.querySelectorAll('.open-dropdown').forEach(toggle => {
    toggle.addEventListener('click', function () {
        this.parentElement.classList.toggle('open');
    });
});
