using Coomon.Constants;
using Coomon.Extentions;
using Main.Models.Attach;
using Main.Models.User;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
            if (userService != null)
            {
                _userService.SetLinkGenerator(x =>
                    Url.Action(nameof(GetUserAvatar),
                    new
                    {
                        userId = x.Id,
                        download = false
                    }));
            }
        }
        #region post methods

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUeser(model);

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
            {
                throw new Exception("You are not authorized");
            }

            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId + ""));
            if (!tempFi.Exists)
            {
                throw new Exception("file not found");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());
            var destFi = new FileInfo(path);
            if (destFi.Directory != null && !destFi.Directory.Exists)
            {
                destFi.Directory.Create();
            }
            System.IO.File.Copy(tempFi.FullName, path, true);

            await _userService.AddAvatarToUser(userId, model, path);
        }

        #endregion

        #region get methods

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
        {
            var attach = await _userService.GetUserAvatar(userId);
            var fs = new FileStream(attach.FilePath, FileMode.Open);
            if (download)
                return File(fs, attach.MimeType, attach.Name);
            else
                return File(fs, attach.MimeType);
        }

        [HttpGet]
        public async Task<FileStreamResult> GetCurrentUserAvatar(bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if(userId == default)
            {
                throw new Exception("You are not aouthorized!");
            }
            return await GetUserAvatar(userId);
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new Exception("You are not authorized!");
            }
            return await _userService.GetUser(userId);
        }

        #endregion
    }
}
