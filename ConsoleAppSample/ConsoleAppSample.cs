using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Unity;
using Unity.RegistrationByConvention;

namespace ConsoleAppSample
{
    static class StringExtensions
    {
        public static string ToOneCharString(this char c)
        {
            return new string(c, 1);
        }

        public static string ToStringInvariant<T>(this T value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public static string ToStringLocal<T>(this T value)
        {
            return Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        public static string FormatInvariant(this string value, params object[] arguments)
        {
            return string.Format(CultureInfo.InvariantCulture, value, arguments);
        }

        public static string FormatLocal(this string value, params object[] arguments)
        {
            return string.Format(CultureInfo.CurrentCulture, value, arguments);
        }

        public static string Spaces(this int value)
        {
            return new string(' ', value);
        }

        public static bool EqualsOrdinal(this string strA, string strB)
        {
            return string.CompareOrdinal(strA, strB) == 0;
        }

        public static bool EqualsOrdinalIgnoreCase(this string strA, string strB)
        {
            return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static int SafeLength(this string value)
        {
            return value == null ? 0 : value.Length;
        }

        public static string JoinTo(this string value, params string[] others)
        {
            var builder = new StringBuilder(value);
            foreach (var v in others)
            {
                builder.Append(v);
            }
            return builder.ToString();
        }

        public static bool IsBooleanString(this string value)
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("false", StringComparison.OrdinalIgnoreCase);
        }

        public static bool ToBoolean(this string value)
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
    static class StringBuilderExtensions
    {
        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params string[] values)
        {
            if (condition)
                foreach (var value in values)
                    builder.Append(value);

            return builder;
        }

        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params char[] values)
        {
            if (condition)
                foreach (var value in values)
                    builder.Append(value);

            return builder;
        }

        public static StringBuilder AppendFormatWhen(this StringBuilder builder, bool condition, string format, params object[] args)
        {
            return condition
                ? builder.AppendFormat(format, args)
                : builder;
        }

        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string ifTrue, string ifFalse)
        {
            return condition
                ? builder.Append(ifTrue)
                : builder.Append(ifFalse);
        }

        public static StringBuilder BimapIf(this StringBuilder builder, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue, Func<StringBuilder, StringBuilder> ifFalse)
        {
            return condition
                ? ifTrue(builder)
                : ifFalse(builder);
        }

        public static StringBuilder MapIf(this StringBuilder builder, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue)
        {
            return condition
                ? ifTrue(builder)
                : builder;
        }

        public static StringBuilder AppendIfNotEmpty(this StringBuilder builder, params string[] values)
        {
            foreach (var value in values)
                if (value.Length > 0)
                    builder.Append(value);

            return builder;
        }

        public static string SafeToString(this StringBuilder builder)
        {
            return builder == null
                ? string.Empty
                : builder.ToString();
        }

        public static int SafeLength(this StringBuilder builder)
        {
            return builder == null ? 0 : builder.Length;
        }

        public static StringBuilder TrimEnd(this StringBuilder builder, char c)
        {
            return builder.Length > 0
                ? builder.Remove(builder.Length - 1, 1)
                : builder;
        }

        public static StringBuilder TrimEndIfMatch(this StringBuilder builder, char c)
        {
            if (builder.Length > 0)
                if (builder[builder.Length - 1] == c)
                    builder.Remove(builder.Length - 1, 1);

            return builder;
        }

        public static StringBuilder TrimEndIfMatchWhen(this StringBuilder builder, bool condition, char c)
        {
            return condition
                ? builder.TrimEndIfMatch(c)
                : builder;
        }

        public static int TrailingSpaces(this StringBuilder builder)
        {
            var bound = builder.Length - 1;
            if (builder.Length == 0) return 0;
            if (builder[bound] != ' ') return 0;
            var c = 0;
            for (var i = bound; i <= bound; i--)
            {
                if (i < 0) break;
                if (builder[i] != ' ') break;
                c++;
            }
            return c;
        }

        /// <summary>
        /// Indicates whether the string value of a <see cref="System.Text.StringBuilder"/>
        /// starts with the input <see cref="System.String"/> parameter. Returns false if either 
        /// the StringBuilder or input string is null or empty.
        /// </summary>
        /// <param name="builder">The <see cref="System.Text.StringBuilder"/> to test.</param>
        /// <param name="s">The <see cref="System.String"/> to look for.</param>
        /// <returns></returns>
        public static bool SafeStartsWith(this StringBuilder builder, string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return builder?.Length >= s.Length
                && builder.ToString(0, s.Length) == s;
        }

