using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace MRK {
    public unsafe class MRKNative {
        public enum MRKPatchType {
            MRK_PATCH_NONE = 0,
            MRK_PATCH_METHOD_SIG = 1,
            MRK_PATCH_METHOD_BODY = 2,
            MRK_PATCH_FIELD = 4
        };

        public struct MRKPatch {
            public int Type;
            public string Owner;
            public string[] Ctx;
            public string[] New;
        }

        const int MRK_SANITY = 0xDEAD999;

        [DllImport("CZ GMKIT Native.dll")]
        static extern void MRKApplyPatches([MarshalAs(UnmanagedType.LPStr)] string projectPath, byte* buf, uint bufLen);

        static void WriteStringCpp(BinaryWriter writer, string str) {
            writer.Write(str.Length);
            writer.Write(Encoding.UTF8.GetBytes(str));
        }

        public static void PatchTest(Project project) {
            MRKPatch[] _patches = new MRKPatch[] {
                new MRKPatch {
                    Type = (int)MRKPatchType.MRK_PATCH_METHOD_SIG,
                    Owner = "MRKTEST.cs",
                    Ctx = new string[1] { "public static void MRKDAGODS(int x, int y)" },
                    New = new string[1] { "private volatile int MRKDAGODS(bool x, int y)" }
                }
            };

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                //sanity
                writer.Write(MRK_SANITY);

                //patch count
                writer.Write(_patches.Length);

                //write patches
                foreach (MRKPatch patch in _patches) {
                    //patch type
                    writer.Write(patch.Type);

                    //owner
                    WriteStringCpp(writer, patch.Owner);

                    //ctx
                    writer.Write(patch.Ctx.Length);

                    foreach (string ctx in patch.Ctx)
                        WriteStringCpp(writer, ctx);

                    foreach (string nw in patch.New)
                        WriteStringCpp(writer, nw);
                }

                byte[] buf = stream.GetBuffer();
                fixed (byte* _buf = buf) {
                    MRKApplyPatches(project.Path, _buf, (uint)buf.Length);
                }

                writer.Close();
            }
        }
    }
}
