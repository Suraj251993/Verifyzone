var selectedCustId = '';
var selectedInstitutionId = '';

//function BindCustomers(id) {
//    $.ajax({
//        type: 'GET',
//        url: "/Customer/GetCustomers",
//        contentType: "application/json",
//        dataType: 'json', // changing data type to json
//        success: function (res) { // here I'm adding data as a parameter which stores the response
//            //console.log(res); // instead of alert I'm changing this to console.log which logs all the response in console.\
//            BindCustomerData(res, id);
//        },
//        error: function (err) {
//            $('#loading').hide();
//            alert(err.statusText);
//        }
//    });
//}
//function BindCustomerData(res, id) {
//    const jsonData = [];
//    var emptyjson = { id: "", name: "- Please choose customer -" };
//    jsonData.push(emptyjson);
//    for (let i = 0; i < res.length; i++) {
//        var json = { id: res[i].id, name: res[i].name };
//        jsonData.push(json);
//    }

//    const customerdropdownMenus = document.querySelectorAll("#" + id);

//    customerdropdownMenus.forEach(dropmenu => {
//        const input = dropmenu.closest(".inputContainer").querySelector("input");

//        // Populate dropdown dynamically
//        jsonData.forEach(item => {
//            const div = document.createElement("div");
//            div.className = "newinput-dropdown-item";
//            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

//            // On selecting an option
//            div.addEventListener("click", () => {
//                input.value = item.name;
//                selectedCustId = item.id;
//                dropmenu.classList.remove("show");
//            });

//            dropmenu.appendChild(div);
//        });

//        // Toggle dropdown on input click
//        input.addEventListener("click", (e) => {
//            e.stopPropagation();
//            dropmenu.classList.toggle("show");
//        });
//    });

//    // Close dropdown if clicked outside
//    document.addEventListener("click", () => {
//        customerdropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
//    });
//}
//function BindInstitutions(id) {
//    $.ajax({
//        type: 'GET',
//        url: "/Customer/GetInstitutions",
//        contentType: "application/json",
//        dataType: 'json', // changing data type to json
//        success: function (res) { // here I'm adding data as a parameter which stores the response
//            //console.log(res); // instead of alert I'm changing this to console.log which logs all the response in console.
//            BindInstitutionData(res, id);
//        },
//        error: function (err) {
//            $('#loading').hide();
//            showErrorMessage(err.statusText);
//        }
//    });
//}
//function BindInstitutionData(res, id) {
//    const jsonData = [];
//    var emptyjson = { id: "", name: "- Please choose institution -" };
//    jsonData.push(emptyjson);
//    for (let i = 0; i < res.length; i++) {
//        var json = { id: res[i].id, name: res[i].name };
//        jsonData.push(json);
//    }

//    const customerdropdownMenus = document.querySelectorAll("#" + id);

//    customerdropdownMenus.forEach(dropmenu => {
//        const input = dropmenu.closest(".inputContainer").querySelector("input");

//        // Populate dropdown dynamically
//        jsonData.forEach(item => {
//            const div = document.createElement("div");
//            div.className = "newinput-dropdown-item";
//            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

//            // On selecting an option
//            div.addEventListener("click", () => {
//                input.value = item.name;
//                selectedInstitutionId = item.id;
//                dropmenu.classList.remove("show");
//            });

//            dropmenu.appendChild(div);
//        });

//        // Toggle dropdown on input click
//        input.addEventListener("click", (e) => {
//            e.stopPropagation();
//            dropmenu.classList.toggle("show");
//        });
//    });

//    // Close dropdown if clicked outside
//    document.addEventListener("click", () => {
//        customerdropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
//    });
//}

