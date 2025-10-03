using System.Collections.Generic;
using Ogur.Fishing.Host.Wpf.Services.Models;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Input;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services.Implementations;



/// <summary>
/// Default bait catalog exposing 8 hotbar slots mapped to keys 1..4 and F1..F4.
/// </summary>
public sealed class BaitCatalog : IBaitCatalog
{
    /// <inheritdoc />
    public IReadOnlyList<BaitOption> GetAll() => new[]
    {
        new BaitOption { Id = "slot_1",  DisplayName = "1",  Key = Key.D1  },
        new BaitOption { Id = "slot_2",  DisplayName = "2",  Key = Key.D2  },
        new BaitOption { Id = "slot_3",  DisplayName = "3",  Key = Key.D3  },
        new BaitOption { Id = "slot_4",  DisplayName = "4",  Key = Key.D4  },
        new BaitOption { Id = "slot_f1", DisplayName = "F1", Key = Key.F1  },
        new BaitOption { Id = "slot_f2", DisplayName = "F2", Key = Key.F2  },
        new BaitOption { Id = "slot_f3", DisplayName = "F3", Key = Key.F3  },
        new BaitOption { Id = "slot_f4", DisplayName = "F4", Key = Key.F4  },
    };
}