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
});
