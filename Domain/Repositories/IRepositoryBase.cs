using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IRepositoryBase<T>
    {
        void Add(T entidade);
        void Update(T entidade);
        void Delete(T entidade);
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> conditionExpression);
    }
}