function showSuccessMessage(message) {
    document.querySelectorAll('.toast-modal').forEach(modal => modal.style.display = 'none');

    const modal = document.getElementById("success");
    modal.style.display = 'flex';
    var msg = document.getElementById("successmessage");
    msg.innerHTML = message;

    //if (id === "success") {
    const bar = document.getElementById("errorProgress");

    bar.style.transition = "none";
    bar.style.width = "0%";

    setTimeout(() => {
        bar.style.transition = "width 2s linear";
        bar.style.width = "100%";
    }, 50);

    setTimeout(() => {
        modal.style.display = "none";
    }, 2000);
    //}
    modal.onclick = e => {
        if (e.target === modal) modal.style.display = 'none';
    };
}
function showErrorMessage(message) {
    document.querySelectorAll('.toast-modal').forEach(modal => modal.style.display = 'none');

    const modal = document.getElementById("error");
    modal.style.display = 'flex';

    var msg = document.getElementById("errormessage");
    msg.innerHTML = message;
    modal.onclick = e => {
        if (e.target === modal) modal.style.display = 'none';
    };
}
function closeModal(btn) {
    btn.closest('.toast-modal').style.display = 'none';
}

//function bindTransactionSummary() {
//    $.ajax({
//        type: "GET",
//        url: "/Customer/GetWalletTransactions",
//        contentType: "application/json",
//        dataType: 'json', // changing data type to json
//        success: function (jsonData) { // here I'm adding data as a parameter which stores the response
//            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
//            $('#transactionsummary').modal('show');
//            var tableData = [];
//            for (let i = 0; i < jsonData.length; i++) {
//                tableData.push({
//                    "text": jsonData[i].text,
//                    "value": jsonData[i].value
//                });
//            }
//            renderTableTS(tableData);
//            //$('#loading').hide();
//        },
//        error: function (err) {
//            showErrorMessage(err.statusText);
//        }
//    });
//}
//function renderTableTS(tableData) {

//    const tbody = document.querySelector("#tableIdxHdr");
//    if (!tbody) return;

//    tbody.innerHTML = "";

//    const start = 0;
//    const end = 10;
//    const pageData = tableData.slice(start, end);

//    pageData.forEach(item => {
//        const row = `
//            <tr>
//                <td>${item.text}</td>
//                <td>${item.value}</td>
//            </tr>
//        `;
//        tbody.insertAdjacentHTML("beforeend", row);
//    });
//}

