$(document).ready(function () {
    // Load categories for dropdown
    loadCategories();

    // Load total news count
    loadNewsCount();

    // Search form
    $('#searchForm').submit(function (e) {
        e.preventDefault();
        const query = $('#searchInput').val().trim();
        if (query) {
            window.location.href = `/News/Search?query=${encodeURIComponent(query)}`;
        }
    });

    // Auto-hide alerts after 5 seconds
    $('.alert').delay(5000).fadeOut(300);

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function (e) {
        if (this.hash !== "") {
            e.preventDefault();
            const hash = this.hash;
            $('html, body').animate({
                scrollTop: $(hash).offset().top - 70
            }, 800);
        }
    });
});

// Load categories via AJAX
function loadCategories() {
    $.ajax({
        url: '/?handler=categories',
        type: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (categories) {
            const menu = $('#categoriesMenu');
            menu.empty();

            if (categories && categories.length > 0) {
                categories.forEach(function (category) {
                    menu.append(`
                        <a class="dropdown-item" href="/News/Category/${category.id}">
                            <i class="fas fa-folder me-2"></i>${category.name}
                        </a>
                    `);
                });

                menu.append('<div class="dropdown-divider"></div>');
                menu.append(`
                    <a class="dropdown-item text-primary" href="/News/Category/All">
                        <i class="fas fa-eye me-2"></i>View All Categories
                    </a>
                `);
            } else {
                menu.append('<div class="dropdown-item text-muted">No categories found</div>');
            }
        },
        error: function () {
            $('#categoriesMenu').html('<div class="dropdown-item text-danger">Failed to load categories</div>');
        }
    });
}

// Load total news count
function loadNewsCount() {
    $.ajax({
        url: '/?handler=newscount',
        type: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (data) {
            $('#totalNewsCount').text(data.count);
        }
    });
}

// Filter news by category (AJAX version)
function filterNewsByCategory(categoryId) {
    const $newsContainer = $('#newsContainer');
    const $loading = $('#loadingSpinner');

    $loading.show();
    $newsContainer.fadeOut(200);

    $.ajax({
        url: `/News?handler=filter&categoryId=${categoryId}`,
        type: 'GET',
        success: function (html) {
            setTimeout(function () {
                $newsContainer.html(html).fadeIn(300);
                $loading.hide();

                // Update browser URL without reload
                if (categoryId) {
                    history.pushState(null, '', `/News/Category/${categoryId}`);
                } else {
                    history.pushState(null, '', '/News');
                }
            }, 300);
        },
        error: function () {
            $loading.hide();
            $newsContainer.html(`
                <div class="alert alert-danger">
                    Failed to load news. Please try again.
                </div>
            `).fadeIn(300);
        }
    });
}

// Sort news (AJAX version)
function sortNews(sortBy) {
    const $newsContainer = $('#newsContainer');
    const $loading = $('#loadingSpinner');

    $loading.show();
    $newsContainer.fadeOut(200);

    $.ajax({
        url: `/News?handler=sort&sortBy=${sortBy}`,
        type: 'GET',
        success: function (html) {
            setTimeout(function () {
                $newsContainer.html(html).fadeIn(300);
                $loading.hide();

                // Update active sort button
                $('.sort-btn').removeClass('active');
                $(`[data-sort="${sortBy}"]`).addClass('active');
            }, 300);
        }
    });
}

// Increment views for news (called when viewing news details)
function incrementViews(newsId) {
    if (!sessionStorage.getItem(`viewed_${newsId}`)) {
        $.ajax({
            url: `/News/Details/${newsId}?handler=incrementviews`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val(),
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function () {
                sessionStorage.setItem(`viewed_${newsId}`, 'true');
            }
        });
    }
}

// Toggle favorite news
function toggleFavorite(newsId, element) {
    const $btn = $(element);
    const $icon = $btn.find('i');
    const $count = $btn.find('.favorite-count');

    $.ajax({
        url: `/News/Details/${newsId}?handler=togglefavorite`,
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val(),
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            if (response.isFavorite) {
                $btn.addClass('active');
                $icon.removeClass('far').addClass('fas text-danger');
                // Animation
                $btn.addClass('pulse');
                setTimeout(() => $btn.removeClass('pulse'), 300);
            } else {
                $btn.removeClass('active');
                $icon.removeClass('fas text-danger').addClass('far');
            }

            if ($count.length) {
                $count.text(response.favoriteCount);
            }
        }
    });
}

// Submit comment via AJAX
function submitComment(newsId) {
    const $form = $('#commentForm');
    const $content = $('#commentContent');
    const content = $content.val().trim();

    if (!content) {
        alert('Please enter a comment');
        return;
    }

    $.ajax({
        url: `/News/Details/${newsId}?handler=addcomment`,
        type: 'POST',
        data: {
            content: content,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            if (response.success) {
                // Add comment to list
                $('#commentsList').prepend(`
                    <div class="comment-item fade-in">
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <i class="fas fa-user-circle fa-2x text-muted"></i>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <div class="d-flex justify-content-between">
                                    <h6 class="mb-1">${response.userName}</h6>
                                    <small class="text-muted">Just now</small>
                                </div>
                                <p class="mb-0">${response.content}</p>
                            </div>
                        </div>
                    </div>
                `);

                // Clear form
                $content.val('');

                // Update comment count
                const $count = $('#commentCount');
                if ($count.length) {
                    $count.text(parseInt($count.text()) + 1);
                }
            }
        },
        error: function () {
            alert('Failed to post comment. Please try again.');
        }
    });
}

// Load more news (pagination)
let currentPage = 1;
function loadMoreNews() {
    const $loadMoreBtn = $('#loadMoreBtn');
    const $loading = $('#loadingMore');

    $loadMoreBtn.hide();
    $loading.show();

    $.ajax({
        url: `/News?handler=loadmore&page=${currentPage + 1}`,
        type: 'GET',
        success: function (response) {
            if (response.html && response.html.trim() !== '') {
                $('#newsContainer').append(response.html);
                currentPage++;

                if (response.hasMore) {
                    $loadMoreBtn.show();
                }
            }
            $loading.hide();
        },
        error: function () {
            $loading.hide();
            $loadMoreBtn.show();
            alert('Failed to load more news.');
        }
    });
}

// Initialize tooltips
$(function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
});

// Initialize popovers
$(function () {
    $('[data-bs-toggle="popover"]').popover();
});