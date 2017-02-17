﻿using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MTry<A> :
        Optional<Try<A>, A>,
        MonadPlus<Try<A>, A>,
        Foldable<Try<A>, A>,
        BiFoldable<Try<A>, Unit, A>
    {
        public static readonly MTry<A> Inst = default(MTry<A>);

        static Try<A> none = () => raise<A>(new BottomException());

        [Pure]
        public MB Bind<MONADB, MB, B>(Try<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B>
        {
            var mr = ma.Try();
            if (mr.IsFaulted) return default(MONADB).Fail(mr.Exception);
            return f(mr.Value);
        }

        [Pure]
        public Try<A> Fail(object err) =>
            Try<A>(() => { throw new BottomException(); });

        [Pure]
        public Try<A> Fail(Exception err = null) =>
            Try<A>(() => { throw err; });

        [Pure]
        public Try<A> Plus(Try<A> ma, Try<A> mb) => () =>
        {
            var res = ma.Try();
            if (!res.IsFaulted) return res.Value;
            return mb().Value;
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <param name="xs">The bound monad value(s)</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Try<A> FromSeq(IEnumerable<A> xs)
        {
            var head = xs.FirstOrDefault();
            return () => head;
        }

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Try<A> Return(A x) =>
            () => x;

        /// <summary>
        /// Monad return
        /// </summary>
        /// <param name="f">The function to invoke to get the bound monad value(s)</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Try<A> Return(Func<A> f) => () =>
            f();

        [Pure]
        public Try<A> Zero() => 
            none;

        [Pure]
        public bool IsNone(Try<A> opt) =>
            opt.Match(
                Succ: __ => false, 
                Fail: ex => true);

        [Pure]
        public bool IsSome(Try<A> opt) =>
            opt.Match(
                Succ: __ => true,
                Fail: ex => false);

        [Pure]
        public bool IsUnsafe(Try<A> opt) =>
            false;

        [Pure]
        public B Match<B>(Try<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted)
                return None();
            else
                return Some(res.Value);
        }

        public Unit Match(Try<A> opt, Action<A> Some, Action None)
        {
            var res = opt.Try();
            if (res.IsFaulted) None(); else Some(res.Value);
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(Try<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted)
                return None();
            else
                return Some(res.Value);
        }

        [Pure]
        public S Fold<S>(Try<A> ma, S state, Func<S, A, S> f)
        {
            var res = ma.Try();
            if (res.IsFaulted) return state;
            return f(state, res.Value);
        }

        [Pure]
        public S FoldBack<S>(Try<A> ma, S state, Func<S, A, S> f)
        {
            var res = ma.Try();
            if (res.IsFaulted) return state;
            return f(state, res.Value);
        }

        [Pure]
        public S BiFold<S>(Try<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted)
                return fa(state, unit);
            else
                return fb(state, res.Value);
        }

        [Pure]
        public S BiFoldBack<S>(Try<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted)
                return fa(state, unit);
            else
                return fb(state, res.Value);
        }

        [Pure]
        public int Count(Try<A> ma) =>
            ma.Try().IsFaulted
                ? 0
                : 1;
    }
}
