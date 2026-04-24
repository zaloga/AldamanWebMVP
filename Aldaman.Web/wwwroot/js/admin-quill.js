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

        // Custom Image Handler for AJAX Upload
        const imageHandler = function() {
            const input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');
            input.click();

            input.onchange = async () => {
                const file = input.files[0];
                if (!file) return;

                if (file.size > 1024 * 1024) {
                    alert('File size exceeds the 1 MB limit.');
                    return;
                }

                const formData = new FormData();
                formData.append('file', file);

                try {
                    const response = await fetch('/Admin/Media/UploadQuill', {
                        method: 'POST',
                        body: formData
                    });
                    
                    const result = await response.json();
                    
                    if (result.success) {
                        const range = quill.getSelection(true);
                        quill.insertEmbed(range.index, 'image', result.url);
                        quill.setSelection(range.index + 1);
                    } else {
                        alert(result.message || 'Image upload failed.');
                    }
                } catch (error) {
                    console.error('Error uploading image:', error);
                    alert('Error uploading image.');
                }
            };
        };

        // Initialize Quill
        const quill = new Quill(container, {
            theme: 'snow',
            modules: {
                toolbar: {
                    container: [
                        [{ 'font': [] }, { 'size': [] }],
                        ['bold', 'italic', 'underline', 'strike'],
                        [{ 'color': [] }, { 'background': [] }],
                        [{ 'script': 'sub' }, { 'script': 'super' }],
                        [{ 'header': 1 }, { 'header': 2 }, { 'header': 3 }, { 'header': 4 }, 'blockquote', 'code-block'],
                        [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'indent': '-1' }, { 'indent': '+1' }],
                        [{ 'direction': 'rtl' }, { 'align': [] }],
                        ['link', 'image', 'video', 'formula'],
                        ['clean']
                    ],
                    handlers: {
                        image: imageHandler
                    }
                }
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
