//////////////////////////////////////// HOTEL DATA ///////////////////////////////

var hotelName = 'KIM NGÂN';
var address = "32-34 KDC Trung Son";
var phone = "08-54333339";
//////////////////////////////////////////////////////////////////////////////////

function CloseRentDetailForm() {
    var roomId = $("#roomId").val();
    var rentType = $("#hdfRentType").val();

    //Kiem tra rentType o mainform neu khac thi load

    var roomRentType = $("#r-" + roomId).attr("data-rentType");
    if  (roomRentType != rentType) {
        UpdateRoomStatus(roomId);
    }
}

function UpdateRoomStatus(roomId) {
   
        $.ajax({
            url: "/Room/GetRoom",
            type: "GET",
            data: { id: roomId },
            success: function (result) {
                $("#contain-" + roomId).fadeOut(200, function () {
                    $("#contain-" + roomId).html(result);
                    $("#contain-" + roomId).fadeIn();

                });
            }
        });
    
}


////////////////////////// SHOW RENT DETAIL //////////////////////////////////



function ShowRent(rentId) {
    $("#modal").html('<img src="../../Content/imgs/loading.gif"/>');
    $("#modal").modal('toggle').on('shown', function () {
        $('body').css('overflow', 'hidden');


    }).on('hide', function () {
        $('body').css('overflow', 'auto');
        CloseRentDetailForm();
    });
    
    
    $.ajax({
        url: "/Rent/Index",
        type: "GET",
        data: { rentId: rentId },
        success: function (result) {
            $("#modal").html(result);
            FocusRentType();
            SetDateTimePicker();
            ChangeStatusUpdateButton(true);
            applyScrollbar();
        }
    });
    
    
    



   
}

function ShowRentDetailReadOnly(rentid) {
    $("#WindowsFromAjax").html('<img src="../../Content/imgs/loading.gif"/>');
    $("#WindowsFromAjax").dialog("open");

    $.ajax({
        url: "/Rent/ViewRentReadonly",
        type: "GET",
        data: { rentId: rentid },
        success: function (result) {
            $("#WindowsFromAjax").html("");
            $("#WindowsFromAjax").html(result);
        }
    });
}


function FocusRentType() {
    //Focus rent type button
    var rentType = $("#hdfRentType").val();
    $('[data-rentType]').removeClass("active");
    var btnCurrentFee = $('[data-rentType =' + rentType + ']');
    btnCurrentFee.addClass("active");
}


function applyScrollbar() {
    $('#rentFeeDetail').popover();
    $("#wrapMenu").mCustomScrollbar({

        advanced:{
            updateOnContentResize: true
        }
    });
    $("#wrapOrderTable").mCustomScrollbar({

        advanced: {
            updateOnContentResize: true
        }
    });
    $("#wrapPaymentTable").mCustomScrollbar({

        advanced: {
            updateOnContentResize: true
        }
    });
    $("#wrapCustomerTable").mCustomScrollbar({

        advanced: {
            updateOnContentResize: true
        }
    });
}


////////////////////// RENT UPDATING - CHECK OUT /////////////////////////

function UpdateRentInfo() {
    var rentId = $("#rentId").val();
    var checkInDate = ConvertDateVNToUS($('#CheckInDate').val());
    var invoiceId = $("#InvoiceID").val();
    var rentType = $("#hdfRentType").val();
    var bikeId = $("#BikeID").val();
    var notes = $("#Notes").val();
    $.ajax({
        url: "Rent/UpdateRentInfo",
        type: "GET",
        data: { rentId: rentId, invoiceId: invoiceId, checkInDate: checkInDate, rentType: rentType, bikeId: bikeId, notes: notes },
        success: function (result) {
            if (result == "fail") {
                alert("Cập nhật không thành công!");
            }
            else {
                $("#rentInfo").fadeOut(200, function () {
                    $("#rentInfo").fadeIn();
                    ChangeStatusUpdateButton(true);
                });
            }
        }
    });

}

