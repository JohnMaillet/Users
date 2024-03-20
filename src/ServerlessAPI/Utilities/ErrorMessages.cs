using ServerlessAPI.Utilities.Types;

namespace ServerlessAPI.Utilities
{

    public class ErrorMessages: IErrorMessages
    {

        private readonly Dictionary<ErrorCode, string> _errorMessages = new Dictionary<ErrorCode, string>
    {
        { ErrorCode.InvalidInput, "Invalid input provided." },
        { ErrorCode.NotFound, "The requested resource was not found." },
        { ErrorCode.UserNameOrEmailExists,"Username or email already exists" },
        { ErrorCode.UnknownError, "Unkown error" },
        // Add more error messages as needed
    };

        public string GetErrorMessage(ErrorCode errorCode)
        {
            if (_errorMessages.TryGetValue(errorCode, out string errorMessage))
            {
                return errorMessage;
            }
            return "Unknown error.";
        }
    }
}
