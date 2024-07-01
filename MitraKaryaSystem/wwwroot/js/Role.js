$(document).ready(function () {
    Table.FillGridRole();
    Buttons.Init();
});
let Table = {
    FillGridPermission: function () {
        let tableID = '#tablePermission';
        $(tableID).DataTable({
            "deferRender": true,
            "processing": true,
            "serverSide": false,
            "destroy": true,
            "filter": true,
            "searching": false,
            "responsive": true,
        });
    },
    FillGridRole: function () {
        let tableID = $("#tableRole");
        let data = Common.GetData.Get('FillGridRole');
        let columns = [
            { data: 'name' },
            { data: 'description' },
            {
                data: null,
                render: function () {
                    return `
                   <div class="btn-group" role="group" aria-label="Action Buttons">
                <a class="btn btn-warning edit" href="#" role="button">
                    <i class="fa fa-pencil"></i> Edit
                </a>
                <a class="btn btn-danger delete" role="button">
                    <i class="fa fa-trash"></i> Delete
                </a>
            </div>
            `;
                },
                "orderable": false
            },
        ];

        tableID.DataTable({
            "deferRender": true,
            "processing": true,
            "serverSide": false,
            "destroy": true,
            "filter": true,
            "scrollX": true,
            "searching": false,
            "responsive": true,
            "data": data,
            "columns": columns
        });
        let tb = tableID.DataTable();
        tableID.find('tbody').unbind();
        tableID.find('tbody').on('click', '.edit', function (e) {

            let row = tb.row($(this).parents('tr')).data();
            Form.FillForm(row.id);
            $('#roleModal').modal('show');
        });
        tableID.find('tbody').on('click', '.delete', function (e) {
            let row = tb.row($(this).parents('tr')).data();
            // Show a confirmation dialog
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it',
                showLoaderOnConfirm: true,
                preConfirm: () => {
                    return fetch(`DeleteRole?id=${row.id}`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        }
                    })
                        .then(response => {
                            toastr.options.onShown = function () {
                                Table.FillGridRole();
                            }
                            response.ok ? toastr.success('Data has been deleted') : toastr.error('Data not deleted');
                        })
                        .catch(error => {
                            Swal.showValidationMessage(`Request failed: ${error}`);
                        });
                },
                allowOutsideClick: () => !Swal.isLoading()
            });
        });
    }

}

let Buttons = {
    Init: function () {
        $('#buttonSave').click(function () {
            Form.Save();
        });
        $('#buttonAddRole').click(function () {
            Form.FillForm(0);
        });
    },
}
var Form = {
    Save: function () {
        // Serialize the form data for Role
        var formData = $('#roleForm').serializeArray();

        // Collect permission data
        var permissions = [];
        $('input[name^="permission"]').each(function () {
            permissions.push({
                ID: $(this).val(),
                IsSelected: $(this).prop('checked')
            });
        });

        var requestData = {
            Role: {
                ID: formData.find(item => item.name === 'Role.ID').value,
                Name: formData.find(item => item.name === 'Role.Name').value,
                Description: formData.find(item => item.name === 'Role.Description').value
            },
            Permissions: permissions
        };
        // Show loading indicator
        $('#buttonSave').prop('disabled', true); // Disable the button
        $('#buttonSave .spinner-border').show(); // Show the spinner
        $.ajax({
            url: 'SaveRole',
            type: 'POST',
            contentType: 'application/json', // Set content type to JSON
            data: JSON.stringify(requestData),
            success: function (result) {
                toastr.options.onShown = function () {
                    Table.FillGridRole();
                    $('#roleModal').modal('hide');
                }
                result.success ? toastr.success('Data saved') : toastr.error('Data not saved');
                // Close loading indicator
                $('#buttonSave').prop('disabled', false); // Enable the button
                $('#buttonSave .spinner-border').hide(); // Hide the spinner
            },
            error: function (error) {
                toastr.error(error, 'Data not saved');
                // Close loading indicator
                $('#buttonSave').prop('disabled', false); // Enable the button
                $('#buttonSave .spinner-border').hide(); // Hide the spinner
            }
        });
    },

    FillForm: function (id) {
        $.ajax({
            url: 'FillFormRole',
            type: 'POST',
            data: { id: id },
            success: function (result) {
                $('#bodyModal').html(result);
                $('#roleModal').modal('show');
                Table.FillGridPermission();
            },
            error: function (error) {
                toastr.error(error, 'Error load data');
            }
        });
    }
}
