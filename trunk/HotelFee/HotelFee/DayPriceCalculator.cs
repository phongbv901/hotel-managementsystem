using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelFee
{
    public class DayPriceCalculator : PriceCalculator
    {
        public DayPriceCalculator(string connectionString):base(connectionString)
        {
            
        }

        public override int CalculatePrice(DateTime startTime, DateTime endTime, int priceGroupId)
        {
            //Giai thuat
            //Chia cach tinh tien thanh 3 phan :
            //Phan 1 tu thoi diem thue den 12h trua gan nhat: TÍNH PHỤ THU SỚM GIỜ
            //Phan 2 la so ngay  từ 12h trua o phan 1 đến 12h trưa gần nhất của thời điểm trả
            //Phan 3 là tu 12h trua cuoi cung den thoi diem tra phong (NẾU CÓ): TÍNH PHỤ THU TRẢ PHÒNG TRỄ

            
            int price = 0;
            if (startTime < endTime)
            {

                var priceGroup = (from p in db.PriceGroups
                                  where p.PriceGroupID == priceGroupId
                                  select p).FirstOrDefault();

                //// Start Time -----> checkpoint1-------n days ------> checkpoint2 ------> endtime

                if ((priceGroup != null) && ((endTime - startTime).TotalMinutes > priceGroup.RoundMinute))
                {

                    //tim thoi diem bat dau ngay (Vd: 12h trua)
                    DateTime checkpoint1 = startTime.Date + priceGroup.StartDayTime.TimeOfDay;

                    if (startTime > checkpoint1)
                    {
                        //Thue sau 12h trua => 12h trua gan nhat la sau do 1 ngay
                        checkpoint1 = checkpoint1.AddDays(1);
                    }


                    //Thoi diem 12h cuoi cung 
                    DateTime checkpoint2 = endTime.Date + priceGroup.StartDayTime.TimeOfDay;
                    if (checkpoint2 > endTime)
                    {
                        //Trả phòng trước 12h trưa => 12h trua gan nhat la truoc do 1 ngay
                        checkpoint2 = checkpoint2.AddDays(-1);
                    }

                    if (checkpoint2 < checkpoint1)
                    {
                        ////Thoi diem vao-ra nam trong cung 1 ngay  => tinh 1 NGÀY
                        this.Description = string.Format("{0}-->{1} <br/> 1 ngày: {2}<br/>", startTime.ToString("dd-MM HH:mm"), endTime.ToString("dd-MM HH:mm"), priceGroup.DayPrice);
                        price = priceGroup.DayPrice; //Tra ve so tien thue 1 ngay
                        return price;

                    }
                    else
                    {
                        //Tính tiền sớm giờ
                        AdditionFeeCalculator additionFeeCalculator = new AdditionFeeCalculator(this.ConnectionString);
                        price = additionFeeCalculator.CalculateAdditionFee(priceGroup.DayAdditionPriceID, startTime,
                                                                           checkpoint1, priceGroup.RoundMinute,
                                                                           priceGroup.DayPrice, false); //Phần 1
                        this.Description += additionFeeCalculator.Description;
                        
                        //Phần nguyên ở giữa
                        int totalDays = (int)(checkpoint2 - checkpoint1).TotalDays;
                        if (totalDays > 0)
                        {
                            int fullDayFee = totalDays*priceGroup.DayPrice;
                            price += fullDayFee;

                            this.Description += string.Format("{0}->{1}: <br/> {2} ngày : {3} <br/>",
                                                              checkpoint1.ToString("dd-MM HH:mm"),
                                                              checkpoint2.ToString("dd-MM HH:mm"),
                                                              totalDays.ToString("N0"), fullDayFee.ToString("N0"));
                        }

                        //Phần còn lại: 
                        //Tính tiền theo giờ kg tính giờ đầu
                        //Tính tiền theo qua đêm hoặc theo ngày
                        price += additionFeeCalculator.CalculateAdditionFee(priceGroup.DayAdditionPriceID, checkpoint2,
                                                                            endTime, priceGroup.RoundMinute,
                                                                            priceGroup.DayPrice, true);
                        this.Description += additionFeeCalculator.Description;
                        return price;

                    }
                }



            }


            return 0;


        }





    }
}
