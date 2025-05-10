// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Toggle sidebar
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    // Initialize dropdown toggles for sidebar menus
    $('.dropdown-toggle').on('click', function () {
        $(this).next('.collapse').collapse('toggle');
    });

    // Auto-expand dropdown menus based on current URL
    const currentUrl = window.location.pathname.toLowerCase();
    const menuItems = $('.list-unstyled li a');
    
    menuItems.each(function() {
        const menuUrl = $(this).attr('href')?.toLowerCase();
        if (menuUrl && currentUrl.includes(menuUrl) && menuUrl !== '/') {
            $(this).addClass('active');
            
            // If it's a submenu item, expand the parent menu
            const parentMenu = $(this).closest('.collapse');
            if (parentMenu.length) {
                parentMenu.addClass('show');
                parentMenu.prev('.dropdown-toggle').attr('aria-expanded', 'true');
            }
        }
    });
});
