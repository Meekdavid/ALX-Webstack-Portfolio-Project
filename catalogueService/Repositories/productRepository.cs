using System.Collections.Generic;
using System.Threading.Tasks;
using catalogueService.Database;
using catalogueService.Interfaces;
using catalogueService.Database.DBContextFiles;
using AutoMapper;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace catalogueService.Repositories
{
    public class FeeRepository : IFee
    {
        private readonly catalogueDBContext _dbcontext;
        private readonly IMapper _mapper;
        public FeeRepository(catalogueDBContext dbcontext, IMapper mapper)
        {
            this._dbcontext = dbcontext;
            this._mapper = mapper;
        }

        public async Task<Fee> AddFeeAsync(Fee pro)
        {
            await _dbcontext.Fees.AddAsync(pro);
            await _dbcontext.SaveChangesAsync();
            return pro;
        }

        public async Task<Fee> DeleteAsync(int id)
        {
            var walk = await _dbcontext.Fees.FindAsync(id);
            if (walk == null)
            {
                return null;
            }

            //Delete the region
            _dbcontext.Fees.Remove(walk);
            await _dbcontext.SaveChangesAsync();
            return walk;
        }

        public async Task<Fee> GetByIdAsync(int id)
        {
            return await _dbcontext.Fees.FirstOrDefaultAsync(x => x.FeeId == id);
        }

        public async Task<(bool, IEnumerable<Fee>)> GetFeeAsync()
        {
            try
            {
                var results = await _dbcontext.Fees.ToListAsync();
                return (true, results);
            }
            catch(System.Exception e)
            {
                throw e;
            }
        }

        public async Task<Fee> UpdateAsync(int id, Fee walk)
        {
            var existingRegion = await _dbcontext.Fees.FindAsync(id);
            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.price = walk.price;
            existingRegion.quantity = walk.quantity;
            existingRegion.description = walk.description;
            existingRegion.name = walk.name;
            existingRegion.categoryId = walk.categoryId;


            await _dbcontext.SaveChangesAsync();
            return existingRegion;
        }
    }
}
