
namespace DAL.Entites
{
    public class PostContent : Attach
    {
        public virtual Post Post { get; set; } = null!;
    }
}
