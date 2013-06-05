using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelFee
{

    public abstract class PriceCalculator
    {
        public abstract int CalculatePrice(DateTime startTime, DateTime endTime, int priceGroupId);
        public string Description { get; set; }

        public string ConnectionString { get; set; } 
        protected HotelDataContext db;

        protected PriceCalculator(string connectionString)
        {
            ConnectionString = connectionString;
            db = new HotelDataContext(connectionString);
        }
    }


    public class PriceFactory
    {
        public static PriceCalculator GetPriceCalculator(RentTypeEnum mode, string connectionString)
        {
            switch (mode)
            {
                case RentTypeEnum.Hour:
                    return new HourPriceCalculator(connectionString);

                case RentTypeEnum.Night:
                    return new NightPriceCalculator(connectionString);
                case RentTypeEnum.Day:
                    return new DayPriceCalculator(connectionString);
            }
            return new HourPriceCalculator(connectionString);
        }

        //public static PriceSummaryModel GetPriceSummary(DateTime startTime, DateTime endTime, int priceGroupID)
        //{
        //    RentTypeEnum lowestFeeMode = RentTypeEnum.Hour;
        //    PriceSummaryModel model = new PriceSummaryModel();

        //    //Hour fee
        //    IPriceCalculator priceCalculator =
        //        PriceFactory.GetPriceCalculator(RentTypeEnum.Hour);
        //    model.HourFee = priceCalculator.CalculatePrice(startTime, endTime, priceGroupID);

        //    //Night fee
        //    priceCalculator =
        //        PriceFactory.GetPriceCalculator(RentTypeEnum.Night);
        //    model.NightFee = priceCalculator.CalculatePrice(startTime, endTime, priceGroupID);

        //    if ((model.NightFee > 0) && (model.NightFee < model.HourFee))
        //    {
        //        lowestFeeMode = RentTypeEnum.Night;
        //    }

        //    //Day Fee

        //    priceCalculator =
        //        PriceFactory.GetPriceCalculator(RentTypeEnum.Day);
        //    model.DayFee = priceCalculator.CalculatePrice(startTime, endTime, priceGroupID);

        //    if (((lowestFeeMode == RentTypeEnum.Hour) && (model.DayFee > 0) && (model.DayFee < model.HourFee)) ||
        //        ((lowestFeeMode == RentTypeEnum.Night) && (model.DayFee > 0) && (model.DayFee < model.NightFee)))
        //    {
        //        lowestFeeMode = RentTypeEnum.Day;
        //    }

        //    model.LowestFeeMode = lowestFeeMode;

        //    return model;

        //}

    }

    //public class PriceSummaryModel
    //{
    //    public int HourFee { get; set; }
    //    public int NightFee { get; set; }
    //    public int DayFee { get; set; }

    //    public string Description { get; set; }

    //    public RentTypeEnum LowestFeeMode { get; set; }

    //}

    }

