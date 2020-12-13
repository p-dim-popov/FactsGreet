namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;

    public interface IEditsService
    {
        Task<ICollection<T>> GetPaginatedOrderByDescAsync<T>(
            int skip,
            int take,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null)
            where T : IMapFrom<Edit>;

        Task<ICollection<T>> GetEditsNewerThan<T>(
            int skip,
            int take,
            Guid articleId,
            DateTime creationDate)
            where T : IMapFrom<Edit>;

        Task<ICollection<T>> GetEditsOlderThan<T>(
            int skip,
            int take,
            Guid articleId,
            DateTime creationDate)
            where T : IMapFrom<Edit>;

        Task<EditDto> GetByIdAsync(Guid targetId, Guid? againstId = null);

        Task CreateAsync(
            Guid articleId,
            string editorId,
            string newTitle,
            string newContent,
            string newDescription,
            string[] newCategories,
            string newThumbnailLink,
            string editComment);

        Task<ICollection<T>> GetFewOlderThanAsync<T>(
            Guid? referenceId, int take, string userId, bool forCurrentUser)
            where T : IMapFrom<Edit>;

        Task<DateTime> GetCreationDateAsync(Guid id);

        Task<Guid> GetArticleIdAsync(Guid id);
    }
}
