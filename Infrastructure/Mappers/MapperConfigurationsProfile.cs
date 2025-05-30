﻿using Application.ViewModels.CategoryDTO;
using Application.ViewModels.CollaboratorDTO;
using Application.ViewModels.CommentDTO;
using Application.ViewModels.FaqDTO;
using Application.ViewModels.FileDTO;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.PledgeDTO;
using Application.ViewModels.PostDTO;
using Application.ViewModels.ProjectDTO;
using Application.ViewModels.ReportDTO;
using Application.ViewModels.RewardDTO;
using Application.ViewModels.UserDTO;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap<User, RegisterDTO>().ReverseMap();
            CreateMap<User, LoginUserDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<Project, CreateProjectDto>().ReverseMap();
            CreateMap<Project, UpdateProjectDto>().ReverseMap();
            CreateMap<Project, ProjectThumbnailDto>().ReverseMap();
            //CreateMap<Platform, PlatformDTO>()
            //    .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.PlatformId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap();
            CreateMap<Platform, PlatformDTO>().ReverseMap();

            CreateMap<Project, UserProjectsDto>()
                            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                            .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail))
                            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                            .ForMember(dest => dest.EndDatetime, opt => opt.MapFrom(src => src.EndDatetime))
                            .ForMember(dest => dest.StartDatetime, opt => opt.MapFrom(src => src.StartDatetime))
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)); CreateMap<Report, CreateReportDto>().ReverseMap();
            CreateMap<Report, ReportDto>().ReverseMap();
            CreateMap<Pledge, PledgeDto>().ReverseMap();
            CreateMap<PledgeDetail, PledgeDetailDto>().ReverseMap();
            CreateMap<Category, AddCategory>().ReverseMap();
            CreateMap<Reward, AddReward>().ReverseMap();
            CreateMap<Reward, ViewReward>().ReverseMap();
            CreateMap<FAQ, FaqDto>().ReverseMap();
            CreateMap<FAQ, ViewFaqDto>().ReverseMap();
            CreateMap<Domain.Entities.File, FileDTO>().ReverseMap();
            CreateMap<User, PostUserDTO>()
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar ?? string.Empty))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
            CreateMap<Post, CreatePostDTO>().ReverseMap();
            CreateMap<Post, PostDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Monitor, opt => opt.MapFrom(src => src.Monitor.Fullname))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.User.Fullname));
            CreateMap<Category, UpdateCategory>().ReverseMap();
            CreateMap<Category, ViewCategory>().ReverseMap();
            CreateMap<ProjectCategory, AddCategoryToProject>().ReverseMap();
            CreateMap<Collaborator, CollaboratorDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project));

            CreateMap<Collaborator, ProjectCollaboratorDTO>()
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project));

            CreateMap<Collaborator, UserCollaboratorDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<PledgeDetail, ProjectBackerDetailDto>().ReverseMap();
        }
    }
}
