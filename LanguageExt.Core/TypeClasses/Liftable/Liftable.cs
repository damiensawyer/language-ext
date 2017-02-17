﻿using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Liftable type-class
    /// <para>
    /// Can lift a value A into a 
    /// </para>
    /// </summary>
    /// <typeparam name="A">The type to be lifted</typeparam>
    [Typeclass]
    public interface Liftable<LA, A>
    {
        /// <summary>
        /// Lift value A into a Liftable<A>
        /// <summary>
        [Pure]
        LA Lift(A x);

        /// <summary>
        /// Lift value A into a Liftable<A>
        /// <summary>
        [Pure]
        LA FromSeq(IEnumerable<A> value);
    }
}
