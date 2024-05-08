using NewsExtractor.Models;

namespace NewsExtractor.Services
{
    public interface INewsExtractor
    {
        Task<InfoTable> InfoReturner();
        Task<PostMessage> InfoSender(InfoTable info);
        Task<List<InfoTable>> ExtractAllInfo();
    }
}
