using AutoMapper;
using ChatAPI.Model.Domain;
using ChatAPI.Model.DTO;

namespace ChatAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Message, MessageItemDto>()
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Text))
            .ForMember(d => d.Time, o => o.MapFrom(s => s.SentAt))
            .ForMember(d => d.IsSender, o => o.MapFrom(s => s.Sender));
        }
    }
}
