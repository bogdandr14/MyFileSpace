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
            CreateMap<User, UserDetailsDTO>()
                .ReverseMap();

            CreateMap<StoredFile, FileDetailsDTO>()
                .ForMember(x => x.Path, y => y.MapFrom(z => RecursivePathBuilder(z.Directory)));

            CreateMap<VirtualDirectory, DirectoryDTO>()
                .ForMember(x => x.FullPath, y => y.MapFrom(z => RecursivePathBuilder(z)));

            CreateMap<VirtualDirectory, DirectoryDetailsDTO>()
                .ForMember(x => x.FullPath, y => y.MapFrom(z => RecursivePathBuilder(z)))
                .ForMember(x => x.FullPath, y => y.MapFrom(z => z.Owner.TagName));

            CreateMap<DirectoryUpdateDTO, VirtualDirectory>()
                .ForMember(x => x.VirtualPath, y => y.MapFrom(z => z.Path));

            CreateMap<User, UserPublicInfoDTO>()
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
