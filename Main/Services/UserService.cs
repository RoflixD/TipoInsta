using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Main.Defaults;
using DAL;
using DAL.Entites;
using Main.Models.Attach;
using Main.Models.User;

namespace Main.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;
        private Func<UserModel, string?>? _linkGenerator;

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> config)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
        }

        #region public methods

        public void SetLinkGenerator(Func<UserModel, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public async Task CreateUeser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DAL.Entites.User>(model);
            await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var dbUser = await GetUserById(id);
            if (dbUser != null)
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserExist(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
            return users.Select(x => new UserAvatarModel(x, _linkGenerator));
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);
            return new UserAvatarModel(_mapper.Map<UserModel>(user), user.Avatar == null ? null : _linkGenerator);            
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var avatar = new Avatar { 
                    Author = user,
                    MimeType = meta.MimeType,
                    FilePath = filePath,
                    Name = meta.Name,
                    Size = meta.Size
                };
                user.Avatar = avatar;

                await _context.SaveChangesAsync();
            }

        }

        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var atach = _mapper.Map<AttachModel>(user.Avatar);
            return atach;
        }

        #endregion
        #region private methods

        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            return user;
        }

        #endregion
    }
}
