using ProcessAutomation.Common.DTO.Contract;
using System;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class ObjectType : IObjectType
    {
        readonly object value;

        public object Value 
        {
            get
            {
                return value;
            }
        }

        public ObjectType(object value)
        {
            this.value = value;
        }

        public static implicit operator int(ObjectType value)
        {
            int result = 0;
            if (value != null)
            {
                result = (int)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(int value)
        {
            return new ObjectType(value);
        }

        public static implicit operator ObjectType(ListObjectType value)
        {
            return new ObjectType(value);
        }

        public static implicit operator ListObjectType?(ObjectType value)
        {
            ListObjectType? result = null;
            if (value != null)
            {
                result = (ListObjectType)value.value;
            }
            return result;
        }

        public static implicit operator bool(ObjectType value)
        {
            bool result = false;
            if (value != null)
            {
                result = (bool)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(bool value)
        {
            return new ObjectType(value);
        }

        public static implicit operator decimal(ObjectType value)
        {
            decimal result = 0;
            if (value != null)
            {
                result = (decimal)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(decimal value)
        {
            return new ObjectType(value);
        }

        public static implicit operator long(ObjectType value)
        {
            long result = 0;
            if (value != null)
            {
                result = (long)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(long value)
        {
            return new ObjectType(value);
        }

        public static implicit operator double(ObjectType value)
        {
            double result = 0;
            if (value != null)
            {
                result = (double)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(double value)
        {
            return new ObjectType(value);
        }

        public static implicit operator string?(ObjectType value)
        {
            string? result = null;
            if (value != null)
            {
                result = (string)value.value;
            }
            return result;
        }

        public static implicit operator ObjectType(string value)
        {
            return new ObjectType(value);
        }

        //public static bool operator ==(ObjectType leftType, ObjectType rightType)
        //{
        //    return (leftType?.value == rightType?.value);
        //}

        //public static bool operator !=(ObjectType leftType, ObjectType rightType)
        //{
        //    return (leftType?.value != rightType?.value);
        //}

        public override string? ToString()
        {
            string? result = null;

            if (value != null)
            {
                result = value.ToString();
            }

            return result;
        }
    }
}
