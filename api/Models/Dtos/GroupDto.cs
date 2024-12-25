using api.Data.Models;

namespace api.Models.Dtos
{
    public class GroupDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OwnerId { get; set; }
    }
}
