using Microsoft.AspNetCore.Identity;

namespace PitchSwitchBackend.Dtos.Account.Responses
{
    public class IdentityResultDto<DataType> : ResultDto<DataType>
    {
        public IEnumerable<IdentityError> Errors { get; private set; }
        private IdentityResultDto(bool isSuccess, DataType data, string errorMessage, IEnumerable<IdentityError> errors)
            : base(isSuccess, data, errorMessage)
        {
            Errors = errors;
        }

        public new static IdentityResultDto<DataType> Succeeded(DataType data)
        {
            return new IdentityResultDto<DataType>(true, data, null, null);
        }
        public new static IdentityResultDto<DataType> Failed(string errorMessage)
        {
            return new IdentityResultDto<DataType>(false, default, errorMessage, default);
        }

        public static IdentityResultDto<DataType> Failed(IEnumerable<IdentityError> errors, string errorMessage = "")
        {
            return new IdentityResultDto<DataType>(false, default, errorMessage, errors);
        }
    }
}
