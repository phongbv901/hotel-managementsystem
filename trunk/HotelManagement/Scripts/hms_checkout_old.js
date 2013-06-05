
///////////////////////////////// CHECKOUT FORM //////////////////////////////////////////////////////
function ShowCheckoutForm(data) {

    $("#WindowsFromAjax").html('<img src="../../Content/imgs/loading.gif"/>');
    var rentid = $(data).attr('data-rentid');
    $("#WindowsFromAjax").dialog("open");
    $.ajax({
        url: "Checkout",
        type: "GET",
        data: { rentID: rentid },
        success: function (result) {
          
            $("#WindowsFromAjax").html(result);
            FocusRentType();
            SetDateTimePicker();
        }
    });
}

function ShowCheckoutSummary(data) {

    $("#WindowsFromAjax").html('<img src="../../Content/imgs/loading.gif"/>');
    var rentid = $(data).attr('data-rentid');
    $("#WindowsFromAjax").dialog("open");
    $.ajax({
        url: "/Checkout/Summary",
        type: "GET",
        data: { rentID: rentid},
        success: function (result) {
            $("#WindowsFromAjax").html(result);
        }
    });
}



function ChangeFee(parameters) {
    var rentFee = parseInt($(parameters).val().replace(/,/g, ""));
    var orderFee = parseInt($("#orderFee").val().replace(/,/g, ""));
    var totalPayment = parseInt($("#totalPayment").val().replace(/,/g,""));
   
    var totalFee = rentFee + orderFee;
    var remainFee = totalFee - totalPayment;
    var priceType = $(parameters).attr("data-rentType"); 
    $("#hdfRentType").val(priceType);
  
    $("#rentFee").val(addCommas(rentFee));
    $("#totalFee").val(addCommas(totalFee));
    $("#remainFee").val(addCommas(remainFee));
}


function UpdateFee() {
   
    var rentID = $("#rentId").val();
    var checkOutDate = $('#Rent_CheckOutDate').val(); //Do co che tu sinh ID cua MVC 
    var checkInDate = $('#Rent_CheckInDate').val();

    var rentType = $("#hdfRentType").val();
//    var roomid = $("#current-roomid").val();
    
    $.ajax({
        url: "/Checkout/UpdateRentInfo",
        type: "GET",
        data: { rentID: rentID, checkInDate: ConvertDateVNToUS(checkInDate), checkOutDate: ConvertDateVNToUS(checkOutDate) },
        success: function (result) {

            $("#btnHourFee").val(result.hourFee);
            $("#btnNightFee").val(result.nightFee);
            $("#btnDayFee").val(result.dayFee);
            $("#hdfLowestPriceMode").val(result.lowestFeeMode);

            if (result.nightFee == 0) {
                $("#btnNightFee").attr("disabled", "disabled");
            }
            else {
                $("#btnNightFee").removeAttr("disabled");

            }

            //Day la ket qua tinh theo tong hop ngay , dem , gio
            if ((result.dayFee == 0) || (result.dayFee == result.hourFee) || (result.dayFee == result.nightFee)) {
                $("#btnDayFee").attr("disabled", "disabled");
            }
            else {
                $("#btnDayFee").removeAttr("disabled");


            }


           

            FocusRentType();

        }
    });

}

function CheckOut() {

    var rentID = $("#rentId").val();
    var checkOutDate = $('#Rent_CheckOutDate').val(); //Do co che tu sinh ID cua MVC 
    var checkInDate = $('#Rent_CheckInDate').val();
    var isPayAll = $('#IsPayAll').val();
    var rentType = $("#hdfRentType").val();
    var roomid = $("#current-roomid").val();
    $.ajax({
        url: "/Checkout/CheckOut",
        type: "GET",
        data: { rentID: rentID, checkInDate: ConvertDateVNToUS(checkInDate), checkOutDate: ConvertDateVNToUS(checkOutDate), rentType: rentType, isPayAll: isPayAll },
        success: function (result) {
            UpdateRoomStatus(roomid);
            $("#WindowsFromAjax").html(result);
        }
    });

}



