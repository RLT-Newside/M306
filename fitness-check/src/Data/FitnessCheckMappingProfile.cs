using AutoMapper;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Data.Entities;

namespace FitnessCheck.Data;

public class FitnessCheckMappingProfile : Profile
{
    public FitnessCheckMappingProfile()
    {
        CreateMap(typeof(DisciplineAttempt<>), typeof(DisciplineAttemptResponseDTO));

        CreateMap<DisciplineAttempt<uint>, DisciplineAttemptResponseDTO>()
            .ForMember(dest => dest.DeletionDeadlineUtc, opt => opt.MapFrom<DeletionDeadlineResolver>())
            .Include<CoreStrengthAttempt, CoreStrengthAttemptResponseDTO>()
            .Include<MedicineBallPushAttempt, MedicineBallPushAttemptResponseDTO>()
            .Include<OneLegStandAttempt, OneLegStandAttemptResponseDTO>()
            .Include<ShuttleRunAttempt, ShuttleRunAttemptResponseDTO>()
            .Include<StandingLongJumpAttempt, StandingLongJumpAttemptResponseDTO>();

        CreateMap<CoreStrengthAttempt, CoreStrengthAttemptResponseDTO>();
        CreateMap<MedicineBallPushAttempt, MedicineBallPushAttemptResponseDTO>();
        CreateMap<OneLegStandAttempt, OneLegStandAttemptResponseDTO>();
        CreateMap<ShuttleRunAttempt, ShuttleRunAttemptResponseDTO>();
        CreateMap<StandingLongJumpAttempt, StandingLongJumpAttemptResponseDTO>();
        CreateMap<DisciplineAttempt<float>, DisciplineAttemptResponseDTO>()
            .ForMember(dest => dest.DeletionDeadlineUtc, opt => opt.MapFrom<DeletionDeadlineResolver>());
            
        CreateMap<TwelveMinutesRunAttempt, TwelveMinutesRunAttemptResponseDTO>()
            .IncludeBase<DisciplineAttempt<float>, DisciplineAttemptResponseDTO>();

        CreateMap<BestAttempt, BestAttemptResponseDTO>();
        CreateMap<Cohort, CohortResponseDTO>();
        CreateMap<DataRecorderConfig, DataRecorderConfigResponseDTO>();
    }
}
