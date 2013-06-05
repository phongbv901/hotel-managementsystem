using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelFee
{
    public class HourPriceCalculator : PriceCalculator
    {
        public HourPriceCalculator(string connectionString):base(connectionString)
        {
            
        }
        public override int CalculatePrice(DateTime startTime, DateTime endTime, int priceGroupId)
        {
            int price = 0;
            if (startTime < endTime)
            {

                var priceGroup = (from p in db.PriceGroups
                                  where p.PriceGroupID == priceGroupId
                                  select p).FirstOrDefault();

                if (priceGroup != null)
                {
                    if ((endTime - startTime).TotalMinutes > priceGroup.RoundMinute)
                    {


                        TimeSpan duration = endTime - startTime;
                        int totalHour = (int)Math.Floor(duration.TotalHours);
                        if (duration.Minutes > priceGroup.RoundMinute)
                            totalHour++;
                        this.Description += string.Format("Thời gian:{0}h{1}' -->{2}h <br/> ", (int) duration.TotalHours,
                                                          duration.Minutes, totalHour);
                        if (totalHour > 0)
                        {
                            this.Description += string.Format("Giờ đầu: {0} <br/> ",
                                                              priceGroup.FirstHourPrice.ToString("N0"));
                            price = priceGroup.FirstHourPrice;

                            if (priceGroup.SecondHourPrice > 0)
                            {
                                if (totalHour > 1)
                                {
                                    this.Description += string.Format("Giờ 2: {0}<br/>",
                                                                      priceGroup.SecondHourPrice.ToString("N0"));
                                    price += priceGroup.SecondHourPrice;
                                    if (totalHour > 2)
                                    {
                                        if (priceGroup.ThirdHourPrice > 0)
                                        {
                                            this.Description += string.Format("Giờ 3:{0}<br/>",
                                                                              priceGroup.ThirdHourPrice.ToString("N0"));
                                            price += priceGroup.ThirdHourPrice;

                                            if (totalHour > 3)
                                            {
                                                int remainFee = (totalHour - 3)*priceGroup.NextHourPrice;
                                                price += remainFee;
                                                this.Description +=
                                                    string.Format("Còn lại {0}h x {1} = {2}", totalHour - 3,
                                                                  priceGroup.NextHourPrice.ToString("N0"), remainFee.ToString("N0")) +
                                                    "<br/>";

                                            }

                                        }
                                        else
                                        {
                                            int remainFee = (totalHour - 2)*priceGroup.NextHourPrice;
                                            price += remainFee;
                                            this.Description +=
                                                string.Format("Còn lại {0}h x {1} = {2} <br/>", totalHour - 2,
                                                              priceGroup.NextHourPrice.ToString("N0"), remainFee.ToString("N0"));
                                        }
                                    }

                                }


                               
                            }
                            else
                            {
                                if (totalHour > 1)
                                {
                                    int remainFee = (totalHour - 1)*priceGroup.NextHourPrice;
                                    price += remainFee;
                                    this.Description += string.Format("Còn lại {0}h x {1} = {2}<br/>", totalHour - 1,
                                                             priceGroup.NextHourPrice.ToString("N0"), remainFee.ToString("N0")) ;
                                }
                            }

                            
                        }
                    }


                }



            }
           
            return price;
        }
    }
}
