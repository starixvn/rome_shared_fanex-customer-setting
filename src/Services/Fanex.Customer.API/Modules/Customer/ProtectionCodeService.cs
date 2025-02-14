using System.Globalization;
using Fanex.Cryptography.Aes;
using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using ProtectionCodeResource = Fanex.Customer.API.Modules.AppResources.Resources;

namespace Fanex.Customer.API.Modules.Customer
{
    public interface IProtectionCodeService
    {
        string GenerateHashKey();

        Task<string> GetAsync(int custId, int subAccId);

        Task<Result> UpdateAsync(UpdateProtectionCodeParam param);
    }

    public class ProtectionCodeService : IProtectionCodeService
    {
        private const int Length = 4;
        private readonly IAesService aes;
        private readonly IEncryption encryption;
        private readonly IDynamicRepository repository;

        public ProtectionCodeService(
            IAesService aes,
            IEncryption encryption,
            IDynamicRepository repository)
        {
            this.aes = aes;
            this.encryption = encryption;
            this.repository = repository;
        }

        public string GenerateHashKey()
            => encryption.RandomString(Length);

        public Task<string> GetAsync(int custId, int subAccId)
            => repository.GetAsync<string>(new GetProtectionCodeCriteria(custId, subAccId));

        public async Task<Result> UpdateAsync(UpdateProtectionCodeParam param)
        {
            if (string.IsNullOrWhiteSpace(param.ProtectionCode))
            {
                return Result.CreateError(ProtectionCodeResource.RequiredNewProtectionCode);
            }

            var rawProtectionCode = aes.Decrypt(param.ProtectionCode, param.HashKey);
            var validateResult = ValidateProtectionCode(rawProtectionCode, LanguageUtils.ToLanguage(param.LangKey));

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var criteria = new UpdateProtectionCodeCriteria(
                encryption.CfsEnCode(rawProtectionCode),
                param.CustId,
                param.SubAccId,
                param.LangKey,
                param.IP);
            await repository.ExecuteAsync(criteria);
            var result = new Result { Code = criteria.ErrCode, Message = criteria.ErrMsg };

            if (result.IsSuccessful)
            {
                result.Message = ProtectionCodeResource.UpdateProtectionCodeSuccessfully;
            }

            return result;
        }

        private static Result ValidateProtectionCode(string protectionCode, string language)
        {
            if (protectionCode.Length < 6)
            {
                return Result.CreateError(ProtectionCodeResource.ResourceManager.GetString("MinLengthNewProtectionCodeError", new CultureInfo(language)));
            }

            const int Space = 32;
            const int MaxAnsiCode = 127;

            if (protectionCode.Any(c => c == Space || c > MaxAnsiCode))
            {
                return Result.CreateError(ProtectionCodeResource.ResourceManager.GetString("ContainsSpaceOrUnicodeCharacterError", new CultureInfo(language)));
            }

            return new Result();
        }
    }
}