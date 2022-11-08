using AutoMapper;
using Coomon.Helpers;
using DAL.Entites;
using Main.Models.Attach;
using Main.Models.Post;
using Main.Models.User;

namespace Main.Defaults
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DAL.Entites.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));
            CreateMap<User, UserModel>();
            CreateMap<Avatar, AttachModel>();
            CreateMap<PostContent, AttachModel>();
            CreateMap<MetadataModel, PostContent>();
            CreateMap<MetaWithPath, PostContent>();
            CreateMap<CreatePostModel, Post>()
                .ForMember(d => d.PostContents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
        }
    }
}
