using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    INT, FLOAT, STRING, READ, WRITE, REPEAT, UNTIL, IF, ELESIF, ELSE, THEN, RETURN, ENDL, Idenifier, Number,
    Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,LBraces, RBraces, NotEqual, AndOp, OROp,
    GreaterThanOp, PlusOp, MinusOp, MultiplyOp, DivideOp,COMMENT, Assign, String

}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.INT);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("string", Token_Class.STRING);
            ReservedWords.Add("read", Token_Class.READ);
            ReservedWords.Add("write", Token_Class.WRITE);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elesif", Token_Class.ELESIF);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("then", Token_Class.THEN);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("endl", Token_Class.ENDL);
            
           
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{",Token_Class.LBraces);
            Operators.Add("}", Token_Class.RBraces);
            Operators.Add("<>",Token_Class.NotEqual);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OROp);

        }

        public void StartScanning(string SourceCode)
        {
            // i: Outer loop to check on lexemes.
            for (int i = 0; i < SourceCode.Length; i++)
            {
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n'||CurrentChar=='\t')
                    continue;

                if (char.IsLetter(CurrentChar))
                {
                    // The possible Token Classes that begin with a character are
                    // an Idenifier or a Reserved Word.

                    // (1) Update the CurrentChar and validate its value.
                    j++;
                    if(j>=SourceCode.Length)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    else
                    {
                        CurrentChar = SourceCode[j];
                    }
                    // (2) Iterate to build the rest of the lexeme while satisfying the
                    // conditions on how the Token Classes should be.
                        // (2.1) Append the CurrentChar to CurrentLexeme.
                        // (2.2) Update the CurrentChar.
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {
                        CurrentLexeme += CurrentChar;
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }

                    // (3) Call FindTokenClass on the CurrentLexeme.
                    FindTokenClass(CurrentLexeme);
                    
                    // (4) Update the outer loop pointer (i) to point on the next lexeme.
                    i = j-1 ;
                }
                else if (char.IsDigit(CurrentChar))
                {
                    j++;
                    if (j >= SourceCode.Length)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    else
                    {
                        CurrentChar = SourceCode[j];
                    }
                    while (char.IsDigit(CurrentChar)||CurrentChar=='.')
                    {
                        
                            CurrentLexeme += CurrentChar;
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];
                        
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }
                else if (CurrentChar == '/')
                {

                    
                    j++;
                    if (j >= SourceCode.Length)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    else
                    {
                        CurrentChar = SourceCode[j];
                    }
                    if (CurrentChar!='*')
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                    else
                    {
                       
                        do
                        {

                            if (CurrentChar != '\r' && CurrentChar != '\n' && CurrentChar != '\t')
                                CurrentLexeme += CurrentChar;
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                           
                            CurrentChar = SourceCode[j];
                            if (j+1 >= SourceCode.Length)
                                break;
                            
                        } while (CurrentChar != '*' && SourceCode[j+1]!='/');
                        CurrentLexeme += CurrentChar;
                        if (j + 1 < SourceCode.Length)
                            CurrentLexeme += SourceCode[j+1];
                        j += 2;
                        FindTokenClass(CurrentLexeme);
                        i = j - 1; 
                    }
                    
                }
                else if (CurrentChar == ';'|| CurrentChar == '-' || CurrentChar == '+' || CurrentChar == '*' || CurrentChar == '<' ||
                            CurrentChar == '>' || CurrentChar == ':' || CurrentChar == '&' || CurrentChar == '|' || CurrentChar == '{'||
                            CurrentChar == '}'|| CurrentChar == '(' || CurrentChar == ')' || CurrentChar == ',' || CurrentChar == '='||
                            CurrentChar=='.')
                {
                         if ((CurrentChar == ':' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '=') ||
                        (CurrentChar == '<' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '>') ||
                        (CurrentChar == '&' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '&') ||
                        (CurrentChar == '|' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '|'))

                         {
                            j++;
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar;
                            i = j;
                         }

                         FindTokenClass(CurrentLexeme);
                }
                else if(CurrentChar=='"')
                {
                    j++;
                    if (j >= SourceCode.Length)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    else
                    {
                        CurrentChar = SourceCode[j];
                    }

                    while (CurrentChar != '"')
                    {
                        CurrentLexeme += CurrentChar;
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    } 
                    CurrentLexeme += CurrentChar;
                    j++;
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else
                {
                    Errors.Error_List.Add("Unrecognized token : "+CurrentLexeme);
                }
            }

            TINY_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            //Is it an comment?
            else if (iscomment(Lex))
            {
                Tok.token_type = Token_Class.COMMENT;
                Tokens.Add(Tok);
            }
            //Is it an string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                // error
                Errors.Error_List.Add("Unrecognized token : " + Lex);
            }
        }

        bool isIdentifier(string lex)
        {
            bool isValid = true;
            var rx = new Regex(@"^([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*$");
            // Check if the lex is an identifier or not.
            if (rx.IsMatch(lex))
                isValid = true;
            else
                isValid = false;
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            var rx = new Regex(@"^[0-9]+(.[0-9]+)?$");
            // Check if the lex is a constant (Number) or not.
            if (rx.IsMatch(lex))
                isValid = true;
            else
                isValid = false;
            return isValid;
        }
        bool iscomment(string lex)
        {
            bool isValid = true;
            var rx = new Regex(@"^/\*.*\*/$");
            // Check if the lex is a constant (Number) or not.
            if (rx.IsMatch(lex))
                isValid = true;
            else
                isValid = false;
            return isValid;
            
        }
        bool isString(string lex)
        {
            bool isValid = true;
            var rx = new Regex(@"^"".*""$");
            // Check if the lex is a constant (Number) or not.
            if (rx.IsMatch(lex))
                isValid = true;
            else
                isValid = false;
            return isValid;

        }
    }
}
