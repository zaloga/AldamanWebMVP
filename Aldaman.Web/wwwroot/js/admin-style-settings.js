/**
 * Admin Style Settings - Reset to Default handler
 */
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (event) {
        const button = event.target.closest('[data-action="reset-style-default"]');
        if (!button) return;

        event.preventDefault();

        const id = button.dataset.id;
        const key = button.dataset.key || '';
        const defaultValue = button.dataset.defaultValue || '';
        const url = button.dataset.url;
        const successMessage = button.dataset.successMsg || 'Updated successfully';

        if (!id || !url) return;

        const i18n = window.AdminI18n || {};
        Swal.fire({
            title: i18n.areYouSure,
            text: key + ' (' + defaultValue + ')',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#6366f1',
            cancelButtonColor: '#64748b',
            confirmButtonText: i18n.yes,
            cancelButtonText: i18n.cancel
        }).then((result) => {
            if (result.isConfirmed) {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
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
                                title: response.message || successMessage,
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
        });
    });
});
