using Api.Application.Models;

namespace Api.Application.Commands
{
    public sealed record GetUsersCommand(Pagination Pagination);
}
