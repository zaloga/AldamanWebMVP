/**
 * Admin Item Actions (Delete, Hard Delete, Restore)
 * Shared JavaScript handling SweetAlert2 confirmations and AJAX requests across Admin area.
 */

document.addEventListener('DOMContentLoaded', function () {
    // Shared event delegation for buttons with data-action
    document.addEventListener('click', function (event) {
        const button = event.target.closest('[data-action="delete"], [data-action="hard-delete"], [data-action="restore"]');
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
        } else if (action === 'restore') {
            restoreItem(id, title, url);
        }
    });
});

/**
 * Standard soft delete confirmation and AJAX handler.
 */
function deleteItem(id, title, url, customConfirmText) {
    const i18n = window.AdminI18n || {};
    const textTemplate = customConfirmText || i18n.confirmDeleteText || 'Opravdu chcete smazat „{0}“?';
    const confirmText = textTemplate.replace('{0}', title);

    Swal.fire({
        title: i18n.areYouSure || 'Jste si jistí?',
        text: confirmText,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: i18n.yesDeleteIt || 'Ano, smazat!',
        cancelButtonText: i18n.cancel || 'Zrušit'
    }).then((result) => {
        if (result.isConfirmed) {
            executeAction(url, id, i18n.deletedSuccessfully || 'Úspěšně smazáno');
        }
    });
}

/**
 * Permanent hard delete confirmation and AJAX handler.
 */
function hardDeleteItem(id, title, url, customConfirmText) {
    const i18n = window.AdminI18n || {};
    const textTemplate = customConfirmText || i18n.confirmDeletePermanentlyText || 'Opravdu chcete TRVALE smazat „{0}“?';
    const confirmText = textTemplate.replace('{0}', title);

    Swal.fire({
        title: i18n.areYouSure || 'Jste si jistí?',
        text: confirmText,
        icon: 'error',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: i18n.deletePermanentlyButton || 'Ano, smazat navždy!',
        cancelButtonText: i18n.cancel || 'Zrušit'
    }).then((result) => {
        if (result.isConfirmed) {
            executeAction(url, id, i18n.deletedSuccessfully || 'Úspěšně smazáno');
        }
    });
}

/**
 * Item restore handler via AJAX.
 */
function restoreItem(id, title, url) {
    const i18n = window.AdminI18n || {};
    executeAction(url, id, i18n.restoredSuccessfully || 'Úspěšně obnoveno');
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
                Swal.fire(defaultSuccessMsg, response.message || '', 'success').then(() => {
                    location.reload();
                });
            } else {
                const msg = (response && response.message) ? response.message : (i18n.errorOccurred || 'Chyba při provádění akce');
                Swal.fire(i18n.error || 'Chyba', msg, 'error');
            }
        },
        error: function () {
            Swal.fire(i18n.error || 'Chyba', i18n.errorOccurred || 'Došlo k neočekávané chybě', 'error');
        }
    });
}
