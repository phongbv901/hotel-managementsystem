using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelFee
{
    internal class AdditionFeeCalculator
    {
        private HotelDataContext db;
        public AdditionFeeCalculator(string connectionString)
        {
            db = new HotelDataContext(connectionString);

        }
        public string Description { get; set; }

        /// <summary>
        /// Tính tiền trả phòng trễ
        /// Nếu nằm trong khung thì tính tiền theo khung
        /// Nếu không tính tiền theo ngày
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="additionPriceId"></param>
        /// <param name="IsEarly"></param>
        /// <returns></returns>
        public int CalculateAdditionFee(int additionPriceID, DateTime fromTime, DateTime toTime, int roundMinutes,
                                        int dayPrice, bool isLatelyFee)
        {
            //Calculate TotalHour
             
            TimeSpan duration = toTime - fromTime;
            int totalHour = (int)Math.Floor(duration.TotalHours);
            if (duration.Minutes > roundMinutes)
                totalHour++;

            if (totalHour > 0)
            {
                var addition = (from add in db.PriceAdditions
                                where add.AdditionPriceID == additionPriceID
                                select add).FirstOrDefault();

                if (addition != null)
                {
                    //Sample: 1,2,3,5 || 0,20000,40000,80000
                    int[] hourRange;
                    int[] priceRange;
                    if (isLatelyFee)
                    {
                        hourRange = addition.LateHourRange.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                        priceRange = addition.LatePriceRange.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                    }
                    else
                    {
                        hourRange = addition.EarlyHourRange.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                        priceRange = addition.EarlyPriceRange.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                    }
                    for (int i = 0; i < hourRange.Length; i++)
                    {
                        if (totalHour <= hourRange[i])
                        {
                            this.Description = string.Format("{0} --> {1} <br/> Phụ thu: {2} <br/>",
                                                             fromTime.ToString("dd-MM HH:mm"),
                                                             toTime.ToString("dd-MM HH:mm"),
                                                             priceRange[i].ToString("N0"));
                            return priceRange[i];
                        }
                    }
                }


                this.Description = string.Format("{0} --> {1} <br/> 1 ngày: {2}<br/>", fromTime.ToString("dd-MM HH:mm"),
                                                     toTime.ToString("dd-MM HH:mm"), dayPrice.ToString("N0"));
                    return dayPrice;
              
                
            }
            else
            {
                return 0;
            }


        }


    }
}
