
////////////// RENT BOOKING FORM  ////////////////////////////////
function ShowBookingForm(roomId,roomName) {
    $("#modal").html('<img src="../../Content/imgs/loading.gif"/>');
    $("#modal").modal('toggle').on('shown', function () {
        $('body').css('overflow', 'hidden');


    }).on('hide', function () {
        $('body').css('overflow', 'auto');
    });
    
    
    
    
    $.ajax({
        url: "/Rent/ShowBookingForm",
        type: "GET",
        data: { roomId: roomId,roomName:roomName },
        success: function (result) {
            $("#modal").html("");
            $("#modal").html(result);
            $('#CheckInDate').datetimepicker({ dateFormat: 'dd-mm-yy' });
            $('#CheckInDate').datetimepicker('setDate', (new Date()));
            $('#InvoiceID').val(GetDateStamp());
        }
    });
}

function ChangeRentType(rentType) {
    //Change rent type button
    $("#hdfRentType").val(rentType);

}

function BookRoom() {
    var roomId = $("#roomId").val();
    var checkInDate = ConvertDateVNToUS($('#CheckInDate').val());
    var invoiceId = $("#InvoiceID").val();
    var rentType = $("#hdfRentType").val();
    var bikeId = $("#BikeID").val();
    var notes = $("#Notes").val();
    $.ajax({
        url: "Rent/BookRoom",
        type: "GET",
        data: { roomId: roomId, invoiceId: invoiceId, checkInDate: checkInDate, rentType: rentType, bikeId: bikeId, notes: notes },
        success: function (result) {
            if (result != -1) {
                ShowRent(result);
            }
            else {
                $("#modal").html("<h2>Có lỗi xảy ra!Phòng đã được đặt</h2>");
                UpdateRoomStatus(roomId);
            }
        }
    });

}

function GetDateStamp() {
    var date = new Date();
    var dateStr = padStr(date.getFullYear() - 2000) +
            padStr(1 + date.getMonth()) +
                padStr(date.getDate()) +
                    padStr(date.getHours()) +
                        padStr(date.getMinutes()) +
                            padStr(date.getSeconds());
    return dateStr;
}

function padStr(i) {
    return (i < 10) ? "0" + i : "" + i;
}



