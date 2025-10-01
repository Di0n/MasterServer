using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerClient.Contracts.Requests
{
    public class GameInfoUpdateRequest
    {
        public string ServerID { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string Map { get; set; } = "";
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
    }
}
