using System.ComponentModel.DataAnnotations;

namespace Server.Api.Contracts.Requests
{
    public record RegisterServerRequest
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Host { get; set; } = "";

        [Required]
        public uint Port { get; set; }
    }
}
