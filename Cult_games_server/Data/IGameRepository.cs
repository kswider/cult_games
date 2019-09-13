using Cult_games_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cult_games_server.Data
{
    public interface IGameRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<Player> GetPlayer(string name);
        Task<IEnumerable<Player>> GetPlayers();
        Task<bool> SaveAll();
    }
}
