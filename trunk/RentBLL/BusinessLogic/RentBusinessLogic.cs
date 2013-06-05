using System;
using System.Collections.Generic;
using System.Linq;
using HotelDAL;
using HotelFee;

namespace HotelOfflineBLL.BusinessLogic
{
    public class RentBusinessLogic
    {
       public class FeeDescription
       {
           public int RentFee { get; set; }
           public string RentDescription { get; set; }
       }
        
        private HotelDataContext db;
        private readonly string _connectionString;

        public RentBusinessLogic(string cs)
        {
            _connectionString = cs;
            db = new HotelDataContext(_connectionString);
        }


        public Rent GetRentById(int rentId)
        {
            Rent rent = db.Rents.Where(r => r.RentID == rentId).Single();

            if ((rent.RentStatus == (int) RentStatusEnum.InStay) && (rent.CheckOutDate == null))
            {
                rent.CheckOutDate = DateTime.Now;


                PriceCalculator calculator = PriceFactory.GetPriceCalculator((RentTypeEnum) rent.RentType,
                                                                             _connectionString);
                rent.RentFee = calculator.CalculatePrice(rent.CheckInDate, rent.CheckOutDate.Value,
                                                         rent.Room.PriceGroupID);
                rent.FeeDescription = calculator.Description;

                rent.OrderFee = rent.OrderDetails.Sum(o => o.TotalPrice);
            }

            return rent;
        }


        public int BookRoom(Rent rent)
        {
            try
            {
                var room = db.Rooms.Where(r => r.RoomID == rent.RoomID).Single();
                if (room.RoomStatus == (int)RoomStatusEnum.Ready)
                {
                    room.RoomStatus = (int)RoomStatusEnum.InUse;
                    if (rent != null)
                    {
                        db.Rents.InsertOnSubmit(rent);
                        db.SubmitChanges();
                        return rent.RentID;
                    }
                }
                return -1;
            }
            catch (Exception)
            {

                return -1;
            }
           
        }

        public FeeDescription CalculateRentFee(int rentId, int rentType, DateTime checkInDate, DateTime checkOutDate)
        {
            var rent = db.Rents.Single(r => r.RentID == rentId);
            int priceGroupId = rent.Room.PriceGroupID;
            // calculate
            PriceCalculator priceCalculator = PriceFactory.GetPriceCalculator((RentTypeEnum)rentType, _connectionString);

            FeeDescription fee = new FeeDescription();
            fee.RentFee = priceCalculator.CalculatePrice(checkInDate, checkOutDate, priceGroupId);
            fee.RentDescription = priceCalculator.Description;

            return fee;
        }

