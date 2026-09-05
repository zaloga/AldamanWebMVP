document.addEventListener('DOMContentLoaded', function () {
    // Handler for Unified Content selector in Content Groups

    const container = document.getElementById('selectedItemsContainer');
    const contentsSelect = document.getElementById('contentsSelect');
    const btnAddItem = document.getElementById('btnAddItem');

    if (!container || !btnAddItem || !contentsSelect) return;

    function updateIndicesAndOrders() {
        const rows = container.querySelectorAll('.content-group-item-row');
        rows.forEach((row, index) => {
            const idInput = row.querySelector('.item-id-input');
            const orderInput = row.querySelector('.item-order-input');
            const titleInput = row.querySelector('.item-title-input');
            const slugInput = row.querySelector('.item-slug-input');

            if (idInput) idInput.name = `SelectedItems[${index}].Id`;
            if (titleInput) titleInput.name = `SelectedItems[${index}].Title`;
            if (slugInput) slugInput.name = `SelectedItems[${index}].Slug`;
            if (orderInput) {
                orderInput.name = `SelectedItems[${index}].Order`;
                orderInput.value = index;
            }

            // Update badge order display
            const orderBadge = row.querySelector('.order-badge');
            if (orderBadge) {
                orderBadge.textContent = (index + 1).toString();
            }

            // Disable / Enable Move Buttons
            const btnUp = row.querySelector('.btn-move-up');
            const btnDown = row.querySelector('.btn-move-down');
            if (btnUp) btnUp.disabled = (index === 0);
            if (btnDown) btnDown.disabled = (index === rows.length - 1);
        });

        const emptyMsg = container.querySelector('.empty-items-message');
        if (emptyMsg) {
            if (rows.length === 0) {
                emptyMsg.classList.remove('d-none');
            } else {
                emptyMsg.classList.add('d-none');
            }
        }
    }

    btnAddItem.addEventListener('click', function () {
        const selectedOption = contentsSelect.options[contentsSelect.selectedIndex];
        if (!selectedOption || !selectedOption.value) return;

        const itemId = selectedOption.value;
        const itemTitle = selectedOption.getAttribute('data-title') || selectedOption.text;
        const itemSlug = selectedOption.getAttribute('data-slug') || '';

        // Check if already in list
        const existing = Array.from(container.querySelectorAll('.content-group-item-row')).find(r => {
            const id = r.querySelector('.item-id-input')?.value;
            return id === itemId;
        });

        if (existing) {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'info',
                    title: itemTitle,
                    text: 'Item is already added.',
                    confirmButtonColor: '#3085d6'
                });
            } else {
                alert('Item is already added.');
            }
            return;
        }

        const row = document.createElement('div');
        row.className = 'list-group-item d-flex justify-content-between align-items-center content-group-item-row p-2 mb-1 bg-white border rounded';
        row.innerHTML = `
            <div class="d-flex align-items-center gap-2 text-truncate me-2">
                <span class="badge bg-secondary order-badge">1</span>
                <span class="fw-medium text-dark text-truncate">${itemTitle}</span>
                <span class="text-muted small font-monospace d-none d-md-inline">/${itemSlug}</span>
            </div>
            <div class="d-flex align-items-center gap-1 flex-shrink-0">
                <input type="hidden" class="item-id-input" value="${itemId}" />
                <input type="hidden" class="item-title-input" value="${itemTitle}" />
                <input type="hidden" class="item-slug-input" value="${itemSlug}" />
                <input type="hidden" class="item-order-input" value="0" />
                <button type="button" class="btn btn-outline-secondary btn-sm btn-move-up" title="Move Up">
                    <i class="bi bi-arrow-up"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm btn-move-down" title="Move Down">
                    <i class="bi bi-arrow-down"></i>
                </button>
                <button type="button" class="btn btn-outline-danger btn-sm btn-remove-item" title="Remove">
                    <i class="bi bi-trash"></i>
                </button>
            </div>
        `;

        container.appendChild(row);
        updateIndicesAndOrders();
    });

    container.addEventListener('click', function (e) {
        const btnRemove = e.target.closest('.btn-remove-item');
        if (btnRemove) {
            const row = btnRemove.closest('.content-group-item-row');
            if (row) {
                row.remove();
                updateIndicesAndOrders();
            }
            return;
        }

        const btnUp = e.target.closest('.btn-move-up');
        if (btnUp) {
            const row = btnUp.closest('.content-group-item-row');
            const prev = row ? row.previousElementSibling : null;
            if (row && prev && prev.classList.contains('content-group-item-row')) {
                container.insertBefore(row, prev);
                updateIndicesAndOrders();
            }
            return;
        }

        const btnDown = e.target.closest('.btn-move-down');
        if (btnDown) {
            const row = btnDown.closest('.content-group-item-row');
            const next = row ? row.nextElementSibling : null;
            if (row && next && next.classList.contains('content-group-item-row')) {
                container.insertBefore(next, row);
                updateIndicesAndOrders();
            }
            return;
        }
    });

    // Initialize state on load
    updateIndicesAndOrders();
});
