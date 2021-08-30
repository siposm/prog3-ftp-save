using System;
using System.Collections.Generic;
using System.Reflection;

namespace Validation.Classes
{
    // public class MaxLengthAttribute(Length)
    // public class RangeAttribute(Min, Max)
    // internal interface IValidation: bool Validate(object instance, PropertyInfo propertyInfo)
    // internal class MaxLengthValidation : IValidation ... ctor(MaxLengthAttribute) ... Validate(xxx)
    // internal class RangeValidation : IValidation ... ctor(RangeAttribute) ... Validate(xxx)
    // ...
    // public class Validator , public bool Validate(object instance)
    
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public MaxLengthAttribute(int length)
        {
            this.Length = length;
        }
    }

    // only support int ranges for simplicity 
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public RangeAttribute(int min = int.MinValue, int max = int.MaxValue)
        {
            Min = min;
            Max = max;
        }
    }

    internal interface IValidation
    {
        bool Validate(object instance, PropertyInfo propertyInfo);
    }

    internal class MaxLengthValidation : IValidation
    {
        MaxLengthAttribute maxLength;

        public MaxLengthValidation(MaxLengthAttribute maxLength)
        {
            this.maxLength = maxLength;
        }

        public bool Validate(object instance, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(string))
            {
                var value = (string)propertyInfo.GetValue(instance);
                return value.Length <= maxLength.Length;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    internal class RangeValidation : IValidation
    {
        RangeAttribute range;
        public RangeValidation(RangeAttribute range)
        {
            this.range = range;
        }
        public bool Validate(object instance, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(int))
            {
                var value = (int)propertyInfo.GetValue(instance);
                return value >= range.Min && value <= range.Max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    //IF not factory:
    // - ugly, repeated code, hard to find new variable names,
    // - no real possibility of generalizing the Validator, we want to be type safe in the methods  
    // ---> let a factory take care of the instantiations & casts,
    // only the factory will have to know the full list of the concrete validator attribute types in our code.
    // Other solutions are also possible 
    // (e.g. the attribute knows how to validate, BUT that solution breaks layering, makes logic appear on the data level)
    internal class ValidationFactory
    {
        public IValidation GetValidation(Attribute attribute)
        {
            if (attribute is MaxLengthAttribute)
            {
                return new MaxLengthValidation((MaxLengthAttribute)attribute);
            }

            if (attribute is RangeAttribute)
            {
                return new RangeValidation((RangeAttribute)attribute);
            }

            return null;
        }
    }

    public class Validator
    {
        public bool Validate(object instance)
        {
            ValidationFactory validationFactory = new ValidationFactory();

            PropertyInfo[] properties = instance.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                IEnumerable<Attribute> allAttributes = propertyInfo.GetCustomAttributes();
                foreach (Attribute attr in allAttributes)
                {
                    IValidation validation = validationFactory.GetValidation(attr);
                    if (validation?.Validate(instance, propertyInfo) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
