using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Repositories
{
    [Obsolete("This should not exist: repositories only for IAggregateRoots")]
    internal interface IValueObjectRepository<TValueObject, TEntityParent>
        where TValueObject : ValueObject<TValueObject>
        where TEntityParent : Entity
    {
        Task<TValueObject> GetByParentAsync(TEntityParent entityParent);
        Task<TValueObject> AddForParentAsync(TEntityParent entityParent, TValueObject valueObject);
        Task<TValueObject> RemoveForParentAsync(TEntityParent entityParent, TValueObject valueObject);
    }
}