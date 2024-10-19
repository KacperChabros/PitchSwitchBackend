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

        public new static IdentityResultDto<DataType> Succeeded(DataType newUserDto)
        {
            return new IdentityResultDto<DataType>(true, newUserDto, null, null);
        }

        public static IdentityResultDto<DataType> Failed(IEnumerable<IdentityError> errors, string errorMessage = "")
        {
            return new IdentityResultDto<DataType>(false, default, errorMessage, errors);
        }
    }
}