function ChangeStatusUpdateButton(disable) {
    if (disable) {
        $("#btnUpdateRentInfo").attr("disabled", "disabled");
        $("#btnUpdateRentInfo").removeClass("btn-info");
        
    }
    else {
        $("#btnUpdateRentInfo").removeAttr("disabled");
        $("#btnUpdateRentInfo").addClass("btn-info");
        
    }
}


function CheckOut() {

    var rentId = $("#rentId").val();
    var checkOutDate = $('#CheckOutDate').val(); //Do co che tu sinh ID cua MVC 
    var checkInDate = $('#CheckInDate').val();
    var isPayAll = $('#IsPayAll').is(':checked');
    var rentType = $("#hdfRentType").val();
    var roomid = $("#roomId").val();

    var isPrint = $("#IsPrint").is(':checked');
   
    $.ajax({
        url: "/Rent/CheckOut",
        type: "GET",
        data: { rentId: rentId, checkInDate: ConvertDateVNToUS(checkInDate), checkOutDate: ConvertDateVNToUS(checkOutDate), rentType: rentType, isPayAll: isPayAll },
        success: function (result) {

            CloseWindowsFromAjax();
            UpdateRoomStatus(roomid);

            if (isPrint) {
                printReport(); 
            }
        }
    });

}

function printReport() {

   
    var docprint = window.open("", "", 'left=100,top=100,width=700,height=400');
    docprint.document.open();
    docprint.document.write('<html><body>');
    docprint.document.write('HÓA ĐƠN TÍNH TIỀN');
    docprint.document.write('</br>');
    docprint.document.write('Khách sạn ' + hotelName + '<br/>ĐC: ' + address + '<br/>ĐT:' + phone);
    docprint.document.write('</br>*********************');
    docprint.document.write('<br/>Giờ vào: ' + $("#Rent_CheckInDate").val());
    docprint.document.write('<br/>Giờ ra: ' + $("#Rent_CheckOutDate").val());
    docprint.document.write('<br/>Tiền phòng:<strong>' + $("#rentFee").val() + '<strong/>');
    docprint.document.write('<br/>Chi tiết tiền phòng:<br/>' + $("#rentFeeDetail").attr('data-content'));
    docprint.document.write('<br/>Dịch vụ <br/>');
    //Print service
    $('#tblServiceDetail tr').each(function () {
        if (!$(this).hasClass('trHead')) {
            var productName = $(this).find('td').eq(1).html();
            var quantity = $(this).find('td').eq(2).html();
            var totalPrice = $(this).find('td').eq(3).html();
            docprint.document.write(productName + '  ' + quantity + '  ' + totalPrice + '</br>');
        }

    });

    docprint.document.write('Tiền dịch vụ:<strong>' + $("#orderFee").val() + '<strong/>');
    docprint.document.write('<br/>Tổng cộng:<strong>' + $("#totalFee").val() + '<strong/>');


    docprint.document.write('</body></html>');
    docprint.document.close();
    docprint.print();
    docprint.close();
}



///////////////////// RENT FEE CALCULATOR //////////////////////////////

function SetDateTimePicker() {
    
    //Chinh datetime picker
    var checkindate = $('#CheckInDate').val();
    $('#CheckInDate').val(ConvertDateVNToUS(checkindate));
    $('#CheckInDate').datetimepicker({
        onClose: function (dateText, inst) {
            var rentType = $("#hdfRentType").val();
            UpdateRentFee(rentType);
        },
        dateFormat: 'dd-mm-yy'
    });
    $('#CheckInDate').datetimepicker('setDate', (new Date($('#CheckInDate').val())));
    //Chinh Datetime picker

    var checkoutdate = $('#CheckOutDate').val();
    $('#CheckOutDate').val(ConvertDateVNToUS(checkoutdate));
    $('#CheckOutDate').datetimepicker({
        onClose: function (dateText, inst) {
            var rentType = $("#hdfRentType").val();
            UpdateRentFee(rentType);
        },
        dateFormat: 'dd-mm-yy'
    });

    $('#CheckOutDate').datetimepicker('setDate', (new Date($('#CheckOutDate').val())));
}



