using project.users.dto;

namespace back.File.Dtos
{
    public class FileDto
    {
        public string Id { get; set; }
        public userDto userUpdate { get; set; }
        public DateTime createAt { get; set; }
        public string OriginalName { get; set; }

    }
}