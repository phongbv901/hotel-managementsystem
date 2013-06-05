
function GetRentByStatus(status,pageIndex,pageSize) {
    $("#RentTable").html('<img src="../../Content/imgs/loading.gif"/>');
    $.ajax({
        url: "/Admin/SearchRentByStatus",
        type: "GET",
        data: { status:status,pageIndex: pageIndex, pageSize:pageSize},
        success: function (result) {
            $("#RentTable").fadeOut(100, function () {
                $('#RentTable').html(result);
                $("#RentTable").fadeIn(100);
            });
        }
    });

}


function ChangeRentStatus(rentID,fromStatus,toStatus,index,isConfirm,confirmText) {

    var rs = true;
    if (isConfirm) {
        rs = confirm(confirmText);
    }

    if (rs) {
        $.ajax({
            url: "/Admin/ChangeRentStatus",
            type: "GET",
            data: { RentID: rentID, fromStatus: fromStatus, toStatus: toStatus, index: index },
            success: function (result) {
                $("#RentID-" + rentID).replaceWith(result);
            }
        });
        
    }
}

function ApproveAllRent() {
    $.ajax({
        url: "/Admin/ApproveAllRents",
        type: "GET",
        success: function (result) {
            GetRentByStatus(1, 1, 20);
            $('#btnInstay').addClass('active');
            $('#btnPaid').removeClass('active');
        }
    });
}







function ExportToExcel() {

    var beginDate = $('#StartDate').val();
    var endDate = $('#EndDate').val();
    beginDate = ConvertDateVNToUS(beginDate);
    endDate = ConvertDateVNToUS(endDate);
    $.ajax({
        url: "/Admin/ExportToExcel",
        type: "GET",
        data: { StartTime: beginDate, EndTime: endDate }
    });
}
    