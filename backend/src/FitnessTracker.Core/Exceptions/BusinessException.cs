namespace FitnessTracker.Core.Exceptions
{
    /// <summary>
    /// 業務邏輯異常，例如：系統預設資料無法修改、資料已被使用無法刪除等
    /// </summary>
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message) : base(message)
        {
            ErrorCode = "BUSINESS_ERROR";
        }

        public BusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = "BUSINESS_ERROR";
        }
    }
}
