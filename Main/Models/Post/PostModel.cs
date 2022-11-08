using Main.Models.Attach;
using Main.Models.User;

namespace Main.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachWithLinkModel> Contents { get; set; } = new List<AttachWithLinkModel>();
    }
}
