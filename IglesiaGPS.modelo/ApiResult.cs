namespace IglesiaGPS.modelo
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResult<T> Ok(T data)
        {
            return new ApiResult<T> { Success = true, Data = data, Message = "Operación exitosa" };
        }

        public static ApiResult<T> Fail(string message)
        {
            return new ApiResult<T> { Success = false, Message = message };
        }
    }
}
