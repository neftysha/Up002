using System;
using System.Linq;

namespace up_02.Services
{
    public class AdService
    {
        private readonly Melnikov_DB_02Entities _db;

        public AdService()
        {
            _db = new Melnikov_DB_02Entities();
        }

     
        public decimal SetAdCompletedAndCalculateProfit(int adId)
        {
            var ad = _db.Ad.FirstOrDefault(a => a.AdId == adId);

            if (ad == null)
            {
                throw new InvalidOperationException("Объявление не найдено.");
            }

            
            var completedStatus = _db.AdStatus
                .FirstOrDefault(s => s.Name == "Завершено");

            if (completedStatus == null)
            {
                throw new InvalidOperationException("Статус 'Завершено' не найден в справочнике.");
            }

           
            ad.AdStatusId = completedStatus.AdStatusId;
            _db.SaveChanges();

            int userId = ad.UserId;

            
            decimal profit = _db.Ad
                .Where(a => a.UserId == userId &&
                            a.AdStatusId == completedStatus.AdStatusId)
                .Select(a => (decimal?)a.Price)
                .Sum() ?? 0m;

            return profit;
        }
    }
}
