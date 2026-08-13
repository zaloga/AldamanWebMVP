/**
 * Admin Media Upload - Drag and Drop File Selection & MS Paint-style Resizing
 */
document.addEventListener('DOMContentLoaded', function () {
    const dropZone = document.getElementById('dropZone');
    const fileInput = document.getElementById('fileInput');
    const filePreview = document.getElementById('filePreview');
    const fileNameDisp = document.getElementById('fileName');
    const fileSizeDisp = document.getElementById('fileSize');
    const origDimBadge = document.getElementById('origDimBadge');
    const imageThumbnail = document.getElementById('imageThumbnail');
    const fileIcon = document.getElementById('fileIcon');
    const submitBtn = document.getElementById('submitBtn');
    const resetFileBtn = document.getElementById('resetFileBtn');
    const resizeSection = document.getElementById('resizeSection');

    // Hidden form inputs
    const targetWidthInput = document.getElementById('targetWidthInput');
    const targetHeightInput = document.getElementById('targetHeightInput');

    // Pixel inputs & lock
    const pixelInputsRow = document.getElementById('pixelInputsRow');
    const pixelWidthInput = document.getElementById('pixelWidthInput');
    const pixelHeightInput = document.getElementById('pixelHeightInput');
    const aspectLockBtn = document.getElementById('aspectLockBtn');
    const aspectLockIcon = document.getElementById('aspectLockIcon');

    // Percentage inputs & lock
    const percentInputsRow = document.getElementById('percentInputsRow');
    const percentWidthInput = document.getElementById('percentWidthInput');
    const percentHeightInput = document.getElementById('percentHeightInput');
    const aspectLockBtnPercent = document.getElementById('aspectLockBtnPercent');
    const aspectLockIconPercent = document.getElementById('aspectLockIconPercent');

    // Unit radio toggles & summary
    const unitPixels = document.getElementById('unitPixels');
    const unitPercent = document.getElementById('unitPercent');
    const targetDimDisplay = document.getElementById('targetDimDisplay');
    const presetButtons = document.querySelectorAll('.preset-btn');

    let originalWidth = 0;
    let originalHeight = 0;
    let isAspectLocked = true;
    let currentObjectUrl = null;

    if (!dropZone || !fileInput) return;

    // DropZone events
    dropZone.addEventListener('click', () => fileInput.click());

    dropZone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropZone.classList.add('drag-over');
    });

    ['dragleave', 'dragend'].forEach(type => {
        dropZone.addEventListener(type, () => {
            dropZone.classList.remove('drag-over');
        });
    });

    dropZone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropZone.classList.remove('drag-over');
        if (e.dataTransfer.files.length) {
            fileInput.files = e.dataTransfer.files;
            handleFileChange(fileInput);
        }
    });

    fileInput.addEventListener('change', function () {
        handleFileChange(this);
    });

    if (resetFileBtn) {
        resetFileBtn.addEventListener('click', resetFile);
    }

    // Aspect Ratio Lock Handlers
    [aspectLockBtn, aspectLockBtnPercent].forEach(btn => {
        if (!btn) return;
        btn.addEventListener('click', () => {
            isAspectLocked = !isAspectLocked;
            updateLockState();
        });
    });

    function updateLockState() {
        const iconClass = isAspectLocked ? 'bi-lock-fill' : 'bi-unlock';
        const activeClass = isAspectLocked;

        [aspectLockBtn, aspectLockBtnPercent].forEach(btn => {
            if (btn) {
                btn.classList.toggle('active', activeClass);
                btn.setAttribute('aria-pressed', activeClass ? 'true' : 'false');
            }
        });

        if (aspectLockIcon) aspectLockIcon.className = 'bi ' + iconClass;
        if (aspectLockIconPercent) aspectLockIconPercent.className = 'bi ' + iconClass;
    }

    // Unit mode toggle
    if (unitPixels) {
        unitPixels.addEventListener('change', () => {
            if (unitPixels.checked) {
                pixelInputsRow?.classList.remove('d-none');
                percentInputsRow?.classList.add('d-none');
            }
        });
    }

    if (unitPercent) {
        unitPercent.addEventListener('change', () => {
            if (unitPercent.checked) {
                if (originalWidth > 0 && originalHeight > 0) {
                    const currentW = parseInt(pixelWidthInput.value, 10) || originalWidth;
                    const currentH = parseInt(pixelHeightInput.value, 10) || originalHeight;
                    if (percentWidthInput) percentWidthInput.value = Math.round((currentW / originalWidth) * 100);
                    if (percentHeightInput) percentHeightInput.value = Math.round((currentH / originalHeight) * 100);
                }
                pixelInputsRow?.classList.add('d-none');
                percentInputsRow?.classList.remove('d-none');
            }
        });
    }

    // Pixel input events
    pixelWidthInput?.addEventListener('input', function () {
        const val = parseInt(this.value, 10);
        if (val > 0) {
            if (isAspectLocked && originalWidth > 0 && originalHeight > 0) {
                const newHeight = Math.max(1, Math.round(val * (originalHeight / originalWidth)));
                if (pixelHeightInput) pixelHeightInput.value = newHeight;
            }
            syncDimensions();
        }
    });

    pixelHeightInput?.addEventListener('input', function () {
        const val = parseInt(this.value, 10);
        if (val > 0) {
            if (isAspectLocked && originalWidth > 0 && originalHeight > 0) {
                const newWidth = Math.max(1, Math.round(val * (originalWidth / originalHeight)));
                if (pixelWidthInput) pixelWidthInput.value = newWidth;
            }
            syncDimensions();
        }
    });

    // Percentage input events
    percentWidthInput?.addEventListener('input', function () {
        const pct = parseFloat(this.value);
        if (pct > 0 && originalWidth > 0) {
            if (isAspectLocked && percentHeightInput) {
                percentHeightInput.value = this.value;
            }
            const newWidth = Math.max(1, Math.round(originalWidth * (pct / 100)));
            if (pixelWidthInput) pixelWidthInput.value = newWidth;

            if (isAspectLocked) {
                const newHeight = Math.max(1, Math.round(originalHeight * (pct / 100)));
                if (pixelHeightInput) pixelHeightInput.value = newHeight;
            }
            syncDimensions();
        }
    });

    percentHeightInput?.addEventListener('input', function () {
        const pct = parseFloat(this.value);
        if (pct > 0 && originalHeight > 0) {
            if (isAspectLocked && percentWidthInput) {
                percentWidthInput.value = this.value;
            }
            const newHeight = Math.max(1, Math.round(originalHeight * (pct / 100)));
            if (pixelHeightInput) pixelHeightInput.value = newHeight;

            if (isAspectLocked) {
                const newWidth = Math.max(1, Math.round(originalWidth * (pct / 100)));
                if (pixelWidthInput) pixelWidthInput.value = newWidth;
            }
            syncDimensions();
        }
    });

    // Presets buttons
    presetButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const preset = this.getAttribute('data-preset');
            if (!originalWidth || !originalHeight) return;

            if (preset === 'orig') {
                setDimensions(originalWidth, originalHeight, 100);
            } else if (preset === '50%') {
                const w = Math.max(1, Math.round(originalWidth * 0.5));
                const h = Math.max(1, Math.round(originalHeight * 0.5));
                setDimensions(w, h, 50);
            } else {
                const targetW = parseInt(preset, 10);
                if (targetW > 0) {
                    const targetH = Math.max(1, Math.round(targetW * (originalHeight / originalWidth)));
                    const pct = Math.round((targetW / originalWidth) * 100);
                    setDimensions(targetW, targetH, pct);
                }
            }
        });
    });

    function setDimensions(w, h, pct) {
        if (pixelWidthInput) pixelWidthInput.value = w;
        if (pixelHeightInput) pixelHeightInput.value = h;
        if (percentWidthInput) percentWidthInput.value = pct;
        if (percentHeightInput) percentHeightInput.value = pct;
        syncDimensions();
    }

    function syncDimensions() {
        const w = parseInt(pixelWidthInput?.value, 10) || originalWidth;
        const h = parseInt(pixelHeightInput?.value, 10) || originalHeight;

        if (targetWidthInput) targetWidthInput.value = w;
        if (targetHeightInput) targetHeightInput.value = h;

        if (targetDimDisplay) {
            targetDimDisplay.textContent = `${w} × ${h} px`;
        }
    }

    function handleFileChange(input) {
        if (input.files && input.files[0]) {
            const file = input.files[0];
            if (fileNameDisp) fileNameDisp.textContent = file.name;
            if (fileSizeDisp) fileSizeDisp.textContent = (file.size / 1024).toFixed(1) + ' KB';
            if (filePreview) filePreview.classList.remove('d-none');
            if (submitBtn) submitBtn.disabled = false;
            dropZone.classList.add('d-none');

            // Handle Image preview and dimension inspection
            if (file.type.startsWith('image/')) {
                if (currentObjectUrl) {
                    URL.revokeObjectURL(currentObjectUrl);
                }
                currentObjectUrl = URL.createObjectURL(file);

                const img = new Image();
                img.onload = function () {
                    originalWidth = this.naturalWidth;
                    originalHeight = this.naturalHeight;

                    if (origDimBadge) {
                        origDimBadge.textContent = `${originalWidth} × ${originalHeight} px`;
                        origDimBadge.classList.remove('d-none');
                    }

                    if (imageThumbnail) {
                        imageThumbnail.src = currentObjectUrl;
                        imageThumbnail.classList.remove('d-none');
                    }
                    if (fileIcon) fileIcon.classList.add('d-none');

                    // Set initial target sizes
                    setDimensions(originalWidth, originalHeight, 100);

                    if (resizeSection) resizeSection.classList.remove('d-none');
                };
                img.src = currentObjectUrl;
            } else {
                if (origDimBadge) origDimBadge.classList.add('d-none');
                if (imageThumbnail) imageThumbnail.classList.add('d-none');
                if (fileIcon) fileIcon.classList.remove('d-none');
                if (resizeSection) resizeSection.classList.add('d-none');
            }
        }
    }

    function resetFile() {
        if (currentObjectUrl) {
            URL.revokeObjectURL(currentObjectUrl);
            currentObjectUrl = null;
        }
        fileInput.value = '';
        originalWidth = 0;
        originalHeight = 0;
        if (filePreview) filePreview.classList.add('d-none');
        if (submitBtn) submitBtn.disabled = true;
        if (imageThumbnail) {
            imageThumbnail.src = '';
            imageThumbnail.classList.add('d-none');
        }
        if (fileIcon) fileIcon.classList.remove('d-none');
        if (origDimBadge) origDimBadge.classList.add('d-none');
        dropZone.classList.remove('d-none');
    }
});
