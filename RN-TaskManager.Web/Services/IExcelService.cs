using System.Collections.Generic;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.Services
{
    public interface IExcelService
    {
        string Report(List<object> items);
    }
}
