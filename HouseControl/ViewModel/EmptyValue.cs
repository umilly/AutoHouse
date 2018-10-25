using System;
using Facade;

namespace ViewModel
{
    public class EmptyValue : IConditionSource
    {
        public static EmptyValue Instance { get; } = new EmptyValue();

        public string SourceName => "�� ����������";
        private EmptyValue() { }
        public string Value
        {
            get
            {
                throw new ArgumentOutOfRangeException("��������� ������� �� ���������, ��� �� �� ����������� ��������");
            }
        }

        public Type ValueType => typeof (object);
    }
}