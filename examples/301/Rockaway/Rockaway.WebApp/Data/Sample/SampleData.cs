// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {
	private static Guid TestGuid(int seed, char pad) => new(seed.ToString().PadLeft(32, pad));
}
