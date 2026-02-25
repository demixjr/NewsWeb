using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace PL.Helpers
{
    /// <summary>
    /// Модуль для керування flash-повідомленнями через TempData.
    /// Усуває дублювання рядків-ключів по всьому проєкті.
    /// </summary>
    public static class FlashMessageHelper
    {
        private const string SuccessKey = "Success";
        private const string ErrorKey = "Error";

        public static void SetSuccess(ITempDataDictionary tempData, string message)
            => tempData[SuccessKey] = message;

        public static void SetError(ITempDataDictionary tempData, string message)
            => tempData[ErrorKey] = message;

        public static void SetError(ITempDataDictionary tempData, Exception ex)
            => SetError(tempData, ex.Message);
    }
}
