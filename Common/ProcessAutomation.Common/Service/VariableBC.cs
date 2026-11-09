using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Contract;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Common.Service
{
    public class VariableBC: IVariableBC, IVariableReadOnlyBC
    {
        private object addVariableLock = new object();        
        //private string getAllScriptLineExpression = null;
        private string getAllScriptCodeExpression = null;
        private string getAllScriptFileExpression = null;
        private string getConverterCodeExpression = null;
        private string getResultConverterCodeExpression = null;
        private string getStatementRegularExpression = null;
        private string getVariableStatementRegularExpression = null;
        private VariableStatementBC variableStatementBC = null;
        private IProcessBC processBC = null;

        private List<ValueInfo> valueListProperty = null;
        private List<ValueInfo> valueList
        {
            get
            {
                if (valueListProperty == null)
                {
                    valueListProperty = new List<ValueInfo>();
                }

                return valueListProperty;
            }
            set
            {
                valueListProperty = value;
            }
        }

        private Dictionary<int, string> fragmentResultDictionary;

        public VariableBC(IProcessBC processBC)
        {
            this.processBC = processBC;
            variableStatementBC = new VariableStatementBC(processBC);
            string languageSource = string.Join("|", Enum.GetNames(typeof(LanguageSourceEnum)).ToList());
            string converterType = string.Join("|", Enum.GetNames(typeof(ConverterTypeEnum)).ToList());
            string resultType = string.Join("|", Enum.GetNames(typeof(ResultConverterTypeEnum)).ToList());
            fragmentResultDictionary = new Dictionary<int, string>();

            //getAllScriptLineExpression = FunctionConstants.GET_ALL_SCRIPT_LINE_EXPRESSION.Replace(FunctionConstants.LANGUAGE_SOURCE_ENUM_VARIABLE, languageSource).Replace(FunctionConstants.OPEN_SYMBOL_VARIABLE, "{").Replace(FunctionConstants.CLOSE_SYMBOL_VARIABLE, "}");
            getAllScriptCodeExpression = FunctionConstants.GET_ALL_SCRIPT_CODE_EXPRESSION.Replace(FunctionConstants.LANGUAGE_SOURCE_ENUM_VARIABLE, languageSource).Replace(FunctionConstants.OPEN_SYMBOL_VARIABLE, "{").Replace(FunctionConstants.CLOSE_SYMBOL_VARIABLE, "}");
            getAllScriptFileExpression = FunctionConstants.GET_ALL_SCRIPT_FILE_EXPRESSION.Replace(FunctionConstants.LANGUAGE_SOURCE_ENUM_VARIABLE, languageSource);
            getStatementRegularExpression = FunctionConstants.GET_STATEMENT_REGULAR_EXPRESSION.Replace(FunctionConstants.OPEN_SYMBOL_VARIABLE, "{").Replace(FunctionConstants.CLOSE_SYMBOL_VARIABLE, "}");
            getVariableStatementRegularExpression = FunctionConstants.GET_VARIABLE_STATEMENT_REGULAR_EXPRESSION.Replace(FunctionConstants.STATEMENT_REGULAR_VARIABLE, getStatementRegularExpression);
            getConverterCodeExpression = FunctionConstants.GET_VARIABLE_CONVERTER_CODE_REGULAR_EXPRESSION.Replace(FunctionConstants.CONVERTER_TYPE_ENUM_VARIABLE, converterType)
                .Replace(FunctionConstants.OPEN_SYMBOL_VARIABLE, "[").Replace(FunctionConstants.CLOSE_SYMBOL_VARIABLE, "]");
            getResultConverterCodeExpression = FunctionConstants.GET_VARIABLE_RESULT_CONVERTER_CODE_REGULAR_EXPRESSION.Replace(FunctionConstants.RESULT_TYPE_ENUM_VARIABLE, resultType)
                .Replace(FunctionConstants.OPEN_SYMBOL_VARIABLE, "[").Replace(FunctionConstants.CLOSE_SYMBOL_VARIABLE, "]");
        }

        private void ReplaceVariableValue(ref dynamic value, Match matchVariable)
        { 
        }

        private FragmentList ProcessFragment(dynamic value, Dictionary<string, VariableRegexTypeInfo> variableDictionary, bool useDefaultVariablesRegEx = true)
        {
            MatchCollection matchList = null;
            //Dictionary<string, object> valueDictionary = null;
            FragmentList fragmentList = new FragmentList();           
            bool valueIsJson = StringIsJson(value);
            dynamic valueTrim = value;
            //bool inStringValue = false;
            
            if (useDefaultVariablesRegEx)
            {
                variableDictionary = new Dictionary<string, VariableRegexTypeInfo>
                {
                    {FunctionConstants.GET_FRAGMENT_WITH_PROPERTY, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_FRAGMENT_WITH_PROPERTY_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE_PROPERTY } },
                    {FunctionConstants.GET_VARIABLE_WITH_PROPERTY, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_VARIABLE_WITH_PROPERTY_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE_PROPERTY } },                
                    {FunctionConstants.GET_VARIABLE_VALUE_STATEMENT, new VariableRegexTypeInfo{ Regex = getVariableStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VALUE_STATEMENT } },
                    {FunctionConstants.GET_VARIABLE_STATEMENT, new VariableRegexTypeInfo{ Regex = getStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE_STATEMENT } },                
                    {FunctionConstants.GET_VARIABLE_VALUE, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_VARIABLE_VALUE_REGULAR_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE } },
                    //{FunctionConstants.GET_VARIABLE_TYPE_CONVERTER_REGULAR_EXPRESSION, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_VARIABLE_TYPE_CONVERTER_REGULAR_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_CONVERTER } },
                    //{FunctionConstants.GET_ALL_SCRIPT_LINE, new VariableRegexTypeInfo{ Regex = getAllScriptLineExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                    {FunctionConstants.GET_ALL_SCRIPT_FILE, new VariableRegexTypeInfo{ Regex = getAllScriptFileExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                    {FunctionConstants.GET_ALL_SCRIPT_CODE, new VariableRegexTypeInfo{ Regex = getAllScriptCodeExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } }
                };
            }

            bool hasMatches = false;
            bool hasVariableMatch = false;
            int fragmentCount = 0;
            int positionDisplacement = 0;

        RUN_VARIABLES:
            
            const string FRAGMENT_NAME_FORMAT = "__FRG{0}__";
            const string GET_FRAGMENT_NAME_REGULAR_EXPRESSION = @"__FRG(?<ID>[0-9]+)__";
            const string GET_FRAGMENT_NAME_ENCAPSULATED_REGULAR_EXPRESSION = @"\[@(?:\s*\b(__FRG(?<ID>[0-9]+)__)\s*)\]";            
            hasMatches = false;
            
            foreach (KeyValuePair<string, VariableRegexTypeInfo> variable in variableDictionary)
            {
                if (value != null && value.GetType() == typeof(string))
                {
                RUN_THIS_VARIABLE:
                    hasVariableMatch = false;
                    matchList = Regex.Matches(value, variable.Value.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                    if (matchList != null && matchList.Count > 0)
                    {
                        hasVariableMatch = true;
                        hasMatches = true;
                        foreach (Match match in matchList.OrderByDescending(o => o.Index))
                        {
                            positionDisplacement = 0;
                            fragmentCount++;

                            dynamic matchFragment = value.ToString().Substring(match.Index, match.Value.Length);
                            if (string.IsNullOrWhiteSpace(matchFragment))
                            {
                                continue;
                            }                            

                            FragmentInfo fragment = null;
                            //if (variable.Key == FunctionConstants.GET_VARIABLE_WITH_PROPERTY
                            //    || variable.Key == FunctionConstants.GET_VARIABLE_VALUE)
                            {
                                MatchCollection fragmentNameMatchCollection = Regex.Matches(matchFragment, GET_FRAGMENT_NAME_REGULAR_EXPRESSION, RegexOptions.RightToLeft);
                                Match fragmentNameMatch = fragmentNameMatchCollection != null && fragmentNameMatchCollection.Any() ? fragmentNameMatchCollection[0] : null;
                                string fragmentName = null;

                                if (fragmentNameMatch != null && fragmentNameMatch.Success)
                                {
                                    fragmentCount--;
                                    fragmentName = fragmentNameMatch.Value;

                                    fragment = fragmentList.FirstOrDefault(f => f.FragmentName == fragmentName);

                                    int minIndex = positionDisplacement = fragmentNameMatchCollection.Min(f => f.Index);
                                    string matchFragmentAux = matchFragment;
                                    FragmentInfo fragmentInternal = null;

                                    foreach (Match fragmentNameMatchInternal in fragmentNameMatchCollection.OrderByDescending(i => i.Index))
                                    {
                                        fragmentInternal = fragmentList.FirstOrDefault(f => f.FragmentName == fragmentNameMatchInternal.Value);
                                        matchFragmentAux = Utilities.ReplaceAt(matchFragmentAux, fragmentNameMatchInternal.Index, fragmentNameMatchInternal.Length, fragmentInternal.OriginalFragment);
                                        fragment.OriginalFragment = matchFragmentAux;

                                        if (fragment != fragmentInternal)
                                        {
                                            if (variable.Key == FunctionConstants.GET_FRAGMENT_WITH_PROPERTY)
                                            {
                                                minIndex = minIndex == 0 || minIndex > fragmentInternal.StartPosition ? fragmentInternal.StartPosition : minIndex;
                                            }
                                            else
                                            {
                                                minIndex = fragment.StartPosition - fragmentInternal.StartPosition + minIndex;
                                                positionDisplacement = minIndex;
                                            }

                                            fragmentList.Remove(fragmentInternal);
                                        }
                                    }

                                    fragment.Fragment = matchFragment;

                                    if (variable.Key == FunctionConstants.GET_FRAGMENT_WITH_PROPERTY)
                                    {
                                        fragment.StartPosition = minIndex;
                                    }
                                    else
                                    {
                                        fragment.StartPosition -= minIndex;
                                    }

                                    fragment.Length = fragment.OriginalFragment.Length;
                                    fragment.EndPosition = fragment.StartPosition + fragment.OriginalFragment.Length;
                                    fragment.LengthFragment = fragment.FragmentName.Length;

                                    if (variable.Key == FunctionConstants.GET_FRAGMENT_WITH_PROPERTY)
                                    {
                                        fragment.StartPositionFragment = match.Index;
                                    }
                                    else
                                    {
                                        fragment.StartPositionFragment = match.Index + minIndex;
                                    }

                                    //fragment.IsEncapsulated = Regex.IsMatch(fragment.Fragment, GET_FRAGMENT_NAME_ENCAPSULATED_REGULAR_EXPRESSION);

                                    if (Regex.IsMatch(fragment.Fragment, GET_FRAGMENT_NAME_ENCAPSULATED_REGULAR_EXPRESSION))
                                    {
                                        fragment.ValueOption |= ValueOptionEnum.ValueInEncapsulated;
                                    }

                                    fragment.ExtractType = variable.Value.ExtractType;

                                    if (variable.Value.ExtractType == ExtractTypeEnum.EXTRACT_SCRIPT)
                                    {
                                        fragment.LanguageType = (LanguageSourceEnum)Enum.Parse(typeof(LanguageSourceEnum), match.Groups["LANGUAGE_TYPE"].Value);
                                    }

                                    if (!fragment.FragmentExtractList.Any(a => a.OriginalFragment == fragment.OriginalFragment))
                                    {
                                        Match matchVariableName = Regex.Match(matchFragment, FunctionConstants.GET_VARIABLE_WITH_ARGUMENT);
                                        fragment.FragmentExtractList.Add(new FragmentExtractInfo
                                        {
                                            OriginalFragment = fragment.OriginalFragment,
                                            StartPosition = fragment.StartPosition,
                                            FragmentLength = fragment.Length,
                                            ExtractType = variable.Value.ExtractType,
                                            Variable = matchVariableName.Success ? matchVariableName.Groups["VARIABLE_NAME"].Value.Trim() : null
                                        });
                                    }
                                }
                                else
                                {
                                    int sumAllAboveIndex = fragmentList
                                        .Where(w => w.StartPositionFragment < match.Index)
                                        .Sum(f => f.OriginalFragment.Length - f.FragmentName.Length);

                                    fragmentName = string.Format(FRAGMENT_NAME_FORMAT, fragmentCount);

                                    Match matchVariableName = Regex.Match(matchFragment, FunctionConstants.GET_VARIABLE_WITH_ARGUMENT);
                                    
                                    fragment = new FragmentInfo
                                    {
                                        FragmentName = fragmentName,
                                        Fragment = matchFragment,
                                        OriginalFragment = matchFragment,
                                        StartPositionFragment = match.Index,
                                        LengthFragment = fragmentName.Length,
                                        StartPosition = match.Index + sumAllAboveIndex,
                                        ValueOption = valueIsJson ? ValueOptionEnum.ValueInJson : ValueOptionEnum.None,
                                        ExtractType = variable.Value.ExtractType,
                                        FragmentExtractList = new List<FragmentExtractInfo>() {
                                            { 
                                                new FragmentExtractInfo
                                                {
                                                    OriginalFragment = matchFragment,
                                                    StartPosition = match.Index + sumAllAboveIndex,
                                                    FragmentLength = fragmentName.Length,
                                                    ExtractType = variable.Value.ExtractType,
                                                    Variable = matchVariableName.Success ? matchVariableName.Groups["VARIABLE_NAME"].Value.Trim() : null
                                                }
                                            }
                                        }
                                    };

                                    fragmentList.Add(fragment, true);
                                }
                            }

                            int valueLengthBeforeReplaced = value.Length;

                            value = Utilities.ReplaceAt(value.ToString(), match.Index, match.Value.Length, fragment.FragmentName);

                            if (positionDisplacement > 0)
                            {
                                fragment.StartPositionFragment -= positionDisplacement;
                                fragment.LengthFragment = fragment.FragmentName.Length;
                            }

                            positionDisplacement = (valueLengthBeforeReplaced - value.Length);
                            foreach (FragmentInfo fragmentIndex in fragmentList.Where(f => f.StartPositionFragment > fragment.StartPositionFragment))
                            {
                                fragmentIndex.StartPositionFragment -= positionDisplacement;
                            }                            

                            if (variable.Key != FunctionConstants.GET_VARIABLE_WITH_PROPERTY)
                            {
                                hasVariableMatch = false;
                                goto RUN_VARIABLES;
                            }
                        }
                    }

                    if (hasVariableMatch)
                    {
                        goto RUN_THIS_VARIABLE;
                    }
                }
            }

            if (hasMatches)
            {
                goto RUN_VARIABLES;
            }

            if (fragmentList.Count > 0)
            {
                MatchCollection fragmentNameMatchCollection;
                {
                    fragmentNameMatchCollection = Regex.Matches(value, FunctionConstants.GET_FRAGMENT_BETWEEN_SYMBOL_EXPRESSION, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                    if (fragmentNameMatchCollection != null && fragmentNameMatchCollection.Any())
                    {
                        foreach (Match fragmentString in fragmentNameMatchCollection)
                        {
                            if (!fragmentString.Groups["INVALID_STRING"].Success)
                            {
                                string stringFragment = fragmentString.Groups["STRING_FRAGMENT_TYPE"].Value;
                                string fragmentName = fragmentString.Groups["FRAGMENT_NAME"].Value;

                                FragmentInfo fragment = fragmentList.FirstOrDefault(f => f.FragmentName == fragmentName);
                                if (fragment != null)
                                {

                                    if (fragmentString.Groups["QUOTES_ESCAPE"].Success)
                                    {
                                        string copyFragmentMatch = fragmentString.Value;

                                    RESOLVE_QUOTES_STRING:

                                        Match matchStringFragment = Regex.Match(copyFragmentMatch, FunctionConstants.GET_FRAGMENT_BETWEEN_QUOTES_WITH_SLASH, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                                        if (matchStringFragment.Success)
                                        {
                                            FragmentInfo stringFragmentInfo = fragmentList.FirstOrDefault(f => f.FragmentName == matchStringFragment.Groups["FRAGMENT_NAME"].Value);
                                            if (stringFragmentInfo != null)
                                            {
                                                copyFragmentMatch = Utilities.ReplaceAt(copyFragmentMatch, matchStringFragment.Groups["FRAGMENT_NAME"].Index
                                                , matchStringFragment.Groups["FRAGMENT_NAME"].Length
                                                , new string('X', matchStringFragment.Groups["FRAGMENT_NAME"].Length));

                                                string literalString = Utilities.ReplaceAt(copyFragmentMatch, matchStringFragment.Groups["FRAGMENT_NAME"].Index
                                                , matchStringFragment.Groups["FRAGMENT_NAME"].Length
                                                , new string(' ', matchStringFragment.Groups["FRAGMENT_NAME"].Length));

                                                //stringFragmentInfo.InStringValue = true;
                                                stringFragmentInfo.ValueOption |= ValueOptionEnum.ValueInString;

                                                if (literalString.Trim() == string.Empty
                                                    || literalString.Substring(1, literalString.Length - 2).Trim() == string.Empty)
                                                {
                                                    //stringFragmentInfo.InLiteralStringValue = true;
                                                    stringFragmentInfo.ValueOption |= ValueOptionEnum.ValueInLiteralString;
                                                }

                                                goto RESOLVE_QUOTES_STRING;
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        int startPosition = fragmentString.Groups["FRAGMENT_NAME"].Index - fragmentString.Groups["STRING_FRAGMENT_TYPE"].Index;

                                        fragment.OriginalFragment = Utilities.ReplaceAt(stringFragment, startPosition
                                            , fragmentString.Groups["FRAGMENT_NAME"].Length
                                            , fragment.OriginalFragment);

                                        fragment.StartPosition -= startPosition;
                                        fragment.Length = fragment.OriginalFragment.Length;
                                        fragment.EndPosition = fragment.StartPosition + fragment.OriginalFragment.Length;
                                        //fragment.InStringValue = true;
                                        fragment.ValueOption |= ValueOptionEnum.ValueInString;

                                        if (fragmentString.Groups["SQUARE_BRACKETS"].Success)
                                        {
                                            //fragment.InArrayObjetValue = true;
                                            fragment.ValueOption |= ValueOptionEnum.ValueInSquareBracket;
                                        }

                                        if (fragmentString.Groups["KEY_BRACKETS"].Success)
                                        {
                                            //fragment.InJsonObjectValue = true;
                                            fragment.ValueOption |= ValueOptionEnum.ValueInKeyBracket;
                                        }

                                        if (fragmentString.Groups["QUOTES"].Success)
                                        {
                                            //fragment.InStringValue = true;
                                            //fragment.InLiteralStringValue = true;

                                            fragment.ValueOption |= ValueOptionEnum.ValueInKeyBracket;
                                            fragment.ValueOption |= ValueOptionEnum.ValueInLiteralString;
                                        }
                                    }                                    
                                }
                            }
                        }
                    }

                    foreach (FragmentInfo fragment in fragmentList)
                    {
                        if (fragment.FragmentName != value.Substring(fragment.StartPositionFragment, fragment.LengthFragment))
                        {
                            throw new Exception(string.Format("Incorrectly formed fragment: {0}", fragment.FragmentName));
                        }

                        if (fragment.OriginalFragment != valueTrim.Substring(fragment.StartPosition, fragment.Length))
                        {
                            throw new Exception(string.Format("Incorrectly formed fragment: {0}", fragment.FragmentName));
                        }
                    }
                }
            }

            return fragmentList;
        }

        /// <summary>
        /// PROCESS THE ALL VARIABLES
        /// </summary>
        /// <param name="value"></param>
        /// <param name="variableDictionary"></param>
        /// <param name="extractTypeEnum"></param>
        private void Process(ref dynamic value, Dictionary<string, VariableRegexTypeInfo> variableDictionary, bool useDefaultVariablesRegEx = true)
        {  
            MatchCollection matchList = null;
            Dictionary<string, object> valueDictionary = null;
            FragmentList fragmentList = ProcessFragment(value, variableDictionary, useDefaultVariablesRegEx);
            List<FragmentExtractInfo> fragmentResultList = new List<FragmentExtractInfo>();

            dynamic valueFragment = string.Empty;
            foreach (FragmentInfo fragment in fragmentList.OrderByDescending(o => o.StartPosition))
            {
                valueFragment = fragment.OriginalFragment;

                foreach (KeyValuePair<string, VariableRegexTypeInfo> variable in variableDictionary)
                {
                    if (valueFragment != null && valueFragment.GetType() == typeof(string))
                    {
                        matchList = Regex.Matches(valueFragment, variable.Value.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                        if (matchList != null && matchList.Count > 0)
                        {
                            foreach (Match match in matchList.OrderByDescending(o => o.Index))
                            {
                                switch (variable.Value.ExtractType)
                                {
                                    case ExtractTypeEnum.EXTRACT_FUNCTION:
                                        valueDictionary = GetFunctionFromString(match);
                                        break;
                                    case ExtractTypeEnum.EXTRACT_SCRIPT:
                                        valueDictionary = GetScriptFromString(match);
                                        break;
                                    default:
                                        valueDictionary = null;
                                        break;
                                }

                                if (fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInJson)
                                    && fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInString)
                                    && !(fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInKeyBracket)
                                        || fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInSquareBracket)))
                                {   
                                    valueFragment = string.Format(@"""{0}""", valueFragment);
                                }

                                ProcessVariable(ref valueFragment, variable.Key, CommonConstants.GET_STEP_VALUE_FORMAT
                                    , match.Value, match.Index, match.Index + match.Length
                                    , valueDictionary, fragment.ValueOption, fragment.ExtractType);

                                if (fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInJson)
                                    && fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInString)
                                    && !(fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInKeyBracket)
                                        || fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInSquareBracket)))
                                {
                                    valueFragment = valueFragment.Substring(1, valueFragment.Length-2);

                                    if (fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInJson)
                                            && fragment.ValueOption.HasFlag(ValueOptionEnum.ValueInString))
                                    {
                                        if (Utilities.NeedsEscape(valueFragment))
                                        {
                                            valueFragment = Utilities.EscapeString(valueFragment);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                string variableJsonType = JsonConvert.DeserializeObject<dynamic>(valueFragment).GetType().Name.ToUpper();
                                                valueFragment = Utilities.EscapeString(valueFragment);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                value = Utilities.ReplaceAt(value, fragment.StartPosition, fragment.Length, valueFragment);
            }            
        }

        public void ProcessScripts(ref dynamic value)
        {
            Dictionary<string, VariableRegexTypeInfo> variableDictionary = new Dictionary<string, VariableRegexTypeInfo>
            {
                //{FunctionConstants.GET_ALL_SCRIPT_LINE, new VariableRegexTypeInfo{ Regex = getAllScriptLineExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                {FunctionConstants.GET_ALL_SCRIPT_FILE, new VariableRegexTypeInfo{ Regex = getAllScriptFileExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                {FunctionConstants.GET_ALL_SCRIPT_CODE, new VariableRegexTypeInfo{ Regex = getAllScriptCodeExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } }
            };

            Process(ref value, variableDictionary);
        }

        public void ProcessVariables(ref dynamic value)
        {
            Dictionary<string, VariableRegexTypeInfo> variableDictionary = new Dictionary<string, VariableRegexTypeInfo>
            {
                {FunctionConstants.GET_VARIABLE_VALUE_STATEMENT, new VariableRegexTypeInfo{ Regex = getVariableStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE } },
                {FunctionConstants.GET_VARIABLE_STATEMENT, new VariableRegexTypeInfo{ Regex = getStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE } },                
                {FunctionConstants.GET_VARIABLE_VALUE, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_VARIABLE_VALUE_REGULAR_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE } },
                //{FunctionConstants.GET_VARIABLE_TYPE_CONVERTER_REGULAR_EXPRESSION, new VariableRegexTypeInfo{ Regex = FunctionConstants.GET_VARIABLE_TYPE_CONVERTER_REGULAR_EXPRESSION, ExtractType = ExtractTypeEnum.EXTRACT_CONVERTER } },
                //{FunctionConstants.GET_ALL_SCRIPT_LINE, new VariableRegexTypeInfo{ Regex = getAllScriptLineExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                {FunctionConstants.GET_ALL_SCRIPT_FILE, new VariableRegexTypeInfo{ Regex = getAllScriptFileExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } },
                {FunctionConstants.GET_ALL_SCRIPT_CODE, new VariableRegexTypeInfo{ Regex = getAllScriptCodeExpression, ExtractType = ExtractTypeEnum.EXTRACT_SCRIPT } }
                //{FunctionConstants.GET_VARIABLE_VALUE, FunctionConstants.GET_VARIABLE_VALUE_REGULAR_EXTENDED_EXPRESSION},
            };

            Process(ref value, variableDictionary);
        }

        /// <summary>
        ///  SE DEDE DE PROCESAR DE LA SIGUIENTE MANERA
        ///  1.- VARIABLES
        ///  2.- FUNCIONES POR COMA
        ///  3.- FUNCIONES
        ///  4.- ARGUMENTOS DE FUNCIONES        
        /// </summary>
        /// <param name="value"></param>
        public void ProcessExtractFunction(ref dynamic value)
        {
            Dictionary<string, string> functionDictionary = new Dictionary<string, string>
            {
                {FunctionConstants.GET_ALL_FUNCTION_COMMA, FunctionConstants.GET_ALL_FUNCTION_EXPRESSION},
                {FunctionConstants.GET_ALL_FUNCTION, FunctionConstants.GET_ALL_FUNCTION_COMMA_EXPRESSION}                
            };

            MatchCollection matchList = null;
            string oldValue = null;
            dynamic matchValue = null;
            bool salir = false;

            foreach (KeyValuePair<string, string> function in functionDictionary)
            {
                salir = false;
                while (!salir)
                {
                    if (value != null && value.GetType() == typeof(string))
                    {
                        oldValue = value;
                        matchList = Regex.Matches(value, function.Value);
                        if (matchList != null && matchList.Count > 0)
                        {
                            foreach (Match match in matchList)
                            {
                                matchValue = match.Value;
                                value = value.Replace(match.Value, matchValue);
                            }

                            if (value == oldValue)
                            {
                                salir = true;
                            }
                        }
                        else
                        {
                            salir = true;
                        }
                    }
                    else
                    {
                        salir = true;
                    }
                }
            }
        }

        public void ProcessOptions(ref dynamic value, List<ValueOptionInfo> valueOptionList)
        {
            if (valueOptionList != null && valueOptionList.Count() > 0)
            {
                foreach (ValueOptionInfo valueOptionInfo in valueOptionList.OrderBy(o => o.Number))
                {
                    if (valueOptionInfo.Type == "DecryptBae64")
                    {
                        value = Utilities.DecodeBase64(value);
                    }
                    else if (valueOptionInfo.Type == "EncryptBae64")
                    {
                        value = Utilities.EncodeBase64(value);
                    }
                }
            }
        }

        private void ProcessVariable(ref dynamic value, string variableName, string variableFormat, string matchValue, int startPositionMatch, int endPositionMatch, Dictionary<string, object> variableDictionary, ValueOptionEnum valueOption, ExtractTypeEnum extractType)
        {
            if (value != null)
            {
                // LUIS VERA: StringIsJson NO VA RECONOCER UN JSON CUANDO EL CAMPO SEA NUMERO Y LLEVE SCRIPT_LINE, EJEMPLO {"Numero": @Script_Line...}
                bool valueIsJson = StringIsJson(value);
                ValueInfo valueInfo = null;

                if (variableDictionary != null)
                {
                    if (variableName == FunctionConstants.GET_ALL_SCRIPT_CODE || variableName == FunctionConstants.GET_ALL_SCRIPT_FILE)
                    {
                        value = ProcessScriptCode(value, matchValue, variableDictionary, valueOption, variableName);
                    }
                }
                else
                {
                    if (variableName == FunctionConstants.GET_VARIABLE_VALUE
                        || variableName == FunctionConstants.GET_VARIABLE_VALUE_STATEMENT)
                    {
                        string cleanMatchValue = null;

                        // LUIS VERA: ESTA SECCIÓN ES PARA OBTENER LA VARIABLE DEL HILO ACTUAL O PRINCIPAL, QUIEBRA EL DISEÑO POR LA SECCIÓN DE ABAJO QUE BUSCA POR TASK NAME
                        // EJEMPLO: [@VARIABLE, TAREA1] VS [@VARIABLE, HILO_PRINCIPAL]
                        // PUEDE ARREGLARSE QUE SEA UN ENTERO UN 1 PARA HILO PRINCIPAL ó 0 PARA HILO ACTUAL

                        //Match variableTypeMatch = null;
                        //if ((variableTypeMatch = Regex.Match(matchValue, FunctionConstants.GET_VARIABLE_VALUE_AND_TYPE_REGULAR_EXPRESSION)) != null)
                        //{
                        //    if (variableTypeMatch.Groups["ONLY_VARIABLE"] != null)
                        //    {
                        //    }
                        //    else
                        //    {
                        //    }
                        //}
                        //else
                        {
                            //cleanMatchValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(matchValue, "[@", ""), "]", "").Trim();
                            //cleanMatchValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(cleanMatchValue, "\"", ""), "\"", "").Trim();

                            Match cleanMatch = Regex.Match(matchValue, FunctionConstants.GET_VARIABLE_VALUE_REGULAR_EXPRESSION);
                            if (cleanMatch.Success)
                            {
                                cleanMatchValue = cleanMatch.Groups[1].Value;
                                cleanMatchValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(cleanMatchValue, "\"", ""), "\"", "").Trim();
                            }
                        }

                        MatchCollection parameterMatchCollection = Regex.Matches(cleanMatchValue, FunctionConstants.GET_BETWEEN_COMMA_EXPRESSION);
                        if (parameterMatchCollection != null && parameterMatchCollection.Count > 1)
                        {
                            List<string> parameterVariable = parameterMatchCollection.Select(s => s.Groups[FunctionConstants.PARAMETER].Value).ToList<string>();
                            if (parameterVariable != null && parameterVariable.Any())
                            {
                                string variableNameParam = parameterVariable[0];
                                string taskNameParam = parameterVariable[1];
                                //string processNameParam = parameterVariable[2];

                                if (!string.IsNullOrWhiteSpace(variableNameParam) && !string.IsNullOrWhiteSpace(taskNameParam))
                                {
                                    variableNameParam = variableNameParam.Trim();
                                    taskNameParam = taskNameParam.Trim();

                                    valueInfo = GetPublicVariable(variableNameParam, taskNameParam);
                                }
                            }
                        }
                        else
                        {
                            valueInfo = GetVariable(cleanMatchValue, processBC.GetCurrentTaskName());
                        }
                    }
                    else if (variableName == FunctionConstants.GET_VARIABLE_STATEMENT)
                    {
                    }

                    value = ProcessVariableValueInfo(value, matchValue, valueInfo, valueOption);
                }
                
            }
        }

        private dynamic ProcessScriptCode(dynamic value, string matchValue, Dictionary<string, object> variableDictionary, ValueOptionEnum valueOption, string scriptCodeType)
        {
            if (variableDictionary.Count >= 2)
            {
                string languageType = (string)variableDictionary[FunctionConstants.LANGUAGE_TYPE];
                string scriptCode = (string)variableDictionary[FunctionConstants.STATEMENT];

                if (languageType != null && scriptCode != null)
                {
                    string returnValueScript = null;
                    IScriptManager scriptManager = null;
                    LanguageSourceEnum languageSourceEnum = (LanguageSourceEnum)Enum.Parse(typeof(LanguageSourceEnum), languageType.Trim().ToUpper());
                    scriptManager = processBC.GetScriptManager(languageSourceEnum);

                    // CUANDO ES ARCHIVO LAS VARIABLES SE PROCESAN DESPUÉS DE OBTENER EL CONTENIDO.
                    if (scriptCodeType == FunctionConstants.GET_ALL_SCRIPT_FILE
                        && File.Exists(scriptCode.Trim()))
                    {
                        string filePath = Path.GetDirectoryName(scriptCode.Trim());
                        if (Directory.Exists(filePath))
                        {
                            scriptManager.SetSearchPaths(new List<string> { filePath });
                        }

                        dynamic scriptCodeDynamic = File.ReadAllText(scriptCode);
                        ProcessVariables(ref scriptCodeDynamic);
                        scriptCode = scriptCodeDynamic.ToString();
                    }

                    scriptCode = ScriptCodeConverter(languageType, scriptCode);
                    
                    Type scriptType = null;
                    scriptCode = ScriptCodeResultConverter(languageType, scriptCode, ref scriptType);

                    switch (scriptCodeType)
                    {
                        //// LUIS VERA, SCRIPT_LINEA NO ES NECESARIO, CON SCRIPT_CODE SE PUEDE RESOLVER DE MANERA SIMPLE
                        //case FunctionConstants.GET_ALL_SCRIPT_LINE:
                        //    returnValueScript = scriptManager.ExecuteScriptImplicitReturn<string>(scriptCode);
                        //    break;

                        case FunctionConstants.GET_ALL_SCRIPT_CODE:
                        case FunctionConstants.GET_ALL_SCRIPT_FILE:
                            returnValueScript = ExecuteScriptCode(scriptCode, scriptType, scriptManager);
                            break;
                    }
                    
                    if (returnValueScript != null && valueOption.HasFlag(ValueOptionEnum.ValueInJson))
                    {
                        returnValueScript = returnValueScript.Replace("\\", "\\\\");
                    }

                    value = value.Replace(matchValue, returnValueScript);

                    scriptManager.ClearSearchPaths();
                }
            }

            return value;
        }

        private string ExecuteScriptCode(string scriptCode, Type scriptType, IScriptManager scriptManager)
        {
            string returnValueScript = null;

            switch (Type.GetTypeCode(scriptType))
            {
                case TypeCode.String:
                    returnValueScript = Convert.ToString(scriptManager.ExecuteScript<string>(scriptCode, FrameworkConstants.SCRIPT_RETURN_RESULT));
                    break;
                case TypeCode.Int32:
                    returnValueScript = Convert.ToString(scriptManager.ExecuteScript<int>(scriptCode, FrameworkConstants.SCRIPT_RETURN_RESULT));
                    break;
                case TypeCode.Single:
                    returnValueScript = Convert.ToString(scriptManager.ExecuteScript<float>(scriptCode, FrameworkConstants.SCRIPT_RETURN_RESULT));
                    break;
                case TypeCode.Decimal:
                    returnValueScript = Convert.ToString(scriptManager.ExecuteScript<decimal>(scriptCode, FrameworkConstants.SCRIPT_RETURN_RESULT));
                    break;
                case TypeCode.Object:
                    returnValueScript = Convert.ToString(scriptManager.ExecuteScript<object>(scriptCode, FrameworkConstants.SCRIPT_RETURN_RESULT));
                    break;
                default:
                    scriptManager.ExecuteScript(scriptCode);
                    returnValueScript = string.Empty;
                    break;
            }
            
            return returnValueScript;
        }

        private string ScriptCodeConverter(string languageType, string scriptCode)
        {
            //const string CONTENT_GROUP = "CONTENT";
            const string TYPE = "TYPE";
            const string STATEMENT = "STATEMENT";
            string result = scriptCode;
            MatchCollection converterMatchCollection = Regex.Matches(scriptCode, getConverterCodeExpression, RegexOptions.IgnorePatternWhitespace);

            if (converterMatchCollection.Any())
            {
                foreach (Match converterMatch in converterMatchCollection)
                {
                    //string contentValue = converterMatch.Groups[CONTENT_GROUP].Value;
                    string typeValue = converterMatch.Groups[TYPE].Value;
                    string statementValue = converterMatch.Groups[STATEMENT].Value;

                    switch (Enum.Parse(typeof(LanguageSourceEnum), languageType))
                    {
                        case LanguageSourceEnum.CSHARP:
                            {
                                switch (Enum.Parse(typeof(ConverterTypeEnum), typeValue))
                                {
                                    case ConverterTypeEnum.LIST_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("JsonConvert.DeserializeObject<List<object>>(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.DICTIONARY_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("JsonConvert.DeserializeObject<Dictionary<string, object>>(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.LIST_DICTIONARY_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.DATATABLE_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("JsonConvert.DeserializeObject<DataTable>(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.INT_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("Convert.ToInt32(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.FLOAT_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("Convert.ToSingle(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.DECIMAL_CONVERTER:
                                        {   
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("Convert.ToDecimal(\"{0}\")", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.STRING_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("\"{0}\"", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    case ConverterTypeEnum.OBJECT_CONVERTER:
                                        {
                                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Format("(object)\"{0}\"", Utilities.EscapeString(statementValue)));
                                        }
                                        break;
                                    default:
                                        result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Empty);
                                        break;
                                }
                            }
                            break;
                        default:
                            result = Utilities.ReplaceAt(result, converterMatch.Index, converterMatch.Length, string.Empty);
                            break;
                    }
                }
            }

            return result;
        }

        private string ScriptCodeResultConverter(string languageType, string scriptCode, ref Type typeValueReturn)
        {
            const string TYPE = "TYPE";
            const string STATEMENT = "STATEMENT";
            string result = scriptCode;
            Match resultConverterMatch = Regex.Match(scriptCode, getResultConverterCodeExpression, RegexOptions.IgnorePatternWhitespace);

            if (resultConverterMatch.Success)
            {
                string typeValue = resultConverterMatch.Groups[TYPE].Value;
                string statementValue = resultConverterMatch.Groups[STATEMENT].Value;

                switch (Enum.Parse(typeof(LanguageSourceEnum), languageType))
                {
                    case LanguageSourceEnum.CSHARP:
                        {
                            switch (Enum.Parse(typeof(ResultConverterTypeEnum), typeValue))
                            {
                                case ResultConverterTypeEnum.RESULT_STRING:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return Convert.ToString({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result, 
                                            resultConverterMatch.Index, 
                                            resultConverterMatch.Length,
                                            string.Format("return JsonConvert.SerializeObject({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result, 
                                            resultConverterMatch.Index, 
                                            resultConverterMatch.Length, 
                                            string.Format("return JsonConvert.SerializeObject({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return JsonConvert.SerializeObject({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DATATABLE:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return JsonConvert.SerializeObject({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_INT:
                                    {
                                        typeValueReturn = typeof(int);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return Convert.ToInt32({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_FLOAT:
                                    {
                                        typeValueReturn = typeof(float);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return Convert.ToSingle({0});", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DECIMAL:
                                    {
                                        typeValueReturn = typeof(decimal);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return Convert.ToDecimal({0});", statementValue));
                                    }
                                    break;                                
                                case ResultConverterTypeEnum.RESULT_OBJECT:
                                    {
                                        typeValueReturn = typeof(object);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return (object){0};", statementValue));
                                    }
                                    break;
                                default:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return Convert.ToString({0});", statementValue));
                                    }
                                    break;
                            }
                        }
                        break;

                    case LanguageSourceEnum.PYTHON:
                        {
                            switch (Enum.Parse(typeof(ResultConverterTypeEnum), typeValue))
                            {
                                case ResultConverterTypeEnum.RESULT_STRING:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = str({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = json.dumps({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = json.dumps({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = json.dumps({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DATATABLE:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = json.dumps({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_INT:
                                    {
                                        typeValueReturn = typeof(int);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("return int({0})", statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_FLOAT:
                                    {
                                        typeValueReturn = typeof(float);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = float({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DECIMAL:
                                    {
                                        typeValueReturn = typeof(decimal);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = float({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_OBJECT:
                                    {
                                        typeValueReturn = typeof(object);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = str({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                default:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("{0} = str({1})", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                            }
                        }
                        break;

                    case LanguageSourceEnum.JAVASCRIPT:
                        {
                            switch (Enum.Parse(typeof(ResultConverterTypeEnum), typeValue))
                            {
                                case ResultConverterTypeEnum.RESULT_STRING:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = {1}.toString();", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = JSON.stringify({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = JSON.stringify({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_LIST_DICTIONARY:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = JSON.stringify({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DATATABLE:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = JSON.stringify({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_INT:
                                    {
                                        typeValueReturn = typeof(int);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = parseInt({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_FLOAT:
                                    {
                                        typeValueReturn = typeof(float);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = parseFloat({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_DECIMAL:
                                    {
                                        typeValueReturn = typeof(float);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = parseFloat({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                case ResultConverterTypeEnum.RESULT_OBJECT:
                                    {
                                        typeValueReturn = typeof(object);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = JSON.parse({1});", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                                default:
                                    {
                                        typeValueReturn = typeof(string);
                                        result = Utilities.ReplaceAt(result,
                                            resultConverterMatch.Index,
                                            resultConverterMatch.Length,
                                            string.Format("var {0} = {1}.toString();", FrameworkConstants.SCRIPT_RETURN_RESULT, statementValue));
                                    }
                                    break;
                            }
                        }
                        break;

                    default:
                        typeValueReturn = typeof(void);
                        result = Utilities.ReplaceAt(result, resultConverterMatch.Index, resultConverterMatch.Length, string.Empty);
                        break;
                }
            }

            return result;
        }
        
        private dynamic ProcessVariableValueInfo(dynamic value, string matchValue, ValueInfo valueInfo, ValueOptionEnum valueOption)
        {
            if (valueInfo != null && valueInfo.Value != null)
            {
                int lastIndex = value.ToString().LastIndexOf(matchValue);
                if (lastIndex >= 0)
                {
                    string matchString = value.ToString().Substring(lastIndex + (lastIndex > 0 ? -1 : 0)).Trim();
                    if (matchString[0] == '[' && matchString[1] == '[') //&& matchString[matchString.Length - 1] == ']')
                    {
                        //matchString = matchString.Substring(1, matchString.Length - 2);
                        string matchValueSquareBrackets = Regex.Match(matchString, FunctionConstants.GET_BETWEEN_SQUARE_BRACKETS_EXPRESSION)?.Value;

                        if (!string.IsNullOrEmpty(matchValueSquareBrackets))
                        {
                            matchString = matchValueSquareBrackets;
                        }
                    }

                    MatchCollection matchList = Regex.Matches(matchString, FunctionConstants.GET_VARIABLE_JSON_PROPERTY_EXPRESSION,
                        RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft);

                    if (matchList.Count == 0
                        && valueInfo.Value.GetType().Namespace != CommonConstants.SYSYTEM_COLLECTIONS_GENERIC)
                    {
                        if (MatchVariableStatement(ref value, matchValue, valueInfo, valueOption))
                        {
                            //value = value.Replace(value, valueInfo.Value.ToString());
                        }
                        else
                        {
                            MatchJsonInString(ref value, matchValue, valueInfo, valueOption);
                        }
                    }
                    else
                    {
                        if (MatchVariableStatement(ref value, matchValue, valueInfo, valueOption))
                        {
                            //value = value.Replace(value, valueInfo.Value.ToString());
                        }
                        else if (matchList != null && matchList.Count > 0)
                        {
                            MatchJsonProperty(ref value, valueInfo, matchList, valueOption);
                        }
                        else if (valueInfo.Value.GetType().Namespace == CommonConstants.SYSYTEM_COLLECTIONS_GENERIC)
                        {
                            MatchGenericCollection(ref value, matchValue, valueInfo, valueOption);
                        }
                    }
                }
            }
            else
            {
                Match matchStatementWithoutVariable = Regex.Match(matchValue, getStatementRegularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                if (matchStatementWithoutVariable.Success)
                {
                    MatchVariableStatement(ref value, string.Format("[@#]{0}", matchStatementWithoutVariable.Value), valueInfo, valueOption);
                }
                else
                {
                    value = value.Replace(matchValue, string.Empty);                    
                }
            }

            return value;
        }

        private FragmentInfo GetFragmentJsonProperty(string value)
        {
            FragmentInfo fragmentResult = null;
            Match match = Regex.Match(value, FunctionConstants.GET_VARIABLE_JSON_PROPERTY_EXPRESSION,
                        RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft);

            if (match.Success)
            {
                int startPosition = match.Index;
                const string propertyGroup = "PROPERTY";
                string property = null;
                MatchCollection unionMatches = null;
                string processValue = string.Empty;
                const string notSpaceExpression = @"[^\s]";
                const string notSpaceBetweenJsonProperty = @"(?<VALUE>(?>[\w\d]+))(?<SPACE>[\s]+)(?=[\w\d]+)";
                int currentEndIndex = 0;
                int nextStartIndex = 0;
                Match matchSpaceProperty = null;
                string unionMatchValue = string.Empty;
                int countMatch = 0;
                //foreach (Match match in matchList)
                {
                    property = match.Groups[propertyGroup].Value;
                    unionMatches = Regex.Matches(property, FunctionConstants.GET_VARIABLE_JSON_UNION_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    processValue = string.Empty;
                    countMatch = 0;
                    unionMatchValue = string.Empty;
                    foreach (Match unionMatch in unionMatches)
                    {
                        if (unionMatch.Success && !string.IsNullOrEmpty(unionMatch.Value))
                        {
                            currentEndIndex = (nextStartIndex > 0 ? unionMatches[countMatch - 1].Index : 0) + unionMatch.Value.Length;
                            unionMatchValue = unionMatch.Value;

                            {
                                if (countMatch + 1 < unionMatches.Count)
                                {
                                    nextStartIndex = unionMatches[countMatch + 1].Index;
                                }

                                if (currentEndIndex == nextStartIndex)
                                {
                                }
                                else
                                {
                                    // LUIS VERA: EN ESTE IF NO ENTRO NUNCA PERO DEBERIA CUANDO LA CADENA ES: [@varTest].Usuarios    #[Name]
                                    // Revisa si existen solo espacios en blanco de lo contrario hace un break hasta antes del caracter
                                    // quedando [@varTest].Usuarios    
                                    if (nextStartIndex > currentEndIndex
                                        && Regex.IsMatch(match.Groups[propertyGroup].Value.Substring(currentEndIndex, nextStartIndex), notSpaceExpression))
                                    {
                                        processValue += unionMatchValue;
                                        break;
                                    }
                                    else if ((matchSpaceProperty = Regex.Match(unionMatch.Value, notSpaceBetweenJsonProperty)).Success)
                                    {
                                        unionMatchValue = unionMatch.Value.Substring(0, matchSpaceProperty.Groups["VALUE"].Index + matchSpaceProperty.Groups["VALUE"].Value.Length);
                                    }
                                    else
                                    {
                                    }
                                }
                            }

                            processValue += unionMatchValue;
                            countMatch++;
                        }
                    }

                    fragmentResult = new FragmentInfo 
                    {
                        StartPosition = startPosition,
                        EndPosition = startPosition + processValue.Length,
                        FragmentName = FunctionConstants.GET_VARIABLE_JSON_PROPERTY,
                        Fragment = string.Concat(match.Groups[FunctionConstants.VARIABLE].Value, processValue)
                    };
                }
            }

            return fragmentResult;
        }

        private void MatchGenericCollection(ref dynamic value, string matchValue, ValueInfo valueInfo, ValueOptionEnum valueOption)
        {
            dynamic jsonValue = null;
            string variableType = null;
            string[] jsonArrayTypeList = { "JOBJECT" }; // "JARRAY"

            try
            {
                jsonValue = JsonConvert.DeserializeObject<dynamic>(value);
                variableType = jsonValue.GetType().Name.ToUpper();

                if (!jsonArrayTypeList.Contains(variableType))
                {
                    jsonValue = JsonConvert.SerializeObject(valueInfo.Value);
                }
            }
            catch (Exception)
            {
                try
                {
                    jsonValue = JsonConvert.SerializeObject(valueInfo.Value);
                    variableType = jsonValue.GetType().Name.ToUpper();
                }
                catch (Exception)
                {
                    jsonValue = null;
                }
            }

            // LUIS VERA: REVISAR SI APLICA JARRAY, AL PARECER SOLO DEBE DE SER JOBJECT
            if (jsonValue != null && jsonArrayTypeList.Contains(variableType))
            {
                foreach (dynamic item in jsonValue)
                {
                    try
                    {
                        if (item.Value.ToString() == matchValue)
                        {
                            try
                            {
                                jsonValue[item.Name] = new Newtonsoft.Json.Linq.JArray(valueInfo.Value);
                            }
                            catch 
                            {
                                jsonValue[item.Name] = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(valueInfo.Value));
                            }
                        }
                        else if (item.Value.ToString().Contains(matchValue))
                        {
                            item.Value = jsonValue.Value.Replace(matchValue, valueInfo.Value.ToString());
                        }
                    }
                    catch (Exception)
                    { }
                }

                value = value.Replace(matchValue, jsonValue.ToString());
            }
            else
            {
                value = value.Replace(matchValue, jsonValue);
            }
        }

        private void MatchJsonProperty(ref dynamic value, ValueInfo valueInfo, MatchCollection matchList, ValueOptionEnum valueOption)
        {
            const string propertyGroup = "PROPERTY";
            string property = null;
            MatchCollection unionMatches = null;
            string processValue = string.Empty;
            const string notSpaceExpression = @"[^\s]";
            const string notSpaceBetweenJsonProperty = @"(?<VALUE>(?>[\w\d]+))(?<SPACE>[\s]+)(?=[\w\d]+)";
            int currentEndIndex = 0;
            int nextStartIndex = 0;
            Match matchSpaceProperty = null;
            string unionMatchValue = string.Empty;
            int countMatch = 0;
            foreach (Match match in matchList)
            {   
                property = match.Groups[propertyGroup].Value;
                unionMatches = Regex.Matches(property, FunctionConstants.GET_JSON_UNION_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                processValue = string.Empty;
                countMatch = 0;
                unionMatchValue = string.Empty;
                foreach (Match unionMatch in unionMatches)
                {
                    if (unionMatch.Success && !string.IsNullOrEmpty(unionMatch.Value))
                    {
                        currentEndIndex = (nextStartIndex > 0 ? unionMatches[countMatch - 1].Index : 0) + unionMatch.Value.Length;
                        unionMatchValue = unionMatch.Value;

                        {
                            if (countMatch + 1 < unionMatches.Count)
                            {
                                nextStartIndex = unionMatches[countMatch + 1].Index;
                            }

                            if (currentEndIndex == nextStartIndex)
                            {
                            }
                            else
                            {
                                // LUIS VERA: EN ESTE IF NO ENTRO NUNCA PERO DEBERIA CUANDO LA CADENA ES: [@varTest].Usuarios    #[Name]
                                // Revisa si existen solo espacios en blanco de lo contrario hace un break hasta antes del caracter
                                // quedando [@varTest].Usuarios    
                                if (nextStartIndex > currentEndIndex
                                    && Regex.IsMatch(match.Groups[propertyGroup].Value.Substring(currentEndIndex, nextStartIndex), notSpaceExpression))
                                {
                                    processValue += unionMatchValue;
                                    break;
                                }
                                else if ((matchSpaceProperty = Regex.Match(unionMatch.Value, notSpaceBetweenJsonProperty)).Success)
                                {
                                    unionMatchValue = unionMatch.Value.Substring(0, matchSpaceProperty.Groups["VALUE"].Index + matchSpaceProperty.Groups["VALUE"].Value.Length);
                                }
                                else
                                {
                                }
                            }
                        }

                        processValue += unionMatchValue;
                        countMatch++;
                    }
                }

                if (valueInfo != null)
                {
                    if (valueInfo.OutputValueType == ValueTypeEnum.LIST.ToString())
                    {
                        value = value.Replace(string.Concat(match.Groups["VARIABLE"].Value, processValue), JsonManager.ProcessProperty(JsonConvert.SerializeObject(valueInfo.Value), processValue));
                    }
                    else
                    {
                        value = value.Replace(string.Concat(match.Groups["VARIABLE"].Value, processValue), JsonManager.ProcessProperty(valueInfo.Value.ToString(), processValue));
                    }
                }
            }
        }

        private void MatchJsonInString(ref dynamic value, string matchValue, ValueInfo valueInfo, ValueOptionEnum valueOption)
        {
            string variableJsonType = null;
            try
            {
                variableJsonType = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(valueInfo.Value.ToString()).GetType().Name.ToUpper();
            }
            catch (Exception)
            { }

            //bool isJson = Regex.IsMatch(value, FunctionConstants.JSON_STRUCTURE_VALIDATE_EXPRESSION,
                    //RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bool isJson = valueOption.HasFlag(ValueOptionEnum.ValueInJson);

            if (variableJsonType == "JARRAY" || variableJsonType == "JOBJECT")
            //if (valueOption.HasFlag(ValueOptionEnum.ValueInSquareBracket) || valueOption.HasFlag(ValueOptionEnum.ValueInKeyBracket))
            {
                // LUIS VERA CUANDO LA ENTRADA ES UN JSON SE DEBE DE SABER QUE SALIDA VA A REQUERIR STRING, JSON, OBJECT ETC.
                // DENTRO DE UN JSON AL SER SALIDA JSON SE DEBE DE QUITAR COMILLAS DOBLES PARA QUE QUEDE SOLO EL OBJECT JSON
                // DENTRO DE UN JSON AL SER SALIDA TEXT SE DEBE DE DEJAR O PONER COMILLAS DOBLES A LOS EXTREMOS A APARTE DARLE UNSCAPE
                
                if (isJson)
                {
                    if (valueInfo.OutputValueType == ValueTypeEnum.JSON.ToString())
                    {
                        //EXAMPLE: 
                        // SI EL VALOR AL QUE VA LA VARIABLE JSON YA CONTIENE UNA CADENA, ESTE SE DEBE DE CONCATENAR A ELLA Y DARLE UNSCAPE
                        // { "Name": "Panchito [@Apellido_Json]"}
                        // { "Name": "Panchito { \"Apellido\" : \"Perez\" }"}

                        // SI EL VALOR NO CONTIENE NINGUNA CADENA, SI ES VACIO ENTONCES SE PASA COMO OBJETO JSON, SE BORRAN LAS COMILLAS DOBLES
                        // { "Name": "[@Apellido_Json]"}
                        // { "Name": {"Apellido": "Perez"} } 

                        bool successBetweenQuotes = false;
                        MatchCollection matchBetweenQuotes = Regex.Matches(value, FunctionConstants.GET_VALUE_BETWEEN_QUOTES);
                        for (int countMatch = matchBetweenQuotes.Count-1; countMatch >= 0; countMatch--)
                        {
                            Match matchQuote = matchBetweenQuotes[countMatch];

                            // IF IS JSON OBJECT INTO ELSE
                            MatchCollection variableMatchets = Regex.Matches(matchQuote.Value, FunctionConstants.GET_VARIABLE_WITH_ARGUMENT);
                            Match matchLastVariable = variableMatchets?.LastOrDefault();
                            if (matchLastVariable != null && matchLastVariable.Success)
                            {
                                // EXTRACT VARIABLE NAME WITHOUT TASK NAME OR COMMA
                                int matchIndex = matchLastVariable.Groups["VARIABLE_NAME"].Value.IndexOf(',');

                                // GET LAST VARIABLE BEFORE COMMA
                                string finalMatchValue = matchIndex > 0 ? matchLastVariable.Groups["VARIABLE_NAME"].Value?.Substring(0, matchIndex) : matchLastVariable.Groups["VARIABLE_NAME"].Value;

                                if (!string.IsNullOrEmpty(finalMatchValue) && finalMatchValue.Trim() == valueInfo.Name)
                                {
                                    // REPLACE LAST VARIABLE WITH VALUE 
                                    //if (matchLastVariable.Groups["VARIABLE"].Value.Trim() == matchQuote.Groups["VALUE"].Value.Trim())
                                    //{
                                    //    value = Utilities.ReplaceLastOccurrence(value, matchQuote.Groups["STRING"].Value, valueInfo.Value.ToString());
                                    //}
                                    //else
                                    {
                                        value = Utilities.ReplaceLastOccurrence(value, matchLastVariable.Groups["VARIABLE"].Value, Utilities.EscapeString(valueInfo.Value.ToString()));
                                    }

                                    successBetweenQuotes = true;
                                    break;
                                }
                            }
                        }

                        if (!successBetweenQuotes)
                        {
                            value = Utilities.ReplaceLastOccurrence(value, matchValue, valueInfo.Value.ToString());
                        }
                    }
                    else
                    {
                        value = Utilities.ReplaceLastOccurrence(value, matchValue, Utilities.EscapeString(valueInfo.Value.ToString()));
                    }
                }
                else
                {
                    value = Utilities.ReplaceLastOccurrence(value, matchValue, valueInfo.Value.ToString());
                }
            }
            else
            {
                // WHEN THE CURRENT VALUE OF THE STEP IS JSON, FIX THE DOUBLE QUOTATION
                if (isJson)
                {
                    //value = Utilities.ReplaceLastOccurrence(value, matchValue, valueInfo.Value.ToString().Replace("\\\"", "\"").Replace("\"", "\\\""));
                    value = Utilities.ReplaceLastOccurrence(value, matchValue, Utilities.EscapeString(valueInfo.Value.ToString()));
                }
                else
                { 
                    value = Utilities.ReplaceLastOccurrence(value, matchValue, valueInfo.Value.ToString());
                }
            }

            //return value;
        }

        public List<string> SplitParameters(string input, char separator = ',')
        {
            List<string> result = new List<string>();

            if (string.IsNullOrWhiteSpace(input))
            {
                return result;
            }

            StringBuilder current = new StringBuilder();

            bool inString = false;
            char stringDelimiter = '\0';
            bool escape = false;

            int braceDepth = 0;
            int bracketDepth = 0;
            int parenthesisDepth = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char character = input[i];
                current.Append(character);

                if (escape)
                {
                    escape = false;
                    continue;
                }

                if (character == '\\')
                {
                    escape = true;
                    continue;
                }

                if (character == '"' || character == '\'')
                {
                    if (!inString)
                    {
                        inString = true;
                        stringDelimiter = character;
                    }
                    else if (stringDelimiter == character)
                    {
                        inString = false;
                    }

                    continue;
                }

                if (inString)
                {
                    continue;
                }

                switch (character)
                {
                    case '{':
                        braceDepth++;
                        break;

                    case '}':
                        braceDepth--;
                        break;

                    case '[':
                        bracketDepth++;
                        break;

                    case ']':
                        bracketDepth--;
                        break;

                    case '(':
                        parenthesisDepth++;
                        break;

                    case ')':
                        parenthesisDepth--;
                        break;
                }

                if (character == separator &&
                    braceDepth == 0 &&
                    bracketDepth == 0 &&
                    parenthesisDepth == 0)
                {
                    current.Length--;

                    result.Add(current.ToString());

                    current.Clear();
                }
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result;
        }

        /// <summary>
        /// CONDITION 	= ?		Example: ?[Puesto='Le%' ó 'Le%',Puesto,NULL]	Options: ?[Puesto='Le%' ó 'Le%']
        /// FILTER 		= =		Example: =[Puesto='Le%' ó 'Le%']                Options: ?[Puesto='Le%' ó 'Le%']
        /// COLUMNAS  	= |		Example: |[IdPuesto|Id,Areas]                   Options: [Column|Rename]
        /// SALIDA 		= >   	Example: >[STRING] 							    Options: JSON,ARRAY,STRING
        /// FORMAT		= #		Example: #['{0}'|Id,{1},"{2}"|Areas]
        /// SUBSTRING	= :		Example: :[0,10]
        /// SPLIT		= ;		Example: ;[,]
        /// JOIN		= .		Example: .[ BYLINE|,|'{0}',{1},"{2}"]|
        ///                              .[ CUSTOM,;,|,{0}|{2}, true , true ], }]
        /// REPLACE		= /		Example: /[CadenaReemplazada,Reemplazo,0] 	    Options: 0 = All, 1 = First ocurrence, 2 = Last ocurrence
        /// 
        /// ANOTHER WAY TO BE ABLE TO MAKE A STATEMENT SYNTAX IS BY JSON COLUMNS
        /// [@varUsersApi][@{ 
        /// 			=[ {Filter: "status = 'active'"} ] 
        /// 			.[ {Separator: ",", FormatColumn: "({0},'{1}','{2}','{3}','{4}')\r\n", InLine: "IN_LINE"} ]| 
        /// 			>[ {OutputType: "TEXT", "FirstColumnAsHeader": 0, "EndLine": "\r\n,"} ];
        /// 		}]
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="valueInfo"></param>
        /// <returns></returns>
        private bool MatchVariableStatement(ref dynamic value, string matchString, ValueInfo valueInfo, ValueOptionEnum valueOption)
        {
            bool processed = false;
            Match matchStatement = Regex.Match(matchString, getVariableStatementRegularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (matchStatement.Success)
            {
                MatchCollection matchList = Regex.Matches(matchStatement.Groups["STATEMENT"].Value, FunctionConstants.GET_OPTION_STRING_TREATMENT_EXPRESSION);
                if (matchList?.Count > 0)
                {
                    dynamic variableValue = valueInfo != null ? (dynamic)valueInfo.Value : null;
                    string variableStatement = matchStatement.Groups["VARIABLE"].Value;
                    string variableJsonType = null;
                    processed = true;
                    bool unescapeActive = false;

                    foreach (Match matchStatementCode in matchList)
                    {
                        string statement = matchStatementCode.Groups["STATEMENT"].Value;
                        string code = Regex.Unescape(matchStatementCode.Groups["CODE"].Value);
                        string argumentSeparator = matchStatementCode.Groups["SEPARATOR"].Value.Trim();
                        dynamic codeValue = code;
                        
                        Process(ref codeValue, 
                                new Dictionary<string, VariableRegexTypeInfo>
                                {
                                    {FunctionConstants.GET_VARIABLE_VALUE_STATEMENT, new VariableRegexTypeInfo{ Regex = getVariableStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VALUE_STATEMENT } },
                                    {FunctionConstants.GET_VARIABLE_STATEMENT, new VariableRegexTypeInfo{ Regex = getStatementRegularExpression, ExtractType = ExtractTypeEnum.EXTRACT_VARIABLE_STATEMENT } }
                                }
                            , false);

                        code = codeValue;

                        List<string> argumentSplit = null;
                        if (string.IsNullOrWhiteSpace(argumentSeparator))
                        {
                            argumentSeparator = ",";
                        }

                        unescapeActive = code.Length != matchStatementCode.Groups["CODE"].Value.Length;

                        argumentSeparator = argumentSeparator[0].ToString().Trim();
                        argumentSplit = SplitParameters(code, argumentSeparator.Length > 0 ? argumentSeparator[0] : ',');
                        try
                        {
                            variableJsonType = processBC.GetJsonType(variableValue.ToString());
                        }
                        catch { }

                        // PROCESS VARIABLE WITH FOREACH ARGUMENTS
                        for (int countArgument = 0; countArgument < argumentSplit.Count; countArgument++)
                        {
                            dynamic argumentDynamic = argumentSplit[countArgument];
                            ProcessVariables(ref argumentDynamic);
                            argumentSplit[countArgument] = argumentDynamic.ToString();
                        }
                        code = string.Join(argumentSeparator, argumentSplit);

                        //// LUIS VERA, VER LA MANERA DE RECUPERAR EL processBC.GetDatabaseDAC() dentro de variableStatementBC
                        //if (statement == StatementConstants.STATEMENT_QUERY_SYMBOL)
                        //{
                        //    argumentSplit.Add(processBC.GetDatabaseName());
                        //}

                        variableValue = variableStatementBC.ProcessStatement(variableValue, variableJsonType, statement, code, argumentSeparator, argumentSplit?.ToArray());

                        // LUIS VERA, SE LE PONE EL ESCAPE PORQUE MARCABA ERRORES PERO AL QUITARSELO HACE COSAS INUSUALES, HAY QUE REVISAR BIEN LA MEJOR SOLUCIÓN
                        //if (unescapeActive)
                        //{
                        //    variableValue = Utilities.EscapeString(variableValue.ToString());
                        //}
                    }
                    
                    value = value.Replace(matchStatement.Value.Replace("[@#]", string.Empty), variableValue.ToString());
                }
            }

            return processed;
        }

        //public Dictionary<string, object> GetVariableFromString(string value)
        //{
        //Dictionary<string, object> variableDictionary = null;

        //if (value != null && value.Trim() != string.Empty)
        //{
        //    //try
        //    //{
        //    string cleanValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(value, "[@", ""), "]", "");
        //    //variableDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(cleanValue);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //}
        //}

        //return variableDictionary;
        //}


        /// <summary>
        /// GET PARAMETERS VALUE FROM FUNCTION STRING INTO DICTIONARY
        /// ALL PARAMETERS/ARGUMENTS MUST BE IN GROUP 3
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetFunctionFromString(Match matchValue)
        {
            const int groupName = 1;
            const int groupArguments = 2;
            Dictionary<string, object> variableDictionary = null;            

            if (matchValue != null && matchValue.Value.Trim() != string.Empty)
            {
                //// OBSOLETE, GET ALL INTO SYMBOL ()
                //MatchCollection matchList = Regex.Matches(matchValue.Groups[groupArguments].Value, FunctionConstants.GET_FUNCTION_ARGUMENTS_EXPRESSION);
                //if (matchList != null && matchList.Count > 0)
                //{
                //    variableDictionary = new Dictionary<string, object>()
                //    {
                //        { matchValue.Groups[groupName].Value, matchList.ToArray<object>() }
                //    };
                //}

                if (matchValue.Groups.Count >= groupArguments)
                {
                    List<object> valueList = new List<object>();                    
                    for (int count = groupArguments; count < matchValue.Groups.Count;)
                    {
                        valueList.Add(matchValue.Groups[count].Value);
                        count++;
                    }

                    variableDictionary = new Dictionary<string, object>()
                    {
                        { matchValue.Groups[groupName].Value, valueList.ToArray<object>() }
                    };
                }
            }

            return variableDictionary;
        }

        private bool StringIsJson(string value)
        {
            bool result = false;
            try
            {
                //Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
                //result = true;

                result = Regex.IsMatch(value, FunctionConstants.JSON_STRUCTURE_VALIDATE_EXPRESSION, 
                    RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace 
                    );
            }
            catch (Exception)
            {
            }

            return result;
        }

        public Dictionary<string, object> GetScriptFromString(Match matchValue)
        { 
            Dictionary<string, object> variableDictionary = null;

            if (matchValue != null && matchValue.Value.Trim() != string.Empty)
            {
                if (matchValue.Groups[FunctionConstants.LANGUAGE_TYPE].Success && matchValue.Groups[FunctionConstants.STATEMENT].Success)
                {
                    string languageType = matchValue.Groups[FunctionConstants.LANGUAGE_TYPE].Value;
                    string scriptCode = matchValue.Groups[FunctionConstants.STATEMENT].Value;
                    
                    scriptCode = scriptCode.ToString().Trim().Replace("\\\"", "\"");
                    variableDictionary = new Dictionary<string, object>()
                            {
                                { FunctionConstants.LANGUAGE_TYPE, languageType },
                                { FunctionConstants.STATEMENT, scriptCode }
                            };
                }
            }

            return variableDictionary;
        }

        public void ClearVariables()
        {
            valueList.Clear();
        }

        public bool VariableExists(string key)
        {
            // LUIS VERA SE DEBE DE VALIDAR EL ID HILO PRINCIPAL QUE SEA DIFERENTE AL DE GetCurrentTaskId
            // AL PARECER ES CORRECTO ASI, CUANDO SE INSERTE POR PRIMERA VEZ, SE VA A DUPLICAR LA PRIMER VARIABLE, NO PASA NADA SOLO PEGA EN PERFORMANCE MINIMO
            return valueList.Any(f => f.Key == key && f.ThreadTaskId == 0);
                //&& valueList.Any(f => f.Key == key && f.ThreadTaskId == ProcessBC.GetCurrentTaskId());
        }

        public bool VariableExistsByName(string name, string taskName = null)
        {
            return (taskName == null ? valueList.Any(f => f.Name == name && f.ThreadTaskId == 0) : valueList.Any(f => f.Name == name && f.TaskName == taskName));
                //&& (taskName == null ? valueList.Any(f => f.Name == name && f.ThreadTaskId == ProcessBC.GetCurrentTaskId()) : valueList.Any(f => f.Name == name && f.TaskName == taskName && f.ThreadTaskId == ProcessBC.GetCurrentTaskId()));
        }

        public void AddVariable(string key, dynamic value, string valueType, string outputValueType, bool isPublic, string name, int stepNumber, string taskName, string processName)
        {
            lock (addVariableLock)
            {
                if (!valueList.Any(f => f.Key == key && f.ThreadTaskId == 0))
                {

                    valueList.Add(new ValueInfo(processName, taskName, stepNumber, key, value, valueType,
                        outputValueType, isPublic, name, DateTime.Now, 0, 0, 0));
                }

                int currentTaskId = ProcessBC.GetCurrentTaskId();
                if (!valueList.Any(f => f.Key == key && f.ThreadTaskId == currentTaskId))
                {
                    valueList.Add(new ValueInfo(processName, taskName, stepNumber, key, value, valueType,
                        outputValueType, isPublic, name, DateTime.Now, currentTaskId, 0, 0));
                }
            }
        }

        public void SetVariableValue(string key, dynamic value, string valueType, string OutputValueType, bool isPublic)
        {
            {
                int currentTaskId = ProcessBC.GetCurrentTaskId();
                ValueInfo valueInfo = valueList.FirstOrDefault(f => f.Key == key && f.ThreadTaskId == currentTaskId);
                if (valueInfo != null)
                {
                    valueInfo.Value = value;
                    valueInfo.ValueType = valueType;
                    valueInfo.OutputValueType = OutputValueType;
                    valueInfo.IsPublic = isPublic;
                    valueInfo.LastUpdate = DateTime.Now;
                }
            }

            {
                ValueInfo valueInfo = valueList.FirstOrDefault(f => f.Key == key && f.ThreadTaskId == 0);
                if (valueInfo != null)
                {
                    valueInfo.Value = value;
                    valueInfo.ValueType = valueType;
                    valueInfo.OutputValueType = OutputValueType;
                    valueInfo.IsPublic = isPublic;
                    valueInfo.LastUpdate = DateTime.Now;
                }
            }
        }

        public void SetVariableValueByName(string name, string taskName, dynamic value)
        {
            {
                ValueInfo valueInfo = valueList.FirstOrDefault(f => f.Name == name && f.TaskName == taskName
                    && f.ThreadTaskId == ProcessBC.GetCurrentTaskId());

                if (valueInfo != null)
                {
                    valueInfo.Value = value;
                    valueInfo.LastUpdate = DateTime.Now;
                }
            }

            {
                ValueInfo valueInfo = valueList.FirstOrDefault(f => f.Name == name && f.TaskName == taskName && f.ThreadTaskId == 0);

                if (valueInfo == null)
                {
                    valueInfo = valueList.OrderByDescending(o => o.LastUpdate).FirstOrDefault(f => f.Name == name && f.TaskName == taskName);
                }

                if (valueInfo != null)
                {
                    valueInfo.Value = value;
                    valueInfo.LastUpdate = DateTime.Now;
                }
            }
        }

        public ValueInfo GetPublicVariable(string name, string taskName)
        {
            return valueList.FirstOrDefault(f => f.IsPublic == true && f.Name == name && f.TaskName == taskName && f.ThreadTaskId == 0);
        }

        public ValueInfo GetVariableByKey(string key)
        {
            ValueInfo valueResult = null;

            int currentTaskId = ProcessBC.GetCurrentTaskId();
            if (valueList.Any(f => f.Key == key && f.ThreadTaskId == currentTaskId))
            {
                valueResult = valueList.FirstOrDefault(f => f.Key == key && f.ThreadTaskId == currentTaskId);
            }
            else
            {
                valueResult = valueList.FirstOrDefault(f => f.Key == key && f.ThreadTaskId == 0);
            }

            return valueResult;
        }

        public ValueInfo GetVariable(string name, string taskName = null)
        {
            ValueInfo valueResult = null;
            int currentTaskId = ProcessBC.GetCurrentTaskId();

            if (valueList.Any(f => f.Name == name && f.TaskName == taskName && f.ThreadTaskId == currentTaskId))
            {
                valueResult = valueList.FirstOrDefault(f => f.Name == name && (taskName == null || f.TaskName == taskName) && f.ThreadTaskId == currentTaskId);
            }
            else
            {
                valueResult = valueList.FirstOrDefault(f => f.Name == name && (taskName == null || f.TaskName == taskName) && f.ThreadTaskId == 0);
            }

            return valueResult;
        }

        public ValueInfo GetVariableCurrentThread(string name, string taskName = null)
        {
            return valueList.FirstOrDefault(f => f.Name == name && (taskName == null || f.TaskName == taskName) && f.ThreadTaskId == ProcessBC.GetCurrentTaskId());
        }

    }
}
