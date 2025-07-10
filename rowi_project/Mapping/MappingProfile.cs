using AutoMapper;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;

namespace rowi_project.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAgentDto, Company>();
        CreateMap<UpdateAgentDto, Company>();

        CreateMap<CreateAgentDto, Agent>()
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src));

        CreateMap<Bank, BankDto>()
            .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.Company.ShortName));

        CreateMap<Agent, AgentDto>()
            .ForMember(dest => dest.RepFullName, opt =>
                opt.MapFrom(src => $"{src.Company.RepSurName} {src.Company.RepName} {src.Company.RepPatronymic}"))
            .ForMember(dest => dest.RepEmail, opt => opt.MapFrom(src => src.Company.RepEmail))
            .ForMember(dest => dest.RepPhoneNumber, opt => opt.MapFrom(src => src.Company.RepPhoneNumber))
            .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.Company.ShortName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Company.FullName))
            .ForMember(dest => dest.Inn, opt => opt.MapFrom(src => src.Company.Inn))
            .ForMember(dest => dest.Kpp, opt => opt.MapFrom(src => src.Company.Kpp))
            .ForMember(dest => dest.Ogrn, opt => opt.MapFrom(src => src.Company.Ogrn))
            .ForMember(dest => dest.OgrnDateOfAssignment, opt => opt.MapFrom(src => src.Company.OgrnDateOfAssignment))
            .ForMember(dest => dest.Important, opt => opt.MapFrom(src => src.Important))
            .ForMember(dest => dest.Banks, opt => opt.MapFrom(a => a.Banks));
    }
}
