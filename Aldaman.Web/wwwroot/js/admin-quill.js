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
            const i18n = window.AdminI18n || {};
            const input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');
            input.click();

            input.onchange = async () => {
                const file = input.files[0];
                if (!file) return;

                if (file.size > 1024 * 1024) {
                    alert(i18n.fileSizeExceedsLimit);
                    return;
                }

                const formData = new FormData();
                formData.append('file', file);

                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

                try {
                    const response = await fetch('/Admin/Media/UploadQuill', {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'RequestVerificationToken': token
                        }
                    });
                    
                    if (!response.ok) {
                        throw new Error(`Server returned ${response.status}: ${response.statusText}`);
                    }
                    
                    const result = await response.json();
                    
                    if (result.success) {
                        const range = quill.getSelection(true);
                        quill.insertEmbed(range.index, 'image', result.url);
                        quill.setSelection(range.index + 1);
                    } else {
                        alert(result.message || i18n.imageUploadFailed);
                    }
                } catch (error) {
                    console.error('Error uploading image:', error);
                    alert(i18n.errorUploadingImage);
                }
            };
        };

        // Helper to prompt native HTML5 color picker
        const customColorHandler = function(format) {
            const input = document.createElement('input');
            input.type = 'color';
            input.value = '#000000';
            input.click();
            input.onchange = () => {
                quill.format(format, input.value);
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
                        image: imageHandler,
                        color: function(value) {
                            if (value === 'custom') {
                                openNativeColorPicker('color', this.quill);
                            } else {
                                this.quill.format('color', value);
                            }
                        },
                        background: function(value) {
                            if (value === 'custom') {
                                openNativeColorPicker('background', this.quill);
                            } else {
                                this.quill.format('background', value);
                            }
                        }
                    }
                }
            }
        });

        // Apply localized tooltips to toolbar buttons and pickers
        addQuillTooltips(quill);

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

/**
 * Prompts the native HTML5 color picker placed in the center of the viewport.
 */
function openNativeColorPicker(format, quill, event) {
    const input = document.createElement('input');
    input.type = 'color';
    input.value = '#000000';
    
    // Position fixed in center of viewport so popup opens centered
    input.style.position = 'fixed';
    input.style.top = '50%';
    input.style.left = '50%';
    input.style.transform = 'translate(-50%, -50%)';
    input.style.width = '1px';
    input.style.height = '1px';
    input.style.opacity = '0';
    input.style.pointerEvents = 'none';
    input.style.zIndex = '999999';

    document.body.appendChild(input);

    const cleanup = () => {
        if (input.parentNode) {
            input.parentNode.removeChild(input);
        }
    };

    input.onchange = () => {
        quill.format(format, input.value);
        cleanup();
    };

    input.onblur = cleanup;

    if (typeof input.showPicker === 'function') {
        input.showPicker();
    } else {
        input.click();
    }
}

/**
 * Attaches localized title attributes to Quill toolbar controls.
 */
function addQuillTooltips(quill) {
    const i18n = window.AdminI18n || {};
    const tooltips = i18n.quillTooltips || {};
    const toolbarModule = quill.getModule('toolbar');
    if (!toolbarModule || !toolbarModule.container) return;

    const toolbar = toolbarModule.container;

    // Attach titles to standard toolbar buttons
    toolbar.querySelectorAll('button').forEach(button => {
        for (const [selector, text] of Object.entries(tooltips)) {
            if (!text) continue;
            if (button.classList.contains(`ql-${selector}`) || button.matches(`.ql-${selector}`)) {
                button.setAttribute('title', text);
                break;
            }
        }
    });

    // Attach titles to picker dropdowns (Font, Size, Align, Header, Color, Background)
    toolbar.querySelectorAll('.ql-picker').forEach(picker => {
        for (const [selector, text] of Object.entries(tooltips)) {
            if (!text) continue;
            if (picker.classList.contains(`ql-${selector}`)) {
                const label = picker.querySelector('.ql-picker-label');
                if (label) {
                    label.setAttribute('title', text);
                }
                break;
            }
        }
    });

    // Add custom color picker swatch button to color and background pickers
    setupCustomColorPickerSwatches(quill, toolbar);
}

/**
 * Appends a styled custom color button to color & background picker dropdowns.
 */
function setupCustomColorPickerSwatches(quill, toolbar) {
    const i18n = window.AdminI18n || {};
    const labelText = (i18n.quillTooltips && i18n.quillTooltips.customColor) || 'Vlastní barva';

    ['color', 'background'].forEach(format => {
        const picker = toolbar.querySelector(`.ql-picker.ql-${format}`);
        if (!picker) return;

        const optionsContainer = picker.querySelector('.ql-picker-options');
        if (!optionsContainer || optionsContainer.querySelector('.ql-custom-color-btn')) return;

        // Create custom button inside dropdown (cleared below grid floats)
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'ql-custom-color-btn';
        btn.setAttribute('title', labelText);
        
        btn.style.clear = 'both';
        btn.style.display = 'flex';
        btn.style.alignItems = 'center';
        btn.style.justifyContent = 'center';
        btn.style.gap = '6px';
        btn.style.width = '100%';
        btn.style.height = '28px';
        btn.style.marginTop = '6px';
        btn.style.padding = '0 8px';
        btn.style.boxSizing = 'border-box';
        btn.style.border = '1px solid #ced4da';
        btn.style.borderRadius = '4px';
        btn.style.background = '#f8f9fa';
        btn.style.cursor = 'pointer';
        btn.style.fontSize = '12px';
        btn.style.lineHeight = '1';
        btn.style.color = '#333';
        btn.style.fontWeight = '500';
        btn.style.outline = 'none';

        btn.innerHTML = `<span style="display:inline-block; width:12px; height:12px; border-radius:50%; background: conic-gradient(red, yellow, lime, aqua, blue, magenta, red); flex-shrink:0;"></span>
                         <span>${labelText}</span>`;

        btn.addEventListener('mouseenter', () => {
            btn.style.background = '#e9ecef';
            btn.style.borderColor = '#adb5bd';
            btn.style.color = '#0d6efd';
        });

        btn.addEventListener('mouseleave', () => {
            btn.style.background = '#f8f9fa';
            btn.style.borderColor = '#ced4da';
            btn.style.color = '#333';
        });

        btn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            // Close picker dropdown
            picker.classList.remove('ql-expanded');
            openNativeColorPicker(format, quill, e);
        });

        optionsContainer.appendChild(btn);
    });
}

