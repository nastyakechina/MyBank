using System;

namespace Presenter
{
    public class WalletException : Exception
    {
        public WalletException(string message) : base(message) { }
    }
}