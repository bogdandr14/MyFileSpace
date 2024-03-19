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
        }
    }
}
