using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerClient.Contracts.Requests
{
    internal class RegisterServerRequest
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Host { get; set; } = "";

        [Required]
        public uint Port { get; set; }
    }
}
