document.addEventListener('DOMContentLoaded', function () {
    const themeToggleDarkIcon = document.getElementById('theme-toggle-dark-icon');
    const themeToggleLightIcon = document.getElementById('theme-toggle-light-icon');
    const themeToggleBtn = document.getElementById('theme-toggle');

    if (!themeToggleBtn) return;

    // Change the icons inside the button based on previous settings
    if (localStorage.getItem('theme') === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        themeToggleLightIcon.classList.remove('hidden');
    } else {
        themeToggleDarkIcon.classList.remove('hidden');
    }

    themeToggleBtn.addEventListener('click', function () {
        // toggle icons inside button
        themeToggleDarkIcon.classList.toggle('hidden');
        themeToggleLightIcon.classList.toggle('hidden');

        // if set via local storage previously
        if (localStorage.getItem('theme')) {
            if (localStorage.getItem('theme') === 'light') {
                document.documentElement.classList.add('dark');
                localStorage.setItem('theme', 'dark');
            } else {
                document.documentElement.classList.remove('dark');
                localStorage.setItem('theme', 'light');
            }

            // if NOT set via local storage previously
        } else {
            if (document.documentElement.classList.contains('dark')) {
                document.documentElement.classList.remove('dark');
                localStorage.setItem('theme', 'light');
            } else {
                document.documentElement.classList.add('dark');
                localStorage.setItem('theme', 'dark');
            }
        }
    });

    // Search Autocomplete
    const searchInput = document.getElementById('site-search');
    const searchResults = document.getElementById('search-results');
    let debounceTimer;

    if (searchInput && searchResults) {
        searchInput.addEventListener('input', function () {
            const query = this.value.trim();
            const culture = this.getAttribute('data-culture') || 'cs';

            clearTimeout(debounceTimer);
            if (query.length < 2) {
                searchResults.innerHTML = '';
                searchResults.classList.add('hidden');
                return;
            }

            debounceTimer = setTimeout(async () => {
                try {
                    const response = await fetch(`/api/search/autocomplete?query=${encodeURIComponent(query)}&culture=${culture}`);
                    if (!response.ok) throw new Error('Search failed');

                    const data = await response.json();
                    renderResults(data.results);
                } catch (error) {
                    console.error('Autocomplete error:', error);
                }
            }, 300);
        });

        // Close results when clicking outside
        document.addEventListener('click', function (e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.classList.add('hidden');
            }
        });

        searchInput.addEventListener('focus', function () {
            if (searchResults.children.length > 0) {
                searchResults.classList.remove('hidden');
            }
        });
    }

    function renderResults(results) {
        if (!results || results.length === 0) {
            searchResults.innerHTML = `<div class="p-4 text-xs text-slate-500 dark:text-slate-400 text-center italic">${document.documentElement.lang === 'cs' ? 'Nenalezeny žádné výsledky' : 'No results found'}</div>`;
            searchResults.classList.remove('hidden');
            return;
        }

        const html = results.map(item => `
            <a href="${item.url}" class="block p-4 hover:bg-slate-50 dark:hover:bg-slate-700/50 transition-colors border-b last:border-0 border-slate-100 dark:border-slate-700">
                <div class="text-sm font-semibold text-slate-900 dark:text-white">${item.title}</div>
            </a>
        `).join('');

        searchResults.innerHTML = html;
        searchResults.classList.remove('hidden');
    }

    // Translation Missing Toast Initialization
    const toast = document.getElementById('translation-missing-toast');
    if (toast) {
        setTimeout(() => {
            toast.classList.remove('-translate-y-10', 'opacity-0');
            toast.classList.add('translate-y-0', 'opacity-100');
        }, 100);

        const duration = 10000;
        const interval = 100;
        let elapsed = 0;
        const progressBar = document.getElementById('toast-progress');
        const closeBtn = document.getElementById('close-toast-btn');

        const timer = setInterval(() => {
            elapsed += interval;
            const percentage = Math.max(0, 100 - (elapsed / duration) * 100);
            if (progressBar) {
                progressBar.style.width = percentage + '%';
            }
            if (elapsed >= duration) {
                dismissToast();
            }
        }, interval);

        function dismissToast() {
            clearInterval(timer);
            toast.classList.remove('translate-y-0', 'opacity-100');
            toast.classList.add('-translate-y-10', 'opacity-0');
            setTimeout(() => {
                const wrapper = document.getElementById('toast-wrapper');
                if (wrapper) {
                    wrapper.remove();
                } else {
                    toast.remove();
                }
            }, 500);
        }

        if (closeBtn) {
            closeBtn.addEventListener('click', dismissToast);
        }
    }
});
