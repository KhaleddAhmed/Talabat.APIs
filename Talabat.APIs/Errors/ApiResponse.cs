
namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statuscode,string?message=null)
        {
            StatusCode = statuscode;
            Message = message ?? GetDefaultMessageForStatusCode(statuscode);
        }

		private string? GetDefaultMessageForStatusCode(int statuscode)
		{
			return statuscode switch
			{
				400 => "Bad Request",
				401 => "Unathorized",
				404=>"Resource Not found",
				500=>"Errors Are the Path To The Dark Side, Error lead To anger,Annger leads to hate, Hate leads to career change",
				_ => null,
			};
		}
	}
}
