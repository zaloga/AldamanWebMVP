/**
 * Quill RTE Integration for Aldaman Admin
 * Handles initialization and synchronization of Quill editors with ASP.NET Core model fields.
 */
document.addEventListener('DOMContentLoaded', function () {
    const editors = document.querySelectorAll('.quill-editor-container');

    editors.forEach(container => {
        const culture = container.getAttribute('data-culture');
        const index = container.getAttribute('data-index');
        
        // Find the hidden fields by their generated IDs or names
        // ASP.NET Core generates IDs like: Translations_0__BodyHtml, Translations_0__BodyDeltaJson, Translations_0__PlainText
        const htmlInput = document.querySelector(`#Translations_${index}__BodyHtml`);
        const deltaInput = document.querySelector(`#Translations_${index}__BodyDeltaJson`);
        const plainTextInput = document.querySelector(`#Translations_${index}__PlainText`);

        if (!htmlInput || !deltaInput) return;

        // Initialize Quill
        const quill = new Quill(container, {
            theme: 'snow',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, 4, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    ['blockquote', 'code-block'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    [{ 'script': 'sub' }, { 'script': 'super' }],
                    ['link', 'image'],
                    ['clean']
                ]
            }
        });

        // Load initial content if available (prefer Delta, fallback to HTML)
        if (deltaInput.value) {
            try {
                quill.setContents(JSON.parse(deltaInput.value));
            } catch (e) {
                console.error('Error parsing Delta JSON for ' + culture, e);
                if (htmlInput.value) {
                    quill.root.innerHTML = htmlInput.value;
                }
            }
        } else if (htmlInput.value) {
            quill.root.innerHTML = htmlInput.value;
        }

        // Sync content on change
        quill.on('text-change', function () {
            const html = quill.root.innerHTML;
            const delta = JSON.stringify(quill.getContents());
            const text = quill.getText();

            // Only sync if content is not just a single newline (default empty state of Quill)
            const isEmpty = quill.getText().trim().length === 0 && quill.root.innerHTML === '<p><br></p>';
            
            htmlInput.value = isEmpty ? '' : html;
            deltaInput.value = isEmpty ? '' : delta;
            if (plainTextInput) {
                plainTextInput.value = isEmpty ? '' : text.trim();
            }
        });
    });
});
