using System.Threading.Tasks;

namespace Dreidel.Spinner.Services
{
    public interface IDreidelService
    {
        Task<string> Spin();
    }
}