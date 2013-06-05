using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelFee;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            NightPriceCalculator calc = new NightPriceCalculator();
            DateTime dt1 = new DateTime(2012, 7, 22, 21, 30, 00);
            DateTime dt2 = new DateTime(2012, 7, 23, 7, 30, 00);

            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 160000);



             dt1 = new DateTime(2012, 7, 22, 21, 30, 00);
             dt2 = new DateTime(2012, 7, 22, 23, 30, 00);
             Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 160000);

             dt1 = new DateTime(2012, 7, 22, 12, 30, 00);
             dt2 = new DateTime(2012, 7, 22, 23, 30, 00);
             Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 300000);


             dt1 = new DateTime(2012, 7, 22, 18, 00, 00);
             dt2 = new DateTime(2012, 7, 23, 05, 00, 00);
             Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 240000);


             dt1 = new DateTime(2012, 7, 22, 18, 00, 00);
             dt2 = new DateTime(2012, 7, 23, 7, 00, 00);
             Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 240000);

             dt1 = new DateTime(2012, 7, 22, 06, 00, 00);
             dt2 = new DateTime(2012, 7, 23, 16, 00, 00);
             Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 0);
        }

        [TestMethod]
        public void TestHourPrice()
        {
            HourPriceCalculator calc = new HourPriceCalculator();
            DateTime dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            DateTime dt2 = new DateTime(2012, 1, 1, 6, 15, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 0);

           
            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 6, 16, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1),60000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 16, 5, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 240000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 5, 5, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 0);

            dt1 = new DateTime(2012, 1, 1, 23, 0, 0);
            dt2 = new DateTime(2012, 1,2, 1, 5, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1),80000);

            dt1 = new DateTime(2012, 1, 1, 23, 0, 0);
            dt2 = new DateTime(2012, 1, 2, 1, 25, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 100000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 2, 0, 0, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 400000);
        }


        [TestMethod]
        public void TestDayPrice()
        {
            IPriceCalculator calc = new DayPriceCalculator();
            DateTime dt1 = new DateTime(2012, 1, 1, 11, 0, 0);
            DateTime dt2 = new DateTime(2012, 1, 1, 13, 15, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 320000);


            dt1 = new DateTime(2012, 1, 1, 11, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 11, 55, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 300000);

            dt1 = new DateTime(2012, 1, 1, 11, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 11, 15, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 0);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 1, 13, 5, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1),420000);

            dt1 = new DateTime(2012, 1, 1, 23, 0, 0);
            dt2 = new DateTime(2012, 1, 2, 1, 5, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 300000);

            dt1 = new DateTime(2012, 1, 1, 12, 0, 0);
            dt2 = new DateTime(2012, 1, 2, 1, 25, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 300000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 2, 0, 0, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 420000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 12, 8, 0, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 3420000);

            dt1 = new DateTime(2012, 1, 1, 6, 0, 0);
            dt2 = new DateTime(2012, 1, 12, 13, 0, 0);
            Assert.AreEqual(calc.CalculatePrice(dt1, dt2, 1), 3720000);
        }
    }
}
