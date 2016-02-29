using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal static class Error
    {
        private const string HttpScheme = "http";

        private const string HttpsScheme = "https";

        internal static ArgumentException Argument(string messageFormat, params object[] messageArgs)
        {
            return new ArgumentException(Error.Format(messageFormat, messageArgs));
        }

        internal static ArgumentException Argument(string parameterName, string messageFormat, params object[] messageArgs)
        {
            return new ArgumentException(Error.Format(messageFormat, messageArgs), parameterName);
        }

        internal static ArgumentOutOfRangeException ArgumentMustBeGreaterThanOrEqualTo(string parameterName, object actualValue, object minValue)
        {
            string argumentMustBeGreaterThanOrEqualTo = "ArgumentMustBeGreaterThanOrEqualTo {0}";
            object[] objArray = new object[] { minValue };
            return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(argumentMustBeGreaterThanOrEqualTo, objArray));
        }

        internal static ArgumentOutOfRangeException ArgumentMustBeLessThanOrEqualTo(string parameterName, object actualValue, object maxValue)
        {
            string argumentMustBeLessThanOrEqualTo = "ArgumentMustBeLessThanOrEqualTo {0}";
            object[] objArray = new object[] { maxValue };
            return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(argumentMustBeLessThanOrEqualTo, objArray));
        }

        internal static ArgumentNullException ArgumentNull(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        internal static ArgumentNullException ArgumentNull(string parameterName, string messageFormat, params object[] messageArgs)
        {
            return new ArgumentNullException(parameterName, Error.Format(messageFormat, messageArgs));
        }

        internal static ArgumentException ArgumentNullOrEmpty(string parameterName)
        {
            string argumentNullOrEmpty = "ArgumentNullOrEmpty {0}";
            object[] objArray = new object[] { parameterName };
            return Error.Argument(parameterName, argumentNullOrEmpty, objArray);
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName, object actualValue, string messageFormat, params object[] messageArgs)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(messageFormat, messageArgs));
        }

        internal static ArgumentException ArgumentUriHasQueryOrFragment(string parameterName, Uri actualValue)
        {
            string argumentUriHasQueryOrFragment = "ArgumentUriHasQueryOrFragment {0}";
            object[] objArray = new object[] { actualValue };
            return new ArgumentException(Error.Format(argumentUriHasQueryOrFragment, objArray), parameterName);
        }

        internal static ArgumentException ArgumentUriNotAbsolute(string parameterName, Uri actualValue)
        {
            string argumentInvalidAbsoluteUri = "ArgumentInvalidAbsoluteUri {0}";
            object[] objArray = new object[] { actualValue };
            return new ArgumentException(Error.Format(argumentInvalidAbsoluteUri, objArray), parameterName);
        }

        internal static ArgumentException ArgumentUriNotHttpOrHttpsScheme(string parameterName, Uri actualValue)
        {
            string argumentInvalidHttpUriScheme = "ArgumentInvalidHttpUriScheme {0}, {1}, {2}";
            object[] objArray = new object[] { actualValue, "http", "https" };
            return new ArgumentException(Error.Format(argumentInvalidHttpUriScheme, objArray), parameterName);
        }

        internal static string Format(string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        internal static ArgumentException InvalidEnumArgument(string parameterName, int invalidValue, Type enumClass)
        {
            return new InvalidEnumArgumentException(parameterName, invalidValue, enumClass);
        }

        internal static InvalidOperationException InvalidOperation(string messageFormat, params object[] messageArgs)
        {
            return new InvalidOperationException(Error.Format(messageFormat, messageArgs));
        }

        internal static InvalidOperationException InvalidOperation(Exception innerException, string messageFormat, params object[] messageArgs)
        {
            return new InvalidOperationException(Error.Format(messageFormat, messageArgs), innerException);
        }

        internal static KeyNotFoundException KeyNotFound()
        {
            return new KeyNotFoundException();
        }

        internal static KeyNotFoundException KeyNotFound(string messageFormat, params object[] messageArgs)
        {
            return new KeyNotFoundException(Error.Format(messageFormat, messageArgs));
        }

        internal static NotSupportedException NotSupported(string messageFormat, params object[] messageArgs)
        {
            return new NotSupportedException(Error.Format(messageFormat, messageArgs));
        }

        internal static ObjectDisposedException ObjectDisposed(string messageFormat, params object[] messageArgs)
        {
            return new ObjectDisposedException(null, Error.Format(messageFormat, messageArgs));
        }

        internal static OperationCanceledException OperationCanceled()
        {
            return new OperationCanceledException();
        }

        internal static OperationCanceledException OperationCanceled(string messageFormat, params object[] messageArgs)
        {
            return new OperationCanceledException(Error.Format(messageFormat, messageArgs));
        }

        internal static ArgumentNullException PropertyNull()
        {
            return new ArgumentNullException("value");
        }
    }
}
