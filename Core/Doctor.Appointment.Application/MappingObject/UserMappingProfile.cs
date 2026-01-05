using AutoMapper;
using Doctor.Appointment.Domain.DTOs.User;
using Doctor.Appointment.Domain.Entities;

namespace Doctor.Appointment.Application.MappingObject
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserEntity, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Handled manually in service

            CreateMap<CreateUserDto, UserEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<UpdateUserDto, UserEntity>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