        public bool UpdateRentInfo(int rentId, string invoiceId, DateTime checkInDate, int rentType, string bikeId, string notes)
        {
            try
            {
                var rent = db.Rents.Where(r => r.RentID == rentId).FirstOrDefault();

                if (rent != null)
                {
                    rent.InvoiceID = invoiceId;
                    rent.CheckInDate = checkInDate;
                    rent.RentType = rentType;
                    rent.BikeID = bikeId;
                    rent.Notes = notes;
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                
                return false;
            }
           
        }

        public bool CheckOut(int rentId, DateTime checkInDate, DateTime checkOutDate, int rentType, bool isPayAll, string username)
        {
            try
            {
                var rent = db.Rents.Single(r => r.RentID == rentId);
                if (rent.RentStatus == (int)RentStatusEnum.InStay)
                {
                    rent.CheckOutDate = checkOutDate;

                    RentTypeEnum mode =
                        (RentTypeEnum)Enum.Parse(typeof(RentTypeEnum), Enum.GetName(typeof(RentTypeEnum), rentType));
                    rent.RentType = (int)mode;

                    PriceCalculator calc = PriceFactory.GetPriceCalculator(mode, _connectionString);
                    rent.RentFee = calc.CalculatePrice(checkInDate, checkOutDate, rent.Room.PriceGroupID);
                    rent.FeeDescription = calc.Description;

                    int orderFee = rent.OrderDetails.Sum(od => od.TotalPrice);
                    rent.OrderFee = orderFee;





                    if (isPayAll)
                    {
                        int paymentAmount = rent.OrderFee + rent.RentFee - rent.TotalPayment;
                        if (paymentAmount > 0)
                        {
                            var payment = new Payment();
                            payment.Amount = paymentAmount;
                            payment.PayTime = DateTime.Now;
                            payment.Username = username;
                            payment.RentID = rent.RentID;

                            rent.Payments.Add(payment);
                            rent.TotalPayment += paymentAmount;

                        }
                    }
                    rent.FeeDescription = rent.FeeDescription;
                    rent.RentStatus = (int)RentStatusEnum.Paid;
                    //checkout person.
                    rent.CheckOutPerson = username;
                    rent.Room.RoomStatus = (int)RoomStatusEnum.Ready;
                    db.SubmitChanges();
                }
                //return RedirectToAction("Summary", new { txtRentID = aRent.RentID });
                return true;
            }
            catch 
            {

                return false;
            }


        }
#region Rent Customer
        public IEnumerable<Customer> GetRentCustomers(int rentId)
        {
            return db.Customers.Where(cus => cus.RentID == rentId);
        }


        public  Customer AddRentCustomers(int rentId,string personId,string customerName, string address)
        {
            Customer customer = new Customer();
            customer.RentID = rentId;
            customer.PersonID = personId;
            customer.CustomerName = customerName;
            customer.Address = address;
            db.Customers.InsertOnSubmit(customer);
            db.SubmitChanges();

            return customer;
        }

        public bool RemoveRentCustomers(int customerId)
        {
            try
            {
                Customer customer = db.Customers.Where(c => c.CustomerID == customerId).FirstOrDefault();
                if (customer != null)
                {
                    db.Customers.DeleteOnSubmit(customer);
                    return true;
                }
                db.SubmitChanges();
                return false;
            }
            catch 
            {
                return false;
            }
           
            
        }
#endregion

        #region Rent Payment
        public IEnumerable<Payment> GetRentPayments(int rentId)
        {
            return db.Payments.Where(p => p.RentID == rentId);
        }

        public Payment AddRentPayment(Payment payment)
        {
            try
            {
                var rent = db.Rents.Where(r => r.RentID == payment.RentID).FirstOrDefault();
                if (rent != null)
                {
                    rent.Payments.Add(payment);
                    rent.TotalPayment += payment.Amount;

                    db.SubmitChanges();
                    return payment;
                }
                return null;
            }
            catch
            {
                return null;
            }
            
           

           
        }

        public bool RemoveRentPayment(int paymentId)
        {
            try
            {
                var payment = db.Payments.Where(p => p.PaymentID == paymentId).FirstOrDefault();
                if (payment != null)
                {
                    payment.Rent.TotalPayment -= payment.Amount;
                    db.Payments.DeleteOnSubmit(payment);

                    db.SubmitChanges();
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                return false;
            }
           
        }


#endregion

        #region Rent FeeChange
        public IEnumerable<RentFeeChange> GetRentFeeChange(int rentId, bool isAddition)
        {
            return db.RentFeeChanges.Where(p => p.RentID == rentId && p.IsAddition == isAddition);
        }

        public RentFeeChange AddRentFeeChange(RentFeeChange feeChange)
        {
            try
            {
                var rent = db.Rents.Where(r => r.RentID == feeChange.RentID).FirstOrDefault();
                if (rent != null)
                {
                    rent.RentFeeChanges.Add(feeChange);
                    if (feeChange.IsAddition)
                    {
                        rent.AdditionFee += feeChange.Amount;
                    }
                    else
                    {
                        rent.DiscountFee += feeChange.Amount;
                    }
                    db.SubmitChanges();
                    return feeChange;
                }
                return null;
            }
            catch
            {
                return null;
            }




        }

        public bool RemoveRentFeeChange(int feeChangeId)
        {
            try
            {
                var feeChange = db.RentFeeChanges.Where(p => p.RentFeeChangeID == feeChangeId).FirstOrDefault();
                if (feeChange != null)
                {
                    if (feeChange.IsAddition)
                    {
                        feeChange.Rent.AdditionFee -= feeChange.Amount;
                    }
                    else
                    {
                        feeChange.Rent.DiscountFee -= feeChange.Amount;
                    }
                   
                    db.RentFeeChanges.DeleteOnSubmit(feeChange);

                    db.SubmitChanges();
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                return false;
            }

        }


        #endregion


#region Rent Service
        public IEnumerable<ProductCategory> GetRentServiceCategory()
        {
            return db.ProductCategories.OrderBy(pc => pc.CateID);
        }

#endregion

    }
}
