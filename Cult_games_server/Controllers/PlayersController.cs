using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cult_games_server.Data;
using Cult_games_server.Dtos;
using Cult_games_server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cult_games_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IGameRepository _repo;
        public PlayersController(IGameRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _repo.GetPlayers();
            return Ok(players);
        }

        [HttpGet("{name}", Name = "GetUser")]
        public async Task<IActionResult> GePlayer(string name)
        {
            var user = await _repo.GetPlayer(name);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateUser(string name, PointsDto body)
        {
            var pointsFromRequest = body.Points;
            var player = await _repo.GetPlayer(name);
            if (player == null)
            {
                player = new Player { Username = name, Points = pointsFromRequest };
                _repo.Add(player);
            }
            else if (player.Points == pointsFromRequest)
                return NoContent();

            player.Points = body.Points;
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating player {name} failed on save");
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteUser(string name)
        {
            var player = await _repo.GetPlayer(name);
            if (player == null)
            {
                return NoContent();
            }

            _repo.Delete(player);
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Deleting player {name} failed on save");
        }
    }
}