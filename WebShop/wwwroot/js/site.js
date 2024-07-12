/* window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 3000);*/

function ShowDeleteDialog(e) {
    Swal.fire({
        title: 'Вы уверены?',
        text: "Данные нельзя будет востановить!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да',
        cancelButtonText: 'Назад',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '@Url.Action("Delete", "Home")',
                type: 'POST',
                data: { id: e },
                success: function (data)
                {
                    if (data.result == "deleted")
                    {
                        Swal.fire({
                            title: 'Удалено!',
                            text: 'Данные были удалены.',
                            icon: 'success',
                            showConfirmButton: true
                        }).then(function ()
                        {
                            window.location.reload();
                        }
                        )
                    }
                },
                error: function ()
                {
                    Swal.fire(
                        'Ошибка!',
                        'Произошла ошибка при удалении.',
                        'error');
                }
            });
        }
    });
}