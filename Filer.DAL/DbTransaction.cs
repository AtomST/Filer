using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Filer.DAL
{
    public class DbTransaction
    {
        private FilerDbContext _dbcontext;
        private ILogger<DbTransaction> _logger;
        public DbTransaction(FilerDbContext dbContext, ILogger<DbTransaction> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }
        public IDbContextTransaction BeginTransaction()
        {
            var transaction = _dbcontext.Database.BeginTransaction();
            return transaction;
        }
    }
}
