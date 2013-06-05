using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelOfflineBLL.BusinessLogic
{
    public class RoomBusinessLogic
    {
        private readonly HotelDataContext db;
        private readonly string _connectionString;

        public RoomBusinessLogic(string cs)
        {
            _connectionString = cs;
            db = new HotelDataContext(_connectionString);
        }

        public  Room GetRoomById(int id)
        {
            return db.Rooms.Where(r => r.RoomID == id).FirstOrDefault();
        }
    }
}
