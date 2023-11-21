using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using OWASP.WebGoat.NET.App_Code;
using OWASP.WebGoat.NET.App_Code.DB;
using OWASP.NET.Core;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using log4netl
using log4net.Config;
using System.Reflection;

namespace OWASP.WebGoat.NET.WebGoatCoins
{
    /// <summary>
    /// Summary description for Autocomplete
    /// </summary>
    public class Autocomplete : IHttpHandler
    {
    
    
        private IDbProvider du = Settings.CurrentDbProvider;

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");


            try
            {
                log.Info("Function Called: ProcessRequest()");
                string codes = context.Request.QueryString["codes"];
                string clazz = context.Request.QueryString["clazz"];
                string method = context.Request.QueryString["method"];
                string[] args = context.Request.QueryString["args"].Split('\n');
                context.Response.ContentType = "text/plain";

                context.Response.Write(this.DynamicRun(codes,clazz,method,args));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProcessRequest Exception Raised: {0}"+ ex.ToString());
            }
        }

        public string DynamicRun(string codes, string clazz, string method, string[] args)
        {
            Console.WriteLine("Instrumentation Called: Dynamic Run()");
            log.Info("Instrumentation Called: Dynamic Run()");
            ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters options = new CompilerParameters();
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.ServiceModel.dll");
            options.ReferencedAssemblies.Add("System.Data.dll");
            options.ReferencedAssemblies.Add("System.Runtime.dll");
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            string source = codes;

            CompilerResults compilerResults = compiler.CompileAssembleFromSource(options, source);
            if (compilerResults.Errors.HasErrors)
            {
                //ISSUE: reference to a compiler-generated field
                //ISSUE: reference to a compiler-generated field
                //ISSUE: reference to a compiler-generated field
                //ISSUE: method pointer
                //string.Join(Enviornment.NewLine, (IEnumberable<string>Enumerable, Select<CompilerError, string>((IEnuerable<M0>compilerResults.Errors.Cast<CompilerError>))))
                Console.WriteLine("Dynamic Run Instrumentation Error");
                return compilerResults.Errors.ToString();
            }
            object instance = compilerResults.CompiledAssembly.CreateInstance(clazz);
            return (string)instance.GetType().GetMethod(method, BindingFlags.Static | BindingFlags.Public).Invoke(instance, (object[])args);
            //return null;


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
