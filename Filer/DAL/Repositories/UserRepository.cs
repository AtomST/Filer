﻿using Filer.DAL.DbModels;
using Filer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Filer.DAL.Repository
{
    public class UserRepository
    {
        private FilerDbContext _dbcontext;
        private ILogger<UserRepository> _logger;
        public UserRepository(FilerDbContext dbContext, ILogger<UserRepository> logger) 
        {
            _dbcontext = dbContext;
            _logger = logger;
        }
        
        public IEnumerable<User> Users => _dbcontext.Users.AsNoTracking().ToList();
        public IEnumerable<User> UsersWithData => _dbcontext.Users.Include(f => f.Files).Include(f => f.Folders).AsNoTracking().ToList();


        public async Task<bool> AddUser(User user)
        {
            try
            {
                await _dbcontext.Users.AddAsync(user);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("При добавлении пользователя произошла ошибка: ", ex);
                return false;
            }

        }

        
        public User? GetUserById(string id)
        {
            User? u = _dbcontext.Users.FirstOrDefault(x => x.Id.ToString() == id);
            return u;
        }

        public bool UserContains(User user)
        {
            User? u = _dbcontext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (u == null) 
                return false;

            return true;
        }
    }
}
