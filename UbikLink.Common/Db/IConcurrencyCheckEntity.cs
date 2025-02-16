using System.ComponentModel.DataAnnotations;

namespace UbikLink.Common.Db;

public interface IConcurrencyCheckEntity
{
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}