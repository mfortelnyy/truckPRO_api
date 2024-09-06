using AutoMapper;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.MappingProfiles
{
    public class UserMappingProfilecs : Profile
    {
        public UserMappingProfilecs() 
        {
            CreateMap<SignUpDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => UserRole.Driver)) // Default role  - driver
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is auto-generated - ignore
                .ForMember(dest => dest.Company, opt => opt.Ignore()); // Company is not set by the driver
        }
    }
}
