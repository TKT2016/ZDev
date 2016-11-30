using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDev.Schema.Models;
using ZDev.Schema.Models.Lang;

namespace ZDev.Schema.Parses
{
    public partial class SchemaParser
    {
        protected List<Token> srcTokens;
        protected int index;
        TKTClassModel tktclass;

        public TKTClassModel Parse(List<Token> tokens)
        {
            this.srcTokens = tokens;
            tktclass = new TKTClassModel ();
            this.srcTokens.Add(Token.EOF);
            this.srcTokens.Add(Token.EOF);
            this.srcTokens.Add(Token.EOF);
            this.index = 0;

            while (CurrentToken.Kind != TokenKind.EOF)
            {
                /*while (CurrentToken.IsKeyIdent("使用包","使用类型","简略使用")&& NextToken.Kind == TokenKind.Colon)
                {
                    ParseImport();
                }*/
                if (CurrentToken.IsKeyIdent("使用包") && NextToken.Kind == TokenKind.Colon)
                {
                    ParseImport();
                }

                if (CurrentToken.IsKeyIdent("简略使用") && NextToken.Kind == TokenKind.Colon)
                {
                    ParseSimpleUse();
                }
                if (CurrentToken.IsKeyIdent("属于") && NextToken.Kind == TokenKind.Colon)
                {
                    parseExtends();
                }
                else if (CurrentToken.IsKeyIdent("名称") && NextToken.Kind == TokenKind.Colon)
                {
                    parseName();
                }
                else if (CurrentKind == TokenKind.Ident && CurrentText == "过程" && NextToken.Kind == TokenKind.Colon)
                {
                    var ast = ParseMethod();
                    string typeName = tktclass.GetTypeName();
                    if (ast.IsContruct())
                    {
                        tktclass.ContructList.Add(ast.ToContruct());
                    }
                    else if (!string.IsNullOrEmpty(typeName) && ast.IsContruct(typeName))
                    {
                        tktclass.ContructList.Add(ast.ToContruct());
                    }
                    else
                    {
                        tktclass.ProcList.Add(ast);
                    }
                    //tktclass. //prog.Add(ast);
                }
                else if (CurrentKind == TokenKind.Ident && CurrentText == "属性" && NextToken.Kind == TokenKind.Colon)
                {
                    parsePropertyPart();
                }
                
                else if (CurrentToken.IsKeyIdent("约定") && NextToken.Kind == TokenKind.Colon)
                {
                    parseAgreement();
                }
                else
                {
                    //error("无法识别'"+CurrentToken.GetText()+"'");
                    MoveNext();
                }
            }
            return tktclass;
        }

        void parseAgreement()
        {
            var tempToken = CurrentToken;
            MoveNext();//跳过"约定"
            MoveNext();//跳过":"
            //AgreementAST agreeast = new AgreementAST();
            //agreeast.KeyToken = tempToken;
            List<Token> items = parseAgreementBlock(tempToken.Postion);
            //agreeast.ValueList.AddRange(items);
            //prog.Add(agreeast);
            foreach(var token in items)
            {
                TKTContentModel tc = new TKTContentModel(token.GetText(),CurrentToken.Postion);
                tktclass.EnumItems.Add(tc);
            }    
        }

        List<Token> parseAgreementBlock(CodePostion parentPostion)
        {
            List<Token> list = new List<Token>();
            while (CurrentToken.Kind != TokenKind.EOF && CurrentToken.Col > parentPostion.Col)
            {
                Token temp = parseEnumItem();
                if (temp != null)
                {
                    list.Add(temp);
                }
            }
            return list;
        }

        Token parseEnumItem()
        {
            if (CurrentKind == TokenKind.Ident)
            {
                Token temp = CurrentToken;
                MoveNext();
                match(TokenKind.Semi);
                return temp;
            }
            else
            {
                //error("规定的成员不正确");
                MoveNext();
            }
            return null;
        }

        void parseExtends()
        {
            MoveNext();
            MoveNext();
            //prog.ExtendsToken = CurrentToken;
            if(CurrentKind== TokenKind.Ident)
            {
                tktclass.BaseType = new TKTContentModel(CurrentToken);
            }
            MoveNext();
            match(TokenKind.Semi);
        }

