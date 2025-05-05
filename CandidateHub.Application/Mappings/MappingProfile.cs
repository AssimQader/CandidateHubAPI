using CandidateHub.Application.DTOs;
using CandidateHub.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace CandidateHub.Application.Mappings
{
    [Mapper]
    public static partial class MappingProfile
    {
        [MapperIgnoreTarget(nameof(Candidate.UpdatedAt))]
        [MapperIgnoreTarget(nameof(Candidate.CreatedAt))]
        [MapperIgnoreTarget(nameof(Candidate.Id))]
        public static partial Candidate ToEntity(this CandidateDto dto);

        [MapperIgnoreSource(nameof(Candidate.UpdatedAt))]
        [MapperIgnoreSource(nameof(Candidate.CreatedAt))]
        [MapperIgnoreSource(nameof(Candidate.Id))]
        public static partial CandidateDto ToDto(this Candidate entity);
    }
}