//Cap nhat lai gia tri cua Fee thue phong 
function UpdateRentFee(rentType) {

    var rentId = $("#rentId").val();
    var checkOutDate = ConvertDateVNToUS($('#CheckOutDate').val()); //Do co che tu sinh ID cua MVC 
    var checkInDate = ConvertDateVNToUS($('#CheckInDate').val());

    //Thay doi phi khi: gio vao thay doi, gio ra thay doi , hinh thuc thue thay doi
    //InvoiceID, checkInDate, Bike, Note duoc bat bang Onchange, chi con hdfRentType
    var oldRentType = $("#hdfRentType").val();
        

    $.ajax({
        url: "/Rent/CalculateRentFee",
        type: "GET",
        data: { rentID: rentId, rentType: rentType, checkInDate: checkInDate, checkOutDate: checkOutDate},
        success: function (result) {

            $('#rentFeeDetail').attr("data-content", result.description);
            $("#rentFee").val(addCommas(result.rentFee));
            UpdateEntireFee(result.rentFee, rentType);
            
            if (rentType != oldRentType) {
                ChangeStatusUpdateButton(false);
                $("#hdfRentType").val(rentType);

            }

        }
    });

}

function UpdateEntireFee() {
    var orderFee = parseInt($("#hdfOrderFee").val()); //Lay tu hidden service form
    var rentFee = parseInt($("#rentFee").val().replace(/,/g, ""));
    var additionFee = parseInt($("#hdfTotalAddition").val());
    var discountFee = parseInt($("#hdfTotalDiscount").val());
    var totalFee = rentFee + orderFee + additionFee - discountFee;
    var totalPayment = parseInt($("#hdfTotalPayment").val());
    var remainFee = totalFee - totalPayment;

    
    $("#orderFee").val(addCommas(orderFee));
    $("#rentFee").val(addCommas(rentFee));
    $("#addionFee").val(addCommas(additionFee));
    $("#discountFee").val(addCommas(discountFee));
    $("#totalFee").val(addCommas(totalFee));
    $("#totalPayment").val(addCommas(totalPayment));
    $("#remainFee").val(addCommas(remainFee));
}





/////////////// RENT CUSTOMERS /////////////////////////////////
function addRentCustomer() {
    var rentId = $("#rentId").val();
    var personId = $("#personId").val();
    var customerName = $("#customerName").val();
    var address = $("#address").val();
    if (personId != "" && customerName != "") {
        $.ajax({
            url: 'Rent/AddRentCustomers',
            type: 'GET',
            data: { rentId: rentId, personId: personId, customerName: customerName, address:address },
            success: function (result) {
                if (result != null) {
                    $("#personId").val("");
                    $("#customerName").val("");
                   $("#address").val("");
                    
                    $("#wrapCustomerTable").fadeOut(200, function () {
                        $("#customerList").prepend(result);
                        $("#wrapCustomerTable").fadeIn(200, function () {
                            $("#wrapCustomerTable").mCustomScrollbar("update");
                        });
                    });
                   
                };

            }
        });
    }

};


function removeCustomer(customerId) {

    $.ajax({
        url: "/Rent/RemoveRentCustomers",
        type: "GET",
        data: { customerId: customerId },
        success: function (result) {

            if (result == "1") {

                $("#wrapCustomerTable").fadeOut(200, function () {
                    $("#customerid-" + customerId).remove();
                    $("#wrapCustomerTable").fadeIn(200, function () {
                        $("#wrapCustomerTable").mCustomScrollbar("update");
                    });
                });
              
            }
        }
    });
}


