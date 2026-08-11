/**
 * Admin Item Actions (Delete, Hard Delete, Restore, Filter Clear, Cover Image File Selection)
 * Shared JavaScript handling SweetAlert2 confirmations and AJAX requests across Admin area.
 */

function initAdminI18n() {
    if (!window.AdminI18n) {
        const i18nElem = document.getElementById('admin-i18n-data');
        if (i18nElem && i18nElem.textContent) {
            try {
                window.AdminI18n = JSON.parse(i18nElem.textContent);
            } catch (e) {
                console.error('Failed to parse admin i18n data', e);
            }
        }
    }
}

document.addEventListener('DOMContentLoaded', function () {
    initAdminI18n();

    // Shared event delegation for buttons with data-action
    document.addEventListener('click', function (event) {
        // Delete / Hard-delete / Restore / Renew buttons
        const actionButton = event.target.closest('[data-action="delete"], [data-action="hard-delete"], [data-action="restore"], [data-action="renew"]');
        if (actionButton) {
            event.preventDefault();

            const action = actionButton.dataset.action;
            const id = actionButton.dataset.id;
            const title = actionButton.dataset.title || '';
            const url = actionButton.dataset.url;
            const customConfirmText = actionButton.dataset.confirmText || null;

            if (!id || !url) return;

            if (action === 'delete') {
                deleteItem(id, title, url, customConfirmText);
            } else if (action === 'hard-delete') {
                hardDeleteItem(id, title, url, customConfirmText);
            } else if (action === 'restore' || action === 'renew') {
                restoreItem(id, title, url, customConfirmText);
            }
            return;
        }

        // Clear Filter buttons
        const clearBtn = event.target.closest('[data-action="clear-filter"]');
        if (clearBtn) {
            event.preventDefault();
            const form = clearBtn.closest('form');
            if (form) {
                if (form.SearchTerm) form.SearchTerm.value = '';
                if (form.SortBy) form.SortBy.value = clearBtn.dataset.defaultSortBy || '';
                if (form.SortDescending) form.SortDescending.value = clearBtn.dataset.defaultSortDesc || 'true';
                form.submit();
            }
            return;
        }
    });

    // Delegated event listener for file inputs with cover image display target
    document.addEventListener('change', function (event) {
        const coverInput = event.target.closest('[data-action="cover-file-input"]');
        if (!coverInput) return;

        const displayId = coverInput.dataset.displayTarget || 'coverFileNameDisplay';
        const display = document.getElementById(displayId);
        if (display) {
            if (coverInput.files && coverInput.files.length > 0) {
                display.textContent = coverInput.files[0].name;
                display.classList.remove('text-muted');
            } else {
                const noFileText = coverInput.dataset.noFileText || '';
                display.textContent = noFileText;
                display.classList.add('text-muted');
            }
        }
    });
});

/**
 * Standard soft delete confirmation and AJAX handler.
 */
function deleteItem(id, title, url, customConfirmText) {
    const i18n = window.AdminI18n || {};
    const textTemplate = customConfirmText || i18n.confirmDeleteText;
    const confirmText = textTemplate ? textTemplate.replace('{0}', title) : title;

    Swal.fire({
        title: i18n.areYouSure,
        text: confirmText,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: i18n.yesDeleteIt,
        cancelButtonText: i18n.cancel
    }).then((result) => {
        if (result.isConfirmed) {
            executeAction(url, id, i18n.deletedSuccessfully);
        }
    });
}

/**
 * Permanent hard delete confirmation and AJAX handler.
 */
function hardDeleteItem(id, title, url, customConfirmText) {
    const i18n = window.AdminI18n || {};
    const textTemplate = customConfirmText || i18n.confirmDeletePermanentlyText;
    const confirmText = textTemplate ? textTemplate.replace('{0}', title) : title;

    Swal.fire({
        title: i18n.areYouSure,
        text: confirmText,
        icon: 'error',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: i18n.deletePermanentlyButton,
        cancelButtonText: i18n.cancel
    }).then((result) => {
        if (result.isConfirmed) {
            executeAction(url, id, i18n.deletedSuccessfully);
        }
    });
}

/**
 * Item restore/renew confirmation and AJAX handler.
 */
function restoreItem(id, title, url, customConfirmText) {
    const i18n = window.AdminI18n || {};
    const textTemplate = customConfirmText || i18n.confirmRestoreText;
    const confirmText = textTemplate ? textTemplate.replace('{0}', title) : title;

    Swal.fire({
        title: i18n.areYouSure,
        text: confirmText,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#16a34a',
        cancelButtonColor: '#64748b',
        confirmButtonText: i18n.yes,
        cancelButtonText: i18n.cancel
    }).then((result) => {
        if (result.isConfirmed) {
            executeAction(url, id, i18n.restoredSuccessfully);
        }
    });
}

/**
 * Helper to perform AJAX POST request and display notification.
 */
function executeAction(url, id, defaultSuccessMsg) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    const i18n = window.AdminI18n || {};

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            id: id,
            __RequestVerificationToken: token
        },
        success: function (response) {
            if (response && response.success) {
                Swal.fire({
                    title: response.message || defaultSuccessMsg,
                    icon: 'success'
                }).then(() => {
                    location.reload();
                });
            } else {
                const msg = (response && response.message) ? response.message : i18n.errorOccurred;
                Swal.fire(i18n.error, msg, 'error');
            }
        },
        error: function () {
            Swal.fire(i18n.error, i18n.errorOccurred, 'error');
        }
    });
}