        /// <summary>
        /// Indicates whether the string value of a <see cref="System.Text.StringBuilder"/>
        /// ends with the input <see cref="System.String"/> parameter. Returns false if either 
        /// the StringBuilder or input string is null or empty.
        /// </summary>
        /// <param name="builder">The <see cref="System.Text.StringBuilder"/> to test.</param>
        /// <param name="s">The <see cref="System.String"/> to look for.</param>
        /// <returns></returns>
        public static bool SafeEndsWith(this StringBuilder builder, string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return builder?.Length >= s.Length
                && builder.ToString(builder.Length - s.Length, s.Length) == s;
        }
    }

    public class ConsoleAppSample
    {
        private string[] args;

        private IUnityContainer _container;

        public ConsoleAppSample(string[] args)
        {
            this.args = args;
        }

        private void CreateMappings()
        {
        }

        private void ParseCommandLine()
        {
            Parser parser = new Parser(with =>
            {
                with.CaseSensitive = false;
                with.HelpWriter = null;
                with.AutoHelp = false;
                with.AutoVersion = false;
            });

            var h = new HelpText();

            var parserResult = parser.ParseArguments<AddOptions, CommitOptions, CloneOptions>(args);
            parserResult.WithParsed<AddOptions>((AddOptions opts) => RunAddAndReturnExitCode(opts))
                .WithParsed<CommitOptions>((CommitOptions opts) => RunCommitAndReturnExitCode(opts))
                .WithParsed<CloneOptions>((CloneOptions opts) => RunCloneAndReturnExitCode(opts))
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));

