using catalogueService.Database;
using catalogueService.Database.DBsets;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace catalogueService.Interfaces
{
    public interface IFee
    {
        public Task<(bool IsSuccess, IEnumerable<Fee> Fees)> GetFeeAsync();
        //public Task<(bool IsSuccess, IEnumerable<Fee> Fees)> GetFeeByIdAsync(int id);
        public Task<Fee> GetByIdAsync(int id);
        public Task<Fee> AddFeeAsync(Fee pro);
        public Task<Fee> DeleteAsync(int id);
        public Task<Fee> UpdateAsync(int id, Fee walk);
    }
}
