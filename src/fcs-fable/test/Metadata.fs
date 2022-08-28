module Metadata

let references_core = [|
    "Fable.Core"
    "FSharp.Core"
    "mscorlib"
    "netstandard"
    "System.Collections"
    "System.Collections.Concurrent"
    "System.ComponentModel"
    "System.ComponentModel.Primitives"
    "System.ComponentModel.TypeConverter"
    "System.Console"
    "System.Core"
    "System.Diagnostics.Debug"
    "System.Diagnostics.Tools"
    "System.Diagnostics.Tracing"
    "System.Globalization"
    "System"
    "System.IO"
    "System.Net.Requests"
    "System.Net.WebClient"
    "System.Numerics"
    "System.Reflection"
    "System.Reflection.Extensions"
    "System.Reflection.Metadata"
    "System.Reflection.Primitives"
    "System.Reflection.TypeExtensions"
    "System.Runtime"
    "System.Runtime.Extensions"
    "System.Runtime.Numerics"
    "System.Text.Encoding"
    "System.Text.Encoding.Extensions"
    "System.Text.RegularExpressions"
    "System.Threading"
    "System.Threading.Tasks"
    "System.Threading.Thread"
    "System.ValueTuple"
    |]

let references_net45 = [|
    "Fable.Core"
    "Fable.Import.Browser"
    "FSharp.Core"
    "mscorlib"
    "System"
    "System.Core"
    "System.Data"
    "System.IO"
    "System.Xml"
    "System.Numerics"
    |]

let references_full = [|
    "Fable.Core"
    "FSharp.Core"
    "mscorlib"
    "netstandard"
    "Microsoft.CSharp"
    "Microsoft.VisualBasic.Core"
    "Microsoft.VisualBasic"
    "Microsoft.Win32.Primitives"
    "Microsoft.Win32.Registry"
    "System.AppContext"
    "System.Buffers"
    "System.Collections.Concurrent"
    "System.Collections.Immutable"
    "System.Collections.NonGeneric"
    "System.Collections.Specialized"
    "System.Collections"
    "System.ComponentModel.Annotations"
    "System.ComponentModel.DataAnnotations"
    "System.ComponentModel.EventBasedAsync"
    "System.ComponentModel.Primitives"
    "System.ComponentModel.TypeConverter"
    "System.ComponentModel"
    "System.Configuration"
    "System.Console"
    "System.Core"
    "System.Data.Common"
    "System.Data.DataSetExtensions"
    "System.Data"
    "System.Diagnostics.Contracts"
    "System.Diagnostics.Debug"
    "System.Diagnostics.DiagnosticSource"
    "System.Diagnostics.FileVersionInfo"
    "System.Diagnostics.Process"
    "System.Diagnostics.StackTrace"
    "System.Diagnostics.TextWriterTraceListener"
    "System.Diagnostics.Tools"
    "System.Diagnostics.TraceSource"
    "System.Diagnostics.Tracing"
    "System.Drawing.Primitives"
    "System.Drawing"
    "System.Dynamic.Runtime"
    "System.Formats.Asn1"
    "System.Globalization.Calendars"
    "System.Globalization.Extensions"
    "System.Globalization"
    "System.IO.Compression.Brotli"
    "System.IO.Compression.FileSystem"
    "System.IO.Compression.ZipFile"
    "System.IO.Compression"
    "System.IO.FileSystem.AccessControl"
    "System.IO.FileSystem.DriveInfo"
    "System.IO.FileSystem.Primitives"
    "System.IO.FileSystem.Watcher"
    "System.IO.FileSystem"
    "System.IO.IsolatedStorage"
    "System.IO.MemoryMappedFiles"
    "System.IO.Pipes.AccessControl"
    "System.IO.Pipes"
    "System.IO.UnmanagedMemoryStream"
    "System.IO"
    "System.Linq.Expressions"
    "System.Linq.Parallel"
    "System.Linq.Queryable"
    "System.Linq"
    "System.Memory"
    "System.Net.Http.Json"
    "System.Net.Http"
    "System.Net.HttpListener"
    "System.Net.Mail"
    "System.Net.NameResolution"
    "System.Net.NetworkInformation"
    "System.Net.Ping"
    "System.Net.Primitives"
    "System.Net.Requests"
    "System.Net.Security"
    "System.Net.ServicePoint"
    "System.Net.Sockets"
    "System.Net.WebClient"
    "System.Net.WebHeaderCollection"
    "System.Net.WebProxy"
    "System.Net.WebSockets.Client"
    "System.Net.WebSockets"
    "System.Net"
    "System.Numerics.Vectors"
    "System.Numerics"
    "System.ObjectModel"
    "System.Reflection.DispatchProxy"
    "System.Reflection.Emit.ILGeneration"
    "System.Reflection.Emit.Lightweight"
    "System.Reflection.Emit"
    "System.Reflection.Extensions"
    "System.Reflection.Metadata"
    "System.Reflection.Primitives"
    "System.Reflection.TypeExtensions"
    "System.Reflection"
    "System.Resources.Reader"
    "System.Resources.ResourceManager"
    "System.Resources.Writer"
    "System.Runtime.CompilerServices.Unsafe"
    "System.Runtime.CompilerServices.VisualC"
    "System.Runtime.Extensions"
    "System.Runtime.Handles"
    "System.Runtime.InteropServices.RuntimeInformation"
    "System.Runtime.InteropServices"
    "System.Runtime.Intrinsics"
    "System.Runtime.Loader"
    "System.Runtime.Numerics"
    "System.Runtime.Serialization.Formatters"
    "System.Runtime.Serialization.Json"
    "System.Runtime.Serialization.Primitives"
    "System.Runtime.Serialization.Xml"
    "System.Runtime.Serialization"
    "System.Runtime"
    "System.Security.AccessControl"
    "System.Security.Claims"
    "System.Security.Cryptography.Algorithms"
    "System.Security.Cryptography.Cng"
    "System.Security.Cryptography.Csp"
    "System.Security.Cryptography.Encoding"
    "System.Security.Cryptography.OpenSsl"
    "System.Security.Cryptography.Primitives"
    "System.Security.Cryptography.X509Certificates"
    "System.Security.Principal.Windows"
    "System.Security.Principal"
    "System.Security.SecureString"
    "System.Security"
    "System.ServiceModel.Web"
    "System.ServiceProcess"
    "System.Text.Encoding.CodePages"
    "System.Text.Encoding.Extensions"
    "System.Text.Encoding"
    "System.Text.Encodings.Web"
    "System.Text.Json"
    "System.Text.RegularExpressions"
    "System.Threading.Channels"
    "System.Threading.Overlapped"
    "System.Threading.Tasks.Dataflow"
    "System.Threading.Tasks.Extensions"
    "System.Threading.Tasks.Parallel"
    "System.Threading.Tasks"
    "System.Threading.Thread"
    "System.Threading.ThreadPool"
    "System.Threading.Timer"
    "System.Threading"
    "System.Transactions.Local"
    "System.Transactions"
    "System.ValueTuple"
    "System.Web.HttpUtility"
    "System.Web"
    "System.Windows"
    "System.Xml.Linq"
    "System.Xml.ReaderWriter"
    "System.Xml.Serialization"
    "System.Xml.XDocument"
    "System.Xml.XPath.XDocument"
    "System.Xml.XPath"
    "System.Xml.XmlDocument"
    "System.Xml.XmlSerializer"
    "System.Xml"
    "System"
    "WindowsBase"
    |]
