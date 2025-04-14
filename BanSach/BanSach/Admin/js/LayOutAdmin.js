// Toggle sidebar and main content
const toggle = document.querySelector('.toggle');
const sidebar = document.querySelector('.sidebar');
const main = document.querySelector('.main');

toggle.onclick = function () {
    sidebar.classList.toggle('active');
    main.classList.toggle('active');
};

// Toggle submenu (parent menu)
const menuToggles = document.querySelectorAll('.menu-toggle');
menuToggles.forEach(toggle => {
    toggle.onclick = function (e) {
        e.preventDefault(); // Prevent default anchor behavior
        const parent = this.parentElement;
        parent.classList.toggle('active');

        // Optional: Close other open submenus
        menuToggles.forEach(otherToggle => {
            const otherParent = otherToggle.parentElement;
            if (otherParent !== parent && otherParent.classList.contains('active')) {
                otherParent.classList.remove('active');
            }
        });
    };
});

// Handle submenu item clicks and persist active state
const submenuLinks = document.querySelectorAll('.submenu-link');
submenuLinks.forEach(link => {
    link.onclick = function (e) {
        // Remove 'active' from all other submenu links
        submenuLinks.forEach(otherLink => {
            if (otherLink !== this) {
                otherLink.classList.remove('active');
            }
        });
        // Add 'active' to the clicked submenu link
        this.classList.add('active');
        // No e.preventDefault() here, allowing navigation
    };
});

// On page load, check current URL and set active states
document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname; // e.g., "/khachhangs/index"

    submenuLinks.forEach(link => {
        const linkPath = link.getAttribute('href'); // e.g., "~/khachhangs/index"

        // Normalize paths for comparison (remove "~" if present and handle relative paths)
        const normalizedLinkPath = linkPath.startsWith('~') ? linkPath.slice(1) : linkPath;
        const normalizedCurrentPath = currentPath.startsWith('/') ? currentPath : '/' + currentPath;

        if (normalizedLinkPath === normalizedCurrentPath) {
            // Add 'active' to the matching submenu link
            link.classList.add('active');
            // Add 'active' to the parent <li> to expand the submenu
            const parentLi = link.closest('li');
            if (parentLi) {
                parentLi.classList.add('active');
            }
        }
    });
});