        void parseName()
        {
            MoveNext();
            MoveNext();
            //prog.NameToken = CurrentToken;
            if (CurrentKind == TokenKind.Ident)
            {
                tktclass.NameModel = new TKTContentModel (CurrentToken);
            }
            MoveNext();
            match(TokenKind.Semi);
        }

        void parsePropertyPart()
        {
            Token tempToken = CurrentToken;
            MoveNext();//跳过"属性"
            MoveNext();//跳过":"
            while (CurrentToken.Kind != TokenKind.EOF && CurrentToken.Col > tempToken.Col)
            {
                var ast = parseProperty(tempToken.Postion);
                tktclass.PropertyList.Add(ast);
            }
        }

        TKTPropertyModel parseProperty(CodePostion parentPostion)
        {
            TKTPropertyModel ast = new TKTPropertyModel();
            ast.Postion = CurrentToken.Postion;
            if(CurrentKind!= TokenKind.Ident)
            {
                //error("不是正确的名称");
            }
            else
            {
                ast.PropertyType = CurrentToken.GetText();
            }
            MoveNext();
            match(TokenKind.Colon);
            if (CurrentKind != TokenKind.Ident)
            {
                //error("不是正确的变量名称");
            }
            else
            {
                ast.PropertyName = CurrentToken.GetText();
            }
            MoveNext();
            if(CurrentKind== TokenKind.Assign)
            {
                MoveNext();
                skipToSemi();
                //Exp exp = parseExp();
                //ast.ValueExp = exp;
            }
            match(TokenKind.Semi);
            return ast;
        }
       /*
        void ParseImport()
        {
            var keyToken = CurrentToken;   
            MoveNext();//跳过"使用"
            MoveNext();//跳过":"
            //CodePostion postion = CurrentToken.Postion;
             TKTContentModel uc = new TKTContentModel() { Postion = CurrentToken.Postion };
            string packageName = parsePackage(); //ast.Package = parsePackage();
            uc.Content = packageName;
            match(TokenKind.Semi);
            string usingText = keyToken.GetText();
           
            if(usingText== "使用包")
            {
                tktclass.UsingPackages.Add(uc);
            }
            //else if (usingText == "使用类型")
            //{
            //    tktclass.UsingTypes.Add(uc);
            //}
            else if (usingText == "简略使用")
            {
                tktclass.RedirectTypes.Add(uc);
            }
        }*/

        void ParseImport()
        {
            //ImportPackageAST ast = new ImportPackageAST();
            var keyToken = CurrentToken;   //ast.KeyToken = CurrentToken;
            MoveNext();//跳过"使用"
            MoveNext();//跳过":"
            parsePackageList(); //ast.Packages = parsePackageList();
            matchSemiOrNewLine();//match(TokenKind.Semi);
            //return ast;
        }

        void parsePackageList()
        {
            //List<PackageAST> list = new List<PackageAST>();
            if (CurrentKind == TokenKind.Ident)
            {
                TKTContentModel uc = new TKTContentModel() { Postion = CurrentToken.Postion };
                string packageName = parsePackage(); //PackageAST ast = parsePackage();
                uc.Content = packageName;
                //if (ast != null)
                //{
                //    list.Add(ast);
                //}
                tktclass.UsingPackages.Add(uc);
                while (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        string packageName2 = parsePackage(); //PackageAST ast2 = parsePackage();
                        TKTContentModel uc2 = new TKTContentModel() { Postion = CurrentToken.Postion };
                        uc2.Content = packageName2;
                        tktclass.UsingPackages.Add(uc2);
                        //if (ast != null)
                        //{
                        //    list.Add(ast2);
                        //}
                    }
                    else
                    {
                        //error("错误的类型名称");
                        break;
                    }
                }
            }
            else
            {
                //error("错误的开发包名称");
                MoveNext();
            }
            //return list;
        }

        void ParseSimpleUse()
        {
            //SimpleUseAST ast = new SimpleUseAST();
            //ast.KeyToken = CurrentToken;
            MoveNext();//跳过"使用"
            MoveNext();//跳过":"
            var NameTokens = parseNames();//ast.NameTokens = parseNames();
            foreach (var token in NameTokens)
            {
                TKTContentModel tcm = new TKTContentModel() { Postion = token.Postion };
                tcm.Content = token.ToCode();
                tktclass.RedirectTypes.Add(tcm);
            }
            matchSemiOrNewLine();// match(TokenKind.Semi);
            //return ast;
        }

