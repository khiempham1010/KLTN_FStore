using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Models.ProductViewModels
{
    public class SearchResultViewModel : CatalogViewModel
    {
        public string Keyword { get; set; }
    }
}
