using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cult_games_server.Dtos
{
    public class PointsDto
    {
        [Required]
        public int Points { get; set; }
    }
}