        List<Token> parseNames()
        {
            List<Token> tokens = new List<Token>();
            if (CurrentKind == TokenKind.Ident)
            {
                tokens.Add(CurrentToken);
                MoveNext();
                while (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        tokens.Add(CurrentToken);
                        MoveNext();
                    }
                    else
                    {
                        //error("错误的类型名称");
                    }
                }
            }
            else
            {
                //error("错误的类型名称");
                MoveNext();
            }
            return tokens;
        }

        string parsePackage()
        {
            List<Token> tokens = new List<Token>();
            if(CurrentKind== TokenKind.Ident )
            {
                tokens.Add(CurrentToken);
                MoveNext();
                while(CurrentKind== TokenKind.DIV)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        tokens.Add(CurrentToken);
                        MoveNext();
                    }
                    else
                    {
                        //error("错误的开发包名称");
                    }
                }
            }
            else
            {
                //error("错误的开发包名称");
                MoveNext();
            }
            List<string> list = new List<string>();
            foreach (Token item in tokens)
            {
                list.Add(item.ToCode());
            }
            string str = string.Join("/", list);
            return str;
        }

        public TKTProcModel ParseMethod()
        {
            var tempToken = CurrentToken;
            MoveNext();//跳过"过程"
            MoveNext();//跳过":"
            var fnname = parseFnName();
            skipBlock(tempToken.Postion, 2);
            return fnname;
        }

        TKTProcModel parseFnName()
        {
            TKTProcModel fname = new TKTProcModel();
            fname.Postion = CurrentToken.Postion;
            var curline = CurrentToken.Line;
            while (CurrentToken.Kind != TokenKind.EOF && curline==CurrentToken.Line)
            {
                if(CurrentKind== TokenKind.LBS)
                {
                    var arg = parseFnMuArg();
                    if(arg!=null)
                    {
                        fname.Elements.Add(arg);
                    }
                }
                else if(CurrentKind== TokenKind.Ident)
                {
                    var textt = parseFnText();
                    if (textt != null)
                    {
                        fname.Elements.Add(textt);
                    }
                }
                else if(CurrentKind== TokenKind.AssignTo)
                {
                    if (fname.Elements == null || fname.Elements.Count == 0)
                    {
                        //error("过程没有名称");
                        MoveNext();
                    }
                    else
                    {
                        MoveNext();
                        if(CurrentKind== TokenKind.Ident)
                        {
                            fname.RetType = CurrentToken.GetText();
                            MoveNext();
                        }
                        else
                        {
                            //error("错误的过程结果");
                            MoveNext();
                        }
                    }
                }
                else
                {
                    //error("错误的过程名称");
                    MoveNext();
                }
            }
            return fname;
        }

        string parseFnText()
        {
            string text = CurrentToken.GetText();
            //FnText textterm = new FnText ();
            //textterm.TextToken = CurrentToken;
            MoveNext();
            return text;
        }

        TKTArgBracket parseFnMuArg()
        {
            TKTArgBracket marg = new TKTArgBracket();
            MoveNext();
            while (!isBracketEnd(CurrentKind))
            {
                var sarg = parseFnArg();
                if(sarg!=null)
                {
                    marg.Args.Add(sarg);
                }
                if (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                }
            }
            if (CurrentKind == TokenKind.RBS)
            {
                //marg.RightBracketToken = CurrentToken;
                MoveNext();
            }
            else
            {
                //error("括号不匹配");
            }
            return marg;
        }

        TKTArgModel parseFnArg()
        {
            TKTArgModel argt = new TKTArgModel();
            if(CurrentKind== TokenKind.Ident)
            {
                argt.ArgType = CurrentToken.GetText();
                MoveNext();
                if(CurrentKind== TokenKind.Colon)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        argt.ArgName = CurrentToken.GetText();
                        MoveNext();
                        return argt;
                    }
                    else
                    {
                        //error("参数名称不正确");
                    }
                }
                else
                {
                    //error("应该是':'");
                }
            }
            else
            {
                //error("参数类型不正确");
                MoveNext();
            }
            return null;
        }
       
    }
}
