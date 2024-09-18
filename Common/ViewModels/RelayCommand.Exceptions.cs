using System;

namespace ViewModels
{
    public partial class RelayCommand
    {
        public static class Exceptions
        {
            public static readonly ArgumentException executeException = new("null не разрешён", "execute");
            public static readonly ArgumentException canExecuteException = new("null не разрешён", "canExecute");
            public static readonly ArgumentException converterException = new("null не разрешён", "converter");
        }
    }
}