async function downloadReportPDF(jsonData) {
    const {
        jsPDF
    } = window.jspdf;

    // 1. Create hidden container
    const hiddenDiv = document.createElement("div");
    hiddenDiv.style.position = "fixed";
    hiddenDiv.style.left = "-9999px";
    hiddenDiv.style.top = "0";
    hiddenDiv.style.width = "300mm"; // A4 width
    hiddenDiv.id = "pdfReportOnly";

    // 2. Report HTML (ONLY for download)
    var comments = ``;
    if (jsonData.hrComments != null || jsonData.hrComments.length == 0) {
        for (let i = 0; i < jsonData.hrComments.length; i++) {
            comments += `<tr>
                            <th scope="col">` + jsonData.hrComments[i].questions + `</th>
                             <td>` + jsonData.hrComments[i].answers + `</td>
                        </tr>`;
        }
    }
    else {
        comments = jsonData.comments;
    }
    var reportData = `
        <div class="container">
        <div class="generate-report-form mt-4 mb-4">
            <img src="/assets/img/report-img.png" alt="" style="height: 150px; width: auto;display: block;border-radius: 10px 0 0 0;">
            <div class="p-3">
                <div class="text-center">
                    <img src="/assets/img/logo-dash.png" alt="Full Logo" class="generate-report-logo">
                </div>
                <h5 class="generate-report-header mt-3">Employment verification report for ` + jsonData.candidateName + `</h5>

                <div>
                    <h6 class="generate-previous-emp-header mt-5"> <span class="header-border-report"></span>From
                        previous
                        employer - ` + jsonData.employer + `</h6>

                          <table class="mt-4 table table-bordered equal-table">
                    <thead>
                        <tr>
                            <th scope="col">Candidate name  :</th>
                             <td>` + jsonData.candidateName + `</td>
                            <th scope="col">Employee code  :</th>
                            <td>` + jsonData.employeeCode + `</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row">Designation  :</th>
                            <td>` + jsonData.desigination + `</td>
                           <th scope="row">Work location :</th>
                            <td>` + jsonData.location + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Date of joining :</th>
                            <td>` + jsonData.dateOfJoining + `</td>
                            <th scope="row">Date of leaving:</th>
                            <td>` + jsonData.dateOfLeaving + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Reporting manager :</th>
                            <td>` + jsonData.reportingManagerName + `</td>
                           <th scope="row">Reporting manager's designation :</th>
                            <td>` + jsonData.reportingManagerDesigination + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Reason of leaving :</th>
                            <td>` + jsonData.reasonforLeaving + `</td>
                           <th scope="row">Last drawn salary  :</th>
                            <td>` + jsonData.lastSalary + `</td>
                        </tr>
                    </tbody>
                </table>                               
                </div>
                <div>
                    <h6 class="generate-previous-emp-header mt-5"><span class="header-border-report"></span> HR's
                        comments
                    </h6>

                      <table class="mt-4 table table-bordered equal-table">
                    <!--<thead>
                        <tr>
                            <th scope="col">Duties & Responsibilities handled</th>
                             <td>Team handling</td>
                        </tr>
                    </thead>-->
                    <tbody>` + comments +

        `
                    </tbody>
                </table>                    
                </div>
                <div>
                    <h6 class="generate-previous-emp-header mt-5"><span class="header-border-report"></span>
                        Verification
                        information</h6>

                        <table class="mt-4 table table-bordered equal-table">
                    <thead>
                        <tr>
                            <th scope="col">Name of the verifier :</th>
                             <td>` + jsonData.hrName + `</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row">Designation of the verifier :</th>
                            <td>` + jsonData.hrDesigination + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Email id of the verifier:</th>
                            <td>` + jsonData.hrEmailId + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Date of the verification :</th>
                            <td>` + jsonData.dateOfVerification + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Report generated on :</th>
                            <td>` + jsonData.reportGeneratedOn + `</td>
                        </tr>
                    </tbody>
                </table>                    
                </div>

                <div class="d-flex justify-content-between mt-5">
                    <h6 class="generate-form-final-desc">Disclaimer: this report is system-generated.</h6>
                    <h6 class="generate-form-final-desc">A product by Tarajyau iServices Pvt.Ltd.</h6>
                </div>
            </div>
            
                <div class="text-end">
                    <img src="/assets/img/Report-down-img.png" alt="" style="height: 150px; width: auto;border-radius: 0 0 10px 0;">
                </div>
        </div>
    </div>
    `;

    hiddenDiv.innerHTML = reportData;

    document.body.appendChild(hiddenDiv);

    // 3. Convert to canvas
    const canvas = await html2canvas(hiddenDiv, {
        scale: 2,
        useCORS: true
    });

    const imgData = canvas.toDataURL("image/png");
    const pdf = new jsPDF("p", "mm", "a4");

    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    const imgWidth = pageWidth;
    const imgHeight = (canvas.height * imgWidth) / canvas.width;

    let heightLeft = imgHeight;
    let position = 0;

    pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
    heightLeft -= pageHeight;

    while (heightLeft > 0) {
        position -= pageHeight;
        pdf.addPage();
        pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
    }

    const today = new Date();

    const year = today.getFullYear();
    // getMonth() is 0-indexed (January is 0), so add 1
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');

    const formattedDate = `${day}-${month}-${year}-${today.getHours()}-${today.getMinutes()}-${today.getSeconds()}`;

    const reportname = "Employment_Verification_Report_" + jsonData.candidateName + "_" + formattedDate + ".pdf";
    pdf.save(reportname);

    // 4. Cleanup (remove hidden HTML)
    document.body.removeChild(hiddenDiv);
}

