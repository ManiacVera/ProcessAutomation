namespace ProcessAutomation.Common.DTO.Constants
{
    public static class FunctionConstants
    {
        public const string IN_LINE = "IN_LINE";
        public const string BY_LINE = "BY_LINE";
        public const string BY_CSV = "BY_CSV";
        public const string CUSTOM = "CUSTOM";
        public const string PARAMETER = "PARAMETER";
        public const string LANGUAGE_TYPE = "LANGUAGE_TYPE";
        //public const string SCRIPT_CODE = "SCRIPT_CODE";
        public const string STATEMENT = "STATEMENT";
        public const string ONLY_VARIABLE = "ONLY_VARIABLE";
        public const string VARIABLE = "VARIABLE";
        public const string TYPE = "TYPE";
        public const string GET_VARIABLE = "GET_VARIABLE";
        //public const string GET_VARIABLE_SHORT = "GET_VARIABLE_SHORT";
        public const string GET_DATE = "GET_DATE";
        public const string ADD_DATE = "ADD_DATE";
        public const string GET_CHAR = "GET_CHAR";
        public const string GET_ONLY_NUMBER = "GET_ONLY_NUMBER";
        public const string TO_UPPER = "TO_UPPER";
        public const string TO_LOWER = "TO_LOWER";
        public const string ROUND = "ROUND";
        public const string ROUND_UP = "ROUND_UP";
        public const string ROUND_DOWN = "ROUND_DOWN";
        public const string SUBSTRING = "SUBSTRING";
        public const string TRIM = "TRIM";
        public const string CONCAT = "CONCAT";

        public const string GET_FRAGMENT_WITH_PROPERTY = "GET_FRAGMENT_WITH_PROPERTY";
        public const string GET_VARIABLE_WITH_PROPERTY = "GET_VARIABLE_WITH_PROPERTY";
        public const string GET_VARIABLE_VALUE = "GET_VARIABLE_VALUE";
        public const string GET_VARIABLE_VALUE_STATEMENT = "GET_VARIABLE_VALUE_STATEMENT";
        public const string GET_VARIABLE_STATEMENT = "GET_VARIABLE_STATEMENT";
        public const string GET_VARIABLE_JSON_PROPERTY = "GET_VARIABLE_JSON_PROPERTY";
        public const string GET_VARIABLE_TYPE_CONVERTER = "GET_VARIABLE_TYPE_CONVERTER";
        public const string IS_ALL_UNDERSCORE = "IS_ALL_UNDERSCORE";

        //public const string GET_VARIABLE_INTO_VALUE = "GET_VARIABLE_INTO_VALUE";
        public const string GET_ALL_FUNCTION_COMMA = "GET_ALL_FUNCTION_COMMA";
        public const string GET_ALL_FUNCTION = "GET_ALL_FUNCTION";
        public const string GET_FUNCTION_ARGUMENTS = "GET_FUNCTION_ARGUMENTS";
        public const string GET_ALL_SCRIPT_LINE = "GET_ALL_SCRIPT_LINE";
        public const string GET_ALL_SCRIPT_CODE = "GET_ALL_SCRIPT_CODE";
        public const string GET_ALL_SCRIPT_FILE = "GET_ALL_SCRIPT_FILE";

        //public const string GET_VARIABLE_SHORT_VARIABLE = "get";
        public const string GET_VARIABLE_FUNCTION = "getVar"; // LUIS VERA CAMBIAR ESTO POR NOMBRE DE LA TAREA LA CUAL DEBE DE SER UNICA
        public const string GET_DATE_FUNCTION = "getDate";
        public const string ADD_DATE_FUNCTION = "addDate";
        public const string GET_CHAR_FUNCTION = "getChar";
        public const string GET_ONLY_NUMBER_FUNCTION = "getOnlyNumber";

        public const string TO_UPPER_FUNCTION = "toUpper";
        public const string TO_LOWER_FUNCTION = "toLower";
        public const string ROUND_FUNCTION = "round";
        public const string ROUND_UP_FUNCTION = "roundUp";
        public const string ROUND_DOWN_FUNCTION = "roundDown";
        public const string SUBSTRING_FUNCTION = "substring";
        public const string TRIM_FUNCTION = "trim";
        public const string CONCAT_FUNCTION = "concat";


        // LUIS VERA: faltan formatos para tomar el valor a nivel proceso->tarea->paso
        public const string PASSWORD_REGULAR_EXPRESSION = @"^(?=.*\d.*\d)(?=.*[A-Za-z\d]).{8,}";
        public const string GET_VARIABLE_REGULAR_EXPRESSION_FORMAT = @"@getVar({{0},{1}})";
        //public const string GET_VARIABLE_SHORT_REGULAR_EXPRESSION_FORMAT = @"@g({{0},{1}})";


        public const string GET_FRAGMENT_WITH_PROPERTY_EXPRESSION = @"(?<VARIABLE_PROPERTY>__[FRG0-9]+__)
                                                                (?:
                                                                (?:\.(?:\s)*__[FRG0-9]+__)
                                                                |(?:\[(?:\s)*(?:""|')*__[FRG0-9]+__(?:""|')*(?:[\s]*)[\]]{1})
                                                                )+";

        /// <summary>
        /// GLOBAL|MULTILINE|IGNORE_SPACE|RIGHT_TO_LEFT
        /// </summary>
        public const string GET_VARIABLE_WITH_PROPERTY_EXPRESSION = @"(?<VARIABLE_PROPERTY>\[@[a-zA-Z0-9\s_,]+\])
                                                                (?:
                                                                (?:\.(?:\s)*[a-zA-Z_$]+)
                                                                |(?:\[(?:\s)*(?:""|')*[a-zA-Z0-9_$]+(?:""|')*(?:[\s]*)[\]]{1})
                                                                )+";


        /// <summary>
        ///  RUN WITH OPTIONS: RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft
        /// </summary>
        public const string GET_JSON_PROPERTY_EXPRESSION = @"(?<VARIABLE>\[@[a-zA-Z0-9]+\])
                                                            (?<PROPERTY>
	                                                        (?:\s*\t*)(?:(?:\.|\[|\[\@)(?:\s*\t*)['""a-zA-Z0-9\]]+)*(?:""\]|'\]|\.|.)(?<=[\]a-zA-Z]+)
                                                            )";

        /// <summary>
        ///  RUN WITH OPTIONS: RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline
        /// </summary>
        public const string GET_VARIABLE_JSON_PROPERTY_EXPRESSION = @"(?<VARIABLE>\[@[a-zA-Z0-9]+\])
                                            (?<PROPERTY>
                                                  (?:\s*\t*)(?:\.(?:\s*\t*)(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+).*
                                                | (?:\[(?:\s*\t*)\[\@(?:\s*\t*)[a-zA-Z0-9]+(?:\s*\t*)\](?:\s*\t*)\]).*
                                                | (?:\[(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+(?:\s*\t*)(?:\s*\t*)\]).*
                                            )";


        public const string GET_VARIABLE_ALL_PROPERTY_EXPRESSION = @"(?<VARIABLE>\[@[a-zA-Z0-9]+\])
                                            (?<PROPERTY>
                                            (?:\s*\t*)(?:\.(?:\s*\t*)(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+).*
                                            | (?:\[(?:\s*\t*)\[\@(?:\s*\t*)[a-zA-Z0-9]+(?:\s*\t*)\](?:\s*\t*)\]).*
                                            | (?:\[(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+(?:\s*\t*)(?:\s*\t*)\]).*
                                            | (?:\[.*?]).*
                                            | (?:\..*?]).*
                                            )";

        /// <summary>
        ///  RUN WITH OPTIONS: IGNORE CASE, IGNORE WHITE SPACE
        /// </summary>
        //public const string GET_JSON_UNION_EXPRESSION = @"(?:^\[(?:\s*\t*)\[\@(?:\s*\t*)[a-zA-Z0-9]+(?:\s*\t*)\](?:\s*\t*)\])|(?:^\[(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+(?:\s*\t*)(?:\s*\t*)\])|(?:^\.(?:\s*\t*)(?:\s*\t*)(?:\s*\t*)[""'\-_a-zA-Z0-9]+)";
        public const string GET_JSON_UNION_EXPRESSION = @" (?<PROPERTY>\.[\w\d\s]+)
                                                        #|(?<VARIABLE>\[\@.*?\])
                                                        #|(?<VARIABLE_ARRAY>\[[\s]*\[\@.*?\][\s]*\])
                                                        |(?<PROPERTY>\[.*?\])
                                                        (?<=\]|\.)";

        public const string GET_VARIABLE_JSON_UNION_EXPRESSION = @" (?<PROPERTY>\.[\w\d\s]+)
                                                        |(?<VARIABLE>\[\@.*?\])
                                                        |(?<VARIABLE_ARRAY>\[[\s]*\[\@.*?\][\s]*\])
                                                        |(?<PROPERTY>\[.*?\])
                                                        (?<=\]|\.)";

        public const string GET_VARIABLE_JAVA_UNION_EXPRESSION = @" (?<PROPERTY>\.[\w\d\]]+)
                                                            |(?<VARIABLE>\[\@.*?\])
                                                            |(?<VARIABLE_ARRAY>\[[\s]*\[\@.*?\][\s]*\])
                                                            |(?<PROPERTY>\[.*?\])
                                                            (?<=\]|\.)";

        //public const string GET_JSON_PROPERTY_EXPRESSION = @"\[@[a-zA-Z0-9]+\]((\.[@\{\}a-zA-Z0-9\._\[\]\?#,|>'=:\s%/;]+)|(\[[@a-zA-Z0-9\{\}\._\[\]\?#,|>'=:\s%/;]+))";

        public const string GET_JSON_ARRAY_FROM_STRING = @"(""(?:[\s\n] +|)\[(?:[\s\n]+|){(?:[\s\n]+|)([\\\""\_a-zA-Z0-9]+)(?:[\s\n]+|):(?:[\s\n]+|)(.*?)(?:[\s\n]+|)}(?:[\s\n]+|)\](?:[\s\n]+|)"")";
        public const string GET_JSON_OBJECT_FROM_STRING = @"(""(?:[\s\n] +|){(?:[\s\n]+|)([\\\""\_a-zA-Z0-9]+)(?:[\s\n]+|):(?:[\s\n]+|)(.*?)(?:[\s\n]+|)}(?:[\s\n]+|)"")";
        public const string GET_OCURRENCE_PARENTHESIS_EXPRESSION = @"\((?<OCCURRENCE>[\W\S_]*?)\)(?=,)";


        //\[@(.*?)\]((?:\.[a-zA-Z]+)|(?:\[[0-9]+\]))

        // LUIS VERA: C# NO RECONOCE LOS GRUPOS DEFINIDOS EN CABEZARA ?(DEFINE)
        //public const string CHECK_STRING_IS_JSON = @"(?(DEFINE)" + "\r\n" +
        //        @"(?<number>   -? (?= [1-9]|0(?!\d) ) \d+ (\.\d+)? ([eE] [+-]? \d+)? )" + "\r\n" +
        //        @"(?<boolean>   true | false | null )" + "\r\n" +
        //        @"(?<string>    "" ([^""\n\r\t\\\\]* | \\\\ [""\\\\bfnrt\/] | \\\\ u [0-9a-f]{4} )* "" )" + "\r\n" +
        //        @"(?<array>     \[  (?:  (?&json)  (?: , (?&json)  )*  )?  \s* \] )" + "\r\n" +
        //        @"(?<pair>      \s* (?&string) \s* : (?&json)  )" + "\r\n" +
        //        @"(?<object>    \{  (?:  (?&pair)  (?: , (?&pair)  )*  )?  \s* \} )" + "\r\n" +
        //        @"(?<json>   \s* (?: (?&number) | (?&boolean) | (?&string) | (?&array) | (?&object) ) \s* )" + "\r\n" +
        //        @")" + "\r\n" +
        //        @"\A (?&json) \Z";

        public const string CHECK_STRING_IS_JSON = @"(?(DEFINE)" + "\r\n" +
                @"(?<number>   -? (?= [1-9]|0(?!\d) ) \d+ (\.\d+)? ([eE] [+-]? \d+)? )" + "\r\n" +
                @"(?<boolean>   ?\k<number> | true | false | null )" + "\r\n" +
                @"(?<string>    "" ([^""\n\r\t\\\\]* | \\\\ [""\\\\bfnrt\/] | \\\\ u [0-9a-f]{4} )* "" )" + "\r\n" +
                ")" + "\r\n" +
                @"\k<boolean>";


        /// <summary>
        /// REPLACE [@OPEN_SYMBOL] AND [@CLOSE_SYMBOL] WITH { AND } RESPECTIVELY OR YOUR SYMBOL
        /// not preceded by {
        /// @ or @@@
        /// 
        /// HERE START
        /// opening {
        /// non-backtracking atomic group
        /// non-backtracking atomic group
        /// literals, spaces, etc
        /// double quoted string with escapes
        /// single quoted string with escapes
        /// open += 1
        /// open -= 1, only if open > 0 (balancing group)
        /// fail if open > 0
        /// final }
        /// </summary>
        public const string GET_STATEMENT_BY_SYMBOL = @"\[@OPEN_SYMBOL]                        
                                                (?<STATEMENT>							
                                                (?>                         
                                                  (?>                       
                                                    [^""'\[@OPEN_SYMBOL]\[@CLOSE_SYMBOL]]+                
                                                  | "" (?: [^""\\]+ | \\. )* ""
                                                  | ' (?: [^'\\]+ | \\. )* '
                                                  | (?<OPEN>       \[@OPEN_SYMBOL] )     
                                                  | (?<PARTIAL_STATEMENT-OPEN> \[@CLOSE_SYMBOL] )     
                                                  )*                        
                                                )							
                                                (?(OPEN) (?!) )
                                                )
                                                \[@CLOSE_SYMBOL]";

        public const string JSON_STRUCTURE_VALIDATE_EXPRESSION = @"\{(?:\r|\n|\t|\s*)(?<PROPERTY>(?:""|') (?:[^""\n\r\t\\\\]* | \\\\ [""\\\\bfnrt\/] | \\\\ u [0-9a-f]{4} )* (?:""|'))(?:\r|\n|\t|\s*)(?:\:)(?<VALUE>(?:[@\{\}\(\)\n\r\t\s*""a-zA-Z0-9\W\S_]+))\}";

        //public const string GET_VARIABLE_CONVERTER_CODE_REGULAR_EXPRESSION = @"@(?<TYPE>[a-zA-Z_]+)(?:[\s]+|)(?<CONTENT>" + GET_STATEMENT_BY_SYMBOL + @")";
        public const string GET_VARIABLE_CONVERTER_CODE_REGULAR_EXPRESSION = @$"@(?<TYPE>{CONVERTER_TYPE_ENUM_VARIABLE})(?:[\s]+|)(?<CONTENT>" + GET_STATEMENT_BY_SYMBOL + @")";
        public const string GET_VARIABLE_RESULT_CONVERTER_CODE_REGULAR_EXPRESSION = @$"@(?<TYPE>{RESULT_TYPE_ENUM_VARIABLE})(?:[\s]+|)(?<CONTENT>" + GET_STATEMENT_BY_SYMBOL + @")";
        public const string GET_STATEMENT_REGULAR_EXPRESSION = @"\[" + GET_STATEMENT_BY_SYMBOL + @"\]";
        public const string GET_JSON_ARRAY_PROPERTY_EXPRESSION = @"[a-zA-Z]+(\[[0-9]+\])";
        public const string GET_JSON_ARRAY_VALUE_PROPERTY_EXPRESSION = @"(\[([0-9]+)\])";
        public const string GET_VARIABLE_VALUE_REGULAR_EXPRESSION = @"\[@(?:\s*\b([a-zA-Z_][a-zA-Z0-9_, ]+))\]";
        public const string GET_VARIABLE_STATEMENT_REGULAR_EXPRESSION = @"\[@(?:(?:\s*)(?<VARIABLE>(?:[a-zA-Z0-9_](?!\#)|\#(?![a-zA-Z0-9_]))*?))\](?:[\n\s\t]*)" + STATEMENT_REGULAR_VARIABLE;
        public const string GET_VARIABLE_TYPE_CONVERTER_REGULAR_EXPRESSION = @"@(?<TYPE>[a-zA-Z_]+)(?:[\s]+|)\[(?:[\s]+|)(?<VARIABLE>\[@(?:\s*\b(?<VARIABLE_NAME>[a-zA-Z_][a-zA-Z0-9_, ]+))\])(?:[\s]+|)\]";

        public const string IS_ALL_UNDERSCORE_EXPRESSION = @"\[@(?:\s*\b([_]+)\s*)\]";

        //public const string GET_VARIABLE_STATEMENT_REGULAR_EXPRESSION = @"\[@(?:\s*\b(?<VARIABLE>[a-zA-Z][a-zA-Z0-9].*?))\](?:[\n\s\t]*)\[@(?:[\n\s\t]*)\{(?:[\n\s\t]*)(?<STATEMENT>.*?)(?:[\n\s\t]*)\}(?:[\n\s\t]*)\]$";
        public const string GET_VARIABLE_VALUE_REGULAR_EXTENDED_EXPRESSION = @"""(?:[\s\r]+|)\[@(?:\s*\b([a-zA-Z][a-zA-Z0-9].*?))\](?:[\s\r]+|)""";
        public const string GET_VARIABLE_WITH_ARGUMENT = @"(?<VARIABLE>\[@(?:\s*\b(?<VARIABLE_NAME>[a-zA-Z][a-zA-Z0-9].*?))\])";
        public const string GET_VALUE_BETWEEN_QUOTES = @"(?<STRING>([""'])(?<VALUE>[\S\s]*?|.*?[^\\])\1)";
        //public const string GET_VALUE_BETWEEN_QUOTES_WITH_SLASH = @"[^\\]?""(.*?[^\\])""|[^\\]?\'(.*?[^\\])\'";
        public const string GET_VALUE_BETWEEN_QUOTES_WITH_SLASH = @"""(?:[^""\\]|\\.)*""|'(?:[^'\\]|\\.)*'";
        public const string GET_FRAGMENT_BETWEEN_QUOTES_WITH_SLASH = @"""(?:[^""\\]|\\.)*(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)(.){0}(?:[^""\\]|\\.)*""|'(?:[^'\\]|\\.)*(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)(.){0}(?:[^'\\]|\\.)*'";

        // TO NEED REPLACE THE VARIABLES @OPEN_SYMBOL WITH { AND @CLOSE_SYMBOL WITH }
        public const string GET_LAST_VARIABLE_BETWEEN_DOUBLE_KEY_BRECKETS_EXPRESSION = @"{" + GET_STATEMENT_BY_SYMBOL + @"}";

        //RIGHT OPTION | IGNORE SPACE | SINGLE_LINE
        public const string GET_LAST_VARIABLE_BETWEEN_QUOTES_EXPRESSION = @"(?<MATCH>(?>"")(?<INIT>(?:.*?)(?:[\s\r]+|))\[@(?:\s*\b(?<VARIABLE>[a-zA-Z][a-zA-Z0-9].*?))\](?<END>(?:[\s\r]+|)(?:.*?))(?<=""))(?:[\s\t]+)(?:\,|\})";


        public const string GET_VARIABLE_VALUE_AND_TYPE_REGULAR_EXPRESSION = @"\[@(?:\s*\b(?<ONLY_VARIABLE>[a-zA-Z\_]+[a-zA-Z0-9\_]+))(?:\s*)\]|\[@(?:\s*\b(?<VARIABLE>[a-zA-Z\_]+[a-zA-Z0-9\_]+)(?:(?:\s*)[\,](?:\s*))(?<TYPE>[a-zA-Z][a-zA-Z0-9]))(?:\s*)\]";

        //public const string GET_VARIABLE_INTO_VALUE_REGULAR_EXPRESSION = @"\[\[@(?:\s*\b([a-zA-Z][a-zA-Z0-9].*?))\]\]";
        //public const string GET_VARIABLE_VALUE_REGULAR_EXPRESSION = @"\[@(\s*\b([a-zA-Z][a-zA-Z0-9].*?))\]";

        #region OBSOLETE FUNCTIONS
        //public const string GET_VARIABLE_REGULAR_FUNCTION_EXPRESSION = @"@(getVar)(\s*\n*\(\s*\n*)([0-9]+\s*\,\s*[0-9]+)(\s*\n*)\)";
        ////public const string GET_VARIABLE_SHORT_FUNCTION_REGULAR_EXPRESSION = @"@(g)(\s*\n*\(\s*\n*)([0-9]+\s*\,\s*[0-9]+)(\s*\n*)\)";
        //public const string GET_DATE_FUNCTION_REGULAR_EXPRESSION = @"@(getDate)(\s*\n*\(\s*\n*)([a-zA-Z\-\/\""\s*]+)\)";
        //public const string ADD_DATE_FUNCTION_REGULAR_EXPRESSION = @"@(addDate)(\s*\n*\(\s*\n*)([a-zA-Z\-\/\""\,\s*]+)\)";
        //public const string GET_CHAR_FUNCTION_REGULAR_EXPRESSION = @"@(getChar)(\s*\n*\(\s*\n*)(.*?\s*\,\s*[0-9]+)(\s*\n*)\)";
        //public const string GET_ONLY_NUMBER_FUNCTION_REGULAR_EXPRESSION = @"@(getOnlyNumber)(\s*\n*\(\s*\n*)(.*?\s*\n*)\)";
        #endregion OBSOLETE FUNCTIONS

        public const string GET_VARIABLE_REGULAR_FUNCTION_EXPRESSION = "INTEGER,TEXT"; //@(getVar)(?:\s*\n*\(\s*\n*)([0-9]+)(?:\s*\n*\,\s*\n*)([0-9]+)(?:\s*\n*)(?:\s*\n*\))";
        public const string GET_VARIABLE_SHORT_FUNCTION_REGULAR_EXPRESSION = "INTEGER,TEXT"; //@"@(g)(?:\s*\n*\(\s*\n*)([0-9]+)(?:\s*\n*\,\s*\n*)([0-9]+)(?:\s*\n*)(?:\s*\n*\))";
        public const string GET_DATE_FUNCTION_REGULAR_EXPRESSION = "DATE";
        public const string ADD_DATE_FUNCTION_REGULAR_EXPRESSION = "TEXT,INTEGER,DATE,DATE";
        public const string GET_CHAR_FUNCTION_REGULAR_EXPRESSION = "TEXT,INTEGER";
        public const string GET_ONLY_NUMBER_FUNCTION_REGULAR_EXPRESSION = "TEXT";
        public const string ROUND_FUNCTION_REGULAR_EXPRESSION = "FLOAT";
        public const string ROUND_UP_FUNCTION_REGULAR_EXPRESSION = "FLOAT";
        public const string ROUND_DOWN_FUNCTION_REGULAR_EXPRESSION = "FLOAT";
        public const string SUBSTRING_FUNCTION_REGULAR_EXPRESSION = "TEXT";
        public const string TRIM_FUNCTION_REGULAR_EXPRESSION = "TEXT";
        public const string TO_UPPER_FUNCTION_REGULAR_EXPRESSION = "TEXT";
        public const string TO_LOWER_FUNCTION_REGULAR_EXPRESSION = "TEXT";
        public const string CONCAT_FUNCTION_REGULAR_EXPRESSION = "TEXT,TEXT";

        //public const string GET_ALL_FUNCTION_EXPRESSION = @"@([0-9a-zA-Z]+\s*|\n*)\:(\s*\n*)\{(\s*\n*)(((?![\{\}]).(\s*\n*))*?)(\s*\n*)\}";
        public const string GET_ALL_FUNCTION_COMMA_EXPRESSION = "@([0-9a-zA-Z]+)\\s*\\n*\\(\\s*\\n*([\"'])(.*?)\\2\\s*\\n*\\)";
        public const string GET_ALL_FUNCTION_EXPRESSION = @"@([0-9a-zA-Z]+\s*|\n*)\((\s*\n*)(((?!(@[0-9a-zA-Z])).(\s*\n*))*?\s*\n*)\)";
        //public const string GET_FUNCTION_ARGUMENTS_EXPRESSION = "\"[^\"\\\\]*(?:\\.[^\"\\\\]*)*\"|[^,]+|(?<=^|,)(?=$|,)";
        public const string GET_FUNCTION_ARGUMENTS_EXPRESSION = @"""[^""\\]*(?:\\.[^""\\]*)*""|[^,]+|(?<=^|,)(?=$|,)";

        public const string GET_ALL_C_FUNCTION_EXPRESSION = @"@([0-9a-zA-Z]+)\((((?![\(\)]).)*?)\)";
        public const string GET_BETWEEN_QUOTE_EXPRESSION = @"([""'])(?<VALUE>[^""']*(?:(?!'\1)[""'][^""']*)*)\1";
        public const string GET_BETWEEN_COMMA_EXPRESSION = "(?<PARAMETER>.+?)(?:,|$)";
        // (@concat|)(?:\(|)(.+?)(?:,|\))

        public const string GET_BETWEEN_DEEP_KEY_BRECKETS_EXPRESSION = @"\{(?>[^{}]+(|\{(?<DEPTH>)|\}(?<-DEPTH>))|(?<=\\).)*(?(DEPTH)(?!))\}";
        public const string GET_BETWEEN_SQUARE_BRACKETS_EXPRESSION = @"\[((?>[^][]+|(?<o>)\[|(?<-o>]))*)]";
        public const string GET_BETWEEN_KEY_BRACKETS_EXPRESSION = @"\{((?>[^}{]+|(?<o>)\{|(?<-o>}))*)}";

        // LUIS VERA: ESTE NO FUNCIONA PORQUE FALTA VERIFICAR CUANDO VIENEN COMAS POR SER MAS DE UN OBJETO JSON
        public const string GET_BETWEEN_SQUARE_KEY_BRACKETS_EXPRESSION = @"\[(?:[\s]+|)\{((?>[^][{}]+|(?<o>)\[\{|(?<-o>\}]))*)\}(?:[\s]+|)]";

        public const string VALID_IS_JSON_PCRE_EXPRESSION = "(?(DEFINE)" +
        @"(?<ws>      [\t\n\r ]* )" +
        @"(?<number>  -? (?: 0|[1-9]\d*) (?: \.\d+)? (?: [Ee] [+-]? \d++)? )" +
        @"(?<boolean> true | false | null )" +
        //@"(?<string>  ""(?: [^\\\\""\x00-\x1f] | \\\\ [""\\\\bfnrt\/] | \\\\ u [0-9A-Fa-f]{4} )* "" )" +
        //@"(?<stringlite>  ' (?: [^\\\\'\x00-\x1f] | \\\\ ['\\\\bfnrt\/] | \\\\ u [0-9A-Fa-f]{4} )* ' )" +
        @"(?<string>  ""(?: [^\\""\x00-\x1f] | \\ [""\\bfnrt\/] | \\ u [0-9A-Fa-f]{4} )* "" )" +
        @"(?<stringlite>  ' (?: [^\\'\x00-\x1f] | \\ ['\\bfnrt\/] | \\ u [0-9A-Fa-f]{4} )* ' )" +
        @"(?<pair>    (?&ws) ((?&string) | (?&stringlite)) (?&ws) : (?&value) )" +
        @"(?<array>   \[ (?: (?&value) (?: , (?&value) )* )? (?&ws) \] )" +
        @"(?<object>  \{ (?: (?&pair) (?: , (?&pair) )* )? (?&ws) \} )" +
        @"(?<value>   (?&ws) (?: (?&number) | (?&boolean) | (?&string) | (?&stringlite) | (?&array) | (?&object) ) (?&ws) )" +
        @")" +
        @" (?&value) ";
        //@"\A (?&value) \Z";

        public const string VALID_IS_LITERAL_JSON_ARRAY_PCRE_EXPRESSION = "(?(DEFINE)" +
        @"(?<ws>      [\t\n\r ]* )" +
        @"(?<number>  -? (?: 0|[1-9]\d*) (?: \.\d+)? (?: [Ee] [+-]? \d++)? )" +
        @"(?<boolean> true | false | null )" +
        @"(?<string>  ""(?: [^\\""\x00-\x1f] | \\ [""\\bfnrt\/] | \\ u [0-9A-Fa-f]{4} )* "" )" +
        @"(?<stringlite>  ' (?: [^\\'\x00-\x1f] | \\ ['\\bfnrt\/] | \\ u [0-9A-Fa-f]{4} )* ' )" +
        @"(?<pair>    (?&ws) ((?&string) | (?&stringlite)) (?&ws) : (?&value) )" +
        @"(?<array>   \[ (?: (?&value) (?: , (?&value) )* )? (?&ws) \] )" +
        @"(?<value>   (?&ws) (?: (?&number) | (?&boolean) | (?&string) | (?&stringlite) | (?&array) ) (?&ws) )" +
        @")" +
        @" \A (?&value) \Z";
        //@"\A (?&value) \Z";

        //    public const string VALID_IS_JSON_PCRE_EXPRESSION = @"""
        //(?(DEFINE)
        //    # An object is an unordered set of name/value pairs.
        //    (?<object> \{
        //        (?: (?&keyvalue) (?: , (?&keyvalue) )* )?
        //    (?&ws) \} )
        //    (?<keyvalue>
        //        (?&ws) (?&string) (?&ws) : (?&value)
        //    )

        //    # An array is an ordered collection of values.
        //    (?<array> \[
        //        (?: (?&value) (?: , (?&value) )* )?
        //    (?&ws) \] )

        //    # A value can be a string in double quotes, or a number,
        //    # or true or false or null, or an object or an array.
        //    (?<value> (?&ws)
        //        (?: (?&string) | (?&number) | (?&object) | (?&array) | true | false | null )
        //    )

        //    # A string is a sequence of zero or more Unicode characters,
        //    # wrapped in double quotes, using backslash escapes.
        //    (?<string>
        //        "" (?: [^""\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ [""\\/bfnrt] )* ""
        //        # \p{Cc} matches control characters
        //    )

        //    # A number is very much like a C or Java number, except that the octal
        //    # and hexadecimal formats are not used.
        //    (?<number>
        //        -? (?: 0 | [1-9][0-9]* ) (?: \. [0-9]+ )? (?: [Ee] [-+]? [0-9]+ )?
        //    )

        //    # Whitespace
        //    (?<ws> \s*+ )
        //)

        //\A (?&ws) (?&object) (?&ws) \z
        //""";


        public const string GET_BETWEEN_PARENTHESIS_EXPRESSION = @"\((?<VALUE>(?>[^)(]+|(?<o>)\(|(?<-o>\)))*)\)";
        public const string GET_CODE_BETWEEN_SQUARE_BRACKETS_EXPRESSION = @"\[(?<CODE>(?>[^][]+|(?<o>)\[|(?<-o>]))*)]";
        public const string GET_ALL_FUNCTION_ARGUMENTS_EXPRESSION = @"\(\s*([^)]+?)\s*\)";
        public const string GET_FRAGMENT_BETWEEN_SYMBOL_EXPRESSION = @"(?<QUOTES_ESCAPE>(?<STRING_FRAGMENT_TYPE>""(?:[^""\\]|\\.)*(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)(.){0}(?:[^""\\]|\\.)*"")|(?<STRING_FRAGMENT_TYPE>'(?:[^'\\]|\\.)*(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)(.){0}(?:[^'\\]|\\.)*'))
                                                                |(?<INVALID_STRING>""(?:[^""\\]|\\.)*"")|(?<INVALID_STRING>'(?:[^'\\]|\\.)*')
                                                                |(?<QUOTES>(?<STRING_FRAGMENT_TYPE>(?:""(?:\s*\b(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)\s*)"")|(?:'(?:\s*\b(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)\s*)')))
                                                                |(?<SQUARE_BRACKETS>(?<STRING_FRAGMENT_TYPE>\[(?:\s*\b(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)\s*)\]))
                                                                |(?<KEY_BRACKETS>(?<STRING_FRAGMENT_TYPE>\{(?:\s*\b(?<FRAGMENT_NAME>__FRG(?<ID>[0-9]+)__)\s*)\}))";

        //[@LANGUAGE_SOURCE_ENUM] REPLACE WITH = SQL|CSHARP|PYTHON|CPP|""SQL""
        public const string OPEN_SYMBOL_VARIABLE = "[@OPEN_SYMBOL]";
        public const string CLOSE_SYMBOL_VARIABLE = "[@CLOSE_SYMBOL]";
        public const string STATEMENT_REGULAR_VARIABLE = "[@STATEMENT_REGULAR]";
        public const string LANGUAGE_SOURCE_ENUM_VARIABLE = "[@LANGUAGE_SOURCE_ENUM]";
        public const string CONVERTER_TYPE_ENUM_VARIABLE = "[@CONVERTER_TYPE_ENUM]";
        public const string RESULT_TYPE_ENUM_VARIABLE = "[@RESULT_TYPE_ENUM]";

        //public const string GET_STRING_TREATMENT_EXPRESSION = @"\@(?:[\n\s\t]*)\{(?:[\n\s\t]*)(.*?)(?:[\n\s\t]*)\}$";
        public const string GET_STRING_TREATMENT_EXPRESSION = @"\[@(?:[\n\s\t]*)\{(?:[\n\s\t]*)(.*?)(?:[\n\s\t]*)\}(?:[\n\s\t]*)\]$";
        //public const string GET_OPTION_STRING_TREATMENT_EXPRESSION = @"(?<STATEMENT>.)\[(?<CODE>.*?)\](?<SEPARATOR>[\-!$%^&*()_+|~=`:"";'<>?,.\/\s\t]*)(?=.\[)|(?<STATEMENT>.)\[(?<CODE>.*?)\](?<SEPARATOR>[\-!$%^&*()_+|~=`:"";'<>?,.\/\s\t]*$)";
        public const string GET_OPTION_STRING_TREATMENT_EXPRESSION = @"(?<STATEMENT>.)\[(?<CODE>[\S\s]*?)\](?<SEPARATOR>[\-!$%^&*()_+|~=`:"";'<>?,.\/\s\t]*)(?=.\[)|(?<STATEMENT>.)\[(?<CODE>[\S\s]*?)\](?<SEPARATOR>[\-!$%^&*()_+|~=`:"";'<>?,.\/\s\t]*$)";

        public const string FUNCTION_STATEMENT_EXPRESSION = @"(?<STATEMENT>\@f)((?:[\n\s\t]*\[(?:[\n\s\t]*)(?<CODE>.*?)(?:[\n\s\t]*)\])((?<SEPARATOR>.|)))";
        //public const string FUNCTION_STATEMENT_EXPRESSION = @"(?<STATEMENT>\@[fF])((?:[\n\s\t]*\[(?:[\n\s\t]*)(?<CODE>.*?)(?:[\n\s\t]*)\])((?<SEPARATOR>.|)))";
        public const string SPLIT_BY_COMMA_EXPRESSION = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)(?=(?:[^']*'[^']*')*[^']*$)";
        //public const string GET_ALL_SCRIPT_LINE_EXPRESSION = @$"@ScriptLine(?:\s*|\n*)\((?:\s*\n*)({LANGUAGE_SOURCE_ENUM_VARIABLE})(?:\s*\n*),(?:\s*\n*){{((?:\s|\n|.)*?)}}(?:\s*\n*)\)";
        //public const string GET_ALL_SCRIPT_CODE_EXPRESSION = @$"@ScriptCode(?:\s*|\n*)\((?:\s*\n*)({LANGUAGE_SOURCE_ENUM_VARIABLE})(?:\s*\n*),(?:\s*\n*){{((?:\s|\n|.)*?)}}(?:\s*\n*)\)";
        //public const string GET_ALL_SCRIPT_LINE_EXPRESSION = @$"@ScriptLine(?:\s*|\n*)\[(?:\s*\n*)({LANGUAGE_SOURCE_ENUM_VARIABLE})(?:\s*\n*),(?:\s*\n*){{((?:\s|\n|.)*?)}}(?:\s*\n*)\]";
        public const string GET_ALL_SCRIPT_LINE_EXPRESSION = @$"@ScriptLine(?:\s*|\n*)\[(?:\s*\n*)(?<LANGUAGE_TYPE>{LANGUAGE_SOURCE_ENUM_VARIABLE})(?:[\s\r\n\t ]+|),(?:[\s\r\n\t ]+|){GET_STATEMENT_BY_SYMBOL}(?:\s*\n*)\]";

        //public const string GET_ALL_SCRIPT_CODE_EXPRESSION = @$"@ScriptCode(?:\s*|\n*)\[(?:\s*\n*)({LANGUAGE_SOURCE_ENUM_VARIABLE})(?:\s*\n*),(?:\s*\n*){{((?:\s|\n|.)*?)}}(?:\s*\n*)\]";
        public const string GET_ALL_SCRIPT_CODE_EXPRESSION = @$"\[\$(?:\s*\n*)(?<LANGUAGE_TYPE>{LANGUAGE_SOURCE_ENUM_VARIABLE})(?:[\s\r\n\t ]+|),(?:[\s\r\n\t ]+|){GET_STATEMENT_BY_SYMBOL}(?:\s*\n*)\]";
        public const string GET_ALL_SCRIPT_FILE_EXPRESSION = @$"\[\$(?:\s*\n*)(?<LANGUAGE_TYPE>{LANGUAGE_SOURCE_ENUM_VARIABLE})(?:[\s\r\n\t ]+|),(?:[\s\r\n\t ]+|)(?<STATEMENT>[ :0-9a-zA-Z\._\\/]+)(?:\s*\n*)\]";

        // (.*[^,\s\t])
        // (?<=") El ultimo caracter sea "
        // (.*[^,\s\t\n]) Que no contenga el ultimo caracter coma, espacio, tabulacion, salto de linea
        #region REGULAR EXPRESSION FOR FUNCTION FORMAT
        public const string QUOTE_PARAMETER = @"(?:"")";
        public const string FORMAT_STRING_PARAMETER = @"(?<OCURRENCE>{(?<FORMAT>(?:(?<LEFT>[\d])+|(?:(?<LEFT>[\d]+)\:(?<RIGHT>.*))))\})";
        public const string FUNCTION_FORMAT = @"@({0})(?:\s*\n*\(\s*\n*){1}(?:\s*\n*)(?:\s*\n*\))";
        //public const string FUNCTION_FORMAT = @"(?:@({0}))(?:\s*\n*\(\s*\n*|){1}(?:\s*\n*)(?:\s*\n*|\))";
        public const string UNION_PARAMETER = @"(?:\s*\n*\,\s*\n*)";
        public const string EMAIL_PARAMETER = @"(?:"")(?:([\w\.\-]+\@[\w\-]+\.\w{2,5})+)(?:"")";
        public const string EMAIL_FORMAT_EXPRESSION = @":(?<User>[a-zA-Z0-9_.-]+)@(?<Domain>[a-zA-Z0-9]+\.[a-zA-Z]+$))";
        public const string TEXT_PARAMETER = @"((?:(?:"")(?:.*?)(?:""))|(?:[^""].*[^,\s\t\n\""]))";        
        public const string TEXT_INTEGER_PARAMETER = @"((?:"")(?:[a-zA-Z0-9\\s]+)(?:"")|(?:[a-zA-Z0-9\\s]+))";
        public const string MULTIPLE_TEXT_PARAMETER = GET_BETWEEN_COMMA_EXPRESSION;
        public const string INTEGER_PARAMETER = @"([0-9]+)";
        public const string FLOAT_PARAMETER = @"([0-9\.]+)";
        public const string DATE_PARAMETER = @"(?:"")((?:(?:[a-zA-Z0-9]{1,4})(?:\/)(?:[a-zA-Z0-9]{1,4})(?:\/)(?:[a-zA-Z0-9]{1,4}))|(?:(?:[a-zA-Z0-9]{1,4})(?:\.)(?:[a-zA-Z0-9]{1,4})(?:\.)(?:[a-zA-Z0-9]{1,4}))|(?:(?:[a-zA-Z0-9]{1,4})(?:\-)(?:[a-zA-Z0-9]{1,4})(?:\-)(?:[a-zA-Z0-9]{1,4})))(?:"")";
        #endregion REGULAR EXPRESSION FOR FUNCTION FORMAT
    }
}
