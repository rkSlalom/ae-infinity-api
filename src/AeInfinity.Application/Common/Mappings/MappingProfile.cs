using AutoMapper;
using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.ListItems.Contracts;
using AeInfinity.Domain.Entities;

namespace AeInfinity.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<User, UserBasicDto>();

        // Role mappings
        CreateMap<Role, RoleDto>();

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // List mappings
        CreateMap<List, ListDto>()
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items.Count))
            .ForMember(dest => dest.PurchasedItems, opt => opt.MapFrom(src => src.Items.Count(i => i.IsPurchased)))
            .ForMember(dest => dest.CollaboratorsCount, opt => opt.MapFrom(src => src.Collaborators.Count));
        
        CreateMap<List, ListDetailDto>()
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items.Count))
            .ForMember(dest => dest.PurchasedItems, opt => opt.MapFrom(src => src.Items.Count(i => i.IsPurchased)))
            .ForMember(dest => dest.CollaboratorsCount, opt => opt.MapFrom(src => src.Collaborators.Count));
        
        CreateMap<List, ListBasicDto>();

        // UserToList (Collaborator) mappings
        CreateMap<UserToList, CollaboratorDto>()
            .ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
            .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.InvitedByDisplayName, opt => opt.MapFrom(src => src.InvitedByUser.DisplayName));

        // ListItem mappings
        CreateMap<ListItem, ListItemDto>()
            .ForMember(dest => dest.Creator, opt => opt.Ignore()); // Creator loaded separately in queries if needed
        CreateMap<ListItem, ListItemBasicDto>();
        
        // Autocomplete mappings
        CreateMap<AutocompleteSuggestion, AutocompleteSuggestionDto>();
    }
}

