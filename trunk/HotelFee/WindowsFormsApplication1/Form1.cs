using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            HotelFee.HourPriceCalculator calc = new HotelFee.HourPriceCalculator();


            DateTime dtStart = dateTimePicker1.Value.Date;
            dtStart =  dtStart.AddHours(int.Parse(txtStartHour.Text));
            dtStart = dtStart.AddMinutes(int.Parse(txtStartMinute.Text));

            DateTime dtEnd = dateTimePicker2.Value.Date;
            dtEnd = dtEnd.AddHours(int.Parse(txtEndHour.Text));
            dtEnd =  dtEnd.AddMinutes(int.Parse(txtEndMinute.Text));


            int iResult = calc.CalculatePrice(dtStart, dtEnd, 1);

            lblResult.Text = iResult.ToString();


        }
    }
}
