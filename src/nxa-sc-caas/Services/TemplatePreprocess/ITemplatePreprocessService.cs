using NXA.SC.Caas.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Services
{
    public interface ITemplatePreprocessService
    {
        Task<IEnumerable<TemplateParam>> FindParams(string templateStr, string fileName);
    }
}
