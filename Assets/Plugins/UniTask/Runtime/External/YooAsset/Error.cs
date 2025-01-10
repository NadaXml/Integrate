using System;
using System.Runtime.CompilerServices;
namespace Cysharp.Threading.Tasks {
    public class Error {
        // 因为UnTask的Error做了Internal保护，所以复制一份
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowArgumentNullExceptionCore(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowArgumentNullException<T>(T value, string paramName)
            where T : class
        {
            if (value == null) ThrowArgumentNullExceptionCore(paramName);
        }
    }
}
