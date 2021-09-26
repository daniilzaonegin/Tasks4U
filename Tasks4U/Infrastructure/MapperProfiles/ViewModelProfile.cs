using AutoMapper;
using Tasks4U.Model;
using Tasks4U.Models.ViewModels;

namespace Tasks4U.Infrastructure.MapperProfiles
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<TodoItem, TodoItemViewModel>()
                .ForMember(d => d.FromUserName, config => config.MapFrom(s => s.FromUserId))
                .ForMember(d => d.ToUserName, config => config.MapFrom(s => s.ToUserId))
                .ForMember(d => d.CompleteMessage, opt => opt.NullSubstitute(""));

            CreateMap<TodoItemViewModel, TodoItem>()
                .ForMember(d => d.FromUserId, config => config.MapFrom(s => s.FromUserName))
                .ForMember(d => d.ToUserId, config => config.MapFrom(s => s.ToUserName))
                .ForMember(d => d.CompleteMessage, opt => opt.NullSubstitute(""))
                .ForMember(d => d.Description, opt => opt.NullSubstitute(""))
                .ForMember(d => d.Id, opt => opt.Ignore());
        }
    }
}