async function downloadNegativeReportPDF(jsonData) {
    const {
        jsPDF
    } = window.jspdf;

    // 1. Create hidden container
    const hiddenDiv = document.createElement("div");
    hiddenDiv.style.position = "fixed";
    hiddenDiv.style.left = "-9999px";
    hiddenDiv.style.top = "0";
    hiddenDiv.style.width = "300mm"; // A4 width
    hiddenDiv.id = "pdfReportOnly";

    // 2. Report HTML (ONLY for download)    
    var reportData = `
        <div class="container">
        <div class="generate-report-form mt-4 mb-4">
            <img src="/assets/img/report-img.png" alt="" style="height: 150px; width: auto;display: block;border-radius: 10px 0 0 0;">
            <div class="p-3">
                <div class="text-center">
                    <img src="/assets/img/logo-dash.png" alt="Full Logo" class="generate-report-logo">
                </div>
                <h5 class="generate-report-header mt-3">Employment verification report for ` + jsonData.candidateName + `</h5>

                <div>
                    <h6 class="generate-previous-emp-header mt-5"> <span class="header-border-report"></span>From
                        previous
                        employer - ` + jsonData.employer + `</h6>

                          <table class="mt-4 table table-bordered equal-table">
                    <thead>
                        <tr>
                            <th scope="col">Candidate name  :</th>
                             <td>` + jsonData.candidateName + `</td>
                            <th scope="col">Employee code  :</th>
                            <td>` + jsonData.employeeCode + `</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row">Designation  :</th>
                            <td>` + jsonData.desigination + `</td>
                           <th scope="row">Work location :</th>
                            <td>` + jsonData.location + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Date of joining :</th>
                            <td>` + jsonData.dateOfJoining + `</td>
                            <th scope="row">Date of leaving:</th>
                            <td>` + jsonData.dateOfLeaving + `</td>
                        </tr>                        
                    </tbody>
                </table>                               
                </div>
                <div>
                    <h6 class="generate-previous-emp-header mt-5"><span class="header-border-report"></span> Results
                    </h6>
                      <table class="mt-4 table table-bordered equal-table">                    
                    <tbody>
                        <tr>
                            <th scope="col">Request result  :</th>
                            <td><b><span style="color: red;">EMPLOYEE NOT FOUND IN OUR RECORDS</span></b></td>                           
                        </tr>
                        <tr>
                            <th scope="col">Comments given by the HR :</th>
                            <td>` + jsonData.comments + `</td>
                        </tr>                        
                    </tbody>
                </table>                    
                </div>
                <div>
                    <h6 class="generate-previous-emp-header mt-5"><span class="header-border-report"></span>
                        Verification
                        information</h6>

                        <table class="mt-4 table table-bordered equal-table">
                    <thead>
                        <tr>
                            <th scope="col">Name of the verifier :</th>
                             <td>` + jsonData.hrName + `</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row">Designation of the verifier :</th>
                            <td>` + jsonData.hrDesigination + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Email id of the verifier:</th>
                            <td>` + jsonData.hrEmailId + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Date of the verification :</th>
                            <td>` + jsonData.dateOfVerification + `</td>
                        </tr>
                        <tr>
                            <th scope="row">Report generated on :</th>
                            <td>` + jsonData.reportGeneratedOn + `</td>
                        </tr>
                    </tbody>
                </table>                    
                </div>

                <div class="d-flex justify-content-between mt-5">
                    <h6 class="generate-form-final-desc">Disclaimer: this report is system-generated.</h6>
                    <h6 class="generate-form-final-desc">A product by Tarajyau iServices Pvt.Ltd.</h6>
                </div>
            </div>
            
                <div class="text-end">
                    <img src="/assets/img/Report-down-img.png" alt="" style="height: 150px; width: auto;border-radius: 0 0 10px 0;">
                </div>
        </div>
    </div>
    `;

    hiddenDiv.innerHTML = reportData;

    document.body.appendChild(hiddenDiv);

    // 3. Convert to canvas
    const canvas = await html2canvas(hiddenDiv, {
        scale: 2,
        useCORS: true
    });

    const imgData = canvas.toDataURL("image/png");
    const pdf = new jsPDF("p", "mm", "a4");

    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    const imgWidth = pageWidth;
    const imgHeight = (canvas.height * imgWidth) / canvas.width;

    let heightLeft = imgHeight;
    let position = 0;

    pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
    heightLeft -= pageHeight;

    while (heightLeft > 0) {
        position -= pageHeight;
        pdf.addPage();
        pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
    }

    const today = new Date();

    const year = today.getFullYear();
    // getMonth() is 0-indexed (January is 0), so add 1
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');

    const formattedDate = `${day}-${month}-${year}-${today.getHours()}-${today.getMinutes()}-${today.getSeconds()}`;

    const reportname = "Employment_Verification_Report_" + jsonData.candidateName + "_" + formattedDate + ".pdf";
    pdf.save(reportname);

    // 4. Cleanup (remove hidden HTML)
    document.body.removeChild(hiddenDiv);
}