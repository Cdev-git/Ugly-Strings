using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ugly_Strings.Runtimeshit;

namespace Ugly_Strings
{
    internal class Program
    {
        static ModuleDefMD Module = null;
        private static MethodDef GetStringMethod = null;

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                try
                {
                    string path = args[0];
                    Module = ModuleDefMD.Load(path);
                    Strings(Module);
                    Console.WriteLine("Strings Fucked");
                    Module.Write(path.Insert(path.Length - 4, "-UglyStrings"));
                }
                catch
                {
                    Console.WriteLine("Error While Obfuscating");
                    Thread.Sleep(5000);
                }
            }
            else
            {
                Console.WriteLine("Please Drag a Assembly onto the exe");
                Thread.Sleep(3000);
            }
        }

        static void Strings(ModuleDefMD module)
        {
            Type MyType = typeof(Runtime);
            TypeDef Runtime = ModuleDefMD.Load(MyType.Module).ResolveTypeDef(MDToken.ToRID(MyType.MetadataToken));
            GetStringMethod = Runtime.Methods.First(M => M.Name == "ReverseUglyStrings");
            Runtime.Methods.Remove(GetStringMethod);
            Module.GlobalType.Methods.Add(GetStringMethod);

            foreach (var type in module.Types)
            {
                foreach (var Method in type.Methods)
                {
                    for (int i = 0; i < Method.Body.Instructions.Count(); i++)
                    {
                        Instruction instr = Method.Body.Instructions[i];
                        if (instr.OpCode == OpCodes.Ldstr)
                        {
                            int key = random.Next();
                            string uglystring = UglyStrings(instr.Operand as string, key);
                            instr.Operand = uglystring;
                            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(key));
                            Method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(GetStringMethod));
                            Method.Body.SimplifyBranches();
                            Method.Body.OptimizeBranches();
                            Method.Body.OptimizeMacros();
                        }
                    }
                }
            }
        }
        static Random random = new Random();
        public static string UglyStrings(string input, int key)
        {
            byte[] str = Encoding.UTF8.GetBytes(input);
            byte[] buffer = new byte[str.Length * 2];

            for (int i = 0; i < str.Length; i++)
            {
                buffer[i * 2] = (byte)(str[i] ^ key);
                buffer[i * 2 + 1] = (byte)random.Next(1, 256);
            }

            byte[] compressed;

            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    gz.Write(buffer, 0, buffer.Length);
                }

                compressed = ms.ToArray();
            }

            return Convert.ToBase64String(compressed);
        }
    }
}
