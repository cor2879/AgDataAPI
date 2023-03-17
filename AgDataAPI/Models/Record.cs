#pragma warning disable CS8767, CS8765, CS8618, CS8604
using System.Diagnostics;
using System.Text.Json;

namespace AgDataAPI.Models;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class Record : IEquatable<Record>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string DebuggerDisplay()
    {
        return JsonSerializer.Serialize(this);
    }

    public bool Equals(Record other)
    {
        if (other == null)
        {
            return false;
        }

        return this.Id == other.Id && this.Name == other.Name && this.Address == other.Address;
    }

    public override bool Equals(object other)
    {
        if (((object)other) == null)
        {
            return false;
        }

        return this.Equals(other as Record);
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }
}
