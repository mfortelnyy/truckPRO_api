using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Graph.Models;
using truckPro_api.DTOs;
using truckPRO_api.Models;

namespace truckPro_api.MappingProfiles
{
    public class LogEntryMappingProfiles : Profile
    {
       public LogEntryMappingProfiles()
        {
            CreateMap<LogEntry, LogEntryParent>()
                .ForMember(dest => dest.ChildLogEntries, 
                           opt => opt.MapFrom(src => src.ParentLogEntryId != null ? new List<LogEntry>() : null))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.LogEntryType, opt => opt.MapFrom(src => src.LogEntryType))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.ParentLogEntryId, opt => opt.MapFrom(src => src.ParentLogEntryId))
                .ForMember(dest => dest.IsApprovedByManager, opt => opt.MapFrom(src => src.IsApprovedByManager));
        }

    }
}