// site.js
// Small enhancements for layout: sidebar toggle, persistence, mobile overlay and active link highlighting

(function () {
    const body = document.body;
    const sidebar = document.getElementById('sidebar');
    const layoutWrapper = document.getElementById('layout-wrapper');

    // Create a mobile overlay element (will be shown when sidebar opened on small screens)
    let overlay = document.querySelector('.sidebar-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'sidebar-overlay';
        document.body.appendChild(overlay);
    }

    // Apply stored collapsed state (persist between sessions)
    const COLLAPSE_KEY = 'oc.sidebarCollapsed';
    const SIDEBAR_OPEN_KEY = 'oc.sidebarOpenMobile';

    function applyCollapsedState() {
        const collapsed = localStorage.getItem(COLLAPSE_KEY) === '1';
        document.body.classList.toggle('sidebar-collapsed', collapsed);
    }

    applyCollapsedState();

    // Toggle collapse (desktop)
    window.toggleSidebarCollapse = function () {
        const isCollapsed = document.body.classList.toggle('sidebar-collapsed');
        localStorage.setItem(COLLAPSE_KEY, isCollapsed ? '1' : '0');
    };

    // Toggle mobile sidebar (slide in/out)
    window.openMobileSidebar = function (open) {
        if (typeof open === 'undefined') open = true;
        document.body.classList.toggle('sidebar-open', open);
        localStorage.setItem(SIDEBAR_OPEN_KEY, open ? '1' : '0');
        overlay.style.display = open ? 'block' : 'none';
    };

    // Close mobile sidebar on overlay click
    overlay.addEventListener('click', function () {
        openMobileSidebar(false);
    });

    // Highlight active link in sidebar based on current path
    function highlightActiveNav() {
        try {
            const links = document.querySelectorAll('#sidebar .nav-link');
            const path = window.location.pathname || '/';
            links.forEach(a => {
                // some links may be relative, do robust check by pathname equality or endsWith
                const href = a.getAttribute('href') || a.dataset.href || '';
                // Normalize
                const url = new URL(href, window.location.origin);
                if (url.pathname === path || path.endsWith(url.pathname) || url.pathname.endsWith(path)) {
                    a.classList.add('active');
                    a.setAttribute('aria-current', 'page');
                } else {
                    a.classList.remove('active');
                    a.removeAttribute('aria-current');
                }
            });
        } catch (e) {
            // ignore
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        highlightActiveNav();

        // Close mobile sidebar when clicking a link inside it (improves UX)
        document.querySelectorAll('#sidebar .nav-link').forEach(a => {
            a.addEventListener('click', function () {
                if (window.innerWidth < 992) {
                    openMobileSidebar(false);
                }
            });
        });

        // If stored mobile-open state exists, restore (useful during dev)
        if (localStorage.getItem(SIDEBAR_OPEN_KEY) === '1' && window.innerWidth < 992) {
            openMobileSidebar(true);
        }
    });

    // Keyboard shortcut: press "Ctrl+B" to toggle sidebar collapse (desktop)
    document.addEventListener('keydown', function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'b') {
            e.preventDefault();
            toggleSidebarCollapse();
        }
    });

})();