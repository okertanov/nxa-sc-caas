using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Models
{
    public interface ITemplatePreprocessService
    {
        Task<IEnumerable<TemplateParam>> FindParams(string templateStr, string fileName);
    }
}
