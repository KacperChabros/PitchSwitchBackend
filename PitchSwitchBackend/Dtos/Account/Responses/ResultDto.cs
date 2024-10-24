namespace PitchSwitchBackend.Dtos.Account.Responses
{
    public class ResultDto<DataType>
    {
        public bool IsSuccess { get; private set; }
        public DataType Data { get; private set; }
        public string ErrorMessage { get; private set; }

        protected ResultDto(bool isSuccess, DataType data, string errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }
        public static ResultDto<DataType> Succeeded(DataType data)
        {
            return new ResultDto<DataType>(true, data, null);
        }

        public static ResultDto<DataType> Failed(string errorMessage)
        {
            return new ResultDto<DataType>(false, default, errorMessage);
        }
    }
}
