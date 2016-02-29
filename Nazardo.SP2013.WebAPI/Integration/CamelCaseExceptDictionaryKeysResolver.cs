using System;
using Newtonsoft.Json.Serialization;

namespace Nazardo.SP2013.WebAPI.Integration
{
    /// <summary>
    /// This contract resolver overrides default dictionary keys resolver
    /// and preserves the case of dictionary keys when serializing into JSON.
    /// </summary>
    internal sealed class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);
            contract.PropertyNameResolver = propertyName => propertyName;
            return contract;
        }
    }
}