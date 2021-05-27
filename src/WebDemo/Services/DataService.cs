using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebDemo.Data;
using WebDemo.Data.Models;

namespace WebDemo.Services
{
    public class DataService : IDisposable
    {
        private readonly CompanyDbContext _dbContext;

        public DataService()
        {
            _dbContext = new CompanyDbContext();
        }

        public async Task<IEnumerable<AlertType>> GetAlertTypesByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbContext.AlertTypes.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<CapturedPC>> GetCapturedPCsByIdsAsync(IEnumerable<long> ids)
        {
            return await _dbContext.CapturedPCs.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Recording>> GetRecordingsByIdsAsync(IEnumerable<long> ids)
        {
            return await _dbContext.Recordings.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetCountriesByCodesAsync(IEnumerable<string> codes)
        {
            return await _dbContext.Countries.Where(x => codes.Contains(x.CountryCode)).ToListAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}