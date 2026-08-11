document.addEventListener('DOMContentLoaded', function () {
    function getCulture() {
        const pathSegment = window.location.pathname.split('/')[1];
        if (pathSegment && pathSegment.length === 2) {
            return pathSegment;
        }
        return document.documentElement.lang || 'cs';
    }

    document.querySelectorAll('.js-show-more-btn').forEach(btn => {
        btn.addEventListener('click', async function (e) {
            e.preventDefault();
            e.stopPropagation();

            const button = this;
            const slug = button.getAttribute('data-slug');
            if (!slug) return;

            const card = button.closest('article');
            if (!card) return;

            const perexText = card.querySelector('.js-perex-text');
            const fullContentContainer = card.querySelector('.js-full-content-container');
            const showLessBtn = card.querySelector('.js-show-less-btn');
            const spinnerIcon = button.querySelector('.js-spinner-icon');
            const culture = getCulture();

            if (card.dataset.loadedContent === "true") {
                if (perexText) perexText.classList.remove('line-clamp-3');
                button.classList.add('hidden');
                if (fullContentContainer) {
                    fullContentContainer.classList.remove('hidden');
                    requestAnimationFrame(() => {
                        fullContentContainer.classList.remove('opacity-0');
                    });
                }
                if (showLessBtn) showLessBtn.classList.remove('hidden');
                return;
            }

            button.disabled = true;
            if (spinnerIcon) spinnerIcon.classList.remove('hidden');

            try {
                const response = await fetch(`/${culture}/blog/content/${encodeURIComponent(slug)}`);
                if (!response.ok) {
                    throw new Error('Failed to fetch content');
                }
                const data = await response.json();

                if (data && data.bodyHtml) {
                    fullContentContainer.innerHTML = data.bodyHtml;
                    card.dataset.loadedContent = "true";

                    if (perexText) perexText.classList.remove('line-clamp-3');
                    button.classList.add('hidden');
                    fullContentContainer.classList.remove('hidden');
                    requestAnimationFrame(() => {
                        fullContentContainer.classList.remove('opacity-0');
                    });
                    if (showLessBtn) showLessBtn.classList.remove('hidden');
                }
            } catch (err) {
                console.error('Error fetching blog content:', err);
                window.location.href = `/${culture}/blog/${slug}`;
            } finally {
                button.disabled = false;
                if (spinnerIcon) spinnerIcon.classList.add('hidden');
            }
        });
    });

    document.querySelectorAll('.js-show-less-btn').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const card = this.closest('article');
            if (!card) return;

            const perexText = card.querySelector('.js-perex-text');
            const fullContentContainer = card.querySelector('.js-full-content-container');
            const showMoreBtn = card.querySelector('.js-show-more-btn');

            this.classList.add('hidden');
            if (fullContentContainer) {
                fullContentContainer.classList.add('opacity-0');
                fullContentContainer.classList.add('hidden');
            }
            if (perexText) perexText.classList.add('line-clamp-3');
            if (showMoreBtn) showMoreBtn.classList.remove('hidden');
        });
    });
});
