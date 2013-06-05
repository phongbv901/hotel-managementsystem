using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelFee
{
    public class NightPriceCalculator : PriceCalculator
    {
        public NightPriceCalculator(string connectionString):base(connectionString)
        {
            
        }

        private int CalculateNightPriceInRange(DateTime startTime, DateTime endTime, DateTime checkTime,
                                               PriceGroup priceGroup)
        {
            int priceGroupID = priceGroup.PriceGroupID;
            TimeSpan duration = endTime - startTime;
            PriceNight nightPrice;
            if (checkTime.Date == startTime.Date)
            {
                //Thuê sau 12h đêm => Lấy khung gio có giá tiền nhỏ nhất thỏa điều kiện
                nightPrice = (from priceNight in db.PriceNights
                              where
                                  priceNight.PriceGroupID == priceGroupID &&
                                  endTime.TimeOfDay <= priceNight.EndTime.TimeOfDay &&
                                  duration <= priceNight.MaxDuration.TimeOfDay
                              orderby priceNight.Price ascending 
                              select priceNight).FirstOrDefault();
            }
            else
            {
                
                //Thuê trước 12h đêm
                if (startTime.Date == endTime.Date)
                {
                    //Trả phòng cũng trước 12h đêm
                    nightPrice = (from priceNight in db.PriceNights
                                  where
                                      priceNight.PriceGroupID == priceGroupID &&
                                      startTime.TimeOfDay >= priceNight.StartTime.TimeOfDay &&
                                      duration <= priceNight.MaxDuration.TimeOfDay
                                  orderby priceNight.Price ascending
                                  select priceNight).FirstOrDefault();
                }
                else
                {
                    //Trả phòng sau 12h đêm
                    nightPrice = (from priceNight in db.PriceNights
                                  where
                                      priceNight.PriceGroupID == priceGroupID &&
                                      startTime.TimeOfDay >= priceNight.StartTime.TimeOfDay &&
                                      endTime.TimeOfDay <= priceNight.EndTime.TimeOfDay&&
                                      duration <= priceNight.MaxDuration.TimeOfDay

                                  orderby priceNight.Price ascending
                                  select priceNight).FirstOrDefault();
                }
            }
            if (nightPrice == null)
            {
                this.Description += string.Format("{0} --> {1} </br> ngoài giờ qua đêm -> 1 ngày : {2}<br/>",
                                                  startTime.ToString("dd-MM HH:mm"), endTime.ToString("dd-MM HH:mm"),priceGroup.DayPrice);
                return priceGroup.DayPrice;
            }
            else
            {
                this.Description += string.Format("{0}-->{1} </br>Khung {2}-->{3}: {4}<br/>",
                                                  startTime.ToString("dd-MM HH:mm"), endTime.ToString("dd-MM HH:mm"),
                                                  nightPrice.StartTime.ToString("HH:mm"),
                                                  nightPrice.EndTime.ToString("HH:mm"),
                                                  nightPrice.Price.ToString("N0"));
                return nightPrice.Price;
            }
        }


        /// <summary>
        ///TÍNH TIỀN THUÊ QUA ĐÊM
        /// KHÔNG CÓ PHỤ THU VÀO SỚM
        /// NẾU KHÁCH RA TRỄ THÌ SẼ TÍNH PHỤ THU RA TRỄ . TỐI ĐA 1 NGÀY
        /// NẾU Ở TRỄ HƠN 1 NGÀY THÌ TÍNH TIỀN ĐẾN HẾT KHUNG GIỜ QUA ĐÊM
        /// PHẦN CÒN LẠI TÍNH NGÀY
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="priceGroupID"></param>
        /// <returns></returns>
        public override int CalculatePrice(DateTime startTime, DateTime endTime, int priceGroupID)
        {

            if (startTime < endTime)
            {


                

                var priceGroup = (from p in db.PriceGroups
                                  where p.PriceGroupID == priceGroupID
                                  select p).FirstOrDefault();

                if ((priceGroup != null) && ((endTime - startTime).TotalMinutes > priceGroup.RoundMinute))
                {

                    //Tìm thời điểm kết thúc thuê qua đêm
                    DateTime checkpoint1 = startTime.Date + priceGroup.EndNightTime.TimeOfDay;
                    if (startTime > checkpoint1)
                    {
                        //Thue sau 12h trua => ket thuc vao 12h trua gan nhat la sau do 1 ngay
                        checkpoint1 = checkpoint1.AddDays(1);
                    }


                    PriceNight nightPrice;
                    if (endTime < checkpoint1)
                    {
                        //Thời điểm vào và ra nằm trong khung gio qua đêm => KHONG CAN TINH PHU THU
                        return CalculateNightPriceInRange(startTime, endTime, checkpoint1, priceGroup);

                    }
                    else
                    {
                        //Chắc chắn quá giờ, Kiểm tra xem có trong ngày hay không
                        DateTime checkpoint2 = checkpoint1.AddDays(1);
                        int price = CalculateNightPriceInRange(startTime, checkpoint1, checkpoint1,priceGroup);

                        if (endTime < checkpoint2)
                        {
                            //Thời điểm trả phòng là quá hạn nằm trong ngày hôm sau => Tính tiền quá hạn
                            AdditionFeeCalculator additionFeeCalculator = new AdditionFeeCalculator(this.ConnectionString);
                            
                            price += additionFeeCalculator.CalculateAdditionFee(priceGroup.NightAdditionPriceID,
                                                                                checkpoint1, endTime,
                                                                                priceGroup.RoundMinute,
                                                                                priceGroup.DayPrice, true);
                            this.Description += additionFeeCalculator.Description;
                        }
                        else
                        {
                            //Thời điểm trả phòng là quá hạn nhiều ngày
                            //Tính toán thuê ngày từ thời điểm hết thuê qua đêm đến phần còn lại 
                            DayPriceCalculator dayPriceCalculator = new DayPriceCalculator(this.ConnectionString);
                            price += dayPriceCalculator.CalculatePrice(checkpoint1, endTime, priceGroupID);

                            this.Description += dayPriceCalculator.Description;
                        }

                        return price;

                    }


                }


            }
            return 0;
        }
    }

}
