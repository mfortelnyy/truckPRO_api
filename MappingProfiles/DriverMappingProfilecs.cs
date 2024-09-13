using AutoMapper;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.MappingProfiles
{
    public class DriverMappingProfilecs : Profile
    {
        public DriverMappingProfilecs() 
        {
            CreateMap<SignUpDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is auto-generated - ignore
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (UserRole) src.Role)) 
                .ForMember(dest => dest.Company, opt => opt.Ignore()) // Company is not set by the driver
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId));
        }
    }
}
