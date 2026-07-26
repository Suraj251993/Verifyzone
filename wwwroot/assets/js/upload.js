const fileInput = document.getElementById('bulkFileUpload');
const progressWrapper = document.querySelector('.custom-upload-progress');
const progressBar = document.querySelector('.custom-progress-bar');
const uploadedFileName = document.getElementById('uploadedFileName');
var chartDom = document.getElementById('main');
var myChart;

var tableData = [];
var jsonResponse;
const recordsPerPage = 10;
let currentPage = 1;
 
fileInput.addEventListener('change', () => {
    if (fileInput.files.length > 0) {
        const fileName = fileInput.files[0].name;
 
        progressWrapper.style.display = 'block';
        progressBar.style.width = '0%';
        uploadedFileName.style.display = 'none';
 
        let progress = 0;
        const interval = setInterval(() => {
            progress += 2;
            if (progress > 100) progress = 100;
 
            progressBar.style.width = progress + '%';
            progressBar.textContent = progress + '%';   // show percent
 
            if (progress >= 100) {
                clearInterval(interval);
                progressWrapper.style.display = 'none';
 
                uploadedFileName.textContent = `Selected: ${fileName}`;
                uploadedFileName.style.display = 'block';
            }
        }, 100);
    }
});
function uploadFile(_url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    
    var type = 'POST';
    var formData = new FormData();
    if (fileInput.files.length == 0) {
        overlay.style.display = "none";
        showErrorMessage("Please choose file before upload");
        return;
    }
    formData.append("file", fileInput.files[0]);
    $.ajax({
        url: _url,
        type: type,
        data: formData,
        processData: false,
        contentType: false,
        error: function (xhr) {
            overlay.style.display = "none";
            console.log(xhr);
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (response) {
            overlay.style.display = "none";
            if (response != null) {
                //$('#stotal').val(response.validrecords);
                $('#flid').val(response.fileid);
                jsonResponse = response;
                $('#uploadPop').modal('show');                
            }
            else {
                //$('#loading').hide();
                showErrorMessage('Unable to upload the file. Please contact support team.');
            }

        },
        async: false,
        processData: false
    });
}
function DownloadTemplate(_url) {
    window.open(_url, '_blank');
}
function DrawChart() {
    if (myChart) {
        myChart.dispose(); // destroy previous chart instance
    }

    myChart = echarts.init(chartDom);

    var option = {
        tooltip: { trigger: 'item' },
        legend: {
            orient: 'horizontal',
            bottom: 0,
            left: 'center',
            icon: 'circle'
        },
        series: [
            {
                name: 'Status',
                type: 'pie',
                radius: ['20%', '70%'],
                center: ['50%', '45%'],
                data: [
                    { value: jsonResponse.validrecords, name: 'Valid', itemStyle: { color: '#47e76dff' } },
                    { value: jsonResponse.invalidrecords, name: 'Invalid', itemStyle: { color: '#ec4d5dff' } }
                ]
            }
        ]
    };

    myChart.setOption(option);
    myChart.resize();
}

function BindFileUploads(_url) {
    
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax(
        {
            type: 'GET',
            dataType: 'JSON',
            async: false,
            url: _url,
            success:
                function (jsonData) {
                    tableData = [];
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "filename": jsonData[i].filename,
                            "uploadeddate": jsonData[i].uploadeddate,
                            "uploadedstatus": jsonData[i].uploadedstatus,
                            "validrecords": jsonData[i].validrecords,
                            "invalidrecords": jsonData[i].invalidrecords,
                            "totalrecords": jsonData[i].totalrecords
                        });
                    }
                    renderTable(currentPage);
                    overlay.style.display = "none";
                },
            error:
                function (response) {
                    overlay.style.display = "none";
                    showErrorMessage("Error: " + response.status);
                }
        }
    );
    //$('#loading').hide();
}
function renderTable(page = 1) {
    
    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);
    let rows = '';
    pageData.forEach(item => {
        const statusHTML = {
            "Rejected": `<span class="bulk-table-reject"><i class="ri-close-line"></i> Rejected</span>`,
            "Approved": `<span class="bulk-table-approve"><i class="ri-check-double-line"></i> Approved</span>`,
            "Uploaded": `<span class="bulk-table-upload"><i class="ri-file-upload-line"></i> Uploaded</span>`
        }[item.uploadedstatus];

        rows += `
                                <tr>
                                    <td>${item.filename}</td>
                                    <td>${item.uploadeddate}</td>
                                    <td>${statusHTML}</td>
                                    <td>${item.validrecords}</td>
                                    <td>${item.invalidrecords}</td>
                                    <td>${item.totalrecords}</td>
                                </tr>
                            `;
        //tbody.insertAdjacentHTML("beforeend", row);
    });
    $('#tableIdx').empty();
    tbody.insertAdjacentHTML("beforeend", rows);
    renderPagination();
}

function renderPagination() {
    const pagination = document.querySelector(".pagination-custom");
    if (!pagination) return;

    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    let paginationHTML = `
        <li class="page-item-custom">
            <a class="page-link-custom prev" href="#" aria-label="Previous">
                <span aria-hidden="true"><i class="ri-arrow-left-s-line"></i></span>
            </a>
        </li>`;

    for (let i = 1; i <= totalPages; i++) {
        paginationHTML += `
            <li class="page-item-custom ${i === currentPage ? 'active' : ''}">
                <a class="page-link-custom" href="#" data-page="${i}">${i}</a>
            </li>`;
    }

    paginationHTML += `
        <li class="page-item-custom">
            <a class="page-link-custom next" href="#" data-page="${currentPage + 1}" aria-label="Next">
                <span aria-hidden="true"><i class="ri-arrow-right-s-line"></i></span>
            </a>
        </li>`;

    pagination.innerHTML = paginationHTML;
}

function changePage(page) {
    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;

    currentPage = page;
    
    renderTable(currentPage);
}
function ApprovedorRejectedFile(mode, _url) {
    var model = { "Mode": mode, "FileId": $('#flid').val() };
    if (mode == "Approve" && $('#stotal').val() == 0) {
        showErrorMessage("No valid records to approve. Please reject the file.");
        return;
    }
    //$('#loading').show();
    $.ajax({
        type: "POST",
        url: _url,
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            console.log(result); // = 'test'
            if (result == true) {
                $('#flid').val('');
                $("#uploadPop").modal('hide');
                if (mode == "Approve")
                    showSuccessMessage('File uploaded successfully.');
                else
                    showSuccessMessage('File rejected successfully.');
                BindFileUploads();                
                //$('#loading').hide();
            }
            else {
                $('#loading').hide();
                showErrorMessage('Error while uploading the file');
            }
        }
    });
}