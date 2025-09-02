#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    // Enables 'init' and records on older TFMs
    internal sealed class IsExternalInit { }
}
#endif
