namespace DAL.Entites
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public Guid AuthorId { get; set; }
        public DateTimeOffset Created { get; set; }

        public virtual User Author { get; set; }
        public virtual ICollection<PostContent> PostContents { get; set; }
    }
}
