namespace VM.Util
{
    public interface IError
    {
        string ToString();
    }

    public class Error : IError
    {
        private readonly string _error;

        public Error(string errMsg)
        {
            _error = errMsg;
        }

        public override string ToString()
        {
            return _error;
        }
    }
}
