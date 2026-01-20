using System.Linq.Expressions;

namespace Application.Common.Interfaces;

public interface IBaseMapper<TEntity, TDto>
{
    static abstract Expression<Func<TEntity, TDto>> EntityDto { get; }
    static abstract TDto ToOutputDto(TEntity entity);
}

public interface IBaseMapper<TEntity, TJoiner, TDto>
{
    // The expression for LINQ projections (e.g., in Entity Framework)
    static abstract Expression<Func<TEntity, TJoiner, TDto>> EntityDto { get; }

    // The method for in-memory mapping
    static abstract TDto ToOutputDto(TEntity entity, TJoiner joiner);
}
