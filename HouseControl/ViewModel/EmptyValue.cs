using System;
using Facade;

namespace ViewModel
{
    public class EmptyValue : IConditionSource
    {
        public static EmptyValue Instance { get; } = new EmptyValue();

        public string SourceName => "Не установлен";
        private EmptyValue() { }
        public string Value
        {
            get
            {
                throw new ArgumentOutOfRangeException("Настройка реакций не завершена, где то не установлено значение");
            }
        }

        public Type ValueType => typeof (object);
    }
}