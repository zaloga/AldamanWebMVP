/**
 * Admin Media Upload - Drag and Drop File Selection script
 */
document.addEventListener('DOMContentLoaded', function () {
    const dropZone = document.getElementById('dropZone');
    const fileInput = document.getElementById('fileInput');
    const filePreview = document.getElementById('filePreview');
    const fileNameDisp = document.getElementById('fileName');
    const fileSizeDisp = document.getElementById('fileSize');
    const submitBtn = document.getElementById('submitBtn');
    const resetFileBtn = document.getElementById('resetFileBtn');

    if (!dropZone || !fileInput) return;

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

    function handleFileChange(input) {
        if (input.files && input.files[0]) {
            const file = input.files[0];
            if (fileNameDisp) fileNameDisp.textContent = file.name;
            if (fileSizeDisp) fileSizeDisp.textContent = (file.size / 1024).toFixed(1) + ' KB';
            if (filePreview) filePreview.classList.remove('d-none');
            if (submitBtn) submitBtn.disabled = false;
            dropZone.classList.add('d-none');
        }
    }

    function resetFile() {
        fileInput.value = '';
        if (filePreview) filePreview.classList.add('d-none');
        if (submitBtn) submitBtn.disabled = true;
        dropZone.classList.remove('d-none');
    }
});
