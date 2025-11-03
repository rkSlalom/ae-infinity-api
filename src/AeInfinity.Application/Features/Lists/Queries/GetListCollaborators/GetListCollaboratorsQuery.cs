using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Queries.GetListCollaborators;

/// <summary>
/// Query to get all collaborators for a list (including accepted and pending)
/// </summary>
public class GetListCollaboratorsQuery : IRequest<List<CollaboratorDto>>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; } // Current user (must have access)
}

