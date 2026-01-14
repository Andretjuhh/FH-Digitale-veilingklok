using Domain.Entities;

namespace Application.Common.Models;

public record KoperInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Telephone,
    Address Address);