function SetDateTimePicker() {
    //Chinh datetime picker
    var checkindate = $('#Rent_CheckInDate').val();
    $('#Rent_CheckInDate').val(ConvertDateVNToUS(checkindate));
    $('#Rent_CheckInDate').datetimepicker({
        onClose: function (dateText, inst) {
            UpdateFee();
        },
        dateFormat: 'dd-mm-yy'
    });
    $('#Rent_CheckInDate').datetimepicker('setDate', (new Date($('#Rent_CheckInDate').val())));

    //Chinh Datetime picker

    var checkoutdate = $('#Rent_CheckOutDate').val();
    $('#Rent_CheckOutDate').val(ConvertDateVNToUS(checkoutdate));
    $('#Rent_CheckOutDate').datetimepicker({
        onClose: function (dateText, inst) {
            UpdateFee();
        },
        dateFormat: 'dd-mm-yy'
    });

    $('#Rent_CheckOutDate').datetimepicker('setDate', (new Date($('#Rent_CheckOutDate').val())));
}


function FocusRentType() {
    

    //Focus rent type button
    var rentType = $("#hdfRentType").val();
    
    if ($('[data-rentType =' + rentType + ']').val() == 0) {
        var lowestPriceMode = $("#hdfLowestPriceMode").val();
        rentType = lowestPriceMode;
        $("#hdfRentType").val(rentType);
        
    }
    
    $('[data-rentType]').removeClass("active");
    var btnCurrentFee = $('[data-rentType =' + rentType + ']');
    btnCurrentFee.addClass("active");

    ChangeFee(btnCurrentFee);
}


function UpdateRoomStatus(roomId) {
    $.ajax({
        url: "/Room/GetRoom",
        type: "GET",
        data: { id: roomId },
        success: function (result) {
            $("#contain-" + roomId).fadeOut(300, function () {
                $("#contain-" + roomId).html(result);
                $("#contain-" + roomId).fadeIn();

            });
        }
    });
}


function AnimateRoomStatus(roomId) {
    $("#r-" + roomId).addClass("aRoomBusy");
    $("#r-" + roomId).fadeOut(300, function () {
        $("#r-" + roomId).removeClass("aRoomBusy");
        $("#r-" + roomId).fadeIn();

        });
   
    
}

function CloseCheckoutSummary(parameter) {

    var roomID = $(parameter).attr("data-roomid");
    CloseWindowsFromAjax();
    AnimateRoomStatus(roomID);
}

function printMe(hotelName,address,phone) {
    
    var dispsetting = "toolbar=yes,location=no,directories=yes,menubar=yes,";
    dispsetting += "scrollbars=yes,width=780, height=780, left=100, top=25";

//    var service = $('#tblServiceDetail').clone();
    //    service.find('tr').find(".imgHeader").remove();

    var i = 0;
    var docprint = window.open("", "", dispsetting);
    docprint.document.open();
    docprint.document.write('<html><head><title>HÓA ĐƠN TÍNH TIỀN</title>');
    docprint.document.write('</head><body>');
    docprint.document.write('Khách sạn ' + hotelName +'<br/>ĐC: ' + address + '<br/>ĐT:' + phone );
    docprint.document.write('</br>*********************');
    docprint.document.write('<br/>Giờ vào: ' + $("#CheckInDate").val());
    docprint.document.write('<br/>Giờ ra: ' + $("#CheckOutDate").val());
    docprint.document.write('<br/>Tiền phòng:<br/>' + $("#rentFee").val());
    docprint.document.write('<br/>Dịch vụ <br/>');
    //Print service
    $('#tblServiceDetail tr').each(function () {
        if (!$(this).hasClass('trHead')) {
            var productName = $(this).find('td').eq(1).html();
            var quantity = $(this).find('td').eq(2).html();
            var totalPrice = $(this).find('td').eq(3).html();
            docprint.document.write(productName +'  ' + quantity + '  ' + totalPrice +'</br>');
        }

    });

    docprint.document.write('Tiền dịch vụ:<br/>' + $("#orderFee").val());
    docprint.document.write('<br/>Tổng cộng: <br/> ' + $("#totalFee").val());
    
  
    docprint.document.write('</body></html>');
    docprint.document.close();
    docprint.print();
   docprint.close();
}












