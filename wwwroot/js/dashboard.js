$(document).ready(function () {
    // Sidebar toggle
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Initialize charts if they exist
    if ($('#occupancyChart').length) {
        var occupancyOptions = {
            series: [{
                name: 'Occupancy',
                data: [30, 40, 35, 50, 49, 60, 70]
            }],
            chart: {
                type: 'area',
                height: 350,
                toolbar: {
                    show: false
                }
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth'
            },
            xaxis: {
                categories: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
            },
            tooltip: {
                x: {
                    format: 'dd/MM/yy HH:mm'
                }
            },
            colors: ['#4e73df']
        };

        var occupancyChart = new ApexCharts(document.querySelector("#occupancyChart"), occupancyOptions);
        occupancyChart.render();
    }

    if ($('#revenueChart').length) {
        var revenueOptions = {
            series: [{
                name: 'Revenue',
                data: [31000, 40000, 35000, 50000, 49000, 60000, 70000]
            }],
            chart: {
                type: 'bar',
                height: 350,
                toolbar: {
                    show: false
                }
            },
            plotOptions: {
                bar: {
                    borderRadius: 4,
                    horizontal: false,
                }
            },
            dataLabels: {
                enabled: false
            },
            xaxis: {
                categories: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
            },
            colors: ['#1cc88a']
        };

        var revenueChart = new ApexCharts(document.querySelector("#revenueChart"), revenueOptions);
        revenueChart.render();
    }

    // Add animation classes to cards
    $('.dashboard-card').addClass('animate__animated animate__fadeIn');

    // Handle dropdown menus
    $('.dropdown-toggle').on('click', function (e) {
        e.preventDefault();
        $(this).next('.dropdown-menu').slideToggle(300);
    });

    // Handle form submissions with loading state
    $('form').on('submit', function () {
        $(this).find('button[type="submit"]').prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...');
    });
}); 