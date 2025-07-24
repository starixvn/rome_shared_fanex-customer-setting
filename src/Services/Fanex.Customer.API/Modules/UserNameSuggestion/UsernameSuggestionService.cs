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
            bool isAccInfo,
            bool excludeMember,
            bool excludeCashMember);

        Task<bool> UpdateUserSearch(
            int userId,
            string search,
            string firstName,
            string lastName,
            string nickname
        );

        Task<IEnumerable<UsernameSuggestion>> GetUsernameSuggestionByRole(int custId, string username, byte roleId);
    }

    public class UsernameSuggestionServiceImpl(IDynamicRepository dynamicRepository) : UsernameSuggestionService
    {
        private readonly IDynamicRepository dynamicRepository = dynamicRepository;

        public async Task<IEnumerable<UsernameSuggestion>> GetUsernamesSuggestion(
            int actorId,
            string suggestionQuery,
            bool isInternal,
            bool directDownline,
            bool isAccInfo,
            bool excludeMember,
            bool excludeCashMember)
            => (await dynamicRepository.FetchAsync<UsernameSuggestion>(
              new SelectAutoSuggestionCriteria(
                  actorId,
                  suggestionQuery,
                  isInternal,
                  directDownline,
                  isAccInfo,
                  excludeMember,
                  excludeCashMember))).OrderBy(item => item.UserName);

        public async Task<IEnumerable<UsernameSuggestion>> GetUsernameSuggestionByRole(int custId, string username, byte roleId)
            => (await dynamicRepository.FetchAsync<UsernameSuggestion>(
              new UsernameSuggestionByRoleCriteria(
                  custId,
                  username,
                  roleId))).OrderBy(item => item.UserName);

        public async Task<bool> UpdateUserSearch(int userId, string search, string firstName, string lastName, string nickname)
        {
            var criteria = new UpdateUserSearchCriteria(userId, search, firstName, lastName, nickname);
            await dynamicRepository.ExecuteAsync(criteria);

            return true;
        }
    }
}