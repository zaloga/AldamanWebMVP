/**
 * Slug Generation for Aldaman Admin
 */
document.addEventListener('DOMContentLoaded', function () {
    // Delegate click event to handle dynamically added elements or multiple tabs
    document.addEventListener('click', async function (event) {
        const button = event.target.closest('.btn-generate-slug');
        if (!button) return;

        event.preventDefault();

        // The button should be inside a container (like a tab-pane) 
        // that also contains the Title and Slug inputs
        const container = button.closest('.tab-pane') || button.closest('form');
        if (!container) return;

        // Find the title input and slug input within this container
        // We use startsWith/endsWith or relative selectors because IDs include indices like Translations_0__Title
        const titleInput = container.querySelector('input[id$="__Title"]');
        const slugInput = container.querySelector('input[id$="__Slug"]');

        if (!titleInput || !slugInput) {
            console.error('Title or Slug input not found in context.');
            return;
        }

        const title = titleInput.value;
        if (!title) return;

        button.disabled = true;
        const originalHtml = button.innerHTML;
        button.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';

        try {
            const response = await fetch(`/Admin/Tools/GenerateSlug?text=${encodeURIComponent(title)}`);
            if (!response.ok) throw new Error('Network response was not ok');
            
            const data = await response.json();
            slugInput.value = data.slug;
            
            // Trigger input event to notify any other listeners (like validation)
            slugInput.dispatchEvent(new Event('input', { bubbles: true }));
        } catch (error) {
            console.error('Error generating slug:', error);
        } finally {
            button.disabled = false;
            button.innerHTML = originalHtml;
        }
    });
});
