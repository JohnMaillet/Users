using ServerlessAPI.Utilities.Types;

namespace ServerlessAPI.Utilities
{

    public interface IErrorMessages
    {
        public string GetErrorMessage(ErrorCode errorCode);
    }
}
