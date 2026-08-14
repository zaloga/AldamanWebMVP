// Extend Quill's default Image Blot to persist width, height, style, alt, and title
// and register custom GalleryBlot for photo galleries
(function () {
    if (typeof Quill !== 'undefined') {
        const BaseImage = Quill.import('formats/image');
        if (BaseImage) {
            class CustomImageBlot extends BaseImage {
                static create(value) {
                    const node = super.create(value);
                    if (typeof value === 'object' && value !== null) {
                        if (value.src) node.setAttribute('src', value.src);
                        if (value.width) node.setAttribute('width', value.width);
                        if (value.height) node.setAttribute('height', value.height);
                        if (value.style) node.setAttribute('style', value.style);
                        if (value.alt) node.setAttribute('alt', value.alt);
                        if (value.title) node.setAttribute('title', value.title);
                    } else if (typeof value === 'string') {
                        node.setAttribute('src', value);
                    }
                    return node;
                }

                static value(domNode) {
                    return {
                        src: domNode.getAttribute('src') || '',
                        width: domNode.getAttribute('width') || domNode.style.width || null,
                        height: domNode.getAttribute('height') || domNode.style.height || null,
                        style: domNode.getAttribute('style') || null,
                        alt: domNode.getAttribute('alt') || null,
                        title: domNode.getAttribute('title') || null
                    };
                }

                static formats(domNode) {
                    return {
                        width: domNode.getAttribute('width') || domNode.style.width || null,
                        height: domNode.getAttribute('height') || domNode.style.height || null,
                        style: domNode.getAttribute('style') || null,
                        alt: domNode.getAttribute('alt') || null,
                        title: domNode.getAttribute('title') || null
                    };
                }

                format(name, value) {
                    if (['width', 'height', 'style', 'alt', 'title'].includes(name)) {
                        if (value) {
                            this.domNode.setAttribute(name, value);
                        } else {
                            this.domNode.removeAttribute(name);
                        }
                    } else {
                        super.format(name, value);
                    }
                }
            }
            Quill.register(CustomImageBlot, true);
        }

        const BlockEmbed = Quill.import('blots/block/embed');
        if (BlockEmbed) {
            class GalleryBlot extends BlockEmbed {
                static create(value) {
                    const node = super.create(value);
                    node.setAttribute('contenteditable', 'false');
                    node.classList.add('quill-gallery');

                    const columns = (value && value.columns) ? value.columns : 'auto';
                    const gap = (value && value.gap) ? value.gap : 'md';
                    const lightbox = (value && value.lightbox !== false);
                    node.setAttribute('data-columns', columns);
                    node.setAttribute('data-gap', gap);
                    node.setAttribute('data-lightbox', lightbox ? 'true' : 'false');

                    const images = (value && Array.isArray(value.images)) ? value.images : [];
                    node.setAttribute('data-images-json', JSON.stringify(images));

                    // Build inner HTML for images
                    let innerHtml = '';
                    images.forEach(img => {
                        const src = typeof img === 'string' ? img : (img.src || img.url || '');
                        const alt = (img && img.alt) ? img.alt : '';
                        const title = (img && img.title) ? img.title : '';

                        if (lightbox) {
                            innerHtml += `<div class="quill-gallery-item"><a href="${src}" data-gallery="gallery" title="${title || alt}"><img src="${src}" alt="${alt}" title="${title}" loading="lazy" /></a></div>`;
                        } else {
                            innerHtml += `<div class="quill-gallery-item"><img src="${src}" alt="${alt}" title="${title}" loading="lazy" /></div>`;
                        }
                    });

                    node.innerHTML = innerHtml;
                    return node;
                }

                static value(domNode) {
                    let images = [];
                    try {
                        const raw = domNode.getAttribute('data-images-json');
                        if (raw) images = JSON.parse(raw);
                    } catch (e) {
                        domNode.querySelectorAll('img').forEach(img => {
                            images.push({
                                src: img.getAttribute('src') || '',
                                alt: img.getAttribute('alt') || '',
                                title: img.getAttribute('title') || ''
                            });
                        });
                    }

                    return {
                        columns: domNode.getAttribute('data-columns') || 'auto',
                        gap: domNode.getAttribute('data-gap') || 'md',
                        lightbox: domNode.getAttribute('data-lightbox') === 'true',
                        images: images
                    };
                }
            }

            GalleryBlot.blotName = 'gallery';
            GalleryBlot.tagName = 'div';
            GalleryBlot.className = 'quill-gallery';
            Quill.register(GalleryBlot, true);
        }
    }
})();

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
        const imageHandler = function () {
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

        // Initialize Quill
        const quill = new Quill(container, {
            theme: 'snow',
            modules: {
                table: true,
                toolbar: {
                    container: [
                        [{ 'font': [] }, { 'size': [] }],
                        ['bold', 'italic', 'underline', 'strike'],
                        [{ 'color': [] }, { 'background': [] }],
                        [{ 'script': 'sub' }, { 'script': 'super' }],
                        [{ 'header': 1 }, { 'header': 2 }, { 'header': 3 }, { 'header': 4 }, 'blockquote', 'code-block'],
                        [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'indent': '-1' }, { 'indent': '+1' }],
                        [{ 'direction': 'rtl' }, { 'align': [] }],
                        ['link', 'image', 'gallery', 'video', 'formula', 'table'],
                        ['clean']
                    ],
                    handlers: {
                        image: imageHandler,
                        gallery: function () {
                            openMediaGalleryModal(this.quill);
                        },
                        table: function () {
                            promptInsertTable(this.quill);
                        },
                        color: function (value) {
                            if (value === 'custom') {
                                openNativeColorPicker('color', this.quill);
                            } else {
                                this.quill.format('color', value);
                            }
                        },
                        background: function (value) {
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

        // Initialize Interactive Image Resizer & Formatting Controller
        initQuillImageResizer(quill);

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
 * Prompts the native HTML5 color picker positioned directly over the ql-picker-label button.
 */
function openNativeColorPicker(format, quill, event) {
    const input = document.createElement('input');
    input.type = 'color';

    // Determine initial color from current selection format if available
    let initialColor = format === 'background' ? '#ffffff' : '#000000';
    if (quill) {
        const formatState = quill.getFormat();
        const activeColor = formatState[format];
        if (typeof activeColor === 'string' && activeColor.trim().length > 0) {
            initialColor = formatColorToHex(activeColor, initialColor);
        }
    }
    input.value = initialColor;

    // Locate the .ql-picker-label button on the toolbar to position picker underneath it
    let pickerLabel = null;
    if (event && event.target && typeof event.target.closest === 'function') {
        const pickerElem = event.target.closest('.ql-picker');
        if (pickerElem) {
            pickerLabel = pickerElem.querySelector('.ql-picker-label') || pickerElem;
        }
    }
    if (!pickerLabel && quill) {
        const toolbarModule = typeof quill.getModule === 'function' ? quill.getModule('toolbar') : null;
        const toolbar = (toolbarModule && toolbarModule.container) ||
            (quill.container && quill.container.parentElement && quill.container.parentElement.querySelector('.ql-toolbar'));
        if (toolbar) {
            const pickerElem = toolbar.querySelector(`.ql-picker.ql-${format}`);
            if (pickerElem) {
                pickerLabel = pickerElem.querySelector('.ql-picker-label') || pickerElem;
            }
        }
    }

    input.style.position = 'fixed';
    if (pickerLabel) {
        const rect = pickerLabel.getBoundingClientRect();
        input.style.left = `${Math.round(rect.left)}px`;
        input.style.top = `${Math.round(rect.top)}px`;
        input.style.width = `${Math.max(Math.round(rect.width), 28)}px`;
        input.style.height = `${Math.max(Math.round(rect.height), 24)}px`;
        input.style.transform = 'none';
    } else {
        input.style.top = '50%';
        input.style.left = '50%';
        input.style.transform = 'translate(-50%, -50%)';
        input.style.width = '40px';
        input.style.height = '40px';
    }

    input.style.opacity = '0.01';
    input.style.margin = '0';
    input.style.padding = '0';
    input.style.border = 'none';
    input.style.outline = 'none';
    input.style.boxSizing = 'border-box';
    input.style.pointerEvents = 'auto';
    input.style.zIndex = '999999';

    document.body.appendChild(input);

    // Force reflow so Chrome layout engine calculates bounds before opening color chooser popup
    void input.offsetWidth;
    void input.getBoundingClientRect();

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
 * Converts a color string (HEX or RGB) into standard 7-character #RRGGBB format for <input type="color">.
 */
function formatColorToHex(colorStr, fallbackHex) {
    if (!colorStr) return fallbackHex;
    colorStr = colorStr.trim();
    if (/^#[0-9A-Fa-f]{6}$/.test(colorStr)) {
        return colorStr;
    }
    if (/^#[0-9A-Fa-f]{3}$/.test(colorStr)) {
        return '#' + colorStr[1] + colorStr[1] + colorStr[2] + colorStr[2] + colorStr[3] + colorStr[3];
    }
    const rgbMatch = colorStr.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)/i);
    if (rgbMatch) {
        const r = parseInt(rgbMatch[1], 10).toString(16).padStart(2, '0');
        const g = parseInt(rgbMatch[2], 10).toString(16).padStart(2, '0');
        const b = parseInt(rgbMatch[3], 10).toString(16).padStart(2, '0');
        return `#${r}${g}${b}`;
    }
    return fallbackHex;
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

    // Ensure table button has visual icon if missing
    const tableBtn = toolbar.querySelector('.ql-table');
    if (tableBtn) {
        if (!tableBtn.innerHTML || tableBtn.innerHTML.trim() === '') {
            tableBtn.innerHTML = '<svg viewBox="0 0 18 18"><rect class="ql-stroke" height="12" width="12" x="3" y="3" fill="none" stroke="currentColor" stroke-width="1.5"></rect><line class="ql-stroke" x1="3" x2="15" y1="9" y2="9" stroke="currentColor" stroke-width="1.5"></line><line class="ql-stroke" x1="9" x2="9" y1="3" y2="15" stroke="currentColor" stroke-width="1.5"></line></svg>';
        }
        if (!tableBtn.hasAttribute('title') && tooltips.table) {
            tableBtn.setAttribute('title', tooltips.table);
        }
    }

    // Ensure gallery button has visual icon if missing
    const galleryBtn = toolbar.querySelector('.ql-gallery');
    if (galleryBtn) {
        if (!galleryBtn.innerHTML || galleryBtn.innerHTML.trim() === '') {
            galleryBtn.innerHTML = '<svg viewBox="0 0 18 18"><rect class="ql-stroke" height="10" width="10" x="2" y="2" fill="none" stroke="currentColor" stroke-width="1.5"></rect><rect class="ql-stroke" height="10" width="10" x="6" y="6" fill="none" stroke="currentColor" stroke-width="1.5"></rect><circle class="ql-fill" cx="5" cy="5" r="1"></circle></svg>';
        }
        if (!galleryBtn.hasAttribute('title') && (tooltips.gallery || tooltips.insertGallery)) {
            galleryBtn.setAttribute('title', tooltips.gallery || tooltips.insertGallery);
        }
    }

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
    const labelText = (i18n.quillTooltips && i18n.quillTooltips.customColor);

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

/**
 * Prompts user for custom table rows and columns before insertion.
 */
function promptInsertTable(quill) {
    const tableModule = quill.getModule('table');
    if (!tableModule) return;

    let range = quill.getSelection();
    if (!range) {
        quill.focus();
        range = quill.getSelection(true);
    }

    const i18n = window.AdminI18n || {};
    const tooltips = i18n.quillTooltips || {};
    const title = tooltips.table;
    const rowsLabel = tooltips.tableRows;
    const colsLabel = tooltips.tableColumns;
    const insertBtnText = tooltips.tableInsert;
    const cancelBtnText = i18n.cancel;

    if (typeof Swal !== 'undefined') {
        Swal.fire({
            title: title,
            html: `
                <div class="text-start mb-3">
                    <label for="swal-table-rows" class="form-label fw-medium mb-1">${rowsLabel}</label>
                    <input id="swal-table-rows" type="number" min="1" max="50" value="2" class="form-control" />
                </div>
                <div class="text-start">
                    <label for="swal-table-cols" class="form-label fw-medium mb-1">${colsLabel}</label>
                    <input id="swal-table-cols" type="number" min="1" max="50" value="2" class="form-control" />
                </div>
            `,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonText: insertBtnText,
            cancelButtonText: cancelBtnText,
            confirmButtonColor: '#0d6efd',
            cancelButtonColor: '#64748b',
            preConfirm: () => {
                const rowsInput = document.getElementById('swal-table-rows');
                const colsInput = document.getElementById('swal-table-cols');
                const rows = parseInt(rowsInput ? rowsInput.value : '2', 10);
                const cols = parseInt(colsInput ? colsInput.value : '2', 10);

                if (isNaN(rows) || rows < 1 || rows > 50) {
                    Swal.showValidationMessage(`${rowsLabel}: 1 - 50`);
                    return false;
                }
                if (isNaN(cols) || cols < 1 || cols > 50) {
                    Swal.showValidationMessage(`${colsLabel}: 1 - 50`);
                    return false;
                }
                return { rows, cols };
            }
        }).then((result) => {
            if (result.isConfirmed && result.value) {
                quill.focus();
                if (range) {
                    quill.setSelection(range.index, range.length);
                }
                tableModule.insertTable(result.value.rows, result.value.cols);
            }
        });
    } else {
        const rows = parseInt(prompt(`${rowsLabel}:`, '2'), 10);
        const cols = parseInt(prompt(`${colsLabel}:`, '2'), 10);
        if (rows > 0 && cols > 0) {
            quill.focus();
            if (range) {
                quill.setSelection(range.index, range.length);
            }
            tableModule.insertTable(rows, cols);
        }
    }
}

/**
 * Interactive Image Resizer & Alignment Controller for Quill 2.
 */
function initQuillImageResizer(quill) {
    ensureImageResizerStyles();

    const editorRoot = quill.root;
    const container = quill.container;
    container.style.position = 'relative';

    let activeImage = null;
    let overlay = null;

    // Create or get the resizer overlay
    function getOverlay() {
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.className = 'ql-image-resizer-overlay';
            overlay.innerHTML = `
                <div class="ql-resizer-handle ql-resizer-nw" data-handle="nw"></div>
                <div class="ql-resizer-handle ql-resizer-ne" data-handle="ne"></div>
                <div class="ql-resizer-handle ql-resizer-se" data-handle="se"></div>
                <div class="ql-resizer-handle ql-resizer-sw" data-handle="sw"></div>
                <div class="ql-resizer-size-badge"></div>
                <div class="ql-resizer-toolbar">
                    <div class="ql-resizer-btn-group">
                        <button type="button" class="ql-resizer-btn" data-action="align-left" title="Align Left">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M2 2h12v2H2V2zm0 4h7v2H2V6zm0 4h12v2H2v-2zm0 4h7v2H2v-2z"/></svg>
                        </button>
                        <button type="button" class="ql-resizer-btn" data-action="align-center" title="Align Center">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M2 2h12v2H2V2zm3 4h6v2H5V6zm-3 4h12v2H2v-2zm3 4h6v2H5v-2z"/></svg>
                        </button>
                        <button type="button" class="ql-resizer-btn" data-action="align-right" title="Align Right">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M2 2h12v2H2V2zm5 4h7v2H7V6zm-5 4h12v2H2v-2zm5 4h7v2H7v-2z"/></svg>
                        </button>
                        <button type="button" class="ql-resizer-btn" data-action="align-inline" title="Inline">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M1 3.5A1.5 1.5 0 0 1 2.5 2h11A1.5 1.5 0 0 1 15 3.5v9a1.5 1.5 0 0 1-1.5 1.5h-11A1.5 1.5 0 0 1 1 12.5v-9zM2.5 3a.5.5 0 0 0-.5.5v9a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5v-9a.5.5 0 0 0-.5-.5h-11z"/></svg>
                        </button>
                    </div>
                    <div class="ql-resizer-divider"></div>
                    <div class="ql-resizer-btn-group">
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-text" data-action="size-25">25%</button>
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-text" data-action="size-50">50%</button>
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-text" data-action="size-75">75%</button>
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-text" data-action="size-100">100%</button>
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-text" data-action="size-auto">Auto</button>
                    </div>
                    <div class="ql-resizer-divider"></div>
                    <div class="ql-resizer-btn-group">
                        <button type="button" class="ql-resizer-btn" data-action="properties" title="Properties">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492zM5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0z"/><path d="M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0l-.094.319a.873.873 0 0 1-1.255.52l-.292-.16c-1.64-.892-3.433.902-2.54 2.541l.159.292a.873.873 0 0 1-.52 1.255l-.319.094c-1.79.527-1.79 3.065 0 3.592l.319.094a.873.873 0 0 1 .52 1.255l-.16.292c-.892 1.64.901 3.434 2.541 2.54l.292-.159a.873.873 0 0 1 1.255.52l.094.319c.527 1.79 3.065 1.79 3.592 0l.094-.319a.873.873 0 0 1 1.255-.52l.292.16c1.64.893 3.434-.902 2.54-2.541l-.159-.292a.873.873 0 0 1 .52-1.255l.319-.094c1.79-.527 1.79-3.065 0-3.592l-.319-.094a.873.873 0 0 1-.52-1.255l.16-.292c.893-1.64-.902-3.433-2.541-2.54l-.292.159a.873.873 0 0 1-1.255-.52l-.094-.319z"/></svg>
                        </button>
                        <button type="button" class="ql-resizer-btn ql-resizer-btn-danger" data-action="delete" title="Delete">
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor"><path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/><path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/></svg>
                        </button>
                    </div>
                </div>
            `;

            // Localize toolbar button titles
            const i18n = window.AdminI18n || {};
            const tooltips = i18n.quillTooltips || {};
            if (tooltips.alignLeft) overlay.querySelector('[data-action="align-left"]').setAttribute('title', tooltips.alignLeft);
            if (tooltips.alignCenter) overlay.querySelector('[data-action="align-center"]').setAttribute('title', tooltips.alignCenter);
            if (tooltips.alignRight) overlay.querySelector('[data-action="align-right"]').setAttribute('title', tooltips.alignRight);
            if (tooltips.alignInline) overlay.querySelector('[data-action="align-inline"]').setAttribute('title', tooltips.alignInline);
            if (tooltips.imageProperties) overlay.querySelector('[data-action="properties"]').setAttribute('title', tooltips.imageProperties);
            if (tooltips.deleteImage) overlay.querySelector('[data-action="delete"]').setAttribute('title', tooltips.deleteImage);

            // Handle toolbar actions
            overlay.querySelector('.ql-resizer-toolbar').addEventListener('click', (e) => {
                e.stopPropagation();
                const btn = e.target.closest('.ql-resizer-btn');
                if (!btn || !activeImage) return;

                const action = btn.getAttribute('data-action');
                handleImageAction(action, activeImage, quill);
            });

            // Handle drag resizing on handles
            overlay.querySelectorAll('.ql-resizer-handle').forEach(handle => {
                handle.addEventListener('mousedown', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    startHandleDrag(e, handle.getAttribute('data-handle'));
                });
            });

            container.appendChild(overlay);
        }
        return overlay;
    }

    function repositionOverlay() {
        if (!activeImage || !overlay || !activeImage.isConnected) {
            hideOverlay();
            return;
        }

        const containerRect = container.getBoundingClientRect();
        const imgRect = activeImage.getBoundingClientRect();

        const top = imgRect.top - containerRect.top + container.scrollTop;
        const left = imgRect.left - containerRect.left + container.scrollLeft;
        const width = imgRect.width;
        const height = imgRect.height;

        overlay.style.top = `${top}px`;
        overlay.style.left = `${left}px`;
        overlay.style.width = `${width}px`;
        overlay.style.height = `${height}px`;
        overlay.style.display = 'block';

        // Update badge
        const badge = overlay.querySelector('.ql-resizer-size-badge');
        if (badge) {
            badge.textContent = `${Math.round(width)} × ${Math.round(height)} px`;
        }

        // Adjust toolbar position (flip below if near top of container)
        const toolbar = overlay.querySelector('.ql-resizer-toolbar');
        if (toolbar) {
            if (top < 45) {
                toolbar.classList.add('ql-resizer-toolbar-bottom');
            } else {
                toolbar.classList.remove('ql-resizer-toolbar-bottom');
            }
        }
    }

    function showOverlay(img) {
        activeImage = img;
        getOverlay();
        repositionOverlay();
    }

    function hideOverlay() {
        activeImage = null;
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    function handleImageAction(action, img, quillInstance) {
        if (!img) return;

        if (action === 'align-left') {
            img.style.float = 'left';
            img.style.margin = '0 1rem 1rem 0';
            img.style.display = 'inline';
        } else if (action === 'align-center') {
            img.style.float = 'none';
            img.style.margin = '0 auto';
            img.style.display = 'block';
        } else if (action === 'align-right') {
            img.style.float = 'right';
            img.style.margin = '0 0 1rem 1rem';
            img.style.display = 'inline';
        } else if (action === 'align-inline') {
            img.style.float = 'none';
            img.style.margin = '0';
            img.style.display = 'inline-block';
        } else if (action === 'size-25') {
            img.style.width = '25%';
            img.style.height = 'auto';
        } else if (action === 'size-50') {
            img.style.width = '50%';
            img.style.height = 'auto';
        } else if (action === 'size-75') {
            img.style.width = '75%';
            img.style.height = 'auto';
        } else if (action === 'size-100') {
            img.style.width = '100%';
            img.style.height = 'auto';
        } else if (action === 'size-auto') {
            img.style.width = '';
            img.style.height = '';
            img.removeAttribute('width');
            img.removeAttribute('height');
        } else if (action === 'properties') {
            openImagePropertiesModal(img, quillInstance, repositionOverlay);
            return;
        } else if (action === 'delete') {
            const blot = Quill.find(img);
            hideOverlay();
            if (blot) {
                blot.deleteAt(0);
            } else {
                img.remove();
            }
            quillInstance.emitter.emit('text-change');
            return;
        }

        repositionOverlay();
        quillInstance.emitter.emit('text-change');
    }

    function startHandleDrag(initialEvent, handleCorner) {
        if (!activeImage) return;

        const startX = initialEvent.clientX;
        const startY = initialEvent.clientY;
        const startWidth = activeImage.offsetWidth;
        const startHeight = activeImage.offsetHeight;
        const aspectRatio = startWidth / (startHeight || 1);

        const onMouseMove = (moveEvent) => {
            const deltaX = moveEvent.clientX - startX;
            const deltaY = moveEvent.clientY - startY;

            let newWidth = startWidth;
            if (handleCorner === 'se' || handleCorner === 'ne') {
                newWidth = startWidth + deltaX;
            } else if (handleCorner === 'sw' || handleCorner === 'nw') {
                newWidth = startWidth - deltaX;
            }

            // Minimum width boundary
            newWidth = Math.max(30, Math.round(newWidth));

            // Keep aspect ratio by default (or allow shift key for freeform)
            activeImage.style.width = `${newWidth}px`;
            if (moveEvent.shiftKey) {
                let newHeight = startHeight;
                if (handleCorner === 'se' || handleCorner === 'sw') {
                    newHeight = startHeight + deltaY;
                } else {
                    newHeight = startHeight - deltaY;
                }
                activeImage.style.height = `${Math.max(20, Math.round(newHeight))}px`;
            } else {
                activeImage.style.height = 'auto';
            }

            repositionOverlay();
        };

        const onMouseUp = () => {
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
            repositionOverlay();
            quill.emitter.emit('text-change');
        };

        window.addEventListener('mousemove', onMouseMove);
        window.addEventListener('mouseup', onMouseUp);
    }

    // Click on image inside editor
    editorRoot.addEventListener('click', (e) => {
        if (e.target && e.target.tagName === 'IMG') {
            e.stopPropagation();
            showOverlay(e.target);
        } else {
            hideOverlay();
        }
    });

    // Double click to open property modal
    editorRoot.addEventListener('dblclick', (e) => {
        if (e.target && e.target.tagName === 'IMG') {
            e.stopPropagation();
            showOverlay(e.target);
            openImagePropertiesModal(e.target, quill, repositionOverlay);
        }
    });

    // Click outside to deselect
    document.addEventListener('click', (e) => {
        if (overlay && !overlay.contains(e.target) && (!activeImage || activeImage !== e.target)) {
            hideOverlay();
        }
    });

    // Reposition on editor scroll or window resize
    editorRoot.addEventListener('scroll', repositionOverlay);
    window.addEventListener('resize', repositionOverlay);
    quill.on('text-change', () => {
        if (activeImage) {
            setTimeout(repositionOverlay, 10);
        }
    });
}

/**
 * Opens a modal dialog (SweetAlert2) to edit detailed image attributes and styling.
 */
function openImagePropertiesModal(img, quill, onUpdate) {
    if (!img) return;

    const i18n = window.AdminI18n || {};
    const tooltips = i18n.quillTooltips || {};

    const currentWidth = img.style.width || img.getAttribute('width') || '';
    const currentHeight = img.style.height || img.getAttribute('height') || '';
    const currentAlt = img.getAttribute('alt') || '';
    const currentTitle = img.getAttribute('title') || '';

    // Determine current alignment
    let currentAlign = 'inline';
    if (img.style.float === 'left') currentAlign = 'left';
    else if (img.style.float === 'right') currentAlign = 'right';
    else if (img.style.margin && img.style.margin.includes('auto') && img.style.display === 'block') currentAlign = 'center';

    const title = tooltips.imageProperties || 'Image Properties';
    const widthLabel = tooltips.width || 'Width';
    const heightLabel = tooltips.height || 'Height';
    const altLabel = tooltips.altText || 'Alternative Text (Alt)';
    const titleLabel = tooltips.imageTitle || 'Image Title';
    const alignLabel = tooltips.align || 'Alignment';
    const alignInline = tooltips.alignInline || 'Inline';
    const alignLeft = tooltips.alignLeft || 'Align Left';
    const alignCenter = tooltips.alignCenter || 'Align Center';
    const alignRight = tooltips.alignRight || 'Align Right';
    const ratioLabel = tooltips.maintainAspectRatio || 'Maintain aspect ratio';
    const saveBtnText = i18n.save || 'Save';
    const cancelBtnText = i18n.cancel || 'Cancel';

    if (typeof Swal !== 'undefined') {
        Swal.fire({
            title: title,
            html: `
                <div class="text-start mb-3">
                    <div class="row g-2">
                        <div class="col-6">
                            <label for="swal-img-width" class="form-label fw-medium mb-1">${widthLabel}</label>
                            <input id="swal-img-width" type="text" value="${currentWidth}" placeholder="e.g. 400px or 50%" class="form-control" />
                        </div>
                        <div class="col-6">
                            <label for="swal-img-height" class="form-label fw-medium mb-1">${heightLabel}</label>
                            <input id="swal-img-height" type="text" value="${currentHeight}" placeholder="e.g. auto or 300px" class="form-control" />
                        </div>
                    </div>
                    <div class="form-check mt-2">
                        <input class="form-check-input" type="checkbox" id="swal-img-ratio" checked>
                        <label class="form-check-label small text-muted" for="swal-img-ratio">${ratioLabel}</label>
                    </div>
                </div>
                <div class="text-start mb-3">
                    <label for="swal-img-align" class="form-label fw-medium mb-1">${alignLabel}</label>
                    <select id="swal-img-align" class="form-select">
                        <option value="inline" ${currentAlign === 'inline' ? 'selected' : ''}>${alignInline}</option>
                        <option value="left" ${currentAlign === 'left' ? 'selected' : ''}>${alignLeft}</option>
                        <option value="center" ${currentAlign === 'center' ? 'selected' : ''}>${alignCenter}</option>
                        <option value="right" ${currentAlign === 'right' ? 'selected' : ''}>${alignRight}</option>
                    </select>
                </div>
                <div class="text-start mb-3">
                    <label for="swal-img-alt" class="form-label fw-medium mb-1">${altLabel}</label>
                    <input id="swal-img-alt" type="text" value="${currentAlt}" class="form-control" />
                </div>
                <div class="text-start">
                    <label for="swal-img-title" class="form-label fw-medium mb-1">${titleLabel}</label>
                    <input id="swal-img-title" type="text" value="${currentTitle}" class="form-control" />
                </div>
            `,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonText: saveBtnText,
            cancelButtonText: cancelBtnText,
            confirmButtonColor: '#0d6efd',
            cancelButtonColor: '#64748b',
            didOpen: () => {
                const wInput = document.getElementById('swal-img-width');
                const hInput = document.getElementById('swal-img-height');
                const ratioCheck = document.getElementById('swal-img-ratio');
                const naturalRatio = (img.naturalWidth || img.offsetWidth) / ((img.naturalHeight || img.offsetHeight) || 1);

                wInput.addEventListener('input', () => {
                    if (ratioCheck.checked) {
                        const num = parseFloat(wInput.value);
                        if (!isNaN(num) && (wInput.value.endsWith('px') || !wInput.value.includes('%'))) {
                            hInput.value = `${Math.round(num / naturalRatio)}px`;
                        } else if (wInput.value === '') {
                            hInput.value = '';
                        }
                    }
                });
            },
            preConfirm: () => {
                return {
                    width: document.getElementById('swal-img-width').value.trim(),
                    height: document.getElementById('swal-img-height').value.trim(),
                    align: document.getElementById('swal-img-align').value,
                    alt: document.getElementById('swal-img-alt').value.trim(),
                    title: document.getElementById('swal-img-title').value.trim()
                };
            }
        }).then((result) => {
            if (result.isConfirmed && result.value) {
                const vals = result.value;

                // Width / Height
                if (vals.width) {
                    img.style.width = vals.width.includes('%') || vals.width.includes('px') || vals.width === 'auto' ? vals.width : `${vals.width}px`;
                } else {
                    img.style.width = '';
                    img.removeAttribute('width');
                }

                if (vals.height) {
                    img.style.height = vals.height.includes('%') || vals.height.includes('px') || vals.height === 'auto' ? vals.height : `${vals.height}px`;
                } else {
                    img.style.height = '';
                    img.removeAttribute('height');
                }

                // Alignment
                if (vals.align === 'left') {
                    img.style.float = 'left';
                    img.style.margin = '0 1rem 1rem 0';
                    img.style.display = 'inline';
                } else if (vals.align === 'center') {
                    img.style.float = 'none';
                    img.style.margin = '0 auto';
                    img.style.display = 'block';
                } else if (vals.align === 'right') {
                    img.style.float = 'right';
                    img.style.margin = '0 0 1rem 1rem';
                    img.style.display = 'inline';
                } else {
                    img.style.float = 'none';
                    img.style.margin = '0';
                    img.style.display = 'inline-block';
                }

                // Alt & Title
                if (vals.alt) img.setAttribute('alt', vals.alt);
                else img.removeAttribute('alt');

                if (vals.title) img.setAttribute('title', vals.title);
                else img.removeAttribute('title');

                if (typeof onUpdate === 'function') onUpdate();
                quill.emitter.emit('text-change');
            }
        });
    }
}

/**
 * Injects required CSS styling for the Quill image resizer overlay and toolbar.
 */
function ensureImageResizerStyles() {
    if (document.getElementById('ql-image-resizer-styles')) return;

    const style = document.createElement('style');
    style.id = 'ql-image-resizer-styles';
    style.textContent = `
        .ql-image-resizer-overlay {
            position: absolute;
            display: none;
            border: 2px dashed #0d6efd;
            box-sizing: border-box;
            pointer-events: none;
            z-index: 100;
        }
        .ql-resizer-handle {
            position: absolute;
            width: 10px;
            height: 10px;
            background: #0d6efd;
            border: 2px solid #ffffff;
            border-radius: 2px;
            box-sizing: border-box;
            pointer-events: auto;
            z-index: 102;
            box-shadow: 0 1px 3px rgba(0,0,0,0.3);
        }
        .ql-resizer-nw { top: -5px; left: -5px; cursor: nwse-resize; }
        .ql-resizer-ne { top: -5px; right: -5px; cursor: nesw-resize; }
        .ql-resizer-se { bottom: -5px; right: -5px; cursor: nwse-resize; }
        .ql-resizer-sw { bottom: -5px; left: -5px; cursor: nesw-resize; }

        .ql-resizer-size-badge {
            position: absolute;
            bottom: 4px;
            right: 4px;
            background: rgba(15, 23, 42, 0.85);
            color: #ffffff;
            font-size: 11px;
            line-height: 1;
            padding: 3px 6px;
            border-radius: 3px;
            pointer-events: none;
            font-family: monospace;
            z-index: 101;
        }

        .ql-resizer-toolbar {
            position: absolute;
            top: -42px;
            left: 50%;
            transform: translateX(-50%);
            display: flex;
            align-items: center;
            background: #ffffff;
            border: 1px solid #ced4da;
            border-radius: 6px;
            padding: 3px 6px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            pointer-events: auto;
            z-index: 105;
            white-space: nowrap;
            gap: 4px;
        }
        .ql-resizer-toolbar.ql-resizer-toolbar-bottom {
            top: auto;
            bottom: -44px;
        }
        .ql-resizer-btn-group {
            display: flex;
            align-items: center;
            gap: 2px;
        }
        .ql-resizer-divider {
            width: 1px;
            height: 18px;
            background: #dee2e6;
            margin: 0 3px;
        }
        .ql-resizer-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 26px;
            height: 26px;
            border: none;
            background: transparent;
            color: #495057;
            border-radius: 4px;
            cursor: pointer;
            padding: 0;
            transition: background 0.15s ease, color 0.15s ease;
        }
        .ql-resizer-btn:hover {
            background: #e9ecef;
            color: #0d6efd;
        }
        .ql-resizer-btn-text {
            width: auto;
            padding: 0 5px;
            font-size: 11px;
            font-weight: 600;
            color: #6c757d;
        }
        .ql-resizer-btn-text:hover {
            background: #e9ecef;
            color: #0d6efd;
        }
        .ql-resizer-btn-danger:hover {
            background: #fee2e2;
            color: #dc2626;
        }
    `;
    document.head.appendChild(style);
}

/**
 * Opens a modal dialog to select media library photos and configure/insert a photo gallery into Quill.
 */
async function openMediaGalleryModal(quill) {
    const i18n = window.AdminI18n || {};
    const tooltips = i18n.quillTooltips || {};

    let selectedImages = [];
    let currentPage = 1;
    let currentSearch = '';
    const pageSize = 12;

    // Ensure modal markup exists in document body
    let modalEl = document.getElementById('quillMediaGalleryModal');
    if (!modalEl) {
        const modalHtml = `
        <div class="modal fade" id="quillMediaGalleryModal" tabindex="-1" aria-labelledby="quillMediaGalleryModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content shadow-lg border-0">
                    <div class="modal-header bg-light">
                        <h5 class="modal-title fw-bold" id="quillMediaGalleryModalLabel">
                            <i class="bi bi-images text-primary me-2"></i>${tooltips.insertGallery || 'Insert Photo Gallery'}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body p-4">
                        <!-- Options Bar -->
                        <div class="card bg-light border-0 mb-4">
                            <div class="card-body">
                                <div class="row g-3 align-items-center">
                                    <div class="col-md-3">
                                        <label class="form-label small fw-bold mb-1">${tooltips.columns || 'Columns'}</label>
                                        <select id="galleryModalColumns" class="form-select form-select-sm">
                                            <option value="auto" selected>${tooltips.columnsAuto || 'Auto (Responsive)'}</option>
                                            <option value="1">1 Column</option>
                                            <option value="2">2 Columns</option>
                                            <option value="3">3 Columns</option>
                                            <option value="4">4 Columns</option>
                                            <option value="5">5 Columns</option>
                                            <option value="6">6 Columns</option>
                                        </select>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="form-label small fw-bold mb-1">${tooltips.gap || 'Gap'}</label>
                                        <select id="galleryModalGap" class="form-select form-select-sm">
                                            <option value="sm">${tooltips.gapSmall || 'Small (8px)'}</option>
                                            <option value="md" selected>${tooltips.gapMedium || 'Medium (16px)'}</option>
                                            <option value="lg">${tooltips.gapLarge || 'Large (24px)'}</option>
                                        </select>
                                    </div>
                                    <div class="col-md-3 pt-3">
                                        <div class="form-check form-switch">
                                            <input class="form-check-input" type="checkbox" id="galleryModalLightbox" checked>
                                            <label class="form-check-label small fw-medium" for="galleryModalLightbox">${tooltips.enableLightbox || 'Enable Lightbox'}</label>
                                        </div>
                                    </div>
                                    <div class="col-md-3 text-end pt-3">
                                        <span id="gallerySelectionCountBadge" class="badge bg-primary fs-6 px-3 py-2">
                                            0 ${tooltips.selected || 'selected'}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Search Bar -->
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <div class="input-group" style="max-width: 320px;">
                                <span class="input-group-text bg-white border-end-0"><i class="bi bi-search text-muted"></i></span>
                                <input type="text" id="gallerySearchInput" class="form-control border-start-0" placeholder="${i18n.search || 'Search images...'}">
                            </div>
                            <small class="text-muted">${tooltips.selectImages || 'Click on images to select/unselect'}</small>
                        </div>

                        <!-- Image Grid -->
                        <div id="galleryImageGrid" class="gallery-picker-grid mb-3">
                            <div class="text-center py-5 text-muted col-span-full">
                                <div class="spinner-border spinner-border-sm text-primary me-2"></div>Loading...
                            </div>
                        </div>

                        <!-- Pagination Controls -->
                        <div class="d-flex justify-content-between align-items-center mt-3 pt-2 border-top">
                            <button type="button" id="galleryPrevPage" class="btn btn-sm btn-outline-secondary" disabled>
                                <i class="bi bi-chevron-left"></i> ${i18n.previous || 'Previous'}
                            </button>
                            <span id="galleryPageIndicator" class="small text-muted">Page 1</span>
                            <button type="button" id="galleryNextPage" class="btn btn-sm btn-outline-secondary" disabled>
                                ${i18n.next || 'Next'} <i class="bi bi-chevron-right"></i>
                            </button>
                        </div>
                    </div>
                    <div class="modal-footer bg-light">
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">${i18n.cancel || 'Cancel'}</button>
                        <button type="button" id="galleryInsertBtn" class="btn btn-primary px-4" disabled>
                            <i class="bi bi-plus-circle me-1"></i> ${tooltips.insertGallery || 'Insert Gallery'}
                        </button>
                    </div>
                </div>
            </div>
        </div>`;
        document.body.insertAdjacentHTML('beforeend', modalHtml);
        modalEl = document.getElementById('quillMediaGalleryModal');
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const gridEl = document.getElementById('galleryImageGrid');
    const searchInput = document.getElementById('gallerySearchInput');
    const prevBtn = document.getElementById('galleryPrevPage');
    const nextBtn = document.getElementById('galleryNextPage');
    const pageIndicator = document.getElementById('galleryPageIndicator');
    const countBadge = document.getElementById('gallerySelectionCountBadge');
    const insertBtn = document.getElementById('galleryInsertBtn');
    const columnsSelect = document.getElementById('galleryModalColumns');
    const gapSelect = document.getElementById('galleryModalGap');
    const lightboxCheck = document.getElementById('galleryModalLightbox');

    // Reset state on open
    selectedImages = [];
    currentPage = 1;
    currentSearch = '';
    searchInput.value = '';
    updateSelectionUI();

    async function loadAssets() {
        gridEl.innerHTML = `
            <div class="d-flex justify-content-center align-items-center py-5 w-100" style="grid-column: 1 / -1;">
                <div class="spinner-border spinner-border-sm text-primary me-2"></div>
                <span class="text-muted small">Loading media assets...</span>
            </div>`;

        try {
            const url = `/Admin/Media/ApiList?Page=${currentPage}&PageSize=${pageSize}&SearchTerm=${encodeURIComponent(currentSearch)}`;
            const response = await fetch(url);
            if (!response.ok) throw new Error('Failed to load media');
            const data = await response.json();

            renderAssets(data.items || []);
            updatePaginationUI(data);
        } catch (error) {
            console.error('Error fetching media list:', error);
            gridEl.innerHTML = `<div class="alert alert-danger w-100" style="grid-column: 1 / -1;">Error loading media library.</div>`;
        }
    }

    function renderAssets(items) {
        if (!items || items.length === 0) {
            gridEl.innerHTML = `<div class="text-center py-5 text-muted w-100" style="grid-column: 1 / -1;">No images found.</div>`;
            return;
        }

        gridEl.innerHTML = '';
        items.forEach(item => {
            const isSelected = selectedImages.some(img => img.src === item.relativePath);
            const card = document.createElement('div');
            card.className = `gallery-picker-card ${isSelected ? 'is-selected' : ''}`;
            card.setAttribute('data-src', item.relativePath);
            card.setAttribute('data-alt', item.altText || '');
            card.setAttribute('data-title', item.title || item.originalFileName || '');

            card.innerHTML = `
                <img src="${item.relativePath}" alt="${item.altText || ''}" loading="lazy" />
                <div class="check-badge"><i class="bi bi-check"></i></div>
            `;

            card.addEventListener('click', () => {
                const src = item.relativePath;
                const alt = item.altText || '';
                const title = item.title || item.originalFileName || '';

                const existingIndex = selectedImages.findIndex(img => img.src === src);
                if (existingIndex >= 0) {
                    selectedImages.splice(existingIndex, 1);
                    card.classList.remove('is-selected');
                } else {
                    selectedImages.push({ src, alt, title });
                    card.classList.add('is-selected');
                }
                updateSelectionUI();
            });

            gridEl.appendChild(card);
        });
    }

    function updatePaginationUI(data) {
        const totalPages = Math.ceil((data.totalCount || 0) / pageSize) || 1;
        pageIndicator.textContent = `Page ${data.page} of ${totalPages} (${data.totalCount || 0} items)`;
        prevBtn.disabled = (data.page <= 1);
        nextBtn.disabled = (data.page >= totalPages);
    }

    function updateSelectionUI() {
        const count = selectedImages.length;
        countBadge.textContent = `${count} ${tooltips.selected || 'selected'}`;
        insertBtn.disabled = (count === 0);
    }

    // Debounced search
    let searchDebounceTimer = null;
    searchInput.oninput = () => {
        clearTimeout(searchDebounceTimer);
        searchDebounceTimer = setTimeout(() => {
            currentSearch = searchInput.value.trim();
            currentPage = 1;
            loadAssets();
        }, 300);
    };

    prevBtn.onclick = () => {
        if (currentPage > 1) {
            currentPage--;
            loadAssets();
        }
    };

    nextBtn.onclick = () => {
        currentPage++;
        loadAssets();
    };

    insertBtn.onclick = () => {
        if (selectedImages.length === 0) return;

        quill.focus();
        let range = quill.getSelection(true);
        if (!range) {
            range = { index: quill.getLength(), length: 0 };
        }

        const galleryData = {
            columns: columnsSelect.value,
            gap: gapSelect.value,
            lightbox: lightboxCheck.checked,
            images: selectedImages
        };

        quill.insertEmbed(range.index, 'gallery', galleryData);
        quill.setSelection(range.index + 1);

        modal.hide();
    };

    modal.show();
    loadAssets();
}



