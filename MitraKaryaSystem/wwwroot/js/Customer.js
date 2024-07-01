$(document).ready(function () {
    Buttons.Init();
    Table.FillGrid();
    Control.Init();
});
let Table = {
    FillGrid: function () {
        let data = Common.GetData.Get('/Customer/GetList');
        let tableID = $("#tableCustomer");
        // Define an array to store the checked checkbox values
        let columns = [
            { data: 'name' },
            { data: 'contactPerson' },
            { data: 'contactNumber' },
            { data: 'isSupplier' },
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
        $(tableID).DataTable({
            "deferRender": true,
            "processing": true,
            "serverSide": false,
            "destroy": true,
            "filter": false,
            "searching": false,
            "responsive": true,
            "data": data,
            "columns": columns,
            "dom": 'lBfrtip',
            "columnDefs": [{ "targets": [0, 1, 2, 3, 4], "className": "text-left" }],
        });
        let tb = tableID.DataTable();
        tableID.find('tbody').unbind();
        tableID.find('tbody').on('click', '.edit', function (e) {

            let row = tb.row($(this).parents('tr')).data();
            Forms.FillForm(row.id);
            $('#userModal').modal('show');
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
                    return fetch(`/Customer/Delete?id=${row.id}`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        }
                    })
                        .then(response => {
                            toastr.options.onShown = function () {
                                $('#customerModal').modal('hide');
                                Table.FillGrid();
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
    },
}
let Buttons = {
    Init: function () {
        $('#buttonAdd').click(function () {
            Forms.FillForm(0);
        });
        $('#buttonSave').click(function () {
            let form = $('#customerForm')[0];

            if (form.checkValidity()) {
                // If the form is valid, save the product and reset the form
                event.preventDefault();
                event.stopPropagation();
                Forms.Save();
                form.classList.remove('was-validated');
            } else {
                // If the form is invalid, add 'was-validated' class to apply Bootstrap validation styling
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            }
        });
    }
}

let Forms = {
    Save: function () {
        // Serialize the form data
        let formData = $('#customerForm').serialize();
        // Show loading indicator
        $('#buttonSave').prop('disabled', true); // Disable the button
        $('#buttonClose').prop('disabled', true); // Disable the button

        $('#buttonSave .spinner-border').show(); // Show the spinner
        $.ajax({
            url: '/Customer/Save',
            type: 'POST',
            data: formData,
            success: function (result) {
                toastr.options.onShown = function () {
                    $('#customerModal').modal('hide');
                    Table.FillGrid();
                }

                result.result.success ? toastr.success('Data saved') : toastr.error('Data not saved');
                // Close loading indicator
                $('#buttonSave').prop('disabled', false); // Enable the button
                $('#buttonClose').prop('disabled', false); // Disable the button

                $('#buttonSave .spinner-border').hide(); // Hide the spinner
            },
            error: function (error) {
                toastr.error(error, 'Data not saved');
                // Close loading indicator
                $('#buttonSave').prop('disabled', false); // Enable the button
                $('#buttonClose').prop('disabled', false); // Disable the button

                $('#buttonSave .spinner-border').hide(); // Hide the spinner
            }
        });
    },

    FillForm: function (id) {
        $.ajax({
            url: '/Customer/FillForm',
            type: 'POST',
            data: { id: id },
            success: function (result) {
                $('#bodyModal').html(result);
                Control.Visibility();
                $('#customerModal').modal('show');
            },
            error: function (error) {
                toastr.error(error, 'Error load data');
            }
        });
    }

};

let Control = {
    Init: function () {
        $(document).on('change', '#isSupplier', function () {
            Control.Visibility();
        });
    },
    Visibility: function () {
        let isSupplierChecked = $("#isSupplier").prop("checked");
        let contactPersonInput = $("#contactPerson");

        // Function to update validation state based on input value
        function updateValidationState() {
            if (contactPersonInput.val() === '') {
                // If input value is empty, add required attribute and show invalid feedback
                contactPersonInput.prop('required', true);
                contactPersonInput.addClass('is-invalid');
                contactPersonInput.next('.invalid-feedback').show();
            } else {
                // If input value is not empty, remove required attribute and hide invalid feedback
                contactPersonInput.prop('required', false);
                contactPersonInput.removeClass('is-invalid');
                contactPersonInput.next('.invalid-feedback').hide();
            }
        }

        // Update validation state when the "Is Supplier" checkbox is changed
        if (isSupplierChecked) {
            $("#contactPersonFormGroup").show();
            updateValidationState();
        } else {
            $("#contactPersonFormGroup").hide();
            // Reset validation state when hiding the form group
            contactPersonInput.prop('required', false);
            contactPersonInput.removeClass('is-invalid');
            contactPersonInput.next('.invalid-feedback').hide();
        }

        // Update validation state when the input field value changes
        contactPersonInput.on('input', updateValidationState);
    }

}