////////////////////////// RENT PREPAID /////////////////////////////////////////
function addPayment() {
    var amount = $("#paymentAmount").val();
    var rentId = $("#rentId").val();
    var notes = $("#paymentNotes").val();

    $.ajax({
        url: "Rent/AddPayment",
        type: "GET",
        data: { rentId: rentId, paymentAmount: amount, notes:notes },
        success: function (result) {
            if (result != null) {
                $("#paymentAmount").val("");
                $("#paymentNotes").val("");
                $("#wrapPaymentTable").fadeOut(200, function () {
                    $("#paymentList").prepend(result);
                    $("#wrapPaymentTable").fadeIn(200, function() {
                        $("#wrapPaymentTable").mCustomScrollbar("update");
                        UpdateTotalPayment(amount);

                        //Update Entire Fee
                        UpdateEntireFee();
                    });
                });



            }

        }
    });
};


function RemovePayment(paymentId,amount) {
    $.ajax({
            url: "Rent/RemovePayment",
            type: "GET",
            data: { paymentId: paymentId },
            success: function(result) {
                if (result == "1") {
                    $("#wrapPaymentTable").fadeOut(200, function() {
                        $("#paymentid-" + paymentId).remove();
                        $("#wrapPaymentTable").fadeIn(200, function() {
                            $("#wrapPaymentTable").mCustomScrollbar("update");
                            UpdateTotalPayment(-amount);

                            //Update Entire Fee
                            UpdateEntireFee();
                        });
                    });
                };
            }


        });
    };


    function UpdateTotalPayment(amount) {
        //Update total payment
        var totalPayment = parseInt($("#hdfTotalPayment").val());
        totalPayment = totalPayment + parseInt(amount);
        $("#hdfTotalPayment").val(totalPayment);
        
     
    }

////////////////////////// RENT ADDITION /////////////////////////////////////////
function addAddition() {
    var amount = $("#additionAmount").val();
    var rentId = $("#rentId").val();
    var notes = $("#additionNotes").val();

    $.ajax({
        url: "Rent/AddFeeChange",
        type: "GET",
        data: { rentId: rentId, feeChangeAmount: amount, notes: notes, isAddition: true },
        success: function (result) {
            if (result != null) {
                $("#additionAmount").val("");
                $("#additionNotes").val("");
                $("#wrapAdditionTable").fadeOut(200, function () {
                    $("#additionList").prepend(result);
                    $("#wrapAdditionTable").fadeIn(200, function () {
                        $("#wrapAdditionTable").mCustomScrollbar("update");
                        UpdateTotalAddition(amount);
                        //Update Entire Fee
                        UpdateEntireFee();
                    });
                });



            }

        }
    });
};


function removeAddition(feeChangeId, amount) {
    $.ajax({
        url: "Rent/RemoveFeeChange",
        type: "GET",
        data: { feeChangeId: feeChangeId },
        success: function (result) {
            if (result == "1") {
                $("#wrapPaymentTable").fadeOut(200, function () {
                    $("#feeChangeId-" + feeChangeId).remove();
                    $("#wrapPaymentTable").fadeIn(200, function () {
                        $("#wrapPaymentTable").mCustomScrollbar("update");
                        UpdateTotalAddition(-amount);
                        //Update Entire Fee
                        UpdateEntireFee();
                    });
                });
            };
        }


    });
};


function UpdateTotalAddition(amount) {
    //Update total payment
    var totalAddition = parseInt($("#hdfTotalAddition").val());
    totalAddition += parseInt(amount); ;
    $("#hdfTotalAddition").val(totalAddition);


}

