/**
 * Admin Item Actions (Delete, Hard Delete, Restore)
 * Shared JavaScript handling SweetAlert2 confirmations and AJAX requests across Admin area.
 */

document.addEventListener('DOMContentLoaded', function () {
    // Shared event delegation for buttons with data-action
    document.addEventListener('click', function (event) {
        const button = event.target.closest('[data-action="delete"], [data-action="hard-delete"], [data-action="restore"], [data-action="renew"]');
        if (!button) return;

        event.preventDefault();

        const action = button.dataset.action;
        const id = button.dataset.id;
        const title = button.dataset.title || '';
        const url = button.dataset.url;
        const customConfirmText = button.dataset.confirmText || null;

        if (!id || !url) return;

        if (action === 'delete') {
            deleteItem(id, title, url, customConfirmText);
        } else if (action === 'hard-delete') {
            hardDeleteItem(id, title, url, customConfirmText);
        } else if (action === 'restore' || action === 'renew') {
            restoreItem(id, title, url, customConfirmText);
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
