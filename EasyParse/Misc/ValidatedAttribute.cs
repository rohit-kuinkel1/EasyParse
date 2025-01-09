using System;

namespace EasyParse.Misc.GlobalAttributes
{
    /// <summary>
    /// does absolutely nothing, just for readability to let the dev know that this property was validated before being set
    /// and no surprises can occur when we try to access it for its normal behavior
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal class ValidatedAttribute : Attribute
    {
        public string? ValidationMessage { get; }
        public ValidatedAttribute( string validationMessage = "Property is already validated." )
        {
            ValidationMessage = validationMessage;
        }
    }

    /// <summary>
    /// does absolutely nothing, just for readability to let the dev know that hey this property still needs validation
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal class NeedsValidationAttribute : Attribute
    {
        public string? ValidationMessage { get; }
        public NeedsValidationAttribute( string validationMessage = "Property needs validation." )
        {
            ValidationMessage = validationMessage;
        }
    }

    /// <summary>
    /// does absolutely nothing, just for readability to let the dev know that hey this propertydoes not need any sort of validation
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    internal class NoValidationRequiredAttribute : Attribute
    {
        public string? ValidationMessage { get; }
        public NoValidationRequiredAttribute( string validationMessage = "Property does not need validation." )
        {
            ValidationMessage = validationMessage;
        }
    }
}
