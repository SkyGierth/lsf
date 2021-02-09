using System.Collections.Generic;
using System.Threading.Tasks;
using Lsf.Grading.Models;

namespace Lsf.Grading.Services.Notifiers
{
    public interface INotifier
    {
        Task NotifyChange(IEnumerable<Degree> degrees);

        Task NotifyError(string message);
    }
}