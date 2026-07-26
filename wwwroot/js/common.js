
'use strict';

function showSuccessMessage1(message) {
    toastr.options = {
        maxOpened: 1,
        autoDismiss: true,
        closeButton: true,
        debug: false,
        newestOnTop: true,
        progressBar: true,
        positionClass: 'toast-top-right',
        preventDuplicates: false,
        onclick: null,
        rtl: isRtl,
        showDuration: 1200,
        hideDuration: 500,
        timeOut: 3000,
        extendedTimeOut: 1000,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
    };
    $('#toastheader').html('Success');
    $('#toastbody').html(message);
    const toastPlacementExample = document.querySelector('.toast-placement-ex');
    let selectedType, selectedAnimation, selectedPlacement, toast, toastAnimation, toastPlacement;
    selectedType = 'text-success';
    selectedPlacement = 'top-0 end-0'.split(' ');

    toastPlacementExample.querySelectorAll('i[class^="ri-"]').forEach(function (element) {
        element.classList.add(selectedType);
    });
    DOMTokenList.prototype.add.apply(toastPlacementExample.classList, selectedPlacement);
    toastPlacement = new bootstrap.Toast(toastPlacementExample);
    toastPlacement.show();
}

function showErrorMessage1(message) {
    toastr.options = {
        maxOpened: 1,
        autoDismiss: true,
        closeButton: true,
        debug: false,
        newestOnTop: true,
        progressBar: true,
        positionClass: 'toast-top-right',
        preventDuplicates: false,
        onclick: null,
        rtl: isRtl,
        showDuration: 1200,
        hideDuration: 500,
        timeOut: 3000,
        extendedTimeOut: 1000,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
    };
    $('#toastheader').html('Error');
    $('#toastbody').html(message);
    const toastPlacementExample = document.querySelector('.toast-placement-ex');
    let selectedType, selectedAnimation, selectedPlacement, toast, toastAnimation, toastPlacement;
    selectedType = 'text-danger';
    selectedPlacement = 'top-0 end-0'.split(' ');

    toastPlacementExample.querySelectorAll('i[class^="ri-"]').forEach(function (element) {
        element.classList.add(selectedType);
    });
    DOMTokenList.prototype.add.apply(toastPlacementExample.classList, selectedPlacement);
    toastPlacement = new bootstrap.Toast(toastPlacementExample);
    toastPlacement.show();
}

function showWarningMessage1(message) {
    toastr.options = {
        maxOpened: 1,
        autoDismiss: true,
        closeButton: true,
        debug: false,
        newestOnTop: true,
        progressBar: true,
        positionClass: 'toast-top-right',
        preventDuplicates: false,
        onclick: null,
        rtl: isRtl,
        showDuration: 1200,
        hideDuration: 500,
        timeOut: 3000,
        extendedTimeOut: 1000,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
    };
    $('#toastheader').html('Warning');
    $('#toastbody').html(message);
    const toastPlacementExample = document.querySelector('.toast-placement-ex');
    let selectedType, selectedAnimation, selectedPlacement, toast, toastAnimation, toastPlacement;
    selectedType = 'text-warning';
    selectedPlacement = 'top-0 end-0'.split(' ');

    toastPlacementExample.querySelectorAll('i[class^="ri-"]').forEach(function (element) {
        element.classList.add(selectedType);
    });
    DOMTokenList.prototype.add.apply(toastPlacementExample.classList, selectedPlacement);
    toastPlacement = new bootstrap.Toast(toastPlacementExample);
    toastPlacement.show();
}

function ShowBalance(_url) {
    //if (@ViewBag.CustomerType == "2") {    
    $.ajax({
        type: 'GET',
        url: _url, //'/Customer/GetCustomerBalance',
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            $('#lblbalance').html(data);
        },
        error: function (err) {
            console.log(err);
        }
    });
    //}
}

function ShowCompanyBalance() {
    //if (@ViewBag.CustomerType == "2") {
    $.ajax({
        type: 'GET',
        url: '/Company/GetCompanyBalance',
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            $('#lblbalance').html(data);
        },
        error: function (err) {
            console.log(err);
        }
    });
    //}
}