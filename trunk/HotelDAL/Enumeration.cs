namespace HotelDAL
{
    public enum RoomStatusEnum
    { 
        Ready = 1,
        InUse = 2,
        Prepare = 3,
        NotUse = 4
    }

    public enum RentTypeEnum
    { 
        Hour = 1,
        Night = 2,
        Day = 3
    }

    public enum RentStatusEnum
    { 
        InStay = 1,
        Paid = 2,
        Approved = 3,
        Disabled = 4,
        WaitDisabled = 5,
        DeletePermanent = 6

    }

    public enum CostStatusEnum
    {   New = 1,
        Approved = 2,
        WaitDisabled = 3,
        Disabled = 4,
        Deleted = 5
    }
}