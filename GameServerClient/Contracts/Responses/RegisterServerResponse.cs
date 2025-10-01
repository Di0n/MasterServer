using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerClient.Contracts.Responses
{
    internal class RegisterServerResponse
    {
        [Required]
        public required string ServerID { get; set; }
        [Required]
        public required string AuthToken { get; set; }
    }
}
