using System.Collections.Generic;

namespace Telefact
{
    public class TeletextPage
    {
        public int PageNumber { get; set; }
        public List<List<string>> Subpages { get; set; } = new();
    }
}
