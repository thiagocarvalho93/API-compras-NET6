namespace ApiDotnet.Application.DTOs
{
    public class PageBasedResponseDTO<T>
    {
        public int TotalRegisters { get; private set; }
        public List<T> Data { get; private set; }

        public PageBasedResponseDTO(int totalRegisters, List<T> data)
        {
            TotalRegisters = totalRegisters;
            Data = data;
        }
    }
}