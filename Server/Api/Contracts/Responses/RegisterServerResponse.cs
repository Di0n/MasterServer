using System.ComponentModel.DataAnnotations;

namespace Server.Api.Contracts.Responses
{
    public record RegisterServerResponse
    {
        [Required]
        public required string ServerID { get; set; }
        [Required]
        public required string AuthToken { get; set; }
    }
}