            //Console.ReadKey();

        }


        static void DisplayHelp<T>(ParserResult<object> parserResult, ParserResult<T> result)
        {

        }

        private void DisplayHelp(ParserResult<object> parserResult, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(parserResult, h =>
            {
                h = new HelpText(new MySentenceBuilder(), "MS Operation Manager - AgentManagement (agtmve)");
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "Myapp 2.0.0-beta"; //change header
                h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
                h.AutoVersion = false;
                h.AutoHelp = false;
                
                return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                //return h;
            }, e => e, verbsIndex: true);
            Console.WriteLine(helpText);

        }

        private HelpText ParsingErrorsHandler<T>(ParserResult<T> parserResult, HelpText current)
        {
            return current;
        }

        private int RunCloneAndReturnExitCode(CloneOptions opts)
        {
            Debug.WriteLine(opts.ToString());
            return 0;
        }

        private int RunCommitAndReturnExitCode(CommitOptions opts)
        {
            Debug.WriteLine(opts.ToString());
            return 0;

        }

        private int RunAddAndReturnExitCode(AddOptions opts)
        {
            Debug.WriteLine(opts.ToString());
            return 0;

        }

        void RunOptions(Options opts)
        {
            //handle options
            Debug.WriteLine(opts.ToString());
        }

        void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            Debug.WriteLine(errs.ToString());

        }

        public void InitializeComponent()
        {

            ParseCommandLine();
            CreateMappings();

            _container = new UnityContainer();
            _container.RegisterTypes(
                AllClasses.FromAssembliesInBasePath(),
                WithMappings.FromMatchingInterface,
                WithName.Default
            );
        }

        public void Run()
        {

        }
    }

    [Verb("add", HelpText = "Add file contents to the index.")]
    class AddOptions
    {

        //normal options here
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        //normal options here
        [Option('f', "file", Required = true, HelpText = "Set file.")]
        public string InputFile { get; set; }


    }
    [Verb("commit", HelpText = "Record changes to the repository.")]
    class CommitOptions
    {

        //commit options here
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    class CloneOptions
    {

        //clone options here
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }

    class MySentenceBuilder : SentenceBuilder
    {
        public override Func<string> RequiredWord
        {
            get { return () => "要求."; }
        }

        public override Func<string> ErrorsHeadingText
        {
            get { return () => "エラー:"; }
        }

        public override Func<string> UsageHeadingText
        {
            get { return () => "使い方:"; }
        }

        public override Func<string> OptionGroupWord
        {
            get { return () => "グループ"; }
        }

        public override Func<bool, string> HelpCommandText
        {
            get
            {
                return isOption => isOption
                    ? "Display this help screen."
                    : "Display more information on a specific command.";
            }
        }

        public override Func<bool, string> VersionCommandText
        {
            get { return _ => "Display version information."; }
        }

        public override Func<Error, string> FormatError
        {
            get
            {
                return error =>
                    {
                        switch (error.Tag)
                        {
                            case ErrorType.BadFormatTokenError:
                                return "Token '".JoinTo(((BadFormatTokenError)error).Token, "' is not recognized.");
                            case ErrorType.MissingValueOptionError:
                                return "Option '".JoinTo(((MissingValueOptionError)error).NameInfo.NameText,
                                    "' has no value.");
                            case ErrorType.UnknownOptionError:
                                return "Option '".JoinTo(((UnknownOptionError)error).Token, "' is unknown.");
                            case ErrorType.MissingRequiredOptionError:
                                var errMisssing = ((MissingRequiredOptionError)error);
                                return errMisssing.NameInfo.Equals(NameInfo.EmptyName)
                                           ? "A required value not bound to option name is missing."
                                           : "要求されたオプション '".JoinTo(errMisssing.NameInfo.NameText, "' がありません.");
                            case ErrorType.BadFormatConversionError:
                                var badFormat = ((BadFormatConversionError)error);
                                return badFormat.NameInfo.Equals(NameInfo.EmptyName)
                                           ? "A value not bound to option name is defined with a bad format."
                                           : "Option '".JoinTo(badFormat.NameInfo.NameText, "' is defined with a bad format.");
                            case ErrorType.SequenceOutOfRangeError:
                                var seqOutRange = ((SequenceOutOfRangeError)error);
                                return seqOutRange.NameInfo.Equals(NameInfo.EmptyName)
                                           ? "A sequence value not bound to option name is defined with few items than required."
                                           : "A sequence option '".JoinTo(seqOutRange.NameInfo.NameText,
                                                "' is defined with fewer or more items than required.");
                            case ErrorType.BadVerbSelectedError:
                                return "Verb '".JoinTo(((BadVerbSelectedError)error).Token, "' is not recognized.");
                            case ErrorType.NoVerbSelectedError:
                                return "No verb selected.";
                            case ErrorType.RepeatedOptionError:
                                return "Option '".JoinTo(((RepeatedOptionError)error).NameInfo.NameText,
                                    "' is defined multiple times.");
                            case ErrorType.SetValueExceptionError:
                                var setValueError = (SetValueExceptionError)error;
                                return "Error setting value to option '".JoinTo(setValueError.NameInfo.NameText, "': ", setValueError.Exception.Message);
                            case ErrorType.MissingGroupOptionError:
                                var missingGroupOptionError = (MissingGroupOptionError)error;
                                return "At least one option from group '".JoinTo(
                                    missingGroupOptionError.Group,
                                    "' (",
                                    string.Join(", ", missingGroupOptionError.Names.Select(n => n.NameText)),
                                    ") is required.");
                            case ErrorType.GroupOptionAmbiguityError:
                                var groupOptionAmbiguityError = (GroupOptionAmbiguityError)error;
                                return "Both SetName and Group are not allowed in option: (".JoinTo(groupOptionAmbiguityError.Option.NameText, ")");
                            case ErrorType.MultipleDefaultVerbsError:
                                return MultipleDefaultVerbsError.ErrorMessage;

                        }
                        throw new InvalidOperationException();
                    };
            }
        }

        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors
        {
            get
            {
                return errors =>
                {
                    var bySet = from e in errors
                                group e by e.SetName into g
                                select new { SetName = g.Key, Errors = g.ToList() };

                    var msgs = bySet.Select(
                        set =>
                        {
                            var names = string.Join(
                                string.Empty,
                                (from e in set.Errors select "'".JoinTo(e.NameInfo.NameText, "', ")).ToArray());
                            var namesCount = set.Errors.Count();

                            var incompat = string.Join(
                                string.Empty,
                                (from x in
                                     (from s in bySet where !s.SetName.Equals(set.SetName) from e in s.Errors select e)
                                    .Distinct()
                                 select "'".JoinTo(x.NameInfo.NameText, "', ")).ToArray());

                            return
                                new StringBuilder("Option")
                                        .AppendWhen(namesCount > 1, "s")
                                        .Append(": ")
                                        .Append(names.Substring(0, names.Length - 2))
                                        .Append(' ')
                                        .AppendIf(namesCount > 1, "are", "is")
                                        .Append(" not compatible with: ")
                                        .Append(incompat.Substring(0, incompat.Length - 2))
                                        .Append('.')
                                    .ToString();
                        }).ToArray();
                    return string.Join(Environment.NewLine, msgs);
                };
            }
        }
    }


}
