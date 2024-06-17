using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyDependancyChecker
{
    public class Checker
    {
        string[] ignoreAssembly = { "system.", "netstandard", "microsoft.csharp" };

        public Checker() { 
        
        }

        //ref https://learn.microsoft.com/en-us/dotnet/standard/assembly/inspect-contents-using-metadataloadcontext
        public void Load(DirectoryInfo dir)
        {
            Console.WriteLine("Checking directory " + dir.FullName);
            FileInfo[] allAssemblyFileInfo = dir.GetFiles( "*.dll");
            List<string> allAssemblies =new List<string>();
            foreach (FileInfo file in allAssemblyFileInfo)
            {
                allAssemblies.Add(file.FullName);
            }
            var resolver = new PathAssemblyResolver(allAssemblies);
            int errcount = 0;
            Dictionary<string,int> resolutionErrors = new Dictionary<string,int>();
            foreach (string assemblyPath in allAssemblies) 
            {
                //is the file an assembly
                if(CheckAssembly(assemblyPath))
                {                    
                    string assemblyFileName  = Path.GetFileName(assemblyPath);
                    string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                    var mlc = new MetadataLoadContext(resolver,assemblyName);
                    Assembly assembly = null;
                    assembly = mlc.LoadFromAssemblyPath(assemblyPath);                                        

                    // Print assembly attribute information.
                    //Console.WriteLine($"{assemblyName} has following attributes: ");
                    var refAssemblies =   assembly.GetReferencedAssemblies();
                    foreach(var ass in refAssemblies)
                    {
                        //Console.WriteLine(ass.FullName);
                        try
                        {
                            if (!IsIgnoreAssembly(ass.FullName))
                            {
                                if (resolutionErrors.ContainsKey(ass.FullName))
                                {
                                    resolutionErrors[ass.FullName]++;                                    
                                    Console.WriteLine($"Assembly resolution error found in assembly {assemblyFileName} could not resolve {ass.FullName} ");
                                    
                                }
                                else
                                    mlc.LoadFromAssemblyName(ass.FullName);
                            }
                            
                        }
                        catch(Exception ex) 
                        {
                            Console.WriteLine($"Assembly resolution error found in assembly {assemblyFileName} could not resolve {ass.FullName} ");
                            resolutionErrors.Add(ass.FullName, 1);
                            errcount++; 
                        }

                    }
                    
                }
            }
            if (errcount == 0)
                Console.WriteLine("All Assemblies Resolved Successfully, Checked Assembly count "+allAssemblies.Count);
            else
                Console.WriteLine("Assembly Resolution Erros found, Errcount "+errcount);            
            Console.ReadLine();


        }

        public bool IsIgnoreAssembly(string assemblyName)
        {
            foreach(string prefix in ignoreAssembly)
            {
                if(assemblyName.ToLower().StartsWith(prefix))
                    return true;
            }
            return false;
        }

        //ref https://learn.microsoft.com/en-us/dotnet/standard/assembly/identify
        public static bool CheckAssembly(string path)
        {
            try
            {                

                AssemblyName testAssembly = AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file cannot be found.");
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("The file is not an assembly.");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("The assembly has already been loaded.");
            }
            return false;
        }
    }
}
