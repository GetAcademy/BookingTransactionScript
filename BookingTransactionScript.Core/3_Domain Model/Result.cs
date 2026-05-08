namespace BookingTransactionScript.Core._3_Domain_Model
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        public Result(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true);
        public static Result Fail(string errorMessage) => new(false, errorMessage);
    }
}
