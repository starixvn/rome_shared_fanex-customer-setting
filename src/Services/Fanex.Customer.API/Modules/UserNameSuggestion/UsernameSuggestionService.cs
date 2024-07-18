using Fanex.Customer.API.Modules.UserNameSuggestion.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.UserNameSuggestion
{
    public interface UsernameSuggestionService
    {
        Task<IEnumerable<UsernameSuggestion>> GetUsernamesSuggestion(
            int actorId,
            string suggestionQuery,
            bool isInternal,
            bool directDownline,
            bool isAccInfo);
    }

    public class UsernameSuggestionServiceImpl(IDynamicRepository dynamicRepository) : UsernameSuggestionService
    {
        private readonly IDynamicRepository dynamicRepository = dynamicRepository;

        public async Task<IEnumerable<UsernameSuggestion>> GetUsernamesSuggestion(
            int actorId,
            string suggestionQuery,
            bool isInternal,
            bool directDownline,
            bool isAccInfo)
            => (await dynamicRepository.FetchAsync<UsernameSuggestion>(
              new SelectAutoSuggestionCriteria(
                  actorId,
                  suggestionQuery,
                  isInternal,
                  directDownline,
                  isAccInfo))).OrderBy(item => item.UserName);

    }
}