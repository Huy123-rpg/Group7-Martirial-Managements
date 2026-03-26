using DAL.DataAccessLayer.Context;
using System;
using System.Linq;

namespace Temp
{
    class Program
    {
        static void Main()
        {
            using var db = new WarehouseDbContext();
            foreach (var status in db.LkpDocumentStatuses.ToList())
            {
                Console.WriteLine(status.StatusId + " : " + status.StatusName);
            }
        }
    }
}
