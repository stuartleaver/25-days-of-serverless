using System;
using System.Threading.Tasks;

namespace Dreidel.Spinner.Services
{
    public class DreidelService : IDreidelService
    {
        private readonly string[] _dreidel = new string[]{"נ","ג","ה","ש"};

        public async Task<string> Spin()
        {
            var random = new Random();

            return _dreidel[random.Next(0, _dreidel.Length)];
        }
    }
}