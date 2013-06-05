//function SearchRent() {
//    var beginDate = $('#StartDate').val();
//    var endDate = $('#EndDate').val();
//    beginDate = ConvertDateVNToUS(beginDate);
//    endDate = ConvertDateVNToUS(endDate);
//    $.ajax({
//        url: "/Admin/SearchRent",
//        type: "GET",
//        data: { StartTime: beginDate, EndTime: endDate },
//        success: function (result) {
//            $("#RentTable").fadeOut(300, function () {
//                $('#RentTable').html(result);
//                $("#RentTable").fadeIn(300);
//            });
//        }
//    });
//}

function GetCostByStatus(status,pageIndex,pageSize) {
    $("#RentTable").html('<img src="../../Content/imgs/loading.gif"/>');
    $.ajax({
        url: "/Cost/SearchCostByStatus",
        type: "GET",
        data: { status:status,pageIndex: pageIndex, pageSize:pageSize},
        success: function (result) {
            $("#CostTable").fadeOut(100, function () {
                $('#CostTable').html(result);
                $("#CostTable").fadeIn(100);
            });
        }
    });

}


function ChangeCostStatus(costId,fromStatus,toStatus,index,isConfirm,confirmText) {

    var rs = true;
    if (isConfirm) {
        rs = confirm(confirmText);
    }

    if (rs) {
        $.ajax({
            url: "/Cost/ChangeCostStatus",
            type: "GET",
            data: { costId: costId, fromStatus: fromStatus, toStatus: toStatus, index: index },
            success: function (result) {
                $("#cost-" + costId).replaceWith(result);
            }
        });
        
    }
}


function ShowAddCostModal() {
    $("#WindowsFromAjax").html('<img src="../../Content/imgs/loading.gif"/>');
    $("#WindowsFromAjax").dialog("open");

    $.ajax({
        url: "/Cost/ShowAddCostModal",
        type: "GET",
        success: function (result) {
            $("#WindowsFromAjax").html(result);
        }
    });
}


function  SubmitAddCost() {
    var date = $('#Cost_CostDate').val();
    date = ConvertDateVNToUS(date);
    $("#Cost_CostDate").val(date);
    alert("dada");
    $.ajax({
        url: "/Cost/AddCost",
        data: $('#formCost').serialize(),
        type: "GET",
        success: function (result) {
            alert("Dada");
            $("#WindowsFromAjax").dialog("close");
            LoadNewCost();
        }
    });
}

function  LoadNewCost() {
     $('#btnNewCost').addClass('active');
    //1: New Status, page 1, 20 item in pages    
    GetCostByStatus(1, 1, 20);
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
    