////////////////////////// RENT DISCOUNT /////////////////////////////////////////
function addDiscount() {
    var amount = $("#discountAmount").val();
    var rentId = $("#rentId").val();
    var notes = $("#discountNotes").val();

    $.ajax({
        url: "Rent/AddFeeChange",
        type: "GET",
        data: { rentId: rentId, feeChangeAmount: amount, notes: notes, isAddition: false },
        success: function (result) {
            if (result != null) {
                $("#discountAmount").val("");
                $("#discountNotes").val("");
                $("#wrapDiscountTable").fadeOut(200, function () {
                    $("#discountList").prepend(result);
                    $("#wrapDiscountTable").fadeIn(200, function () {
                        $("#wrapDiscountTable").mCustomScrollbar("update");
                        UpdateTotalDiscount(amount);
                        //Update Entire Fee
                        UpdateEntireFee();
                    });
                });


            }

        }
    });
};


function RemoveDiscount(feeChangeId, amount) {
    $.ajax({
        url: "Rent/RemoveFeeChange",
        type: "GET",
        data: { feeChangeId: feeChangeId },
        success: function (result) {
            if (result == "1") {
                $("#wrapPaymentTable").fadeOut(200, function () {
                    $("#feeChangeId-" + feeChangeId).remove();
                    $("#wrapPaymentTable").fadeIn(200, function () {
                        $("#wrapPaymentTable").mCustomScrollbar("update");
                        UpdateTotalDiscount(-amount);
                        //Update Entire Fee
                        UpdateEntireFee();
                    });
                });
            };
        }


    });
};


function UpdateTotalDiscount(amount) {
    //Update total payment
    var totalDiscount = parseInt($("#hdfTotalDiscount").val());
    totalDiscount += parseInt(amount); ;
    $("#hdfTotalDiscount").val(totalDiscount);


}


////////////////////////////////// RENT Service //////////////////////////////////////////////



function LoadItemByCategory(rentId, cateId) {
    $("#wrapMenu").html('<img src="../../Content/imgs/loading.gif"/>');
    $.ajax({
        url: "/Service/LoadItemByCategory",
        type: "GET",
        data: { cateId: cateId, txtRentId: rentId },
        success: function (result) {


            $("#wrapMenu").fadeOut(200, function () {
                $("#wrapMenu").html(result);
                $("#wrapMenu").fadeIn(200, function () {
                    $("#wrapMenu").mCustomScrollbar({

                        advanced: {
                            updateOnContentResize: true
                        }
                    });
                });

            });


        }
    });
}

function addItem(productId,price) {
    var rentId = $('#txtRentId').val();
    var quantity = $('#txtQuantity-' + productId).val();

    var sumPrice = quantity * price;
    


    $.ajax({
        url: 'Service/AddItem',
        type: 'GET',
        data: { rentId: rentId, productId: productId, quantity: quantity },
        success: function (result) {
            if (result != null) {
                var newResult = $(result).addClass("warning").clone().wrap('<p>').parent().html();

                $("#wrapOrderTable").fadeOut(200, function () {
                    
                    if ($("#ordered-item .warning") != null) {
                        $("#ordered-item .warning").removeClass("warning").addClass("success");
                    }
                    $("#ordered-item").prepend(newResult);
                    $("#wrapOrderTable").fadeIn(200, function () {
                        $("#wrapOrderTable").mCustomScrollbar("update");

                        updateOrderFee(sumPrice);

                        //Cap nhat lai toan bo chi phi
                        UpdateEntireFee();
                    });
                });
            }
        }
    });

}

function removeItem(orderId, amount) {
    $.ajax({
        url: "/Service/RemoveItem",
        type: "GET",
        data: { orderID: orderId },
        success: function (result) {
            if (result == "1") {
                $("#wrapOrderTable").fadeOut(200, function() {
                    $("#order-item-" + orderId).remove();
                    $("#wrapOrderTable").fadeIn(200, function() {
                        $("#wrapOrderTable").mCustomScrollbar("update");
                        updateOrderFee(-amount);

                        //Cap nhat lai toan bo chi phi
                        UpdateEntireFee();
                    });
                });
            };
        }});
};






function updateOrderFee(amount) {
    var orderFee = parseInt($("#hdfOrderFee").val());
    orderFee += parseInt(amount); ;
    $("#hdfOrderFee").val(orderFee);
}