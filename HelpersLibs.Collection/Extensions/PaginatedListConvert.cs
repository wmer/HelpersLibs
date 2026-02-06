using System;
using System.Collections.Generic;
using System.Text;

namespace HelpersLibs.Collection.Extensions; 
public static class PaginatedListConvert {
    public static PaginatedList<U> Convert<T, U>(this PaginatedList<T> source, Func<PaginatedList<T>, PaginatedList<U>> paginatedList) {
        return paginatedList.Invoke(source);
    }
}
