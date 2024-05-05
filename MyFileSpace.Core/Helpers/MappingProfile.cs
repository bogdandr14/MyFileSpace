using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Helpers
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureMappings();
        }

        /// <summary>
        /// Creates a mapping between source (Domain) and destination (ViewModel)
        /// </summary>
        ///
        private void ConfigureMappings()
        {
            CreateMap<User, CurrentUserDTO>()
                .ForMember(x => x.RoleType, y => y.MapFrom(z => z.Role))
                .ReverseMap();

            CreateMap<StoredFile, FileDTO>();
            CreateMap<StoredFile, FileDownloadDTO>()
                .ForMember(x => x.DownloadName, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.LastModified, y => y.MapFrom(z => z.ModifiedAt));

            CreateMap<StoredFile, FileDetailsDTO>()
                .ForMember(x => x.DirectoryName, y => y.MapFrom(z => z.Directory.VirtualPath))
                .ForMember(x => x.AllowedUsers, y => y.MapFrom(z => z.AllowedUsers.Select(au => au.AllowedUser.TagName)));

            CreateMap<VirtualDirectory, DirectoryDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.VirtualPath));

            CreateMap<VirtualDirectory, DirectoryDetailsDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.VirtualPath));

            CreateMap<DirectoryUpdateDTO, VirtualDirectory>()
                .ForMember(x => x.VirtualPath, y => y.MapFrom(z => z.Name));

            CreateMap<DirectoryCreateDTO, VirtualDirectory>()
                .ForMember(x => x.VirtualPath, y => y.MapFrom(z => z.Name));

            CreateMap<User, UserDTO>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.Id));

            CreateMap<User, UserDetailsDTO>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.Id));

            CreateMap<User, CurrentUserDTO>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.Id));

            CreateMap<AccessKey, KeyAccessDetailsDTO>();

        }

        private string RecursivePathBuilder(VirtualDirectory directory)
        {
            string path = string.Empty;
            while (directory != null)
            {
                path = $"{directory.VirtualPath}/{path}";
                directory = directory.ParentDirectory!;
            }

            return path;
        }
    }
}
