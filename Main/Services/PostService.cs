using AutoMapper;
using DAL;
using DAL.Entites;
using Main.Models.Attach;
using Main.Models.Post;
using Main.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Main.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;

        private Func<AttachModel, string?>? _linkContentGenerator;
        private Func<UserModel, string?>? _linkAvatarGenerator;

        public PostService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void SetLinkGenerator(Func<AttachModel, string?> linkContentGenerator, Func<UserModel, string?> linkAvatarGenerator)
        {
            _linkContentGenerator = linkContentGenerator;
            _linkAvatarGenerator = linkAvatarGenerator;
        }

        public async Task CreatePost(CreatePostModel model)
        {
            var dbModel = _mapper.Map<Post>(model);
            await _context.Posts.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.PostContents).AsNoTracking().Take(take).Skip(skip).ToListAsync();

            var res = posts.Select(post =>
                new PostModel
                {
                    Author = new UserAvatarModel(
                        _mapper.Map<UserModel>(post.Author),
                        post.Author.Avatar == null ? null : _linkAvatarGenerator),
                    Description = post.Description,
                    Id = post.Id,
                    Contents = post.PostContents?.Select(x => new AttachWithLinkModel(
                            _mapper.Map<AttachModel>(x),
                            _linkContentGenerator)).ToList()
                }).ToList();
            return res;
        }

        public async Task<AttachModel> GetPostContent(Guid postContentId)
        {
            var res = await _context.PostContents.FirstOrDefaultAsync(x => x.Id == postContentId);
            return _mapper.Map<AttachModel>(res);
        }
    }
}
