using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Common.Exporters;

public interface IExcelWriter : ITransientService
{
    Stream WriteToStream<T>(IList<T> data);
}