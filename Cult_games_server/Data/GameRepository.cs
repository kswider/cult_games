using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cult_games_server.Models;
using Microsoft.EntityFrameworkCore;

namespace Cult_games_server.Data
{
    public class GameRepository : IGameRepository
    {
        private readonly DataContext _context;
        public GameRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Player> GetPlayer(string name)
        {
            return await _context.Players.FirstOrDefaultAsync(player => player.Username == name);
        }

        public async Task<IEnumerable<Player>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
